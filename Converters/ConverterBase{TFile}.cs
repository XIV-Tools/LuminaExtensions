// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Converters
{
	using System;
	using System.IO;
	using Lumina.Data;

	public abstract class ConverterBase<TFile> : ConverterBase
		where TFile : FileResource
	{
		public abstract bool Convert(TFile source, FileStream destination);
		public abstract bool ConvertBack(FileStream source, TFile destination);

		public sealed override bool Convert(FileResource source, FileStream destination)
		{
			if (source is TFile file)
				return this.Convert(file, destination);

			throw new Exception($"Attempt to use converter intended for file type: {typeof(TFile)} for file: {source}");
		}

		public sealed override bool ConvertBack(FileStream source, FileResource destination)
		{
			if (destination is TFile file)
				return this.ConvertBack(source, file);

			throw new Exception($"Attempt to use converter intended for file type: {typeof(TFile)} for file: {source}");
		}

		public sealed override bool CanConvert(Type sourceType)
		{
			return sourceType == typeof(TFile);
		}
	}
}
