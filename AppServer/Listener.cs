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
        Socket clientSocket;
        IPAddress ipAddress;
        int port;
        StringBuilder logs;
        List<Task> tasks = new List<Task>();
        bool stopListener = false;
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

                
                while (!stopListener)
                {
                    clientSocket = sc.Accept(); // Waiting for client connection

                    var action = new Action<object>((socketObject) => {
                        var asc = (Socket)socketObject;
                        var buffer = new byte[1024 * 4];
                        using (var sw = new StreamWriter(new MemoryStream(buffer)))
                        {
                            logs.Append(Environment.NewLine);
                            sw.Write("HELLO?");
                            logs.Append("[Sent] : HELLO?");
                            logs.Append(Environment.NewLine);
                        }

                        asc.Send(buffer);

                        while (true)
                        {
                            buffer = new byte[1024 * 4];
                            var bytesRec = asc.Receive(buffer); // Waiting for connected client's data

                            var text = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                            logs.Append("[Received] : " + text);
                            if (text.IndexOf("<EOF>") > -1)
                            {
                                logs.Append(Environment.NewLine);
                                break;
                            }
                        }
                    });

                    Task t = new Task(action, clientSocket);
                    tasks.Add(t);
                    t.Start();
                }

                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            if (sc != null)
            {
                if (sc.Connected)
                {
                    sc.Disconnect(false);
                }
                sc.Close();
            }
            if (clientSocket != null)
            {
                if (clientSocket.Connected)
                {
                    clientSocket.Disconnect(false);
                }
                clientSocket.Close();
            }
            stopListener = true;
        }
    }
}
