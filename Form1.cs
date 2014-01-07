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
using SlimDX;
using SlimDX.Direct3D9;
namespace FifaMulti_v1
{
    public partial class Form1 : Form
    {
        #region Variable Stuff
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
        private IntPtr hwnd;
        private Thread picSendThread;
        private DxScreenCapture _dxScreenCapture;
        private FifaDisplay fifaDisplayInstance;
        #endregion
        public Form1()
        {
            InitializeComponent();
          //  this.WindowState = FormWindowState.Maximized;
            

        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            connect_button.EnabledChanged += ((a, b) => { Disconnect.Enabled = !connect_button.Enabled; });
            

            sendrecive_label = new System.Windows.Forms.ToolStripLabel();
            picSendThread = new Thread(new ThreadStart(picSendServer_thread));
            picSendThread.Name = "Pic Send Thread";
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
            _dxScreenCapture = new DxScreenCapture();
            
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
            if (picSendThread != null && picSendThread.IsAlive)
                picSendThread.Abort();
            Application.ExitThread();
            Application.Exit();

        }
        #region CLientCode

        static TcpClient Connect(String server,Int32 port)
        {
            TcpClient client = null;
            try
            {
                 client = new TcpClient(server, port);
              
                
            }
            catch (ArgumentNullException e)
            {
               // System.Diagnostics.Debug.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
               // System.Diagnostics.Debug.WriteLine("SocketException: {0}", e);
            }

           
            return client;
        }

        static void SendData(TcpClient client, UInt32 message)
        {
            stream = client.GetStream();
            Byte[] data = BitConverter.GetBytes(message);
            stream.Write(data, 0, data.Length);
            
            sendrecive_label.Text = string.Join(" ",data.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray());
            System.Diagnostics.Debug.WriteLine("Sent: {0}", message.ToString());
      
                     
        }
    
        private void connect_button_Click(object sender, EventArgs e)
        {

            _tcp_client = Connect(ipaddress_textbox.Text, 1167);
            if (_tcp_client != null)
            {
                ipaddress_textbox.Enabled = false;
                makeserver_button.Enabled = false;
               this.connect_button.Enabled = false;
           
                StatusBarLabel.Text = "Conneced Successfully on "+ipaddress_textbox.Text+ "...";
                fifaDisplayInstance= new FifaDisplay(this);
                fifaDisplayInstance.Show();
                IconLabel.ForeColor = Color.Yellow;
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
          
                StatusBarLabel.Text = "Disconnected";
                IconLabel.ForeColor = Color.Black;
                if (fifaDisplayInstance.imageReciving_thread != null && fifaDisplayInstance.imageReciving_thread.IsAlive)
                {
                    fifaDisplayInstance.imageReciving_thread.Abort();
                }

            }
            if (stream != null)
                stream.Close();
            
        }

        #endregion

        #region keystrok and dll
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            UInt32 sendDataCode = 0x00;
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                CurrentKey = (Keys)vkCode;
              

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
                
                while (true )
                {
                    BeginInvoke((Action)(() =>{IconLabel.ForeColor = Color.Yellow;}));
                    System.Diagnostics.Debug.WriteLine("Waiting for a connection... ");
                    sendrecive_label.Text = "Waiting for a connection... ";
                     server_tcp_client = server.AcceptTcpClient();
                    System.Diagnostics.Debug.WriteLine("Connected!");

                    BeginInvoke((Action)(() => { IconLabel.ForeColor = Color.Green; }));
                    if (!picSendThread.IsAlive)
                    {
                        picSendThread = new Thread(new ThreadStart(picSendServer_thread));
                        picSendThread.Start();
                    }
                    sendrecive_label.Text ="Connceted to :"+ ((IPEndPoint)server_tcp_client.Client.RemoteEndPoint).Address.ToString();
                    System.Diagnostics.Debug.WriteLine(((IPEndPoint)server_tcp_client.Client.RemoteEndPoint).Address.ToString());
                    data = null;
                    NetworkStream stream = server_tcp_client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        
                        data = BitConverter.ToUInt32(bytes, 0).ToString();
                        if (data == "0")
                        {
                            m_vjoy.Reset();
                            m_vjoy.Update(0);
                        }
                        else
                        {
                            UInt32 newdata = BitConverter.ToUInt32(bytes, 0);
                            FifaControlls.updateFifaControls(newdata);
                        }
                        sendrecive_label.Text = "Recived:" + string.Join(" ", bytes.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).ToArray()) + "/" + data;
           


                    }
                
                 
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
        private void picSendServer_thread()
        {
            hwnd = (IntPtr)FindWindow(null,"Idea Net Setter");
            if (hwnd == IntPtr.Zero)
                MessageBox.Show("Please Start Fifa Application...!!!");
            while (hwnd == IntPtr.Zero)
            {
                Thread.Sleep(2000);
            
            }
            
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
          


            while (server_tcp_client!=null && server_tcp_client.Connected && hwnd!=IntPtr.Zero)
            {
                try
                {
                    // Set the TcpListener on port 42127.
                    Int32 port = 12316;
                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                    IPEndPoint sending_end_point = new IPEndPoint(localAddr, port);
                    //     sending_socket.SendBufferSize = 1024*504;
                    byte[] send_buffer = new byte[0];
                    using (MemoryStream stream = new MemoryStream())
                    {

                        Bitmap bmp = new Bitmap(Surface.ToStream(_dxScreenCapture.CaptureScreen(), ImageFileFormat.Png));
                        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        
                        
                        send_buffer = stream.ToArray();
                        int count=0,read=int.MaxValue;
                        byte[] readbuffer=new byte[60*1024];
                        while (read <60*1024)
                        {
                            read=stream.Read(readbuffer, count, readbuffer.Length);
                            count = read;
                            sending_socket.SendTo(readbuffer, sending_end_point);
                        }
                        stream.Close();

                    }
                    // Start listening for client requests.
                    StatusBarLabel.Text = "Sending "+(send_buffer.Length/1024).ToString()+" kB |";
                  //  sending_socket.SendTo(send_buffer, sending_end_point);
                }
                catch (Exception e)
                {
                    sendrecive_label.Text ="Problem :"+ e.Message.ToString();
                }
              
                Thread.Sleep(50);
            }
            sendrecive_label.Text += "...Server Image Sender CLose..!!";

        }
        private Bitmap getCaptureWindow()
        {
            #region previous code 
            /* if (hwnd == IntPtr.Zero)
               return null;
        //   RECT rc;
         
          // GetWindowRect(hwnd, out rc);
          
           Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
           Graphics gfxBmp = Graphics.FromImage(bmp);
           IntPtr hdcBitmap = gfxBmp.GetHdc();
            
           PrintWindow(hwnd, hdcBitmap, 0);

           gfxBmp.ReleaseHdc(hdcBitmap);
           gfxBmp.Dispose();
             */
            /*
            IntPtr hdcSrc = GetWindowDC(hwnd);

            RECT windowRect = new RECT();
            GetWindowRect(hwnd, out windowRect);

            int width = windowRect.Width;
            int height = windowRect.Height;

            IntPtr hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);

            IntPtr hOld = Gdi32.SelectObject(hdcDest, hBitmap);
            //13369376
            Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, 13369376);
            Gdi32.SelectObject(hdcDest, hOld);
            Gdi32.DeleteDC(hdcDest);
            ReleaseDC(hwnd, hdcSrc);*/

            #endregion

            return new Bitmap(Surface.ToStream(_dxScreenCapture.CaptureScreen(),ImageFileFormat.Png)); ;
        }
     
        private void makeserver_button_Click(object sender, EventArgs _e)
        {
            try
            {
                m_server = new Thread(new ThreadStart(ServerThread));
                m_server.Name = "MainServer Thread";
                m_server.Start();
                IconLabel.ForeColor = Color.Yellow;

                StatusBarLabel.Text = "Server is Running>>";
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
                IconLabel.ForeColor = Color.Red;
                sendrecive_label.Text = "";
            }
            catch
            {
            }

        }
        private void server_tab_Click(object sender, EventArgs e)
        {

        }
        public TcpClient getServerTCPClient()
        {
            return _tcp_client;
        }

        #region PicDLLImport

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string sClass, string sWindow);
        
    [DllImport("user32.dll",EntryPoint="GetWindowDC")]
    public static extern IntPtr GetWindowDC(IntPtr ptr);
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,int nWidth, int nHeight);
         [DllImport("user32.dll")]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        
        #endregion



    }
    public class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }
}
