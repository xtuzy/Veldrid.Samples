using CoreAnimation;
using Foundation;

namespace Veldrid.Maui.Controls.Platforms.iOS
{
    /// <summary>
    /// https://stackoverflow.com/questions/61594608/ios-equivalent-of-androids-valueanimator
    /// </summary>
    internal class ValueAnimator
    {
        CADisplayLink displayLink;
        Action Action { get; set; }
        public bool isRunning = false;
        public void set(Action action)
        {
            Action = action;
        }

        public void start()
        {
            isRunning = true;
            displayLink = CADisplayLink.Create(update);
            displayLink?.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
        }

        private void update()
        {
            if (isRunning)
            {
                Action?.Invoke();
            }
        }

        public void cancel()
        {
            isRunning = false;
            this.displayLink?.RemoveFromRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
            this.displayLink = null;
        }
    }
}
