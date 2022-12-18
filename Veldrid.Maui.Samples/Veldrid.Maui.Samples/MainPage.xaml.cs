using System.Runtime.InteropServices;
using Veldrid.Maui.Controls;
using Veldrid.Maui.Controls.Base;
using Vulkan;

namespace Veldrid.Maui.Samples
{
    public partial class MainPage : ContentPage
    {
        //[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        //public static unsafe extern VkResult vkCreateInstance(VkInstanceCreateInfo* pCreateInfo, VkAllocationCallbacks* pAllocator, VkInstance* pInstance);

        public MainPage()
        {
            InitializeComponent();
#if IOS
            var success = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK.a", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.AssemblyDirectory, out var handle);
            var success01 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.AssemblyDirectory, out var handle01);
            var success02 = System.Runtime.InteropServices.NativeLibrary.TryLoad("MoltenVK", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.AssemblyDirectory, out var handle02);
            var success8 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK.a", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.ApplicationDirectory, out var handle8);
            var success81 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.ApplicationDirectory, out var handle81);
            var success9 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK.a", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.UseDllDirectoryForDependencies, out var handle9);
            var success91 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.UseDllDirectoryForDependencies, out var handle91);
            var success1 = System.Runtime.InteropServices.NativeLibrary.TryLoad("@rpath/libMoltenVK.a", out var handle1);
            var success2 = System.Runtime.InteropServices.NativeLibrary.TryLoad("__Internal", out var handle2);
            var success3 = System.Runtime.InteropServices.NativeLibrary.TryLoad("MoltenVK", out var handle3);
            var success4 = System.Runtime.InteropServices.NativeLibrary.TryLoad("@rpath/MoltenVK", out var handle4);
            var success5 = System.Runtime.InteropServices.NativeLibrary.TryLoad("@rpath/libMoltenVK", out var handle5);
            var success6 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libMoltenVK", out var handle6);
            var success7 = System.Runtime.InteropServices.NativeLibrary.TryLoad("MoltenVK.a", out var handle7);
            var success10 = System.Runtime.InteropServices.NativeLibrary.TryLoad("MoltenVK.a", out var handle10);
            var success11 = System.Runtime.InteropServices.NativeLibrary.TryLoad("libvulkan.dylib", typeof(MainPage).Assembly, System.Runtime.InteropServices.DllImportSearchPath.UseDllDirectoryForDependencies, out var handle11);
            var documentsPath = Foundation.NSBundle.MainBundle.BundlePath;
            var filePath = System.IO.Path.Combine(documentsPath);
            var path = new DirectoryInfo(filePath);
            var ds = path.EnumerateDirectories();
            var fs = path.EnumerateFiles();
            var p10 = ObjCRuntime.Dlfcn.dlopen(null, 0x002);
            var p11 = ObjCRuntime.Dlfcn.dlsym(p10, "vkCreateInstance");
            var p12 = ObjCRuntime.Dlfcn.dlsym(p10, "_vkCreateInstance");
            var p21 = ObjCRuntime.Dlfcn.dlsym(p10, "CompileGlslToSpirv");
            var p22 = ObjCRuntime.Dlfcn.dlsym(p10, "_CompileGlslToSpirv");
            var library = Vulkan.NativeLibrary.Load(null);
            IntPtr p0 = library.LoadFunctionPointer("vkCreateInstance");
            IntPtr p01 = library.LoadFunctionPointer("_vkCreateInstance");
            IntPtr p02 = library.LoadFunctionPointer("vkEnumerateInstanceLayerProperties");
            IntPtr p03 = library.LoadFunctionPointer("_vkEnumerateInstanceLayerProperties");
            var ios = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            if (success == true)
                throw new Exception();
#endif
            /*            if (OperatingSystem.IsWindows())
                        {
                            //veldridView.Drawable = new ComputeTextureApplication(null);//Success
                            //veldridView.Drawable = new OffscreenApplication(null);//Success
                            //veldridView.Drawable = new InstancingApplication(null);//Success
                            //veldridView.Drawable = new AnimatedMeshApplication(null);//Success
                            veldridView.Drawable = new ComputeParticlesApplication(null);//Fail
                            //veldridView.Drawable = new GettingStartedApplication(null);//Success
                        }
                        else if (OperatingSystem.IsAndroid())
                        {
            #if ANDROID
                            AssimpNet.Platform.Init();
            #endif
                            veldridView.Drawable = new ComputeTextureApplication(null);//Success
                            //veldridView.Drawable = new OffscreenApplication(null);//Success
                            //veldridView.Drawable = new InstancingApplication(null);//Success
                            //veldridView.Drawable = new AnimatedMeshApplication(null);//Fail
                            //veldridView.Drawable = new ComputeParticlesApplication(null);//Fail
                            //veldridView.Drawable = new GettingStartedApplication(null);//Success
                        }
                        else if (OperatingSystem.IsIOS())
                        {
                            //veldridView.Drawable = new ComputeTextureApplication(null);//Success
                            //veldridView.Drawable = new OffscreenApplication(null);
                            veldridView.Drawable = new InstancingApplication(null);
                            //veldridView.Drawable = new AnimatedMeshApplication(null);
                            //veldridView.Drawable = new ComputeParticlesApplication(null);
                            //veldridView.Drawable = new GettingStartedApplication(null);//Success
                        }
                        else if (OperatingSystem.IsMacCatalyst())
                        {
                            //Current not work for MacCatalyst, because need build SPIRV for Maccatalyst.
                            //veldridView.Drawable = new ComputeTextureApplication(null);
                            //veldridView.Drawable = new OffscreenApplication(null);
                            //veldridView.Drawable = new InstancingApplication(null);
                            //veldridView.Drawable = new AnimatedMeshApplication(null);
                            //veldridView.Drawable = new ComputeParticlesApplication(null);
                        }*/

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
                    new Button(){ Text = "Remove"},
                }
            };
            var platformView = new VeldridView() { };
#if IOS
            platformView.Backend = GraphicsBackend.Vulkan;
#elif ANDROID
            platformView.Backend = GraphicsBackend.OpenGLES;
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
            platformView.GestureRecognizers.Add(panGesture);
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
