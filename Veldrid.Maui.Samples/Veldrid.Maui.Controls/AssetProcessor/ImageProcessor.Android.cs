#if ANDROID
using Android.Graphics;
using Java.IO;
using Java.Lang;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Veldrid.Maui.Controls.AssetPrimitives;
using Image = Android.Graphics.Bitmap;
using Math = System.Math;

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
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            int formatSize = 4;
            Image image = BitmapFactory.DecodeStream(stream, null, options);
            Image[] mipmaps = GenerateMipmaps(image, formatSize, out int totalSize);

            byte[] allTexData = new byte[totalSize];
            int offset = 0;
            /*foreach (var mipmap in mipmaps)
            {
                int mipSize = mipmap.Width * mipmap.Height * formatSize;
                using MemoryStream baos = new MemoryStream();
                mipmap.Compress(Bitmap.CompressFormat.Png, 100, baos);
                baos.Read(allTexData, offset, mipSize);

                offset += mipSize;
            }*/
            fixed (byte* allTexDataPtr = allTexData)
            {
                foreach (var mipmap in mipmaps)
                {
                    int mipSize = (int)mipmap.Width * (int)mipmap.Height * formatSize;
                    var data = bitmapToRgba(mipmap);
                    fixed (byte* pixelPtr = data)
                    {
                        Buffer.MemoryCopy(pixelPtr, allTexDataPtr + offset, mipSize, mipSize);
                    }
                    offset += mipSize;
                }
            }

            ProcessedTexture texData = new ProcessedTexture(
                    PixelFormat.R8_G8_B8_A8_UNorm, TextureType.Texture2D,
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

        /// <summary>
        /// https://stackoverflow.com/questions/59786742/android-bitmap-to-rgba-and-back
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        /// <exception cref="IllegalArgumentException"></exception>
        public static byte[] bitmapToRgba(Bitmap bitmap)
        {
            if (bitmap.GetConfig() != Bitmap.Config.Argb8888)
                throw new IllegalArgumentException("Bitmap must be in ARGB_8888 format");
            int[] pixels = new int[bitmap.Width * bitmap.Height];
            byte[] bytes = new byte[pixels.Length * 4];
            bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);
            int i = 0;
            foreach (int pixel in pixels)
            {
                // Get components assuming is ARGB
                int A = (pixel >> 24) & 0xff;
                int R = (pixel >> 16) & 0xff;
                int G = (pixel >> 8) & 0xff;
                int B = pixel & 0xff;
                bytes[i++] = (byte)R;
                bytes[i++] = (byte)G;
                bytes[i++] = (byte)B;
                bytes[i++] = (byte)A;
            }
            return bytes;
        }

        public static Bitmap bitmapFromRgba(int width, int height, byte[] bytes)
        {
            int[] pixels = new int[bytes.Length / 4];
            int j = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                int R = bytes[j++] & 0xff;
                int G = bytes[j++] & 0xff;
                int B = bytes[j++] & 0xff;
                int A = bytes[j++] & 0xff;

                int pixel = (A << 24) | (R << 16) | (G << 8) | B;
                pixels[i] = pixel;
            }


            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);
            return bitmap;
        }
    }
}
#endif