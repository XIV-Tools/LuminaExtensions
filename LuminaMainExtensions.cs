// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina
{
	using global::Lumina.Excel;
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

		/*public static ImageSource GetImage(this TexFile self)
		{
			if (self == null)
				return null;

			BitmapSource bmp = BitmapSource.Create(self.Header.Width, self.Header.Height, 96, 96, PixelFormats.Bgra32, null, self.ImageData, self.Header.Width * 4);
			bmp.Freeze();

			return bmp;
		}*/
	}
}
