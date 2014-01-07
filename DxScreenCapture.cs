using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX.Direct3D9;

namespace FifaMulti_v1
{
    public class DxScreenCapture
    {
        Device d;

        public DxScreenCapture()
        {
            PresentParameters present_params = new PresentParameters();
            present_params.Windowed = true;
            present_params.BackBufferFormat = Format.A8R8G8B8;
        
            present_params.SwapEffect = SwapEffect.Discard;
            present_params.EnableAutoDepthStencil = false;
            present_params.MultisampleQuality = 0;
            
            d = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, present_params);
        }

        public Surface CaptureScreen()
        {

            Surface s = Surface.CreateOffscreenPlain(d,Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);

            d.GetFrontBufferData(0, s);
            
            return s;
        }
    }
}
