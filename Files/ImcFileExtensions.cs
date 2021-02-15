// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina.Data.Files
{
	using System;
	using LuminaExtensions;

	public static class ImcFileExtensions
	{
		public static ImcFile.ImageChangeParts GetPart(this ImcFile self, ItemSlots slot)
		{
			int index = slot switch
			{
				ItemSlots.Head => 0, // met
				ItemSlots.Body => 1, // top
				ItemSlots.Hands => 2, // glv
				ItemSlots.Legs => 3, // dwn
				ItemSlots.Feet => 4, // sho
				ItemSlots.Ears => 0, // ear
				ItemSlots.Neck => 1, // nek
				ItemSlots.Wrists => 2, // wrs
				ItemSlots.RightRing => 3, // rir
				ItemSlots.LeftRing => 4, // ril

				_ => throw new Exception($"Invalid item slot for Imc: {slot}")
			};

			return self.GetPart(index);
		}

		public static ImcFile.ImageChangeData GetVariant(this ImcFile self, ItemSlots slot, ushort variantId)
		{
			ImcFile.ImageChangeParts parts = self.GetPart(slot);
			return parts.Variants[variantId];
		}
	}
}
