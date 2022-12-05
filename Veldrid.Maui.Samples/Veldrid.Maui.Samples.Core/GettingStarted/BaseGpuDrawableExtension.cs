using System.Reflection;
using System.Xml.Linq;
using Veldrid.Maui.Controls.Base;

namespace Veldrid.Maui.Samples.Core.GettingStarted
{
    internal static class BaseGpuDrawableExtension
    {
        public static string ReadEmbedAssetPath(this BaseGpuDrawable drawable, string resourcePath = "foler.fileName.extention", Type type = null)
        {
            if (type == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());
                resourcePath = resourcePath.Replace(@"/", ".");
                resourcePath = resourceNames.FirstOrDefault(r => r.Contains(resourcePath));
            }
            else
            {
                List<string> resourceNames = new List<string>(type.Assembly.GetManifestResourceNames());
                resourcePath = resourcePath.Replace(@"/", ".");
                resourcePath = resourceNames.FirstOrDefault(r => r.Contains(resourcePath));
            }

            if (resourcePath == null)
                throw new FileNotFoundException("Resource not found");
            return resourcePath;
        }

        public static byte[] ReadEmbedAsset(this BaseGpuDrawable drawable, string resourcePath = "foler.fileName.extention", Type type = null)
        {
            resourcePath = ReadEmbedAssetPath(drawable, resourcePath);
            return drawable.ReadEmbeddedAssetBytes(resourcePath);
        }

        public static Stream ReadEmbedAssetStream(this BaseGpuDrawable drawable, string resourcePath = "foler.fileName.extention", Type type = null)
        {
            var bytes = ReadEmbedAsset(drawable, resourcePath);
            MemoryStream destination = new MemoryStream(bytes);
            return destination;
        }
    }
}
