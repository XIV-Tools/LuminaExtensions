// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Converters
{
	using System;
	using System.IO;
	using Lumina.Data;

	public abstract class ConverterBase
	{
		public abstract string Name { get; }
		public abstract string FileExtension { get; }
		public abstract string ResourceExtension { get; }

		public abstract bool CanConvert(Type sourceType);
		public abstract void Convert(FileResource source, Stream destination);
		public abstract void ConvertBack(Stream source, FileResource destination);
	}
}
