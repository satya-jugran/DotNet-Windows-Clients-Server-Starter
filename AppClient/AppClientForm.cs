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
            if (txtIPAddress.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Server IP Address can not be blank", this.Text);
                return;
            }
            IPAddress serverIP = null;
            if (!IPAddress.TryParse(txtIPAddress.Text.Trim(), out serverIP))
            {
                MessageBox.Show("Server IP Address format is not valid", this.Text);
                return;
            }

            
            // serverIP = Dns.GetHostEntry(string.Empty).AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork && a.ToString() == serverIP.ToString()).FirstOrDefault();

            if (serverIP == null)
            {
                MessageBox.Show("Not a valid", this.Text);
                return;
            }

            if (txtPort.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Server Port can not be blank", this.Text);
                return;
            }
            int serverPort = 12345;
            if (!Int32.TryParse(txtPort.Text.Trim(), out serverPort))
            {
                MessageBox.Show("Server Port format is not valid", this.Text);
                return;
            }
            try
            {
                if (socket == null || !socket.Connected)
                {
                    var address = serverIP;
                    var endPoint = new IPEndPoint(address, Int32.Parse(txtPort.Text.Trim()));
                    socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    var connectResult = socket.BeginConnect(endPoint, null, null);
                    var resultPostWait = connectResult.AsyncWaitHandle.WaitOne(5000, true);

                    // socket.Connect(endPoint);

                    if (socket == null || !socket.Connected)
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
                var sendMessage = String.Format("{0} | {1} | {2} | {3} ",
                    Environment.UserName,
                    currentDateTime.ToShortDateString(),
                    currentDateTime.ToLongTimeString(),
                    TimeZone.CurrentTimeZone.StandardName);

                sendMessage += "<EOF>";
                using (var sw = new StreamWriter(new MemoryStream(buffer)))
                {
                    sw.Write(sendMessage);
                }

                txtLogs.Text += "[Sent] : " + sendMessage;
                txtLogs.Text += Environment.NewLine;
                socket.Send(buffer);

                socket.Shutdown(SocketShutdown.Both);
                socket.Disconnect(false);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server not running", this.Text);
            }
            
        }

        private void AppClientForm_Load(object sender, EventArgs e)
        {

        }

        private void txtIPAddress_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
