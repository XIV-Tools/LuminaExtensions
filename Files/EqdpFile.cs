// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.IO;
	using Lumina.Data;

	public class EqdpFile : FileResource
	{
		// Full EQDP entries are 2 bytes long.
		public const int EquipmentDeformerParameterEntrySize = 2;

		public EquipmentDeformationParameter GetParameters(int setId, bool accessory, ItemSlots slot)
		{
			int offset = this.GetEntryOffset(setId);
			ushort param = 0;

			if (offset >= 0)
			{
				this.Reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				param = this.Reader.ReadUInt16();
			}

			int index = slot.ToImcIndex();

			// this is some crazy bit-shifting nonsense that I am not going to touch.
			for (int idx = 0; idx < 5; idx++)
			{
				EquipmentDeformationParameter? entry = new EquipmentDeformationParameter();

				entry.bit0 = (param & 1) != 0;
				param = (ushort)(param >> 1);
				entry.bit1 = (param & 1) != 0;
				param = (ushort)(param >> 1);

				if (idx == index)
				{
					return entry;
				}
			}

			throw new Exception($"Failed to get deformation parameter for set: {setId} and slot {slot}");
		}

		/// <summary>
		/// Resolves the entry offset to a given SetId within an EQDP File.
		/// </summary>
		private int GetEntryOffset(int setId)
		{
			const ushort basicHeaderSize = 6;
			const ushort blockHeaderSize = 2;

			ushort unknown = this.Reader.ReadUInt16();
			ushort blockSize = this.Reader.ReadUInt16();
			ushort blockCount = this.Reader.ReadUInt16();

			int headerEntryId = setId / blockSize;
			int subEntryId = setId % blockSize;

			// Offset to the block table entry.
			int headerEntryOffset = basicHeaderSize + (blockHeaderSize * (setId / blockSize));

			// This gets us the offset after the full header to the start of the data block.
			this.Reader.BaseStream.Seek(headerEntryOffset, SeekOrigin.Begin);
			ushort baseDataOffset = this.Reader.ReadUInt16();

			// If the data offset is MAX-SHORT, that data block was omitted from the file.
			if (baseDataOffset == 65535) return -1;

			// 6 Byte basic header, then block table.
			int fullHeaderLength = basicHeaderSize + (blockHeaderSize * blockCount);

			// Start of the data block our entry lives in.
			int blockStart = fullHeaderLength + (baseDataOffset * EquipmentDeformerParameterEntrySize);

			// Then move the appropriate number of entries in.
			int dataOffset = blockStart + (subEntryId * EquipmentDeformerParameterEntrySize);

			return dataOffset;
		}

		/// <summary>
		/// Class representing an Equipment Deformation parameter for a given race/slot.
		/// </summary>
		public class EquipmentDeformationParameter
		{
			public bool bit0;
			public bool bit1;
		}
	}
}
