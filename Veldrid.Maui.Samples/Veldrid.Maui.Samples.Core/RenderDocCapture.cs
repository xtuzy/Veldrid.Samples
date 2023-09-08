namespace Veldrid.Maui.Samples.Core
{
    public class RenderDocCapture
    {
        static RenderDoc rd;
        public static void Init()
        {
            if (rd == null)
            {
                RenderDoc.Load(out RenderDoc Rd); // Load RenderDoc from the default locations.
                rd = Rd;
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "renderdocResult");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                rd.SetCaptureSavePath(folder); // Save captures into a particular folder.
            }
        }

        public static void StartCapture()
        {
            rd?.TriggerCapture(); // Capture the next frame.
            rd?.StartFrameCapture(); // Start capturing.
        }

        static bool haveLunchUI = false;
        public static void EndCapture()
        {
            rd?.EndFrameCapture(); // Stop capturing and save.
            if (haveLunchUI == false && rd != null)
            {
                rd?.LaunchReplayUI(); // Launch the replay UI, with previous captures already loaded in.
                haveLunchUI = true;
            }
        }
    }
}
