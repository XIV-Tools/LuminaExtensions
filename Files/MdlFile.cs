// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Lumina.Data;

	public class MdlFile : FileResource
	{
		public List<string> Attributes { get; set; } = new List<string>();
		public List<string> Bones { get; set; } = new List<string>();
		public List<string> Materials { get; set; } = new List<string>();
		public List<string> Shapes { get; set; } = new List<string>();
		public List<string> ExtraPaths { get; set; } = new List<string>();

		// See XivModdingFramework Mdl.cs GetRawMdlData method for reference
		// https://github.com/TexTools/xivModdingFramework/blob/master/xivModdingFramework/Models/FileTypes/Mdl.cs#L417
		public override void LoadFile()
		{
			base.LoadFile();

			// not sure what the first 12 bytes are for
			this.Reader.BaseStream.Seek(12, SeekOrigin.Begin);

			ushort meshCount = this.Reader.ReadUInt16();
			ushort materialCount = this.Reader.ReadUInt16();

			// We skip the Vertex Data Structures for now
			// This is done so that we can get the correct number of meshes per LoD first
			int pathDataOffset = 64 + (136 * meshCount) + 4;
			this.Reader.BaseStream.Seek(pathDataOffset, SeekOrigin.Begin);
			int pathCount = this.Reader.ReadInt32();
			int pathBlockSize = this.Reader.ReadInt32();

			// Read all paths and attributes
			this.Reader.BaseStream.Seek(pathDataOffset + 8, SeekOrigin.Begin);
			List<string> paths = new List<string>();
			for (int i = 0; i < pathCount; i++)
			{
				byte a;
				List<byte> bytes = new List<byte>();
				while ((a = this.Reader.ReadByte()) != 0)
				{
					bytes.Add(a);
				}

				paths.Add(Encoding.ASCII.GetString(bytes.ToArray()).Replace("\0", string.Empty));
			}

			// Ensure we are at the end of the path block
			int modelDataOffset = pathDataOffset + pathBlockSize + 8;
			this.Reader.BaseStream.Seek(modelDataOffset, SeekOrigin.Begin);
			int unknown0 = this.Reader.ReadInt32();
			short meshCount2 = this.Reader.ReadInt16();
			short attributeCount = this.Reader.ReadInt16();
			short meshPartCount = this.Reader.ReadInt16();
			short materialCount2 = this.Reader.ReadInt16();
			short boneCount = this.Reader.ReadInt16();
			short boneListCount = this.Reader.ReadInt16();
			short shapeCount = this.Reader.ReadInt16();
			short shapePartCount = this.Reader.ReadInt16();
			ushort shapeDataCount = this.Reader.ReadUInt16();
			short unknown1 = this.Reader.ReadInt16();
			short unknown2 = this.Reader.ReadInt16();
			short unknown3 = this.Reader.ReadInt16();
			short unknown4 = this.Reader.ReadInt16();
			short unknown5 = this.Reader.ReadInt16();
			short unknown6 = this.Reader.ReadInt16();
			short unknown7 = this.Reader.ReadInt16();
			short unknown8 = this.Reader.ReadInt16(); // Used for transform count with furniture
			short unknown9 = this.Reader.ReadInt16();
			byte unknown10a = this.Reader.ReadByte();
			byte unknown10b = this.Reader.ReadByte();
			short unknown11 = this.Reader.ReadInt16();
			short unknown12 = this.Reader.ReadInt16();
			short unknown13 = this.Reader.ReadInt16();
			short unknown14 = this.Reader.ReadInt16();
			short unknown15 = this.Reader.ReadInt16();
			short unknown16 = this.Reader.ReadInt16();
			short unknown17 = this.Reader.ReadInt16();

			// Now that we have the paths and the data, unpack the paths in order
			int index = 0;
			index = paths.CopyTo(this.Attributes, index, attributeCount);
			index = paths.CopyTo(this.Bones, index, boneCount);
			index = paths.CopyTo(this.Materials, index, materialCount);
			index = paths.CopyTo(this.Shapes, index, shapeCount);
			index = paths.CopyTo(this.ExtraPaths, index);
		}
	}
}
