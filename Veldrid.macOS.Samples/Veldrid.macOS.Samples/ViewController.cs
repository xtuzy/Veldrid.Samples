using CoreVideo;
using ObjCRuntime;

namespace Veldrid.macOS.Samples
{
    public partial class ViewController : NSViewController
    {
        protected ViewController(NativeHandle handle) : base(handle)
        {
            // This constructor is required if the view controller is loaded from a xib or a storyboard.
            // Do not put any initialization here, use ViewDidLoad instead.

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            var rootLayout = new NSStackView() { Orientation = NSUserInterfaceLayoutOrientation.Vertical};
            this.View = rootLayout;
            var buttonContainer = new NSStackView() { Orientation = NSUserInterfaceLayoutOrientation.Horizontal };
            rootLayout.AddView(buttonContainer, NSStackViewGravity.Center);
            buttonContainer.AddView(new NSButton() { Title = nameof(Veldrid.Maui.Samples.GettingStartedApplication) }, NSStackViewGravity.Center);
            buttonContainer.AddView(new NSButton() { Title = nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication) }, NSStackViewGravity.Center);
            buttonContainer.AddView(new NSButton() { Title = nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication) }, NSStackViewGravity.Center);
            buttonContainer.AddView(new NSButton() { Title = nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication) }, NSStackViewGravity.Center);
            buttonContainer.AddView(new NSButton() { Title = nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication) }, NSStackViewGravity.Center);
            buttonContainer.AddView(new NSButton() { Title = nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication) }, NSStackViewGravity.Center);
            var platformView = new VeldridPlatformView();
            var platformInterface = new VeldridPlatformInterface(platformView, GraphicsBackend.OpenGL);
            foreach (var view in buttonContainer.Subviews)
            {
                if (view is NSButton)
                {
                    var button = view as NSButton;
                    button.Activated += (s, e) =>
                    {
                        if (button.Title == nameof(Veldrid.Maui.Samples.GettingStartedApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.GettingStartedApplication();
                        else if (button.Title == nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication();
                        else if (button.Title == nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication();
                        else if (button.Title == nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication();//bug
                        else if (button.Title == nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.Instancing.InstancingApplication();
                        else if (button.Title == nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication();
                    };
                }
            }
            rootLayout.AddView(platformView,NSStackViewGravity.Center);
        }

        public override void ViewDidLayout()
        {
            base.ViewDidLayout();
        }

        public override NSObject RepresentedObject
        {
            get => base.RepresentedObject;
            set
            {
                base.RepresentedObject = value;

                // Update the view, if already loaded.
            }
        }
    }
}