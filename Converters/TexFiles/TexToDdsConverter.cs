// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Converters.TexFiles
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using LuminaExtensions.Files;

	public class TexToDdsConverter : ConverterBase<TexFileEx>
	{
		public override string Name => "Direct Draw Surface";
		public override string FileExtension => ".dds";
		public override string ResourceExtension => ".tex";

		public override void Convert(TexFileEx source, Stream destination)
		{
			throw new NotImplementedException();
		}

		public override void ConvertBack(Stream source, TexFileEx destination)
		{
			throw new NotImplementedException();
		}
	}
}
