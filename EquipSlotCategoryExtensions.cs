// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina.Excel.GeneratedSheets
{
	using LuminaExtensions;

	public static class EquipSlotCategoryExtensions
	{
		public static bool Contains(this EquipSlotCategory self, ItemSlots slot)
		{
			switch (slot)
			{
				case ItemSlots.MainHand: return self.MainHand != 0;
				case ItemSlots.Head: return self.Head != 0;
				case ItemSlots.Body: return self.Body != 0;
				case ItemSlots.Hands: return self.Gloves != 0;
				case ItemSlots.Waist: return self.Waist != 0;
				case ItemSlots.Legs: return self.Legs != 0;
				case ItemSlots.Feet: return self.Feet != 0;
				case ItemSlots.OffHand: return self.OffHand != 0;
				case ItemSlots.Ears: return self.Ears != 0;
				case ItemSlots.Neck: return self.Neck != 0;
				case ItemSlots.Wrists: return self.Wrists != 0;
				case ItemSlots.RightRing: return self.FingerR != 0;
				case ItemSlots.LeftRing: return self.FingerL != 0;
				case ItemSlots.SoulCrystal: return self.SoulCrystal != 0;
			}

			return false;
		}
	}
}
