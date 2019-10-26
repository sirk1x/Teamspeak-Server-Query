using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLittleTeamspeakServerQuery
{
    public enum QueryAction
    {
        QuerySelectPort,
        QueryServerInfo,
        QueryClient,
        QueryClientInfo,
        QueryChannelList,
        QueryChannelInfo,
        QueryGroups,
        QueryIcons,
        QueryDisconnect
    }
    public class ServerQuery : IDisposable
    {
        public const int BUFFER_SIZE = 8192;
        private static byte[] Error = new byte[] { 101, 114, 114, 111, 114, 32 };
        private static byte[] Success = new byte[] { 101, 114, 114, 111, 114, 32, 105, 100, 61, 48, 32, 109, 115, 103, 61, 111, 107, 10 };
        private static byte[] Welcome = new byte[] { 84, 83, 51, 10, };
        private TcpClient _tcpclient = null;
        private NetworkStream _networkstream = null;

        private string _hostname = "";
        private ushort _port = 0;
        private ushort _queryport = 0;

        private QueryAction _current = QueryAction.QuerySelectPort;

        private long ping_triptime = 0;

        private static readonly Random _randomForFileClientTransferId = new Random();

        public Dataset.ServerInfo _serverinfo = new Dataset.ServerInfo();

        private int currentchannel = 0;

        private string current_filename = "";

        private string current_path = "";

        private uint current_icon = 0;

        public List<Dataset.Error> _errors = new List<Dataset.Error>();


        public bool QuerySuccess = false;

        private bool CancellationPending = false;

        //public List<Tuple<uint, string>> Icons = new List<Tuple<uint, string>>();
        public List<Tuple<string, string, uint>> ServerIcons = new List<Tuple<string, string, uint>>();

        private string icoPth = "";

        public ServerQuery(string hostname, ushort port, ushort queryport, string cachePath, out string iconPath)
        {
            try
            {
                _hostname = hostname;
                _port = port;
                _queryport = queryport;
                icoPth = cachePath + Guid.NewGuid().ToString();
                iconPath = icoPth;
                if (PingHost(_hostname, out ping_triptime))
                {
                    //Console.WriteLine("Ping was successfull " + ping_triptime.ToString());
                    ping_triptime = ping_triptime * 2;
                }
                else
                {
                    //Console.WriteLine("Couldn't get a ping response, falling back to 250ms");
                    ping_triptime = 250;
                }
                _tcpclient = new TcpClient();
                _tcpclient.ReceiveBufferSize = BUFFER_SIZE;
                _tcpclient.SendBufferSize = BUFFER_SIZE;
                _tcpclient.Connect(_hostname, _queryport);
                _networkstream = _tcpclient.GetStream();

                while (!_networkstream.DataAvailable)
                    Thread.Sleep(16);
                ConvertBytes(ReceiveTcpData().Result);
                Thread.Sleep(100);
                bool mustdostuff = false;
                while (_tcpclient.Connected && CancellationPending == false)
                {
                    switch (_current)
                    {
                        case QueryAction.QuerySelectPort:
                            //Console.WriteLine("Setting Port");
                            SendSockData("use port=" + port);
                            break;
                        case QueryAction.QueryServerInfo:
                            //Console.WriteLine("serverinfo Port");
                            SendSockData("serverinfo");
                            break;
                        case QueryAction.QueryClient:
                            //Console.WriteLine("Clientlist");
                            SendSockData("clientlist");
                            break;
                        case QueryAction.QueryClientInfo:
                            //Console.WriteLine("clientinfo");
                            SendSockData("clientinfo " + GetFormattedClids(CLID));
                            break;
                        case QueryAction.QueryChannelList:
                            SendSockData("channellist");
                            break;
                        case QueryAction.QueryChannelInfo:
                            for (int i = 0; i < _serverinfo.channels.Length; i++)
                            {
                                SendSockData("channelinfo cid=" + _serverinfo.channels[i].cid);
                                currentchannel = i;
                                while (!_networkstream.DataAvailable)
                                    Thread.Sleep((int)ping_triptime);
                                ConvertBytes(ReceiveTcpData().Result);

                                Thread.Sleep(333);
                            }
                            mustdostuff = true;
                            break;
                        case QueryAction.QueryGroups:
                            mustdostuff = false;
                            SendSockData("servergrouplist");
                            break;
                        case QueryAction.QueryIcons:
                            uint[] _ids = GetIconIds();
                            uint randomClientId = (uint)_randomForFileClientTransferId.Next(1, 10000);
                            if (!cachePath.EndsWith(@"\"))
                                cachePath += @"\";
                            current_path = cachePath + @"data\" + _hostname + port + queryport + @"\";
                            if (!Directory.Exists(current_path))
                                Directory.CreateDirectory(current_path);
                            for (int i = 0; i < _ids.Length; i++)
                            {
                                current_icon = _ids[i];
                                current_filename = "icon_" + _ids[i].ToString();
                                if (!File.Exists(current_path + current_filename))
                                {
                                    SendSockData(string.Format(@"ftinitdownload clientftfid={0} name=\/icon_{1} cid=0 cpw= seekpos=0", randomClientId, _ids[i]));
                                    while (!_networkstream.DataAvailable)
                                        Thread.Sleep((int)ping_triptime);
                                    ConvertBytes(ReceiveTcpData().Result);

                                    Thread.Sleep(333);
                                }
                                else
                                {
                                    ServerIcons.Add(
                                        Tuple.Create(Parser.CalculateMD5Hash(_hostname + _port.ToString()) + current_filename,
                                        current_path + current_filename,
                                        current_icon
                                        ));
                                }
                            }
                            mustdostuff = true;
                            break;
                        case QueryAction.QueryDisconnect:
                            QuerySuccess = true;

                            File.WriteAllText(current_path + "setings.json", JsonConvert.SerializeObject(ServerIcons));

                            return;
                        default:
                            break;
                    }

                    if (!mustdostuff)
                    {
                        while (!_networkstream.DataAvailable)
                            Thread.Sleep((int)ping_triptime);
                        ConvertBytes(ReceiveTcpData().Result);
                        mustdostuff = false;
                    }

                    _current++; // = (_current + 1);
                    Thread.Sleep(333);
                }
            }
            catch (Exception e)
            {
                if (e.GetType().IsAssignableFrom(typeof(SocketException)))
                {
                    _errors.Add(new Dataset.Error { id = 10061, msg = "Connection Refused. Is the server online?" });
                    iconPath = null;
                    return;
                }
                _errors.Add(new Dataset.Error { id = 401, msg = "Internal Server Error" });
                iconPath = null;
                return;
            }

        }

        private void SendSockData(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s + Environment.NewLine);
            _networkstream.Write(bytes, 0, bytes.Length);
            _networkstream.Flush();
            //_tcpwriter.Write(Encoding.UTF8.GetBytes(s));
            //_tcpwriter.Flush();
        }

        private async Task<byte[]> ReceiveTcpData()
        {
            try
            {
                const int bufferSize = 4096;
                using (var ms = new MemoryStream())
                {
                    byte[] buffer = new byte[bufferSize];
                    int count;
                    do
                    {
                        count = await _networkstream.ReadAsync(buffer, 0, buffer.Length);
                        //count = _networkstream.Read(buffer, 0, buffer.Length);
                        Thread.Sleep((int)ping_triptime); //Sometimes the stream just cuts off because the complete set of bytes 
                                                          //isn't received yet and the DataAvailable bool flicks to false for a brief moment, 
                                                          //so we get fragmented byte arrays. 
                        ms.Write(buffer, 0, count);
                    } while (_networkstream.DataAvailable);

                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ConvertBytes(byte[] b)
        {
            byte[][] _messagebytearray = Parser.ByteToChunks(b, (int)0x0D).ToArray();
            if (_messagebytearray.Length == 0)
            {
                //Console.WriteLine("Received 0 bytes");
                return;
            }
            if (Parser.ByteCompare(_messagebytearray[0], Welcome))
            {
                //Console.WriteLine("Received welcome message");
                return;

            }
            byte[] _lastmessage = Parser.TrimEnd(_messagebytearray[_messagebytearray.Length - 1]);
            //Console.WriteLine("BYTE : " + DisplayAssistance.ByteToInt(_lastmessage));
            //Console.WriteLine("HEX: " + DisplayAssistance.ByteArrayToString(_lastmessage));

            if (Parser.ByteCompare(_lastmessage, Error))
            {
                //Console.WriteLine("Received an status update");
                if (!Parser.ByteCompare(_lastmessage, Success))
                {
                    byte[] _message = Parser.RemoveSignature(_lastmessage, Error.Length);
                    byte[][] _params = Parser.ByteToChunks(Parser.TrimEnd(_message), (int)0x20).ToArray();
                    Dataset.Error e = Parser.Parse<Dataset.Error>(typeof(Dataset.Error), _params, new Dataset.Error());
                    _errors.Add(e);
                    //Console.WriteLine("There was an error: " + Environment.NewLine + JsonConvert.SerializeObject(e, Formatting.Indented));
                    //Console.WriteLine("RAW " + Encoding.UTF8.GetString(_message));
                    CancellationPending = true;
                    return;
                    //Console.ReadKey();
                }
                //Console.WriteLine("Everything seems fine.");
            }


            if (_messagebytearray.Length == 1)
            {
                //Console.WriteLine("Only received OK");
                return;
            }

            if (_messagebytearray.Length > 2)
            {
                //Console.WriteLine("Too many arrays");
                return;
            }

            //Console.WriteLine(Convert.ToInt32(0x20));

            if (Parser.IsArray(_messagebytearray[0], (int)0x7C))
            {

                //byte[][] ienumerable users as whole chunks.
                byte[][] _arrayparts = Parser.ByteToChunks(_messagebytearray[0], (int)0x7C).ToArray();
                List<byte[][]> _userparams = new List<byte[][]>();
                //Each iteration is one user
                for (int i = 0; i < _arrayparts.Length; i++)
                {
                    //Split user by the BLANK SPACE byte
                    byte[][] _params = Parser.ByteToChunks(_arrayparts[i], (int)0x20).ToArray();
                    _userparams.Add(_params);
                }

                switch (_current)
                {
                    case QueryAction.QueryClient:

                        _serverinfo.clients = new Dataset.Client[_userparams.Count];
                        for (int i = 0; i < _userparams.Count; i++)
                        {
                            _serverinfo.clients[i] = Parser.Parse<Dataset.Client>(typeof(Dataset.Client), _userparams[i], new Dataset.Client());
                        }


                        break;
                    case QueryAction.QueryClientInfo:
                        for (int i = 0; i < _userparams.Count; i++)
                        {
                            _serverinfo.clients[i].properties = Parser.Parse<Dataset.ClientProperties>(typeof(Dataset.ClientProperties), _userparams[i], new Dataset.ClientProperties());
                        }
                        break;
                    case QueryAction.QueryChannelList:
                        _serverinfo.channels = new Dataset.Channel[_userparams.Count];
                        for (int i = 0; i < _userparams.Count; i++)
                        {
                            _serverinfo.channels[i] = Parser.Parse<Dataset.Channel>(typeof(Dataset.Channel), _userparams[i], new Dataset.Channel());
                        }
                        break;

                    case QueryAction.QueryGroups:
                        _serverinfo.groups = new Dataset.ServerGroup[_userparams.Count];
                        for (int i = 0; i < _userparams.Count; i++)
                        {
                            _serverinfo.groups[i] = Parser.Parse<Dataset.ServerGroup>(typeof(Dataset.ServerGroup), _userparams[i], new Dataset.ServerGroup());
                        }
                        break;
                    default:
                        break;
                }


            }
            else
            {
                byte[][] _params = Parser.ByteToChunks(_messagebytearray[0], (int)0x20).ToArray();

                switch (_current)
                {

                    case QueryAction.QueryClient:

                        _serverinfo.clients = new Dataset.Client[1];

                        _serverinfo.clients[0] = Parser.Parse<Dataset.Client>(typeof(Dataset.Client), _params, new Dataset.Client());
                        break;
                    case QueryAction.QueryServerInfo:
                        _serverinfo = Parser.Parse<Dataset.ServerInfo>(typeof(Dataset.ServerInfo), _params, new Dataset.ServerInfo());
                        break;
                    case QueryAction.QueryChannelInfo:
                        _serverinfo.channels[currentchannel].channelproperties = Parser.Parse<Dataset.ChannelProperties>(typeof(Dataset.ChannelProperties), _params, new Dataset.ChannelProperties());
                        break;

                    case QueryAction.QueryGroups:
                        break;
                    case QueryAction.QueryIcons:
                        Dataset.Download d = Parser.Parse<Dataset.Download>(typeof(Dataset.Download), _params, new Dataset.Download());
                        DownloadClient.ReceiveDownload(_hostname, d.ftkey, d.size, current_filename, current_path, d.port);
                        ServerIcons.Add(
                            Tuple.Create(Parser.CalculateMD5Hash(_hostname + _port.ToString()) + current_filename,
                            current_path + current_filename,
                            current_icon
                            ));
                        break;
                    default:
                        break;
                }

            }

            return;


        }

        private string CLID = "clid={0}";
        private string GetFormattedClids(string t)
        {
            string cids = "";
            for (int i = 0; i < _serverinfo.clients.Length; i++)
            {
                if (i == _serverinfo.clients.Length - 1)
                    cids += string.Format(t, _serverinfo.clients[i].clid);
                else
                    cids += string.Format(t, _serverinfo.clients[i].clid + "|");
            }
            return cids;
        }

        private bool PingHost(string nameOrAddress, out long ms)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
                ms = reply.RoundtripTime;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
                ms = 0;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
        private uint[] GetIconIds()
        {
            List<uint> _iconids = new List<uint>();
            if (_serverinfo.channels != null)
                foreach (var c in _serverinfo.channels)
                {
                    if (c != null)
                        if (c.channelproperties != null)
                            if (ValidIcon(c.channelproperties.channel_icon_id))
                                _iconids.Add(c.channelproperties.channel_icon_id);
                }
            if (_serverinfo.groups != null)
                foreach (var s in _serverinfo.groups)
                {
                    if (ValidIcon(s.iconid))
                        _iconids.Add(s.iconid);
                }
            if (_serverinfo.clients != null)
                foreach (var c in _serverinfo.clients)
                {
                    if (c != null)
                        if (c.properties != null)
                            if (ValidIcon(c.properties.client_icon_id))
                                _iconids.Add(c.properties.client_icon_id);

                }
            return _iconids.ToArray();
        }
        private bool ValidIcon(uint? number)
        {
            return (number != 0 && number != 100 && number != 200 && number != 300 && number != 400 && number != 500 && number != 600);
        }
        public void Dispose()
        {
            if (_tcpclient != null)
                if (_tcpclient.Connected)
                    SendSockData("quit");
            if (_networkstream != null)
            {
                _networkstream.Flush();
                _networkstream.Close();
                _networkstream.Dispose();
            }
            if (_tcpclient != null)
            {
                if (_tcpclient.Connected)
                    _tcpclient.Close();

                _tcpclient.Dispose();
            }
            _networkstream = null;
            _tcpclient = null;
            GC.SuppressFinalize(this);
        }
    }
}

