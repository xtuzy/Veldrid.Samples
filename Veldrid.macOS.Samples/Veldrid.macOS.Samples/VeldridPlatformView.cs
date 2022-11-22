using CoreAnimation;
using CoreGraphics;
namespace Veldrid.macOS.Samples
{
    public class VeldridPlatformView : NSView
    {
        public VeldridPlatformView()
        {
            //this.Layer.MasksToBounds = true; // IMPORTANT

            this.PostsFrameChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.FrameChangedNotification, FrameChanged);
        }
        
        /// <summary>
        /// 遇到了NSView的Frame只能在UI线程调用的Exception, 因此自定义一个
        /// </summary>
        internal CGSize CorrectedFrame = CGSize.Empty;
        /// <summary>
        /// 构建GraphicsDevice时大小不能为0, Maui会调用此方法计算UIView的大小,因此在该方法中判断何时大小不为0
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public void FrameChanged(NSNotification notification)
        {
            var result = this.Frame.Size;
            if (this.firstTimeLoad && result.Width > 0 && result.Height > 0)//初次有大小
            {
                CorrectedFrame = result;
                ViewLoaded?.Invoke();
                firstTimeLoad = false;
            }
            else if (result != CorrectedFrame)//大小更新
            {
                CorrectedFrame = result;
                SizeChanged?.Invoke();
            }
        }

        public Action SizeChanged;
        public Action ViewLoaded;
        public Action ViewRemoved;
        bool firstTimeLoad = true;
        public override void ViewDidMoveToWindow()
        {
            base.ViewDidMoveToWindow();
            /*if (firstTimeLoad)
            {
                ViewLoaded?.Invoke();
                firstTimeLoad = false;
            }
            else*/
            if (!firstTimeLoad)
            {
                ViewRemoved?.Invoke();
                firstTimeLoad = true;
            }
        }
    }
}
