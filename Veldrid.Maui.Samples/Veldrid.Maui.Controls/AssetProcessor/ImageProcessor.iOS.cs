#if AIOS || AMACCATALYST
using CoreGraphics;
using Foundation;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UIKit;
using Veldrid.Maui.Controls.AssetPrimitives;
using Image = UIKit.UIImage;
namespace Veldrid.Maui.Controls.AssetProcessor
{
    /// <summary>
    /// 可以用来在iOS App运行时加载, 但由于处理图像可能较慢, 建议在构建时处理, 运行时直接加载处理后的数据.
    /// </summary>
    public class ImageProcessor : BinaryAssetProcessor<ProcessedTexture>
    {
        public unsafe override ProcessedTexture ProcessT(Stream stream, string extension)
        {
            int formatSize = 4;
            var image = new UIImage(NSData.FromStream(stream));
            Image[] mipmaps = GenerateMipmaps(image, formatSize, out int totalSize);

            byte[] allTexData = new byte[totalSize];
            int offset = 0;
            fixed (byte* allTexDataPtr = allTexData)
            {
                foreach (var mipmap in mipmaps)
                {
                    int mipSize = (int)mipmap.Size.Width * (int)mipmap.Size.Height * formatSize;
                    var data = convertUIImageToBitmapRGBA8(mipmap);
                    fixed (byte* pixelPtr = data)
                    {
                        Buffer.MemoryCopy(pixelPtr, allTexDataPtr + offset, mipSize, mipSize);
                    }
                    offset += mipSize;
                }
            }

            ProcessedTexture texData = new ProcessedTexture(
                    PixelFormat.R8_G8_B8_A8_UNorm, TextureType.Texture2D,
                    (uint)image.Size.Width, (uint)image.Size.Height, 1,
                    (uint)mipmaps.Length, 1,
                    allTexData);
            return texData;
        }

        // Taken from Veldrid.ImageSharp

        private static Image[] GenerateMipmaps(Image baseImage, int formatSize, out int totalSize)
        {
            int mipLevelCount = ComputeMipLevels((int)baseImage.Size.Width, (int)baseImage.Size.Height);
            Image[] mipLevels = new Image[mipLevelCount];
            mipLevels[0] = baseImage;
            totalSize = (int)baseImage.Size.Width * (int)baseImage.Size.Height * formatSize;
            int i = 1;

            int currentWidth = (int)baseImage.Size.Width;
            int currentHeight = (int)baseImage.Size.Height;
            while (currentWidth != 1 || currentHeight != 1)
            {
                int newWidth = Math.Max(1, currentWidth / 2);
                int newHeight = Math.Max(1, currentHeight / 2);
                //Image newImage = baseImage.Clone(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));
                Image newImage = SimpleResize(baseImage, newWidth, newHeight);
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

        static Image SimpleResize(Image image, int width, int height)
        {
            var render = new UIGraphicsImageRenderer(new CGSize(width, height));

            return render.CreateImage((c) =>
            {
                c.CGContext.DrawImage(new CGRect(0, 0, width, height), image.CGImage);
            });
        }

        unsafe CGBitmapContext newBitmapRGBA8ContextFromImage(CGImage image)
        {
            CGBitmapContext context = null;
            CGColorSpace colorSpace;
            byte[] bitmapData;

            var bitsPerPixel = 32;
            var bitsPerComponent = 8;
            var bytesPerPixel = bitsPerPixel / bitsPerComponent;

            var width = image.Width;
            var height = image.Height;

            var bytesPerRow = width * bytesPerPixel;
            var bufferLength = bytesPerRow * height;

            colorSpace = CGColorSpace.CreateDeviceRGB();

            if (colorSpace == null)
            {
                Debug.WriteLine(@"Error allocating color space RGB\n");
                return null;
            }

            // Allocate memory for image data
            bitmapData = new byte[bufferLength];

            //Create bitmap context

            context = new CGBitmapContext(bitmapData,
                width,
                height,
                bitsPerComponent,
                bytesPerRow,
                colorSpace,
                CGImageAlphaInfo.PremultipliedLast);    // RGBA

            if (context == null)
            {
                //free(bitmapData);
                Debug.WriteLine(@"Bitmap context not created");
            }

            colorSpace.Dispose();

            return context;
        }

        unsafe byte[] convertUIImageToBitmapRGBA8(UIImage image)
        {
            CGImage imageRef = image.CGImage;

            // Create a bitmap context to draw the uiimage into
            CGBitmapContext context = newBitmapRGBA8ContextFromImage(imageRef);

            if (context == null)
            {
                return null;
            }

            var width = imageRef.Width;
            var height = imageRef.Height;

            CGRect rect = new CGRect(0, 0, width, height);

            // Draw image into the context to get the raw image data
            context.DrawImage(rect, imageRef);

            // Get a pointer to the data	
            var bitmapData = context.Data;

            // Copy the data and release the memory (return memory allocated with new)
            var bytesPerRow = context.BytesPerRow;
            var bufferLength = bytesPerRow * height;

            byte[] newBitmap = default;

            if (bitmapData != default)
            {
                newBitmap = new byte[bytesPerRow * height];

                // Copy the data
                for (int i = 0; i < bufferLength; ++i)
                {
                    byte* src = (byte*)bitmapData.ToPointer();
                    newBitmap[i] = *(src + i);
                }
                //free(bitmapData);
            }
            else
            {
                Debug.WriteLine(@"Error getting bitmap pixel data\n");
            }

            context.Dispose();

            return newBitmap;
        }
    }
}
#endif