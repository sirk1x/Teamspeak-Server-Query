using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyLittleTeamspeakServerQuery
{
    public static class DownloadClient
    {
        public static void ReceiveDownload(string host, string key, ulong size, string filename, string path, ushort _ftport)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                s.ReceiveBufferSize = ServerQuery.BUFFER_SIZE;
                s.SendBufferSize = ServerQuery.BUFFER_SIZE;
                IPAddress ipV4;
                EndPoint endPoint = IPAddress.TryParse(host, out ipV4) ? new IPEndPoint(ipV4, _ftport) : (EndPoint)new DnsEndPoint(host, _ftport);
                s.Connect(endPoint);

                byte[] transferkey = Encoding.UTF8.GetBytes(key);
                int _b = s.Send(transferkey);

                uint process = 0;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (FileStream fs = File.Create(path + filename + "."))
                {
                    do
                    {
                        byte[] buf = ReceiveSockData(s);
                        process += (uint)buf.Length;
                        fs.Write(buf, 0, buf.Length);

                    } while (process < size);
                }
                s.Disconnect(false);
            }
        }
        private static byte[] ReceiveSockData(Socket _s)
        {
            // _s.beginre
            byte[] buf = new byte[_s.ReceiveBufferSize];
            int receive = _s.Receive(buf, SocketFlags.None);
            byte[] result = new byte[receive];
            Array.Copy(buf, 0, result, 0, receive);
            return result;
        }
    }
}
