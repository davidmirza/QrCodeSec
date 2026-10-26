

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using zvr.Common;

namespace zvr.Rendering
{
   /// <summary>
   /// Renders a <see cref="BitMatrix" /> to a <see cref="Bitmap" />
   /// (pengganti WriteableBitmapRenderer Silverlight agar jalan di .NET Framework desktop)
   /// </summary>
   public class BitmapRenderer : IBarcodeRenderer<Bitmap>
   {
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      public Color Foreground { get; set; }
      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      public Color Background { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="BitmapRenderer"/> class.
      /// </summary>
      public BitmapRenderer()
      {
         Foreground = Color.Black;
         Background = Color.White;
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, null);
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      public virtual Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         int width = matrix.Width;
         int height = matrix.Height;
         int pixelsize = 1;

         if (options != null)
         {
            if (options.Width > width)
            {
               width = options.Width;
            }
            if (options.Height > height)
            {
               height = options.Height;
            }
            // calculating the scaling factor
            pixelsize = width / matrix.Width;
            if (pixelsize > height / matrix.Height)
            {
               pixelsize = height / matrix.Height;
            }
         }

         int foreground = Foreground.ToArgb();
         int background = Background.ToArgb();
         var pixels = new int[width * height];
         var index = 0;

         for (int y = 0; y < matrix.Height; y++)
         {
            for (var pixelsizeHeight = 0; pixelsizeHeight < pixelsize; pixelsizeHeight++)
            {
               for (var x = 0; x < matrix.Width; x++)
               {
                  var color = matrix[x, y] ? foreground : background;
                  for (var pixelsizeWidth = 0; pixelsizeWidth < pixelsize; pixelsizeWidth++)
                  {
                     pixels[index++] = color;
                  }
               }
               for (var x = pixelsize * matrix.Width; x < width; x++)
               {
                  pixels[index++] = background;
               }
            }
         }
         while (index < pixels.Length)
         {
            pixels[index++] = background;
         }

         var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
         var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
         try
         {
            for (int row = 0; row < height; row++)
            {
               Marshal.Copy(pixels, row * width, data.Scan0 + row * data.Stride, width);
            }
         }
         finally
         {
            bmp.UnlockBits(data);
         }

         return bmp;
      }
   }
}
