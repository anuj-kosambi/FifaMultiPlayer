using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FifaMulti_v1
{
    public partial class Form1 : Form
    {
        public static VirtualJoy m_vjoy = null;
        public static ToolStripLabel sendrecive_label;
        private TcpClient server_tcp_client = null;
        private Thread m_server = null;
        private TcpListener server = null;
        private static IntPtr _hookID = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        static NetworkStream stream = null;
        private static TcpClient _tcp_client = null;
        private TextBox[] keyTextBox;
        private Button[] keyButton;
        private static Keys CurrentKey = 0;
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sendrecive_label = new System.Windows.Forms.ToolStripLabel();
            // this.ipaddress_textbox.Enabled = false;
            toolStrip1.Items.Add(sendrecive_label);
            this.KeyPreview = true;
            this.ipaddress_textbox.Text = "127.0.0.1";
            m_vjoy = new VirtualJoy();
            m_vjoy.Initialize();
            m_vjoy.Reset();
            m_vjoy.Update(0);
            m_vjoy.Update(1);
            this.serverClose_button.Enabled = false;
            this.Disconnect.Enabled = false;
            new FifaControlls();
            BuildKeyMappingTab();
            _hookID = SetHook(_proc);
        }
        private void BuildKeyMappingTab()
        {
        
             keyTextBox= new TextBox[24];
             keyButton = new Button[24];
             GroupBox[] groupBox = new GroupBox[24];
            for(int i=0;i<24;i++)
                {
        
                    keyTextBox[i] = new TextBox();keyButton[i] = new Button();groupBox[i] = new GroupBox();
                    StringBuilder data=new StringBuilder(255);
                    GetPrivateProfileString("KeyMap", "Button_" + (i+1), "", data, 255, ".\\ButtonData.ini");
                    keyTextBox[i].Text = "" + data.ToString();
                    keyButton[i].Text = ((FifaControlls.Buttons)i+1).ToString();
                    
                    keyTextBox[i].MinimumSize =new Size(100,20);
                    keyButton[i].MinimumSize = new Size(0, 30);

                    keyTextBox[i].Anchor = AnchorStyles.Left; keyButton[i].Anchor = AnchorStyles.Right;
                    keyTextBox[i].Dock = DockStyle.Fill; keyButton[i].Dock = DockStyle.Right;

                    groupBox[i].Controls.Add(keyTextBox[i]);groupBox[i].Controls.Add(keyButton[i]);
                    groupBox[i].MinimumSize = new Size(100, 0);groupBox[i].MaximumSize = new Size(500, 60);
                    keyButton[i].Name = "keyButton_" + i;
                    keyTextBox[i].Name = "keyTextBox_" + i;
                    keyTextBox[i].ReadOnly = true;
                    keyTextBox[i].BackColor = Color.White; 

                    keyButton[i].Click += keyButtonClick;
                    grid.Controls.Add( groupBox[i]);
                    keyTextBox[i].Click += ((sender, e) => { keyTextBox[DecodeIndex(sender)].Text = "Press Any Key"; });
                    
                    keyTextBox[i].LostFocus+=((sender, e) =>
                        {if(keyTextBox[ DecodeIndex(sender)].Text =="Press Any Key")
                            keyTextBox[ DecodeIndex(sender)].Text = "";                         
                      });
                
                    keyTextBox[i].KeyDown+=(  (sender, e) =>{
                        keyTextBox[DecodeIndex(sender)].Text = CurrentKey.ToString();
                        WritePrivateProfileString("KeyMap", "Button_" + (DecodeIndex(sender)+1), "" + CurrentKey, ".\\ButtonData.ini");
                    });

                }
        }

        int  DecodeIndex(object sender)
        {
            int j;
            string name = ((Control)sender).Name.ToString();
            char[] deli = { '_' };
            Int32.TryParse(name.Split(deli, 2).Last(), out j);
           
            return j;
        }
        void keyButtonClick(object sender, EventArgs e)
        {
          
            keyTextBox[DecodeIndex(sender)].Text = CurrentKey.ToString();


        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tcp_client != null)
                _tcp_client.Close();

            if (stream != null)
                stream.Close();
            if (server != null)
                server.Stop();
            if (m_server != null && m_server.IsAlive)
                m_server.Abort();
            if(server_tcp_client!=null)
            server_tcp_client.Close();


        }
//Client Friend

        static TcpClient Connect(String server,Int32 port)
        {
            TcpClient client = null;
            try
            {
                 client = new TcpClient(server, port);
              
                
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                System.Diagnostics.Debug.WriteLine("SocketException: {0}", e);
            }

           
            return client;
        }

        static void SendData(TcpClient client, UInt32 message)
        {
            stream = client.GetStream();
            Byte[] data = BitConverter.GetBytes(message);
            stream.Write(data, 0, data.Length);
            sendrecive_label.Text = string.Join(" ",data.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
            System.Diagnostics.Debug.WriteLine("Sent: {0}", message);
            
      
                     
        }
    
    
        private void connect_button_Click(object sender, EventArgs e)
        {

            _tcp_client = Connect(ipaddress_textbox.Text, 1167);
            if (_tcp_client != null)
            {
                ipaddress_textbox.Enabled = false;
                makeserver_button.Enabled = false;
                this.connect_button.Enabled = false;
                this.Disconnect.Enabled = true;
                StatusBarLabel.Text = "Conneced Successfully on "+ipaddress_textbox.Text+ "...";
            }

        }
        private void Disconnect_Click(object sender, EventArgs e)
        {
           

            if (_tcp_client != null)
            {
                _tcp_client.Close();
                makeserver_button.Enabled = true;
                ipaddress_textbox.Enabled = true;
                this.connect_button.Enabled = true;
                this.Disconnect.Enabled = false;
                StatusBarLabel.Text = "Disconnected";

            }
            if (stream != null)
                stream.Close();

        }

      
//keystroks
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            UInt32 sendDataCode = 0x00;
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                CurrentKey = (Keys)vkCode;
                System.Diagnostics.Debug.WriteLine("heheh"+(Keys)vkCode);

                if (_tcp_client != null)
                {
                    FifaControlls.StatusRegister |= (UInt32)(1 << (FifaControlls.getSendCode[vkCode] - 1));
                    sendDataCode = (UInt32)FifaControlls.StatusRegister;

                
                        SendData(_tcp_client, sendDataCode);
                    
                    

                }
            }
            else if (nCode >= 0 && (wParam == (IntPtr)0x101))
                {

                    int vkCode = Marshal.ReadInt32(lParam);

                    System.Diagnostics.Debug.WriteLine((Keys)vkCode);

                    if (_tcp_client != null)
                    {

                        FifaControlls.StatusRegister &= (UInt32)~(1 << (FifaControlls.getSendCode[vkCode] - 1));
                        sendDataCode = (UInt32)FifaControlls.StatusRegister;

                        SendData(_tcp_client, sendDataCode);
                      

                    }
                } 
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #region DLLIMPORT


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // The two dll imports below will handle the window hiding.

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        /// <summary>
        
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);
        #endregion

//makeServer

        private void ServerThread()
        {
           
            try
            {
                BeginInvoke((Action)(() =>
                {
                    this.ipaddress_textbox.Enabled = false;
                    this.connect_button.Enabled = false;
                    this.Disconnect.Enabled = false;
                    this.makeserver_button.Enabled = false;
                    this.serverClose_button.Enabled = true;
                    TabController.Controls[1].Enabled = false;
                }
                ));
                Int32 port = 1167;
                IPAddress localAddr = IPAddress.Parse(ipaddress_textbox.Text);
                server = new TcpListener(localAddr, port);
                server.Start();
                Byte[] bytes = new Byte[4];
                String data =null;
                while (true)
                {
                    System.Diagnostics.Debug.WriteLine("Waiting for a connection... ");
                    sendrecive_label.Text = "Waiting for a connection... ";
                     server_tcp_client = server.AcceptTcpClient();
                    System.Diagnostics.Debug.WriteLine("Connected!");
                    sendrecive_label.Text = ((IPEndPoint)server_tcp_client.Client.RemoteEndPoint).Address.ToString();
                    System.Diagnostics.Debug.WriteLine(((IPEndPoint)server_tcp_client.Client.RemoteEndPoint).Address.ToString());
                    data = null;
                    NetworkStream stream = server_tcp_client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.


                        data = BitConverter.ToUInt32(bytes, 0).ToString();
                        if (data == "0")
                        {
                            m_vjoy.Reset();
                            m_vjoy.Update(0);
                        }
                        else
                        {
                           // FifaControlls.StatusRegister |= 0x01;
                            UInt32 newdata = BitConverter.ToUInt32(bytes, 0);
                            FifaControlls.updateFifaControls(newdata);
                        }
                        sendrecive_label.Text = "Recived:" + String.Join(" ", bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0'))) + "/" + data;
           


                    }
                
                    // Shutdown and end connection
                    server_tcp_client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {

                server.Stop();
            }


        }
        
        private void makeserver_button_Click(object sender, EventArgs _e)
        {
            try
            {
                m_server = new Thread(new ThreadStart(ServerThread));
                m_server.Start();
                

                StatusBarLabel.Text = "Server Started!!!";
            }
            catch
            {
            }

        }

        private void serverClose_button_Click(object sender, EventArgs e)
        {
            try
            {

                this.ipaddress_textbox.Enabled = true;
                this.connect_button.Enabled = true;
                this.Disconnect.Enabled = false;
                this.serverClose_button.Enabled = false;
                this.makeserver_button.Enabled = true;
                server.Stop();
                m_server.Abort();

                StatusBarLabel.Text = "Server Closed!!!";
                sendrecive_label.Text = "";
            }
            catch
            {
            }

        }

       



    }

}
