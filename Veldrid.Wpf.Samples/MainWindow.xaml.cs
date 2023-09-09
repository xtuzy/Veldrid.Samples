using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Veldrid.Wpf.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VeldridPlatformView platformView;
        private VeldridPlatformInterface platformInterface { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var buttonContainer = new WrapPanel()
            {
                Children =
                {
                    new Button(){ Content = "切换Backend到Direct3D11" },
                    new Button(){ Content = "ReleaseResource" },
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.GettingStarted.GettingStartedDrawable)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_TextureUnits)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.Headless.HeaderlessTextures)},
                    new Button(){ Content = nameof(Veldrid.Maui.Samples.Core.Headless.HeadlessHelloTriangle)},
                }
            };
            Vk.VkDeviceMemoryManagerCustomOption.MinDedicatedAllocationSizeDynamic = 1024 * 1024 * 1;
            Vk.VkDeviceMemoryManagerCustomOption.PersistentMappedChunkSize = 1024 * 1024 * 8;
            platformView = new VeldridPlatformView() { Width = 500, Height = 500 };
            //platformInterface = new VeldridPlatformInterface(platformView, GraphicsBackend.Direct3D11);
            
            var camera = new Maui.Controls.Base.SimpleCamera();
            foreach (var view in buttonContainer.Children)
            {
                if (view is Button)
                {
                    var button = view as Button;
                    button.Click += (s, e) =>
                    {
                        platformInterface?.Drawable?.Dispose();
                        Debug.WriteLine("已经释放Drawable");
                        if (button.Content == nameof(Veldrid.Maui.Samples.Core.GettingStarted.GettingStartedDrawable))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.GettingStarted.GettingStartedDrawable();
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
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle();
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Textures();
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_TextureUnits))
                            platformInterface.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_TextureUnits();
                        else if (button.Content == "ReleaseResource")
                            ;
                        else if ((button.Content as string).Contains("Backend"))
                        {
                            platformInterface?.Dispose();
                            if ((button.Content as string).Contains("Direct3D11"))
                            {
                                platformInterface = new VeldridPlatformInterface(platformView, GraphicsBackend.Direct3D11);
                                button.Content = "切换Backend到Vulkan";
                            }
                            else
                            {
                                platformInterface = new VeldridPlatformInterface(platformView, GraphicsBackend.Vulkan);
                                button.Content = "切换Backend到Direct3D11";
                            }
                            platformInterface.OnLoaded();
                        }
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.Headless.HeaderlessTextures))
                        {
                            var headerless = new Veldrid.Maui.Samples.Core.Headless.HeaderlessTextures();
                        }
                        else if (button.Content == nameof(Veldrid.Maui.Samples.Core.Headless.HeadlessHelloTriangle))
                        {
                            var headerless = new Veldrid.Maui.Samples.Core.Headless.HeadlessHelloTriangle();
                        }
                    };
                }
            }

            layout.Children.Add(buttonContainer);
            layout.Children.Add(platformView);
            Grid.SetRow(buttonContainer, 0);
            Grid.SetRow(platformView, 1);

            Maui.Samples.Core.RenderDocCapture.Init();
        }
    }
}
