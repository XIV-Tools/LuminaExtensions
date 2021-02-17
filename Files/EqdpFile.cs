// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.IO;
	using Lumina.Data;

	public class EqdpFile : FileResource
	{
		[Flags]
		private enum Flags : ushort
		{
			None = 0,

			HeadOrEars = 1,
			HeadOrEarsB = 2,
			BodyOrNeck = 4,
			BodyOrNeckB = 8,
			HandsOrWrists = 16,
			HandsOrWristsB = 32,
			LegsOrRightRing = 64,
			LegsOrRightRingB = 128,
			FeetOrLeftRing = 256,
			FeetOrLeftRingB = 512,
			Unknown = 1024,

			All = HeadOrEars | HeadOrEarsB | BodyOrNeck | BodyOrNeckB | HandsOrWrists | HandsOrWristsB | LegsOrRightRing | LegsOrRightRingB | FeetOrLeftRing | FeetOrLeftRingB,
		}

		public bool IsSet(int setId, ItemSlots slot)
		{
			int offset = this.GetEntryOffset(setId);

			if (offset <= 0)
				return false;

			if (offset >= 0)
			{
				this.Reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				Flags flags = (Flags)this.Reader.ReadUInt16();

				Flags flag = SlotToFlag(slot);
				return flags.HasFlag(flag);
			}

			throw new Exception($"Failed to get deformation parameter for set: {setId} and slot {slot}");
		}

		private static Flags SlotToFlag(ItemSlots slot)
		{
			switch (slot)
			{
				case ItemSlots.None: return Flags.None;
				case ItemSlots.Head: return Flags.HeadOrEars;
				case ItemSlots.Body: return Flags.BodyOrNeck;
				case ItemSlots.Hands: return Flags.HandsOrWrists;
				case ItemSlots.Legs: return Flags.LegsOrRightRing;
				case ItemSlots.Feet: return Flags.FeetOrLeftRing;
				case ItemSlots.Ears: return Flags.HeadOrEars;
				case ItemSlots.Neck: return Flags.BodyOrNeck;
				case ItemSlots.Wrists: return Flags.HandsOrWrists;
				case ItemSlots.RightRing: return Flags.LegsOrRightRing;
				case ItemSlots.LeftRing: return Flags.FeetOrLeftRing;
			}

			throw new Exception($"Unsupported item slot: {slot}");
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
			int blockStart = fullHeaderLength + (baseDataOffset * 2);

			// Then move the appropriate number of entries in.
			int dataOffset = blockStart + (subEntryId * 2);

			return dataOffset;
		}
	}
}
