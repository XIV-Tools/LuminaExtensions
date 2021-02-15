// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions
{
	using System;

	[Flags]
	public enum ItemSlots
	{
		None = 0,

		Head = 1,
		Body = 2,
		Hands = 4,
		Waist = 8,
		Legs = 16,
		Feet = 32,
		Ears = 64,
		Neck = 128,
		Wrists = 256,
		RightRing = 512,
		LeftRing = 1024,
		MainHand = 2048,
		OffHand = 4096,
		SoulCrystal = 8192,

		Rings = RightRing | LeftRing,
		Equipment = Head | Body | Hands | Waist | Legs | Feet,
		Accessories = Ears | Neck | Wrists | Rings,
		Weapons = MainHand | OffHand,
	}

	#pragma warning disable SA1649
	public static class ItemSlotsExtensions
	{
		public static string ToAbbreviation(this ItemSlots self)
		{
			switch (self)
			{
				case ItemSlots.Head: return "met";
				case ItemSlots.Body: return "top";
				case ItemSlots.Hands: return "glv";
				case ItemSlots.Legs: return "dwn";
				case ItemSlots.Feet: return "sho";
				case ItemSlots.Ears: return "ear";
				case ItemSlots.Neck: return "nek";
				case ItemSlots.Wrists: return "wrs";
				case ItemSlots.RightRing: return "rir";
				case ItemSlots.LeftRing: return "ril";
			}

			throw new Exception($"Missing abbreviation for slot: {self}");
		}

		public static int ToImcIndex(this ItemSlots self)
		{
			switch (self)
			{
				case ItemSlots.Head: return 0;
				case ItemSlots.Body: return 1;
				case ItemSlots.Hands: return 2;
				case ItemSlots.Legs: return 3;
				case ItemSlots.Feet: return 4;

				case ItemSlots.Ears: return 0;
				case ItemSlots.Neck: return 1;
				case ItemSlots.Wrists: return 2;
				case ItemSlots.RightRing: return 3;
				case ItemSlots.LeftRing: return 4;
			}

			throw new Exception($"Invalid slot for Imc: {self}");
		}
	}
}
