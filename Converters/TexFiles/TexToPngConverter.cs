// © XIV-Tools.
// Licensed under the MIT license.

// Special thanks to xivModdingFramework for the initial texture
// conversion logic upon which this is based.
// Copyright © 2018 Rafael Gonzalez - All Rights Reserved
namespace LuminaExtensions.Converters.TexFiles
{
	using System;
	using System.IO;
	using Lumina.Data.Files;
	using LuminaExtensions.Files;
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.Formats.Png;
	using SixLabors.ImageSharp.PixelFormats;
	using TeximpNet;
	using TeximpNet.Compression;
	using TeximpNet.DDS;

	public class TexToPngConverter : TexToDdsConverter
	{
		public override string Name => "Image";
		public override string FileExtension => ".png";
		public override string ResourceExtension => ".tex";

		public override void Convert(TexFileEx source, Stream destination)
		{
			PngEncoder encoder = new PngEncoder();
			encoder.BitDepth = PngBitDepth.Bit16;

			////Span<byte> rawImageData = source.DataSpan.Slice(Unsafe.SizeOf<TexFile.TexHeader>());

			// TODO: Split ImageData based on lods / mips
			Image img = Image.LoadPixelData<Bgra32>(source.ImageData, source.Header.Width, source.Header.Height);
			img.Save(destination, encoder);
			img.Dispose();
		}

		public override void ConvertBack(Stream source, TexFileEx destination)
		{
			using Surface surface = Surface.LoadFromStream(source);

			using Compressor compressor = new Compressor();
			compressor.Input.SetMipmapGeneration(true, destination.Header.MipLevels);
			compressor.Input.SetData(surface);

			compressor.Compression.Format = destination.Header.Format switch
			{
				TexFile.TextureFormat.DXT1 => CompressionFormat.BC1a,
				TexFile.TextureFormat.DXT5 => CompressionFormat.BC3,
				TexFile.TextureFormat.A8R8G8B8 => CompressionFormat.BGRA,

				_ => throw new NotSupportedException($"The destination texture format: {destination.Header.Format} is not supported."),
			};

			compressor.Compression.SetBGRAPixelFormat();

			DDSContainer ddsContainer;
			compressor.Process(out ddsContainer);

			using MemoryStream ddsStream = new MemoryStream();
			ddsContainer.Write(ddsStream);

			base.ConvertBack(ddsStream, destination);
		}
	}
}
