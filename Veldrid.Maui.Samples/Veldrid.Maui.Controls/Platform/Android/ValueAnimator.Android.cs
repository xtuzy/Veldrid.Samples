using System.Diagnostics;

namespace Veldrid.Maui.Controls.Platforms.Android
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
            Task.Factory.StartNew(() => Loop(), TaskCreationOptions.LongRunning);
        }

        int frameTime = 1000 / 60;//每秒60帧
        private void Loop()
        {
            while (isRunning)
            {
                try
                {
                    Thread.Sleep(frameTime);

                    update();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Encountered an error while rendering: " + e);
                    //throw;
                }
            }
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
        }
    }
}
