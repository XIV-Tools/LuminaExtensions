// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Lumina.Data;

	using LuminaMain = global::Lumina.Lumina;

	public static class FileExtensionUtility
	{
		private static readonly Dictionary<Type, string> ExtensionLookup = new Dictionary<Type, string>();
		private static readonly Dictionary<string, Type> ResourceLookup = new Dictionary<string, Type>();

		static FileExtensionUtility()
		{
			RegisterExtension<MtrlFile>(".mtrl");
			RegisterExtension<MdlFile>(".mdl");
			RegisterExtension<EqdpFile>(".eqdp");
			RegisterExtension<TexFileEx>(".tex");
			RegisterExtension<StmFile>(".stm");
		}

		public static FileResource GetFile(LuminaMain lumina, string path)
		{
			string extension = Path.GetExtension(path);

			// GetFile is a generic method so we must know what type each extension is ahead of time.
			switch (extension)
			{
				case ".mtrl": return lumina.GetFile<MtrlFile>(path);
				case ".mdl": return lumina.GetFile<MdlFile>(path);
				case ".eqdp": return lumina.GetFile<EqdpFile>(path);
				case ".tex": return lumina.GetFile<TexFileEx>(path);
				case ".stm": return lumina.GetFile<StmFile>(path);
			}

			return lumina.GetFile(path);
		}

		public static Type GetResourceType(string extension)
		{
			Type t;
			if (ResourceLookup.TryGetValue(extension, out t))
				return t;

			throw new Exception($"Unknown file extension: {extension}");
		}

		public static string GetFileExtension(this FileResource self)
		{
			return GetFileExtension(self.GetType());
		}

		public static string GetFileExtension<T>()
			where T : FileResource
		{
			return GetFileExtension(typeof(T));
		}

		public static string GetFileExtension(Type type)
		{
			string extension;
			if (ExtensionLookup.TryGetValue(type, out extension))
				return extension;

			throw new Exception($"No file extension registered for file resource type: {type}");
		}

		private static void RegisterExtension<T>(string extension)
			where T : FileResource
		{
			if (!extension.StartsWith('.'))
				extension = '.' + extension;

			ExtensionLookup.Add(typeof(T), extension);
			ResourceLookup.Add(extension, typeof(T));
		}
	}
}
