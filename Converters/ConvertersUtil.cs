// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Converters
{
	using System;
	using System.Collections.Generic;
	using Lumina.Data;

	public static class ConvertersUtil
	{
		private static readonly List<ConverterBase> AllConverters = new List<ConverterBase>();

		static ConvertersUtil()
		{
			foreach (Type t in typeof(ConverterBase).Assembly.GetTypes())
			{
				if (t.IsAbstract)
					continue;

				if (!t.IsSubclassOf(typeof(ConverterBase)))
					continue;

				ConverterBase converter = (ConverterBase)Activator.CreateInstance(t);
				AllConverters.Add(converter);
			}
		}

		public static ConverterBase[] GetConverters<TFile>()
			where TFile : FileResource
		{
			return GetConverters(typeof(TFile));
		}

		public static ConverterBase[] GetConverters(Type fileType)
		{
			if (!fileType.IsSubclassOf(typeof(FileResource)))
				throw new Exception("Converters are only valid for FileResources");

			List<ConverterBase> results = new List<ConverterBase>();

			foreach (ConverterBase converter in AllConverters)
			{
				if (converter.CanConvert(fileType))
				{
					results.Add(converter);
				}
			}

			return results.ToArray();
		}
	}
}
