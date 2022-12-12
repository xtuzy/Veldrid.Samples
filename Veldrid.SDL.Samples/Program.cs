using Veldrid.SDL.Samples.Base;
using Veldrid;
using Veldrid.Maui.Samples.Core.LearnOpenGL;
using Veldrid.Maui.Controls.Base;

namespace Veldrid.SDL.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            VeldridPlatformWindow window = new VeldridPlatformWindow();
            var platformInterface = new VeldridPlatformInterface(window, GraphicsBackend.Direct3D11);

            var camera = new SimpleCamera();
            //platformInterface.Drawable = new Maui.Samples.Core.GettingStarted.GettingStartedDrawable();
            //platformInterface.Drawable = new Maui.Samples.Core.ComputeTexture.ComputeTextureApplication();
            //platformInterface.Drawable = new Maui.Samples.Core.AnimatedMesh.AnimatedMeshApplication(camera);
            //platformInterface.Drawable = new Maui.Samples.Core.ComputeParticles.ComputeParticlesApplication();
            //platformInterface.Drawable = new Maui.Samples.Core.Instancing.InstancingApplication(camera);
            //platformInterface.Drawable = new Maui.Samples.Core.Offscreen.OffscreenApplication(camera);
            //platformInterface.Drawable = new Maui.Samples.Core.TexturedCube.TexturedCubeDrawable();

            //platformInterface.Drawable = new HelloTriangle();
            //platformInterface.Drawable = new HelloTriangle_ElementBufferObject();
            //platformInterface.Drawable = new Shaders_InsAndOuts();
            //platformInterface.Drawable = new Shaders_Uniform();
            //platformInterface.Drawable = new Shaders_MoreAttributes();
            //platformInterface.Drawable = new Textures();
            //platformInterface.Drawable = new Textures_ApplyingTextures();
            //platformInterface.Drawable = new Textures_TextureUnits();
            //platformInterface.Drawable = new Transformations_InPractice();
            //platformInterface.Drawable = new CoordinateSystems_Going3D();
            //platformInterface.Drawable = new CoordinateSystems_More3D();
            platformInterface.Drawable = new CoordinateSystems_MoreCubes();


            window.Window.Title = platformInterface.Drawable.GetType().Name;
            while (window.Window.Exists)
            {
                window.Window.PumpEvents();
            }
        }
    }
}