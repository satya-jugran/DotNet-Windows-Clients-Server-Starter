using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AppServer
{
    public partial class AppServerForm : Form
    {
        CancellationToken cancellationToken;
        Listener listener;
        Task task;
        StringBuilder logs;
        public AppServerForm()
        {
            InitializeComponent();
        }

        private void AppServerForm_Load(object sender, EventArgs e)
        {
            txtIPAddress.Text = IPAddress.Loopback.ToString();
            cancellationToken = new CancellationToken();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            txtPort.Enabled = false;
            btnStop.Enabled = true;
            btnConnect.Enabled = false;
            logs = new StringBuilder();
            try
            {
                listener = new Listener(txtIPAddress.Text.Trim(), Int32.Parse(txtPort.Text.Trim()), logs);

                task = new Task(() =>
                {
                    listener.Listen();
                }, cancellationToken);

                task.Start();

                timer1.Interval = 1000;
                timer1.Start();

                toolStripStatus.Text = "Server started successfully. Listening now...";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                toolStripStatus.Text = "Error";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            txtPort.Enabled = true;
            btnStop.Enabled = false;
            btnConnect.Enabled = true;

            if (listener != null)
            {
                listener.Stop();
            }

            toolStripStatus.Text = "Server stopped.";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtLogs.Text += logs.ToString();
            logs.Clear();
        }
    }
}
