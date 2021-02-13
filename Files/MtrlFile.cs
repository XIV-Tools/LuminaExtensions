// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina.Data.Files
{
	using System.Collections.Generic;
	using System.Text;
	using LuminaExtensions.Files.Mtrl;
	using LuminaExtensions.Types;

	public class MtrlFile : FileResource
	{
		/// <summary>
		/// Gets or sets the MTRL file signature.
		/// </summary>
		/// <remarks>
		/// 0x00000301 (16973824).
		/// </remarks>
		public int Signature { get; set; }

		/// <summary>
		/// Gets or sets the size of the MTRL file.
		/// </summary>
		public short FileSize { get; set; }

		/// <summary>
		/// Gets or sets the size of the thiserial Data section.
		/// </summary>
		/// <remarks>
		/// This is the size of the data chunk containing all of the path and filename strings.
		/// </remarks>
		public ushort MaterialDataSize { get; set; }

		/// <summary>
		/// Gets or sets the size of the Texture Path Data section.
		/// </summary>
		/// <remarks>
		/// This is the size of the data chucnk containing only the texture paths.
		/// </remarks>
		public ushort TexturePathsDataSize { get; set; }

		/// <summary>
		/// Gets or sets the number of textures paths in the mtrl.
		/// </summary>
		public byte TextureCount { get; set; }

		/// <summary>
		/// Gets or sets the number of map paths in the mtrl.
		/// </summary>
		public byte MapCount { get; set; }

		/// <summary>
		/// Gets or sets the amount of color sets in the mtrl.
		/// </summary>
		/// <remarks>
		/// It is not known if there are any instances where this is greater than 1.
		/// </remarks>
		public byte ColorSetCount { get; set; }

		/// <summary>
		/// Gets or sets the number of bytes to skip after path section.
		/// </summary>
		public byte UnknownDataSize { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Texture Path offsets.
		/// </summary>
		public List<int> TexturePathOffsetList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Texture Path Unknowns.
		/// </summary>
		public List<short> TexturePathUnknownList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Map Path offsets.
		/// </summary>
		public List<int> MapPathOffsetList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Map Path Unknowns.
		/// </summary>
		public List<short> MapPathUnknownList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the ColorSet Path offsets.
		/// </summary>
		public List<int> ColorSetPathOffsetList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the ColorSet Path Unknowns.
		/// </summary>
		public List<short> ColorSetPathUnknownList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Texture Path strings.
		/// </summary>
		public List<string> TexturePaths { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Map Path strings.
		/// </summary>
		public List<string> MapPathList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the ColorSet Path strings.
		/// </summary>
		public List<string> ColorSetPathList { get; set; }

		/// <summary>
		/// Gets or sets the name of the shader used by the item.
		/// </summary>
		public string Shader { get; set; }

		public byte[] Unknown2 { get; set; }
		public ushort Unknown3 { get; set; }

		/// <summary>
		/// Gets or sets the list of half floats containing the ColorSet data.
		/// </summary>
		public List<Half> ColorSetData { get; set; }

		/// <summary>
		/// Gets or sets the byte array containing the extra ColorSet data.
		/// </summary>
		public byte[] ColorSetDyeData { get; set; }

		/// <summary>
		/// Gets or sets the shader number used by the item.
		/// </summary>
		/// <remarks>
		/// This is a guess and has not been tested to be true
		/// Seems to be more likely that this is some base argument passed into the shader.
		/// </remarks>
		public ushort ShaderNumber { get; set; }

		/// <summary>
		/// Gets or sets the list of Type 1 data structures.
		/// </summary>
		public List<TextureUsageStruct> TextureUsageList { get; set; }

		/// <summary>
		/// Gets or sets the list of Type 2 data structures.
		/// </summary>
		public List<ShaderParameterStruct> ShaderParameterList { get; set; }

		/// <summary>
		/// Gets or sets the list of Parameter data structures.
		/// </summary>
		public List<TextureDescriptorStruct> TextureDescriptorList { get; set; }

		/// <summary>
		/// Gets the number of type 1 data sturctures.
		/// </summary>
		public ushort TextureUsageCount => (ushort)this.TextureUsageList.Count;

		/// <summary>
		/// Gets the number of type 2 data structures.
		/// </summary>
		public ushort ShaderParameterCount => (ushort)this.ShaderParameterList.Count;

		/// <summary>
		/// Gets the number of parameter stuctures.
		/// </summary>
		public ushort TextureDescriptorCount => (ushort)this.TextureDescriptorList.Count;

		/// <summary>
		/// Gets the size of the ColorSet Data section.
		/// </summary>
		/// <remarks>
		/// Can be 0 if there is no ColorSet Data.
		/// </remarks>
		public ushort ColorSetDataSize
		{
			get
			{
				int size = this.ColorSetData.Count * 2;
				size += this.ColorSetDyeData == null ? 0 : this.ColorSetDyeData.Length;
				return (ushort)size;
			}
		}

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
					size += x.Args.Count * 4;
				});

				return (ushort)size;
			}
		}

		public override void LoadFile()
		{
			this.Reader.BaseStream.Position = 0;

			int signature = this.Reader.ReadInt32();
			int fileSize = this.Reader.ReadInt16();

			ushort colorSetDataSize = this.Reader.ReadUInt16();
			this.MaterialDataSize = this.Reader.ReadUInt16();
			this.TexturePathsDataSize = this.Reader.ReadUInt16();
			this.TextureCount = this.Reader.ReadByte();
			this.MapCount = this.Reader.ReadByte();
			this.ColorSetCount = this.Reader.ReadByte();
			this.UnknownDataSize = this.Reader.ReadByte();

			List<int> pathSizeList = new List<int>();

			// get the texture path offsets
			this.TexturePathOffsetList = new List<int>(this.TextureCount);
			this.TexturePathUnknownList = new List<short>(this.TextureCount);
			for (int i = 0; i < this.TextureCount; i++)
			{
				this.TexturePathOffsetList.Add(this.Reader.ReadInt16());
				this.TexturePathUnknownList.Add(this.Reader.ReadInt16());

				// add the size of the paths
				if (i > 0)
				{
					pathSizeList.Add(this.TexturePathOffsetList[i] - this.TexturePathOffsetList[i - 1]);
				}
			}

			// get the map path offsets
			this.MapPathOffsetList = new List<int>(this.MapCount);
			this.MapPathUnknownList = new List<short>(this.MapCount);
			for (int i = 0; i < this.MapCount; i++)
			{
				this.MapPathOffsetList.Add(this.Reader.ReadInt16());
				this.MapPathUnknownList.Add(this.Reader.ReadInt16());

				// add the size of the paths
				if (i > 0)
				{
					pathSizeList.Add(this.MapPathOffsetList[i] - this.MapPathOffsetList[i - 1]);
				}
				else
				{
					if (this.TextureCount > 0)
					{
						pathSizeList.Add(this.MapPathOffsetList[i] -
										 this.TexturePathOffsetList[this.TextureCount - 1]);
					}
				}
			}

			// get the color set offsets
			this.ColorSetPathOffsetList = new List<int>(this.ColorSetCount);
			this.ColorSetPathUnknownList = new List<short>(this.ColorSetCount);
			for (int i = 0; i < this.ColorSetCount; i++)
			{
				this.ColorSetPathOffsetList.Add(this.Reader.ReadInt16());
				this.ColorSetPathUnknownList.Add(this.Reader.ReadInt16());

				// add the size of the paths
				if (i > 0)
				{
					pathSizeList.Add(this.ColorSetPathOffsetList[i] -
									 this.ColorSetPathOffsetList[i - 1]);
				}
				else
				{
					pathSizeList.Add(this.ColorSetPathOffsetList[i] -
									 this.MapPathOffsetList[this.MapCount - 1]);
				}
			}

			pathSizeList.Add(this.TexturePathsDataSize -
							 this.ColorSetPathOffsetList[this.ColorSetCount - 1]);

			int count = 0;

			// get the texture path strings
			this.TexturePaths = new List<string>(this.TextureCount);
			for (int i = 0; i < this.TextureCount; i++)
			{
				string texturePath = Encoding.UTF8.GetString(this.Reader.ReadBytes(pathSizeList[count]));
				texturePath = texturePath.Replace("\0", string.Empty);

				if (string.IsNullOrEmpty(texturePath))
					continue;

				////string dx11FileName = Path.GetFileName(texturePath).Insert(0, "--");
				////if (await index.FileExists(Path.GetDirectoryName(texturePath).Replace("\\", "/") + "/" + dx11FileName, df))
				////{
				////texturePath = texturePath.Insert(texturePath.LastIndexOf("/") + 1, "--");
				////}

				this.TexturePaths.Add(texturePath);
				count++;
			}

			// get the map path strings
			this.MapPathList = new List<string>(this.MapCount);
			for (int i = 0; i < this.MapCount; i++)
			{
				this.MapPathList.Add(Encoding.UTF8.GetString(this.Reader.ReadBytes(pathSizeList[count]))
					.Replace("\0", string.Empty));
				count++;
			}

			// get the color set path strings
			this.ColorSetPathList = new List<string>(this.ColorSetCount);
			for (int i = 0; i < this.ColorSetCount; i++)
			{
				this.ColorSetPathList.Add(Encoding.UTF8.GetString(this.Reader.ReadBytes(pathSizeList[count]))
					.Replace("\0", string.Empty));
				count++;
			}

			int shaderPathSize = this.MaterialDataSize - this.TexturePathsDataSize;

			this.Shader = Encoding.UTF8.GetString(this.Reader.ReadBytes(shaderPathSize)).Replace("\0", string.Empty);
			this.Unknown2 = this.Reader.ReadBytes(this.UnknownDataSize);

			this.ColorSetData = new List<Half>();
			this.ColorSetDyeData = null;
			if (colorSetDataSize > 0)
			{
				// Color Data is always 512 (6 x 14 = 64 x 8bpp = 512)
				int colorDataSize = 512;

				for (int i = 0; i < colorDataSize / 2; i++)
				{
					this.ColorSetData.Add(new Half(this.Reader.ReadUInt16()));
				}

				// If the color set is 544 in length, it has an extra 32 bytes at the end
				if (colorSetDataSize == 544)
				{
					this.ColorSetDyeData = this.Reader.ReadBytes(32);
				}
			}

			ushort originalShaderParameterDataSize = this.Reader.ReadUInt16();
			ushort originalTextureUsageCount = this.Reader.ReadUInt16();
			ushort originalShaderParameterCount = this.Reader.ReadUInt16();
			ushort originalTextureDescriptorCount = this.Reader.ReadUInt16();

			this.ShaderNumber = this.Reader.ReadUInt16();
			this.Unknown3 = this.Reader.ReadUInt16();

			this.TextureUsageList = new List<TextureUsageStruct>((int)originalTextureUsageCount);
			for (int i = 0; i < originalTextureUsageCount; i++)
			{
				this.TextureUsageList.Add(new TextureUsageStruct
				{
					TextureType = this.Reader.ReadUInt32(),
					Unknown = this.Reader.ReadUInt32(),
				});
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

			base.LoadFile();
		}
	}
}
