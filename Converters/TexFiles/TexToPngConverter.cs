// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Converters.TexFiles
{
	using System;
	using System.IO;
	using System.Runtime.CompilerServices;
	using Lumina.Data.Files;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.Formats.Png;
	using SixLabors.ImageSharp.PixelFormats;

	public class TexToPngConverter : ConverterBase<TexFile>
	{
		public override string Name => "Image";
		public override string FileExtension => "png";

		public override bool Convert(TexFile source, FileStream destination)
		{
			PngEncoder encoder = new PngEncoder();
			encoder.BitDepth = PngBitDepth.Bit16;

			////Span<byte> rawImageData = source.DataSpan.Slice(Unsafe.SizeOf<TexFile.TexHeader>());

			// TODO: Split ImageData based on lods / mips
			Image img = Image.LoadPixelData<Bgra32>(source.ImageData, source.Header.Width, source.Header.Height);
			img.Save(destination, encoder);
			img.Dispose();

			return true;
		}

		public override bool ConvertBack(FileStream source, TexFile destination)
		{
			throw new NotImplementedException();
		}
	}
}
