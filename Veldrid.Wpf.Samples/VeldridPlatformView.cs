using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Veldrid.Wpf.Samples
{
    public class VeldridPlatformView : Win32HwndControl
    {
        public IntPtr NativeHwnd => this.Hwnd;
        public Action SizeChanged;
        public Action Loaded;
        public Action Unloaded;
        public VeldridPlatformView()
        {
        }

        protected override void Initialize()
        {
            Loaded?.Invoke();
        }

        protected override void Resized()
        {
            SizeChanged?.Invoke();
        }

        protected override void Uninitialize()
        {
            Unloaded?.Invoke();
        }

        public double CompositionScaleX =>GetDpiScale();
        public double CompositionScaleY =>GetDpiScale();
        private double GetDpiScale()
        {
            PresentationSource source = PresentationSource.FromVisual(this);
            if(source != null ) 
                return source.CompositionTarget.TransformToDevice.M11;
            else
                return 1;
        }
    }
}
