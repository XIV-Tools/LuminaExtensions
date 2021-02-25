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
		public abstract void Convert(TFile source, Stream destination);
		public abstract void ConvertBack(Stream source, TFile destination);

		public sealed override void Convert(FileResource source, Stream destination)
		{
			if (source is TFile file)
			{
				this.Convert(file, destination);
			}
			else
			{
				throw new Exception($"Attempt to use converter intended for file type: {typeof(TFile)} for file: {source}");
			}
		}

		public sealed override void ConvertBack(Stream source, FileResource destination)
		{
			if (destination is TFile file)
			{
				this.ConvertBack(source, file);
			}
			else
			{
				throw new Exception($"Attempt to use converter intended for file type: {typeof(TFile)} for file: {destination}");
			}
		}

		public sealed override bool CanConvert(Type sourceType)
		{
			return sourceType == typeof(TFile);
		}
	}
}
