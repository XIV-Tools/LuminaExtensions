// © XIV-Tools.
// Licensed under the MIT license.

// Special thanks to xivModdingFramework for the initial texture
// conversion logic upon which this is based.
// xivModdingFramework Copyright © 2018 Rafael Gonzalez - All Rights Reserved
namespace LuminaExtensions.Converters.TexFiles
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using LuminaExtensions.Files;

	public class TexToDdsConverter : ConverterBase<TexFileEx>
	{
		public override string Name => "Direct Draw Surface";
		public override string FileExtension => ".dds";

		public override void Convert(TexFileEx source, Stream destination)
		{
			throw new NotImplementedException();
		}

		public override void ConvertBack(Stream source, TexFileEx destination)
		{
			TexFileEx.TexHeader header = default;
			header.Type = destination.Header.Type;
			header.Format = destination.Header.Format;
			header.Depth = destination.Header.Depth;

			BinaryReader br = new BinaryReader(source);

			br.BaseStream.Seek(12, SeekOrigin.Begin);

			header.Height = (ushort)br.ReadInt32();
			header.Width = (ushort)br.ReadInt32();
			br.ReadBytes(8);
			header.MipLevels = (ushort)br.ReadInt32();

			if (header.Height % 2 != 0 || header.Width % 2 != 0)
				throw new Exception("Resolution must be a multiple of 2");

			byte[] b = br.ReadBytes((int)br.BaseStream.Length - 88);
			destination.SetDds(b);
		}
	}
}
