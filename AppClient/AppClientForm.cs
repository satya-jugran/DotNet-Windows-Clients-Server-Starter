using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppClient
{
    public partial class AppClientForm : Form
    {
        Socket socket;
        public AppClientForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (socket == null || !socket.Connected)
            {
                var address = IPAddress.Parse(txtIPAddress.Text.Trim());
                var endPoint = new IPEndPoint(address, Int32.Parse(txtPort.Text.Trim()));
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(endPoint);

                if (socket == null)
                {
                    MessageBox.Show("Connection Failed");
                }
            }

            

            var buffer = new byte[1024 * 4];

            var bytesRec = socket.Receive(buffer);

            var message = Encoding.ASCII.GetString(buffer, 0, bytesRec);
            txtLogs.Text += "[Received] : " + message;
            txtLogs.Text += Environment.NewLine;

            buffer = new byte[1024 * 4];

            var currentDateTime = DateTime.Now;
            var sendMessage = String.Format("{0} | {1} | {2} | {3}",
                Environment.UserName,
                currentDateTime.ToShortDateString(),
                currentDateTime.ToShortTimeString(),
                TimeZone.CurrentTimeZone);

            sendMessage += "<EOF>";
            using (var sw = new StreamWriter(new MemoryStream(buffer)))
            {
                sw.Write(sendMessage);
            }

            txtLogs.Text += "[Sent] : " + sendMessage;
            txtLogs.Text += Environment.NewLine;
            socket.Send(buffer);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
