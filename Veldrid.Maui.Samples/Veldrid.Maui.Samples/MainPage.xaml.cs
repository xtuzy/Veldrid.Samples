using Veldrid.Maui.Controls;
using Veldrid.Maui.Controls.Base;

namespace Veldrid.Maui.Samples
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            AssimpNet.Platform.Init();
#endif
            var buttonContainer = new HorizontalStackLayout()
            {
                Children =
                {
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.GettingStarted.GettingStartedDrawable)},
                    new Button(){ Text =nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication)},
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication)},
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication)},
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication)},
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication)},
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable)},
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle_ElementBufferObject) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_InsAndOuts) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_Uniform) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_MoreAttributes) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_ApplyingTextures) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_TextureUnits) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Transformations_InPractice) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_Going3D) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_More3D) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_MoreCubes) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Camera_LookAt) },
                    new Button(){ Text = nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Camera_WalkAround) },
                    new Button(){ Text = "Remove"},
                }
            };
            var platformView = new VeldridView() { };
            platformView.AutoReDraw = true;
#if IOS || MACCATALYST
            platformView.Backend = GraphicsBackend.Vulkan;
#elif ANDROID
            //platformView.Backend = GraphicsBackend.OpenGLES;
#endif
            var camera = new SimpleCamera();
            PanGestureRecognizer panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (sender, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        camera.UpdatePanPoint((float)e.TotalX, (float)e.TotalY);
                        break;
                }
            };
            //platformView.GestureRecognizers.Add(panGesture);
            foreach (var view in buttonContainer.Children)
            {
                if (view is Button)
                {
                    var button = view as Button;
                    button.Clicked += (s, e) =>
                    {
                        if (button.Text == nameof(Veldrid.Maui.Samples.Core.GettingStarted.GettingStartedDrawable))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.GettingStarted.GettingStartedDrawable();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.ComputeTexture.ComputeTextureApplication();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication(camera);
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication();//bug
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.Instancing.InstancingApplication))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.Instancing.InstancingApplication(camera);
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.Offscreen.OffscreenApplication(camera);
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.TexturedCube.TexturedCubeDrawable();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle_ElementBufferObject))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.HelloTriangle_ElementBufferObject();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_InsAndOuts))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_InsAndOuts();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_Uniform))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_Uniform();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_MoreAttributes))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Shaders_MoreAttributes();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Textures();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_ApplyingTextures))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_ApplyingTextures();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_TextureUnits))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Textures_TextureUnits();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Transformations_InPractice))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Transformations_InPractice();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_Going3D))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_Going3D();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_More3D))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_More3D();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_MoreCubes))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.CoordinateSystems_MoreCubes();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Camera_LookAt))
                            platformView.Drawable = new Veldrid.Maui.Samples.Core.LearnOpenGL.Camera_LookAt();
                        else if (button.Text == nameof(Veldrid.Maui.Samples.Core.LearnOpenGL.Camera_WalkAround))
                        {
                            var d = new Veldrid.Maui.Samples.Core.LearnOpenGL.Camera_WalkAround();
                            SwipeGestureRecognizer upSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Up};
                            SwipeGestureRecognizer downSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Down};
                            SwipeGestureRecognizer leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left};
                            SwipeGestureRecognizer rightSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Right};
                            
                            var action = (object? sender, SwipedEventArgs e) =>
                            {
                                switch (e.Direction)
                                {
                                    case SwipeDirection.Right:
                                        d.processInput('D');
                                        break;
                                    case SwipeDirection.Left:
                                        d.processInput('A');
                                        break;
                                    case SwipeDirection.Up:
                                        d.processInput('W');
                                        break;
                                    case SwipeDirection.Down:
                                        d.processInput('S');
                                        break;
                                }
                            };
                            upSwipeGesture.Swiped += action.Invoke;
                            downSwipeGesture.Swiped += action.Invoke;
                            leftSwipeGesture.Swiped += action.Invoke;
                            rightSwipeGesture.Swiped += action.Invoke;
                            platformView.GestureRecognizers.Add(upSwipeGesture);
                            platformView.GestureRecognizers.Add(downSwipeGesture);
                            platformView.GestureRecognizers.Add(leftSwipeGesture);
                            platformView.GestureRecognizers.Add(rightSwipeGesture);
                            platformView.Drawable = d;
                        }
                        else if (button.Text == "Remove")
                            platformView.Drawable = null;
                    };
                }
            }

            scrollView.Content = (buttonContainer);
            veldridContainer.Children.Add(platformView);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }
    }
}
