using Veldrid.SDL.Samples.Base;
using Veldrid;
using Veldrid.Maui.Samples.Core.LearnOpenGL;

namespace Veldrid.SDL.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            VeldridPlatformWindow window = new VeldridPlatformWindow();
            var platformInterface = new VeldridPlatformInterface(window, GraphicsBackend.Direct3D11);

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