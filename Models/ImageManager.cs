using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SecurityTest.Models
{
	internal class ImageManager
	{
		internal static byte[] CroppedToBytes(BitmapImage image)
		{
			var encode = new JpegBitmapEncoder();
			encode.Frames.Add(BitmapFrame.Create(image));
			using var ms = new MemoryStream();
			encode.Save(ms);
			var result = ms.ToArray();

			return result;
		}

		internal static BitmapImage CroppedToBitmapImage(string path)
		{
			var imageToBytes = File.ReadAllBytes(path);
			var fs = new MemoryStream(imageToBytes);
			var image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = fs;
			image.DecodePixelWidth = 200;
			image.EndInit();
			return image;
		}
	}
}
