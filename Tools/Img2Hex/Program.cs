using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Img2Hex
{
	class Program
	{
		static void Main(String[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("img2hex.exe <path to image>");
				return;
			}
			
			Bitmap bitmap = new Bitmap(args[0]);
			if (bitmap.Width%8 != 0)
			{
				Console.WriteLine("Width must be multiple of 8.");
				return;
			}

			FrameDimension dim = null;
			Int32 frames = 1;
			if (bitmap.FrameDimensionsList.Length > 0)
			{
				dim = new FrameDimension(bitmap.FrameDimensionsList[0]);
				frames = bitmap.GetFrameCount(dim);
			}

			Console.WriteLine("uint8_t img_"+ Path.GetFileNameWithoutExtension(args[0]) +"["+ frames +"]["+ (bitmap.Width + 1) +"]={");
			for (Int32 frame = 0; frame < frames; frame++)
			{
				if (dim != null)
					bitmap.SelectActiveFrame(dim, frame);

				GraphicsUnit unit = GraphicsUnit.Pixel;
				RectangleF bounds = bitmap.GetBounds(ref unit);
				BitmapData data = bitmap.LockBits(Rectangle.Round(bounds), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);

				// Copy the RGB values into the array.
				Byte[] bytes = new Byte[data.Stride*data.Height];
				System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

				Int32 width = data.Width / 8 + (data.Width % 8 > 0 ? 1 : 0);
				Int32 height = data.Height / 8 + (data.Height % 8 > 0 ? 1 : 0);
				for (Int32 k = 0; k < height; k++)
				{
					Int32 offset = k * 8 * data.Stride;
					Byte[] row = new Byte[8 * width];
					for (Int32 i = 0; i < width; i++)
					{
						for (Int32 j = 0; j < 8 && k*8+j < data.Height; j++, offset += data.Stride)
						{
							Byte b = bytes[offset + i];
							row[i + 0] |= (Byte) (((b >> 7) & 0x01) << j);
							row[i + 1] |= (Byte)(((b >> 6) & 0x01) << j);
							row[i + 2] |= (Byte)(((b >> 5) & 0x01) << j);
							row[i + 3] |= (Byte)(((b >> 4) & 0x01) << j);
							row[i + 4] |= (Byte)(((b >> 3) & 0x01) << j);
							row[i + 5] |= (Byte)(((b >> 2) & 0x01) << j);
							row[i + 6] |= (Byte)(((b >> 1) & 0x01) << j);
							row[i + 7] |= (Byte)((b & 0x01) << j);
						}
					}
					Console.WriteLine("  {" + data.Width + ", " + String.Join(", ", row.Select(r => "0x" + r.ToString("X02"))) + "},");
				}
				bitmap.UnlockBits(data);
			}
			Console.WriteLine("};");

			//Console.ReadKey();
		}
	}
}
