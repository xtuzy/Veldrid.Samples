#if ANDROID
using Android.Graphics;
using Java.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Veldrid.Maui.Controls.AssetPrimitives;
using Image = Android.Graphics.Bitmap;
namespace Veldrid.Maui.Controls.AssetProcessor
{
    /// <summary>
    /// 可以用来在Android App运行时加载, 但由于Android处理图像可能较慢和使用了RgbaF16占内存较大, 建议在构建时处理, 运行时直接加载处理后的数据.
    /// </summary>
    public class ImageProcessor : BinaryAssetProcessor<ProcessedTexture>
    {
        public unsafe override ProcessedTexture ProcessT(Stream stream, string extension)
        {
            var options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.RgbaF16;
            int formatSize = 16 * 4;
            Image image = BitmapFactory.DecodeStream(stream, null, options);
            Image[] mipmaps = GenerateMipmaps(image, formatSize, out int totalSize);

            byte[] allTexData = new byte[totalSize];
            int offset = 0;
            foreach (var mipmap in mipmaps)
            {
                int mipSize = mipmap.Width * mipmap.Height * formatSize;
                using MemoryStream baos = new MemoryStream();
                mipmap.Compress(Bitmap.CompressFormat.Png, 100, baos);
                baos.Read(allTexData, offset, mipSize);

                offset += mipSize;
            }

            ProcessedTexture texData = new ProcessedTexture(
                    PixelFormat.R16_G16_B16_A16_Float, TextureType.Texture2D,
                    (uint)image.Width, (uint)image.Height, 1,
                    (uint)mipmaps.Length, 1,
                    allTexData);
            return texData;
        }

        // Taken from Veldrid.ImageSharp

        private static Image[] GenerateMipmaps(Image baseImage, int formatSize, out int totalSize)
        {
            int mipLevelCount = ComputeMipLevels(baseImage.Width, baseImage.Height);
            Image[] mipLevels = new Image[mipLevelCount];
            mipLevels[0] = baseImage;
            totalSize = baseImage.Width * baseImage.Height * formatSize;
            int i = 1;

            int currentWidth = baseImage.Width;
            int currentHeight = baseImage.Height;
            while (currentWidth != 1 || currentHeight != 1)
            {
                int newWidth = Math.Max(1, currentWidth / 2);
                int newHeight = Math.Max(1, currentHeight / 2);
                //Image newImage = baseImage.Clone(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));
                Image newImage = Bitmap.CreateScaledBitmap(baseImage, newWidth, newHeight, false);
                Debug.Assert(i < mipLevelCount);
                mipLevels[i] = newImage;

                totalSize += newWidth * newHeight * formatSize;
                i++;
                currentWidth = newWidth;
                currentHeight = newHeight;
            }

            Debug.Assert(i == mipLevelCount);

            return mipLevels;
        }

        public static int ComputeMipLevels(int width, int height)
        {
            return 1 + (int)Math.Floor(Math.Log(Math.Max(width, height), 2));
        }
    }
}
#endif