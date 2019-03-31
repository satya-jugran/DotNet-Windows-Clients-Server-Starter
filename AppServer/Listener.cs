using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AppServer
{
    class Listener
    {
        Socket sc;
        IPAddress ipAddress;
        int port;
        StringBuilder logs;
        public Listener(string ipAddress, int port, StringBuilder logs)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.logs = logs;
        }

        public void Listen()
        {
            var endPoint = new IPEndPoint(this.ipAddress, this.port);
            sc = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (sc == null)
            {
                return;
            }

            try
            {
                sc.Bind(endPoint);
                sc.Listen(10);
                
                while (true)
                {
                    Socket asc = sc.Accept();
                    var buffer = new byte[1024 * 4];
                    using (var sw = new StreamWriter(new MemoryStream(buffer)))
                    {
                        sw.Write("HELLO?");
                        logs.Append("[Sent] : HELLO?");
                        logs.Append(Environment.NewLine);
                    }

                    asc.Send(buffer);

                    while (true)
                    {
                        buffer = new byte[1024 * 4];
                        var bytesRec = asc.Receive(buffer);

                        var text = Encoding.ASCII.GetString(buffer, 0 , bytesRec );
                        logs.Append("[Received] : " + text);
                        logs.Append(Environment.NewLine);

                        if (text.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            if (sc.Connected)
            {
                sc.Disconnect(true);
            }
        }
    }
}
