// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina
{
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using global::Lumina.Data.Files;
	using global::Lumina.Excel;
	using global::Lumina.Extensions;
	using LuminaExtensions.Excel;
	using LuminaMain = global::Lumina.Lumina;

	public static class LuminaMainExtensions
	{
		public static ExcelSheetViewModel<TViewModel, TExcelRow> GetExcelSheet<TViewModel, TExcelRow>(this LuminaMain self)
		where TViewModel : class, IExcelRowViewModel
		where TExcelRow : class, IExcelRow
		{
			return new ExcelSheetViewModel<TViewModel, TExcelRow>(self);
		}

		public static void GetModel(ulong val, bool isWeapon, out ushort modelSet, out ushort modelBase, out ushort modelVariant)
		{
			if (isWeapon)
			{
				modelSet = (ushort)val;
				modelBase = (ushort)(val >> 16);
				modelVariant = (ushort)(val >> 32);
			}
			else
			{
				modelSet = 0;
				modelBase = (ushort)val;
				modelVariant = (ushort)(val >> 16);
			}
		}

		public static ImageSource GetImage(this LuminaMain self, uint imageId)
		{
			return self.GetImage((int)imageId);
		}

		public static ImageSource GetImage(this LuminaMain self, int imageId)
		{
			TexFile tex = self.GetIcon(imageId);
			return tex.GetImage();
		}

		public static ImageSource GetImage(this TexFile self)
		{
			if (self == null)
				return null;

			BitmapSource bmp = BitmapSource.Create(self.Header.Width, self.Header.Height, 96, 96, PixelFormats.Bgra32, null, self.ImageData, self.Header.Width * 4);
			bmp.Freeze();

			return bmp;
		}
	}
}
