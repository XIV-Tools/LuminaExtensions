// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina.Data.Files
{
	using LuminaExtensions;

	public static class ImcFileExtensions
	{
		public static ImcFile.ImageChangeParts GetPart(this ImcFile self, ItemSlots slot)
		{
			return self.GetPart(slot.ToImcIndex());
		}

		public static bool HasVariant(this ImcFile self, ItemSlots slot, ushort variantId)
		{
			ImcFile.ImageChangeParts parts = self.GetPart(slot);

			if (variantId < 0 || variantId > parts.Variants.Length)
				return false;

			return true;
		}

		public static ImcFile.ImageChangeData GetVariant(this ImcFile self, ItemSlots slot, ushort variantId)
		{
			ImcFile.ImageChangeParts parts = self.GetPart(slot);
			return parts.Variants[variantId];
		}

		public static bool TryGetVariant(this ImcFile self, ItemSlots slot, ushort variantId, out ImcFile.ImageChangeData variant)
		{
			ImcFile.ImageChangeParts parts = self.GetPart(slot);
			variant = default;

			if (variantId < 0 || variantId > parts.Variants.Length)
				return false;

			variant = parts.Variants[variantId];
			return true;
		}

		public static string GetMaterialKey(this ImcFile.ImageChangeData self)
		{
			return "v" + (self.MaterialId - 1).ToString().PadLeft(4, '0');
		}
	}
}
