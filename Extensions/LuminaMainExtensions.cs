// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions
{
	using Lumina.Data;
	using LuminaExtensions.Files;

	using LuminaMain = global::Lumina.Lumina;

	public static class LuminaMainExtensions
	{
		public static FileResource GetFileEx(this LuminaMain lumina, string path)
		{
			return FileExtensionUtility.GetFile(lumina, path);
		}
	}
}
