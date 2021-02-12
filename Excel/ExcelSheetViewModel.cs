// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Excel
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Lumina.Excel;

	using LuminaMain = global::Lumina.Lumina;

	public class ExcelSheetViewModel<TViewModel, TExcelRow> : IEnumerable<TViewModel>
		where TViewModel : class, IExcelRowViewModel
		where TExcelRow : class, IExcelRow
	{
		private readonly Dictionary<TExcelRow, TViewModel> viewModelCache = new Dictionary<TExcelRow, TViewModel>();
		private readonly ExcelSheet<TExcelRow> items;
		private readonly LuminaMain lumina;

		public ExcelSheetViewModel(LuminaMain lumina)
		{
			this.lumina = lumina;
			this.items = lumina.GetExcelSheet<TExcelRow>();
		}

		public TViewModel GetRow(uint row)
		{
			TExcelRow excelRow = this.items.GetRow(row);
			return this.GetViewModel(excelRow);
		}

		public TViewModel GetRow(uint row, uint subRow)
		{
			TExcelRow excelRow = this.items.GetRow(row, subRow);
			return this.GetViewModel(excelRow);
		}

		IEnumerator<TViewModel> IEnumerable<TViewModel>.GetEnumerator()
		{
			IEnumerator<TExcelRow> enu = this.items.GetEnumerator();
			while (enu.MoveNext())
			{
				yield return this.GetViewModel(enu.Current);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerator<TExcelRow> enu = this.items.GetEnumerator();
			while (enu.MoveNext())
			{
				yield return this.GetViewModel(enu.Current);
			}
		}

		private TViewModel GetViewModel(TExcelRow row)
		{
			TViewModel viewModel;
			if (this.viewModelCache.TryGetValue(row, out viewModel))
				return viewModel;

			viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), row, this.lumina);
			this.viewModelCache.Add(row, viewModel);
			return viewModel;
		}
	}
}
