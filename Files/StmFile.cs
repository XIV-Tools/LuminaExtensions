// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using Lumina.Data;
	using LuminaExtensions.Extensions;
	using LuminaExtensions.Types;

	using LuminaMain = Lumina.Lumina;

	public class StmFile : FileResource
	{
		public const string GearStainingTemplatePath = "chara/base_material/stainingtemplate.stm";

		private readonly Dictionary<ushort, StainingTemplateEntry> templates = new Dictionary<ushort, StainingTemplateEntry>();

		public enum StainingTemplateArrayType
		{
			Singleton,
			OneToOne,
			Indexed,
		}

		public static StmFile GetFile(LuminaMain lumina)
		{
			return lumina.GetFile<StmFile>(GearStainingTemplatePath);
		}

		public IEnumerable<ushort> GetKeys()
		{
			return this.templates.Keys;
		}

		public StainingTemplateEntry GetTemplate(ushort key)
		{
			StainingTemplateEntry template;
			this.templates.TryGetValue(key, out template);
			return template;
		}

		public override void LoadFile()
		{
			base.LoadFile();

			int signature = this.Reader.ReadInt32();
			ushort entryCount = this.Reader.ReadUInt16();

			this.Reader.BaseStream.Seek(8, System.IO.SeekOrigin.Begin);
			List<ushort> keys = new List<ushort>();
			for (int i = 0; i < entryCount; i++)
			{
				keys.Add(this.Reader.ReadUInt16());
			}

			int endOfHeader = 8 + (4 * entryCount);
			List<ushort> offsets = new List<ushort>();
			for (int i = 0; i < entryCount; i++)
			{
				offsets.Add(this.Reader.ReadUInt16());
			}

			for (int i = 0; i < entryCount; i++)
			{
				ushort key = keys[i];
				int offset = (offsets[i] * 2) + endOfHeader;

				this.Reader.BaseStream.Seek(offset, System.IO.SeekOrigin.Begin);
				this.templates.Add(key, this.ReadTemplate());
			}
		}

		private StainingTemplateEntry ReadTemplate()
		{
			StainingTemplateEntry entry = new StainingTemplateEntry();

			int diffuseEnd = this.Reader.ReadUInt16();
			int specularEnd = this.Reader.ReadUInt16();
			int emissiveEnd = this.Reader.ReadUInt16();
			int specularPowerEnd = this.Reader.ReadUInt16();
			int glossEnd = this.Reader.ReadUInt16();

			int numDiffuse = diffuseEnd;
			int numSpecular = specularEnd - diffuseEnd;
			int numEmissive = emissiveEnd - specularEnd;
			int numSpecularPower = specularPowerEnd - emissiveEnd;
			int numGloss = glossEnd - specularPowerEnd;

			// Get the entries for diffuse
			for (int i = 0; i < numDiffuse; i += 3)
			{
				entry.DiffuseEntries.Add(this.Reader.ReadRgbColorHalf());
			}

			entry.SpecularEntries = this.ReadIndexedArray(numSpecular, 3, Color.Black, this.Reader.ReadRgbColorHalf);
			entry.EmissiveEntries = this.ReadIndexedArray(numEmissive, 3, Color.Black, this.Reader.ReadRgbColorHalf);
			entry.SpecularPowerEntries = this.ReadIndexedArray(numSpecular, 1, Half.Zero, this.Reader.ReadHalf);
			entry.GlossEntries = this.ReadIndexedArray(numGloss, 1, Half.Zero, this.Reader.ReadHalf);

			return entry;
		}

		private List<T> ReadIndexedArray<T>(int size, int elementSize, T defaultValue, Func<T> read)
		{
			if (size < 64)
				return new List<T>();

			int entryBlockSize = size - 64;
			List<T> entries = new List<T>();
			for (int i = 0; i < entryBlockSize; i += elementSize)
			{
				entries.Add(read.Invoke());
			}

			List<T> results = new List<T>();

			// Get the indexed entries for specular
			for (int i = 0; i < 128; i++)
			{
				byte index = this.Reader.ReadByte();

				if (index <= 0 || index > entries.Count)
				{
					results.Add(defaultValue);
				}
				else
				{
					results.Add(entries[index - 1]);
				}
			}

			return results;
		}

		public class StainingTemplateEntry
		{
			public List<Color> DiffuseEntries = new List<Color>();
			public List<Color> SpecularEntries = new List<Color>();
			public List<Color> EmissiveEntries = new List<Color>();
			public List<Half> SpecularPowerEntries = new List<Half>();
			public List<Half> GlossEntries = new List<Half>();
		}
	}
}
