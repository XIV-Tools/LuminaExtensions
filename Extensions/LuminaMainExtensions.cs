// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions
{
	using System.IO;
	using Lumina.Data;
	using Lumina.Data.Files;
	using LuminaExtensions.Files;

	using LuminaMain = global::Lumina.Lumina;

	public static class LuminaMainExtensions
	{
		public static FileResource GetFileEx(this LuminaMain lumina, string path)
		{
			string extension = Path.GetExtension(path);
			switch (extension)
			{
				case ".mtrl": return lumina.GetFile<MtrlFile>(path);
				case ".mdl": return lumina.GetFile<MdlFile>(path);
				case ".eqdp": return lumina.GetFile<EqdpFile>(path);
				case ".tex": return lumina.GetFile<TexFileEx>(path);
			}

			return lumina.GetFile(path);
		}
	}
}
