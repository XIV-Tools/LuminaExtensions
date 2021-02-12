// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Excel
{
	using Lumina.Excel;

	using LuminaMain = global::Lumina.Lumina;

	public class ExcelRowViewModel<T> : IExcelRowViewModel
		where T : IExcelRow
	{
		protected readonly T Value;
		protected readonly LuminaMain Lumina;

		public ExcelRowViewModel(T item, LuminaMain lumina)
		{
			this.Value = item;
			this.Lumina = lumina;
		}
	}
}
