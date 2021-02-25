// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina.Data
{
	using System.IO;
	using LuminaExtensions.Converters;

	public static class FileResourceExtensions
	{
		public static ConverterBase[] GetConverters(this FileResource self)
		{
			return ConvertersUtil.GetConverters(self.GetType());
		}

		public static ConverterBase? GetConverter(this FileResource self, string extension)
		{
			return ConvertersUtil.GetConverter(self.GetType(), extension);
		}

		public static void ConvertFile(this FileResource self, ConverterBase converter, string path)
		{
			using FileStream file = File.OpenWrite(path);
			converter.Convert(self, file);
		}
	}
}
