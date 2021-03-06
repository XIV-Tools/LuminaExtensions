// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Numerics;
	using System.Text;
	using Lumina.Data;
	using LuminaExtensions.Extensions;
	using LuminaExtensions.Files.Mtrl;
	using LuminaExtensions.Types;

	public class MtrlFile : FileResource
	{
		public readonly List<string> TexturePaths = new List<string>();
		public readonly List<string> MapNames = new List<string>();
		public readonly List<string> ColorSetNames = new List<string>();
		public readonly ColorSet ColorSet = new ColorSet();
		public readonly List<TextureUsageStruct> TextureUsage = new List<TextureUsageStruct>();

		public string? ShaderName;

		/// <summary>
		/// Gets or sets the shader number used by the item.
		/// </summary>
		/// <remarks>
		/// This is a guess and has not been tested to be true
		/// Seems to be more likely that this is some base argument passed into the shader.
		/// </remarks>
		public ushort ShaderNumber { get; set; }

		/// <summary>
		/// Gets or sets the list of Type 2 data structures.
		/// </summary>
		public List<ShaderParameterStruct> ShaderParameterList { get; set; } = new List<ShaderParameterStruct>();

		/// <summary>
		/// Gets or sets the list of Parameter data structures.
		/// </summary>
		public List<TextureDescriptorStruct> TextureDescriptorList { get; set; } = new List<TextureDescriptorStruct>();

		/// <summary>
		/// Gets the number of type 1 data sturctures.
		/// </summary>
		public ushort TextureUsageCount => (ushort)this.TextureUsage.Count;

		/// <summary>
		/// Gets the number of type 2 data structures.
		/// </summary>
		public ushort ShaderParameterCount => (ushort)this.ShaderParameterList.Count;

		/// <summary>
		/// Gets the number of parameter stuctures.
		/// </summary>
		public ushort TextureDescriptorCount => (ushort)this.TextureDescriptorList.Count;

		/// <summary>
		/// Gets the size of the additional MTRL Data.
		/// </summary>
		public ushort ShaderParameterDataSize
		{
			get
			{
				int size = 0;
				this.ShaderParameterList.ForEach(x =>
				{
					if (x.Args == null)
						return;

					size += x.Args.Count * 4;
				});

				return (ushort)size;
			}
		}

		public override void LoadFile()
		{
			base.LoadFile();

			this.Reader.BaseStream.Position = 0;

			int signature = this.Reader.ReadInt32();
			int fileSize = this.Reader.ReadInt16();

			ushort colorSetDataSize = this.Reader.ReadUInt16();
			ushort materialDataSize = this.Reader.ReadUInt16();
			ushort texturePathsDataSize = this.Reader.ReadUInt16();
			byte textureCount = this.Reader.ReadByte();
			byte mapCount = this.Reader.ReadByte();
			byte colorSetCount = this.Reader.ReadByte();
			byte unknownDataSize = this.Reader.ReadByte();

			int stringCount = textureCount + mapCount + colorSetCount;
			long stringBlockOffset = 10 + (stringCount * 2);

			// TODO: Dont skip the offsets! some materials have empty texture paths in them, and
			// we need to handle those cases.
			// Skip over the path offsets and just read all the string at once
			this.Reader.BaseStream.Seek(stringBlockOffset, System.IO.SeekOrigin.Current);
			List<string> strings = new List<string>();
			for (int i = 0; i < stringCount; i++)
			{
				string str = this.Reader.ReadTerminatedString();
				strings.Add(str);
			}

			int index = 0;
			index = strings.CopyTo(this.TexturePaths, index, textureCount);
			index = strings.CopyTo(this.MapNames, index, mapCount);
			index = strings.CopyTo(this.ColorSetNames, index, colorSetCount);

			int shaderNameSize = materialDataSize - texturePathsDataSize;
			this.ShaderName = this.Reader.ReadTerminatedString(shaderNameSize);

			if (!this.ShaderName.EndsWith(".shpk"))
				throw new Exception("Failed to read valid shader name");

			byte[] unknown2 = this.Reader.ReadBytes(unknownDataSize);

			if (colorSetDataSize > 0)
			{
				for (int x = 0; x < 16; x++)
				{
					this.ColorSet.Rows[x] = new ColorSet.Row();
					this.ColorSet.Rows[x].Diffuse = this.Reader.ReadRgbColorHalf();
					this.ColorSet.Rows[x].SpecularPower = this.Reader.ReadHalf();
					this.ColorSet.Rows[x].Specular = this.Reader.ReadRgbColorHalf();
					this.ColorSet.Rows[x].Gloss = this.Reader.ReadHalf();
					this.ColorSet.Rows[x].Emissive = this.Reader.ReadRgbColorHalf();

					// Tile material is a half, but is floored into one of 64 possible values.
					this.ColorSet.Rows[x].TileMaterial = (byte)Math.Floor(this.Reader.ReadHalf() * 64.0);

					// tile transform is in X, SkewX, SkewY, Y.
					Half tileX = this.Reader.ReadHalf();
					Half tileSkewX = this.Reader.ReadHalf();
					Half tileSkewY = this.Reader.ReadHalf();
					Half tileY = this.Reader.ReadHalf();

					this.ColorSet.Rows[x].Tile = new Vector2(tileX, tileY);
					this.ColorSet.Rows[x].TileSkew = new Vector2(tileSkewX, tileSkewY);
				}

				// If the color set is 544 in length, it has dye info for each row.
				if (colorSetDataSize == 544)
				{
					for (int x = 0; x < 16; x++)
					{
						ushort dyeData = this.Reader.ReadUInt16();

						this.ColorSet.Rows[x].DyeTemplate = (ushort)(dyeData >> 5);

						this.ColorSet.Rows[x].DyeFlag |= (dyeData & 1) != 0 ? ColorSet.Row.DyeFlags.Diffuse : 0;
						this.ColorSet.Rows[x].DyeFlag |= (dyeData & 2) != 0 ? ColorSet.Row.DyeFlags.Specular : 0;
						this.ColorSet.Rows[x].DyeFlag |= (dyeData & 4) != 0 ? ColorSet.Row.DyeFlags.Emissive : 0;
						this.ColorSet.Rows[x].DyeFlag |= (dyeData & 8) != 0 ? ColorSet.Row.DyeFlags.Gloss : 0;

						// TexTools has specular power as 0x10 instead of 1x16 but that doesn't make sense, so its 0x16 here.
						this.ColorSet.Rows[x].DyeFlag |= (dyeData & 16) != 0 ? ColorSet.Row.DyeFlags.SpecularPower : 0;
					}
				}
			}

			ushort originalShaderParameterDataSize = this.Reader.ReadUInt16();
			ushort textureUsageCount = this.Reader.ReadUInt16();
			ushort originalShaderParameterCount = this.Reader.ReadUInt16();
			ushort originalTextureDescriptorCount = this.Reader.ReadUInt16();

			this.ShaderNumber = this.Reader.ReadUInt16();
			ushort unknown3 = this.Reader.ReadUInt16();

			for (int i = 0; i < textureUsageCount; i++)
			{
				TextureUsageStruct usage = new TextureUsageStruct();
				usage.TextureType = (TextureUsageStruct.Types)this.Reader.ReadUInt32();
				uint unknown = this.Reader.ReadUInt32();
				this.TextureUsage.Add(usage);
			}

			this.ShaderParameterList = new List<ShaderParameterStruct>(originalShaderParameterCount);
			for (int i = 0; i < originalShaderParameterCount; i++)
			{
				this.ShaderParameterList.Add(new ShaderParameterStruct
				{
					ParameterID = (ShaderParameterStruct.MtrlShaderParameterId)this.Reader.ReadUInt32(),
					Offset = this.Reader.ReadInt16(),
					Size = this.Reader.ReadInt16(),
				});
			}

			this.TextureDescriptorList = new List<TextureDescriptorStruct>(originalTextureDescriptorCount);
			for (int i = 0; i < originalTextureDescriptorCount; i++)
			{
				this.TextureDescriptorList.Add(new TextureDescriptorStruct
				{
					TextureType = this.Reader.ReadUInt32(),
					FileFormat = this.Reader.ReadInt16(),
					Unknown = this.Reader.ReadInt16(),
					TextureIndex = this.Reader.ReadUInt32(),
				});
			}

			int bytesRead = 0;
			foreach (ShaderParameterStruct shaderParam in this.ShaderParameterList)
			{
				short offset = shaderParam.Offset;
				short size = shaderParam.Size;
				shaderParam.Args = new List<float>();
				if (bytesRead + size <= originalShaderParameterDataSize)
				{
					for (short idx = offset; idx < offset + size; idx += 4)
					{
						float arg = this.Reader.ReadSingle();
						shaderParam.Args.Add(arg);
						bytesRead += 4;
					}
				}
				else
				{
					// Just use a blank array if we have missing/invalid shader data.
					shaderParam.Args = new List<float>(new float[size / 4]);
				}
			}

			// Chew through any remaining padding.
			while (bytesRead < originalShaderParameterDataSize)
			{
				this.Reader.ReadByte();
				bytesRead++;
			}
		}
	}
}
