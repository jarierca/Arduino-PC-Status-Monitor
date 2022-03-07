using Newtonsoft.Json;
using System.IO.Ports;
using PC_Monitor_GUI;
using System.Diagnostics;
using System.Windows;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;

namespace Form
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        SerialPort port = null;
        Thread thr = null;
        public Form1()
        {
            InitializeComponent();
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipText = "Closing App";
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }
        
        private void moreInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/jarierca/Arduino-PC-Status-Monitor",
                UseShellExecute = true
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InfoText("Starting Connection");

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            port = new SerialPort("COM" + comNum.Value, 9600);
            port.DataBits = 8;
            port.DtrEnable = true;
            port.RtsEnable = true;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.Parity = Parity.None;

            try
            {
                port.Open();

                thr = new(StartConnection);
                thr.Start();
            }
            catch {
                InfoTxt.Text = "Error assigning COM port";  
            }         
        }

        private void StartConnection()
        {
            while (true)
            {
                PC pc = SysInfo.GetSystemInfo();
                String json = JsonConvert.SerializeObject(pc);

                try { 
                    port.Write("<" + json + ">");
                    InfoText(pc.ToString());
                }catch {
                    InfoText("Connection Close");
                }
                
            }      
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            port.Close();
            thr.Interrupt();

            InfoTxt.Text = "Connection Close";
        }

        private void InfoText(String message)
        {
            try
            {
                Action action = () => InfoTxt.Text = message;
                this.Invoke(action);
            }
            catch {}   
        }  
        
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                //notifyIcon1.Icon = SystemIcons.Application;
                notifyIcon1.BalloonTipText = "The App is running in a second plane";
                notifyIcon1.ShowBalloonTip(1000);
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.Show();
                //notifyIcon1.Icon = SystemIcons.Application;
            }
        }

        public static void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("PC_Status_Monitor", "\"" + Application.ExecutablePath + "\"");
            }
        }

        public static void RemoveApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue("PC_Status_Monitor", false);
            }
        }

        private void wStartup_CheckStateChanged(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (Convert.ToBoolean(wStartup.CheckState))
                {
                    key.SetValue("PC_Status_Monitor", "\"" + Application.ExecutablePath + "\"");
                }
                else if (!Convert.ToBoolean(wStartup.CheckState))
                {
                    key.DeleteValue("PC_Status_Monitor", false);
                }
            }
        }

        private void wStartup_Click(object sender, EventArgs e)
        {
            //Run On Windows Startup
            if (Convert.ToBoolean(wStartup.CheckState))
            {
                wStartup.Checked = false;
            }
            else if (!Convert.ToBoolean(wStartup.CheckState))
            {
                wStartup.Checked = true;
            }
        }
    }
}