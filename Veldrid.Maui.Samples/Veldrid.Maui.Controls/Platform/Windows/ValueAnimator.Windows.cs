using Microsoft.UI.Xaml.Media;
using System.Diagnostics;

namespace Veldrid.Maui.Controls.Platforms.Windows
{
    /// <summary>
    /// https://stackoverflow.com/questions/61594608/ios-equivalent-of-androids-valueanimator
    /// </summary>
    internal class ValueAnimator
    {
        Action Action { get; set; }
        public bool isRunning = false;
        public void set(Action action)
        {
            Action = action;
        }

        public void start()
        {
            isRunning = true;
            CompositionTarget.Rendering += RenderLoop;
        }

        private void RenderLoop(object sender, object e)
        {
            update();
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
            CompositionTarget.Rendering -= RenderLoop;
        }
    }
}
