// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Excel
{
	using System;
	using Lumina;
	using Lumina.Excel.GeneratedSheets;

	using LuminaMain = global::Lumina.Lumina;

	public class ItemViewModel : ExcelRowViewModel<Item>
	{
		private readonly ushort modelSet;
		private readonly ushort modelBase;
		private readonly ushort modelVariant;

		private readonly ushort subModelSet;
		private readonly ushort subModelBase;
		private readonly ushort subModelVariant;

		private readonly ClassJobCategory classJob;

		public ItemViewModel(Item item, LuminaMain lumina)
			: base(item, lumina)
		{
			this.classJob = this.Value.ClassJobCategory.Value;

			this.GetModel(true, out this.modelSet, out this.modelBase, out this.modelVariant);
			this.GetModel(false, out this.subModelSet, out this.subModelBase, out this.subModelVariant);

			EquipSlotCategory equip = this.Value.EquipSlotCategory.Value;
			this.FitsInSlots = ItemSlots.None;

			for (int i = 1; i < (int)ItemSlots.SoulCrystal; i *= 2)
			{
				this.AddSlotIfFits((ItemSlots)i, equip);
			}
		}

		public string Name => this.Value.Name;
		public string Description => this.Value.Description;
		public ushort Icon => this.Value.Icon;
		public ushort ModelSet => this.modelSet;
		public ushort ModelBase => this.modelBase;
		public ushort ModelVariant => this.modelVariant;
		public bool HasSubModel => this.Value.ModelSub != 0;
		public ushort SubModelSet => this.subModelSet;
		public ushort SubModelBase => this.subModelBase;
		public ushort SubModelVariant => this.subModelVariant;
		public ClassJobCategory ClassJob => this.classJob;
		public Classes EquipableClasses => this.classJob.ToFlags();

		public ItemSlots FitsInSlots { get; private set; }

		public bool IsWeapon => this.FitsInSlot(ItemSlots.MainHand)
			|| this.FitsInSlot(ItemSlots.OffHand);

		public bool IsEquipment => this.FitsInSlot(ItemSlots.Head)
			|| this.FitsInSlot(ItemSlots.Body)
			|| this.FitsInSlot(ItemSlots.Hands)
			|| this.FitsInSlot(ItemSlots.Legs)
			|| this.FitsInSlot(ItemSlots.Feet);

		public bool IsAccessory => this.FitsInSlot(ItemSlots.Ears)
			|| this.FitsInSlot(ItemSlots.Neck)
			|| this.FitsInSlot(ItemSlots.Wrists)
			|| this.FitsInSlot(ItemSlots.LeftRing)
			|| this.FitsInSlot(ItemSlots.RightRing);

		public bool FitsInSlot(ItemSlots slot)
		{
			return this.FitsInSlots.HasFlag(slot);
		}

		private void GetModel(bool main, out ushort modelSet, out ushort modelBase, out ushort modelVariant)
		{
			ulong val = main ? this.Value.ModelMain : this.Value.ModelSub;

			if (this.IsWeapon)
			{
				modelSet = (ushort)val;
				modelBase = (ushort)(val >> 16);
				modelVariant = (ushort)(val >> 32);
			}
			else
			{
				modelSet = 0;
				modelBase = (ushort)val;
				modelVariant = (ushort)(val >> 16);
			}
		}

		private void AddSlotIfFits(ItemSlots slot, EquipSlotCategory equip)
		{
			if (equip.Contains(slot))
			{
				this.FitsInSlots |= slot;
			}
		}
	}
}
