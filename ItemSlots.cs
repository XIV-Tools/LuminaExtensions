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
	}
}
