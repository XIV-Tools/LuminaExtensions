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
	}
}
