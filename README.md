# nfo
ServerQuery utilizing the tcp-layer to query general teamspeak server informations such as the server info, clients online, channels and handles the downloading of icons from the server.

Utilizes byte manipulation to handle incoming messages, if you are interested you might want to check out the parser.cs

## Usage

```csharp
using (ServerQuery ts3query = new ServerQuery(ip, port, queryport, "C:\ICONS"))
{
  if (ts3query.QuerySuccess)
  {
    //Serialize or use for whatever
    ServerInfo _serverinfo = ts3query._serverinfo;
  }
  else
  {
    foreach(var err in ts3query._errors)
    {
      Console.WriteLine("Error Id {0} - {1}", err.id, err.msg);                                 
    }
  }
}
```

https://arpsel.ws
