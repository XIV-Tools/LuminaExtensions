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

		public abstract bool CanConvert(Type sourceType);
		public abstract bool Convert(FileResource source, FileStream destination);
		public abstract bool ConvertBack(FileStream source, FileResource destination);
	}
}
