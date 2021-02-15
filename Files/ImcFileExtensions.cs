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
			return self.GetPart(slot.ToImcIndex());
		}

		public static ImcFile.ImageChangeData GetVariant(this ImcFile self, ItemSlots slot, ushort variantId)
		{
			ImcFile.ImageChangeParts parts = self.GetPart(slot);
			return parts.Variants[variantId];
		}
	}
}
