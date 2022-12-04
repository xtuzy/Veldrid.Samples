using System.Windows;
using System.Windows.Controls;

namespace Veldrid.Wpf.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var buttonContainer = new WrapPanel()
            {
                Children =
                {
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.GettingStartedApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable)},
                }
            };
            var platformView = new VeldridPlatformView();
            var platformInterface = new VeldridPlatformInterface(platformView);
            var camera = new Maui.Controls.Base.SimpleCamera();
            foreach (var view in buttonContainer.Children)
            {
                if (view is Button)
                {
                    var button = view as Button;
                    button.Click += (s, e) =>
                    {
                        if (button.Content == nameof(Veldrid.Maui.Samples.GettingStartedApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.GettingStartedApplication();
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication();
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication(camera);
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication();//bug
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.Instancing.InstancingApplication(camera);
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication(camera);
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable();
                    };
                }
            }

            layout.Children.Add(buttonContainer);
            layout.Children.Add(platformView);
            Grid.SetRow(buttonContainer, 0);
            Grid.SetRow(platformView, 1);
        }
    }
}
