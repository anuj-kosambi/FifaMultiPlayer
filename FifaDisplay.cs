using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.IO;
using SlimDX;
namespace FifaMulti_v1
{
    public partial class FifaDisplay : Form
    {
        public Thread imageReciving_thread = null;
        Form1 mainForm = null;
       
        UdpClient listener = new UdpClient(12316);
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 12316);
        byte[] receive_byte_array;
        public FifaDisplay(Form1 f)
        {
            
            InitializeComponent();
            imageReciving_thread = new Thread(new ThreadStart(display_thread));
            imageReciving_thread.Name = "ImageRecivingThread";
            imageReciving_thread.Start();
            mainForm = f;
            //this.FormBorderStyle = FormBorderStyle.None;
        }

        private void display_thread()
        {

          while(true)//mainForm.getServerTCPClient()!=null &&   mainForm.getServerTCPClient().Connected)
          {
              BeginInvoke((Action)(()=>{ mainForm.IconLabel.ForeColor = Color.Green;}));
              try
              {
               
                      receive_byte_array = listener.Receive(ref groupEP);
                
                 //   MemoryStream ms = new MemoryStream(receive_byte_array);
                  //    Image returnImage = Image.FromStream(ms);
                  //    this.pictureBox1.Image = returnImage;
                      mainForm.StatusBarLabel.Text = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                
              }
              catch (Exception e)
              {
                  MessageBox.Show("Problem:"+e.Message);
                  listener.Close();
              }
            //  String received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
           //./   System.Diagnostics.Debug.Write(received_data);
              
          }
          Form1.sendrecive_label.Text = "Client Display Server Exit";
          listener.Close();
         
         
        }

  
        private void FifaDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.IconLabel.ForeColor = Color.Red;
            if (imageReciving_thread != null && imageReciving_thread.IsAlive)
                imageReciving_thread.Abort();
            if (listener != null)
                listener.Close();
        }
       
    }
}
