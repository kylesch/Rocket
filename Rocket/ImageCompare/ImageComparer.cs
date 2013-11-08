using System;
using System.Drawing;
using System.Drawing.Imaging;

public class ImageComparer
{
    public enum ImageRelation
    {
        Equal,
        PixelInqeuality,
        PixelFormatDiffer,
        PixelFormatUndefined,
        DimensionsUnequal,
        UnsupportedPixelFormat,
    }

    public ImageRelation CompareImages(Bitmap FirstImage, Bitmap SecondImage)
    {
        BitmapData bmdFirstImage, bmdSecondImage;
        Int32 intPixelSize;

        // Don't compare images with different pixelformats
        if (FirstImage.PixelFormat != SecondImage.PixelFormat)
        {
            return (ImageRelation.PixelFormatDiffer);
        }

        // Don't compare images with undefined pixelformats
        if (FirstImage.PixelFormat == PixelFormat.Undefined)
        {
            return (ImageRelation.PixelFormatUndefined);
        }

        // Images of different dimensions can't be equal
        if (FirstImage.Size != SecondImage.Size)
        {
            return (ImageRelation.DimensionsUnequal);
        }


        // Calculate the pixel size (bytes per pixel)
        switch (FirstImage.PixelFormat)
        {
            // 8 bit - 1 byte
            case (PixelFormat.Format8bppIndexed):
                {
                    intPixelSize = 1;
                    break;
                }

            // 16 bit - 2 bytes
            case (PixelFormat.Format16bppArgb1555):
                {
                    intPixelSize = 2;
                    break;
                }
            case (PixelFormat.Format16bppGrayScale):
                {
                    intPixelSize = 2;
                    break;
                }
            case (PixelFormat.Format16bppRgb555):
                {
                    intPixelSize = 2;
                    break;
                }
            case (PixelFormat.Format16bppRgb565):
                {
                    intPixelSize = 2;
                    break;
                }

            // 24 bit - 3 bytes
            case (PixelFormat.Format24bppRgb):
                {
                    intPixelSize = 3;
                    break;
                }

            // 32 bit - 4 bytes
            case (PixelFormat.Format32bppArgb):
                {
                    intPixelSize = 4;
                    break;
                }
            case (PixelFormat.Format32bppPArgb):
                {
                    intPixelSize = 4;
                    break;
                }
            case (PixelFormat.Format32bppRgb):
                {
                    intPixelSize = 4;
                    break;
                }

            // 48 bit - 5 bytes
            case (PixelFormat.Format4bppIndexed):
                {
                    intPixelSize = 5;
                    break;
                }

            // 64 bit - 6 bytes
            case (PixelFormat.Format64bppArgb):
                {
                    intPixelSize = 6;
                    break;
                }
            case (PixelFormat.Format64bppPArgb):
                {
                    intPixelSize = 6;
                    break;
                }

            // Unsupported size
            default:
                {
                    return (ImageRelation.UnsupportedPixelFormat);
                }
        }

        // Lock both bitmap bits to initialize comparison of pixels
        bmdFirstImage = FirstImage.LockBits(new Rectangle(0, 0, FirstImage.Width, FirstImage.Height),
                                             ImageLockMode.ReadOnly,
                                             FirstImage.PixelFormat);

        bmdSecondImage = SecondImage.LockBits(new Rectangle(0, 0, SecondImage.Width, SecondImage.Height),
                                               ImageLockMode.ReadOnly,
                                               SecondImage.PixelFormat);

        // Compare each pixel in the images
        unsafe
        {
            for (Int32 y = 0; y < bmdFirstImage.Height; ++y)
            {
                byte* rowFirstImage = (byte*)bmdFirstImage.Scan0 + (y * bmdFirstImage.Stride);
                byte* rowSecondImage = (byte*)bmdSecondImage.Scan0 + (y * bmdSecondImage.Stride);

                for (Int32 x = 0; x < bmdFirstImage.Width; ++x)
                {
                    if (Math.Abs(rowFirstImage[x * intPixelSize] - rowSecondImage[x * intPixelSize])>5)
                    {
                        // Unlock bitmap bits
                        FirstImage.UnlockBits(bmdFirstImage);
                        SecondImage.UnlockBits(bmdSecondImage);

                        return (ImageRelation.PixelInqeuality);
                    }
                }
            }
        }
        // Unlock bitmap bits
        FirstImage.UnlockBits(bmdFirstImage);
        SecondImage.UnlockBits(bmdSecondImage);

        return ImageRelation.Equal;
    }
}
