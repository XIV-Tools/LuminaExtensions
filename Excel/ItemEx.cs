// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Excel
{
	using System;
	using Lumina.Data;
	using Lumina.Data.Files;
	using Lumina.Excel;
	using Lumina.Excel.GeneratedSheets;
	using Lumina.Extensions;
	using Lumina.Text;

	[Sheet("Item", columnHash: 0xa4a9422a)]
	public class ItemEx : IExcelRow
	{
		private Lumina.Lumina? lumina;

		public SeString? Singular { get; private set; }
		public sbyte Adjective { get; private set; }
		public SeString? Plural { get; private set; }
		public sbyte PossessivePronoun { get; private set; }
		public sbyte StartsWithVowel { get; private set; }
		public sbyte Pronoun { get; private set; }
		public sbyte Article { get; private set; }
		public SeString? Description { get; private set; }
		public SeString? Name { get; private set; }
		public ushort Icon { get; private set; }
		public LazyRow<ItemLevel>? LevelItem { get; private set; }
		public byte Rarity { get; private set; }
		public byte FilterGroup { get; private set; }
		public uint AdditionalData { get; private set; }
		public LazyRow<ItemUICategory>? ItemUICategory { get; private set; }
		public LazyRow<ItemSearchCategory>? ItemSearchCategory { get; private set; }
		public LazyRow<EquipSlotCategory>? EquipSlotCategory { get; private set; }
		public LazyRow<ItemSortCategory>? ItemSortCategory { get; private set; }
		public uint StackSize { get; private set; }
		public bool IsUnique { get; private set; }
		public bool IsUntradable { get; private set; }
		public bool IsIndisposable { get; private set; }
		public bool Lot { get; private set; }
		public uint PriceMid { get; private set; }
		public uint PriceLow { get; private set; }
		public bool CanBeHq { get; private set; }
		public bool IsDyeable { get; private set; }
		public bool IsCrestWorthy { get; private set; }
		public LazyRow<ItemAction>? ItemAction { get; private set; }
		public ushort Cooldowns { get; private set; }
		public LazyRow<ClassJob>? ClassJobRepair { get; private set; }
		public LazyRow<Item>? ItemRepair { get; private set; }
		public LazyRow<Item>? ItemGlamour { get; private set; }
		public ushort Desynth { get; private set; }
		public bool IsCollectable { get; private set; }
		public bool AlwaysCollectable { get; private set; }
		public ushort AetherialReduce { get; private set; }
		public byte LevelEquip { get; private set; }
		public byte EquipRestriction { get; private set; }
		public LazyRow<ClassJobCategory>? ClassJobCategory { get; private set; }
		public LazyRow<GrandCompany>? GrandCompany { get; private set; }
		public LazyRow<ItemSeries>? ItemSeries { get; private set; }
		public byte BaseParamModifier { get; private set; }
		public LazyRow<ClassJob>? ClassJobUse { get; private set; }
		public ushort DamagePhys { get; private set; }
		public ushort DamageMag { get; private set; }
		public ushort Delayms { get; private set; }
		public ushort BlockRate { get; private set; }
		public ushort Block { get; private set; }
		public ushort DefensePhys { get; private set; }
		public ushort DefenseMag { get; private set; }
		public LazyRow<ItemSpecialBonus>? ItemSpecialBonus { get; private set; }
		public byte ItemSpecialBonusParam { get; private set; }
		public byte MaterializeType { get; private set; }
		public byte MateriaSlotCount { get; private set; }
		public bool IsAdvancedMeldingPermitted { get; private set; }
		public bool IsPvP { get; private set; }
		public bool IsGlamourous { get; private set; }

		public ItemModelBase? Model { get; private set; }
		public ItemModelBase? SubModel { get; private set; }

		public Classes EquipableClasses { get; private set; }
		public ItemSlots FitsInSlots { get; private set; }

		public TexFile? IconFile => this.lumina?.GetIcon(this.Icon);

		public uint RowId { get; set; }
		public uint SubRowId { get; set; }

		public void PopulateData(RowParser parser, Lumina.Lumina lumina, Language language)
		{
			this.lumina = lumina;

			this.RowId = parser.Row;
			this.SubRowId = parser.SubRow;

			this.Singular = parser.ReadColumn<SeString>(0);
			this.Adjective = parser.ReadColumn<sbyte>(1);
			this.Plural = parser.ReadColumn<SeString>(2);
			this.PossessivePronoun = parser.ReadColumn<sbyte>(3);
			this.StartsWithVowel = parser.ReadColumn<sbyte>(4);
			this.Pronoun = parser.ReadColumn<sbyte>(6);
			this.Article = parser.ReadColumn<sbyte>(7);
			this.Description = parser.ReadColumn<SeString>(8);
			this.Name = parser.ReadColumn<SeString>(9);
			this.Icon = parser.ReadColumn<ushort>(10);
			this.LevelItem = new LazyRow<ItemLevel>(lumina, parser.ReadColumn<ushort>(11), language);
			this.Rarity = parser.ReadColumn<byte>(12);
			this.FilterGroup = parser.ReadColumn<byte>(13);
			this.AdditionalData = parser.ReadColumn<uint>(14);
			this.ItemUICategory = new LazyRow<ItemUICategory>(lumina, parser.ReadColumn<byte>(15), language);
			this.ItemSearchCategory = new LazyRow<ItemSearchCategory>(lumina, parser.ReadColumn<byte>(16), language);
			this.EquipSlotCategory = new LazyRow<EquipSlotCategory>(lumina, parser.ReadColumn<byte>(17), language);
			this.ItemSortCategory = new LazyRow<ItemSortCategory>(lumina, parser.ReadColumn<byte>(18), language);
			this.StackSize = parser.ReadColumn<uint>(20);
			this.IsUnique = parser.ReadColumn<bool>(21);
			this.IsUntradable = parser.ReadColumn<bool>(22);
			this.IsIndisposable = parser.ReadColumn<bool>(23);
			this.Lot = parser.ReadColumn<bool>(24);
			this.PriceMid = parser.ReadColumn<uint>(25);
			this.PriceLow = parser.ReadColumn<uint>(26);
			this.CanBeHq = parser.ReadColumn<bool>(27);
			this.IsDyeable = parser.ReadColumn<bool>(28);
			this.IsCrestWorthy = parser.ReadColumn<bool>(29);
			this.ItemAction = new LazyRow<ItemAction>(lumina, parser.ReadColumn<ushort>(30), language);
			this.Cooldowns = parser.ReadColumn<ushort>(32);
			this.ClassJobRepair = new LazyRow<ClassJob>(lumina, parser.ReadColumn<byte>(33), language);
			this.ItemRepair = new LazyRow<Item>(lumina, parser.ReadColumn<int>(34), language);
			this.ItemGlamour = new LazyRow<Item>(lumina, parser.ReadColumn<int>(35), language);
			this.Desynth = parser.ReadColumn<ushort>(36);
			this.IsCollectable = parser.ReadColumn<bool>(37);
			this.AlwaysCollectable = parser.ReadColumn<bool>(38);
			this.AetherialReduce = parser.ReadColumn<ushort>(39);
			this.LevelEquip = parser.ReadColumn<byte>(41);
			this.EquipRestriction = parser.ReadColumn<byte>(43);
			this.ClassJobCategory = new LazyRow<ClassJobCategory>(lumina, parser.ReadColumn<byte>(44), language);
			this.GrandCompany = new LazyRow<GrandCompany>(lumina, parser.ReadColumn<byte>(45), language);
			this.ItemSeries = new LazyRow<ItemSeries>(lumina, parser.ReadColumn<byte>(46), language);
			this.BaseParamModifier = parser.ReadColumn<byte>(47);
			this.ClassJobUse = new LazyRow<ClassJob>(lumina, parser.ReadColumn<byte>(50), language);
			this.DamagePhys = parser.ReadColumn<ushort>(52);
			this.DamageMag = parser.ReadColumn<ushort>(53);
			this.Delayms = parser.ReadColumn<ushort>(54);
			this.BlockRate = parser.ReadColumn<ushort>(56);
			this.Block = parser.ReadColumn<ushort>(57);
			this.DefensePhys = parser.ReadColumn<ushort>(58);
			this.DefenseMag = parser.ReadColumn<ushort>(59);
			this.ItemSpecialBonus = new LazyRow<ItemSpecialBonus>(lumina, parser.ReadColumn<byte>(72), language);
			this.ItemSpecialBonusParam = parser.ReadColumn<byte>(73);
			this.MaterializeType = parser.ReadColumn<byte>(86);
			this.MateriaSlotCount = parser.ReadColumn<byte>(87);
			this.IsAdvancedMeldingPermitted = parser.ReadColumn<bool>(88);
			this.IsPvP = parser.ReadColumn<bool>(89);
			this.IsGlamourous = parser.ReadColumn<bool>(91);

			// Equip slots
			this.EquipableClasses = this.ClassJobCategory.Value.ToFlags();
			this.FitsInSlots = this.EquipSlotCategory.Value.ToFlags();

			// Models
			ulong modelMain = parser.ReadColumn<ulong>(48);
			this.Model = this.CreateItemModel(modelMain);

			ulong modelSub = parser.ReadColumn<ulong>(49);
			this.SubModel = this.CreateItemModel(modelSub);
		}

		public bool FitsInSlot(ItemSlots slot)
		{
			return this.FitsInSlots.HasFlag(slot);
		}

		private ItemModelBase? CreateItemModel(ulong v)
		{
			if (v == 0)
				return null;

			if (this.FitsInSlot(ItemSlots.MainHand) || this.FitsInSlot(ItemSlots.OffHand))
				return new WeaponModel(v);

			if (this.FitsInSlot(ItemSlots.Head)
				|| this.FitsInSlot(ItemSlots.Body)
				|| this.FitsInSlot(ItemSlots.Hands)
				|| this.FitsInSlot(ItemSlots.Legs)
				|| this.FitsInSlot(ItemSlots.Feet))
			{
				return new EquipmentModel(v);
			}

			if (this.FitsInSlot(ItemSlots.Ears)
				|| this.FitsInSlot(ItemSlots.Neck)
				|| this.FitsInSlot(ItemSlots.Wrists)
				|| this.FitsInSlot(ItemSlots.LeftRing)
				|| this.FitsInSlot(ItemSlots.RightRing))
			{
				return new AccessoryModel(v);
			}

			// TODO: some items have models, such as food items.
			////throw new Exception($"Unknown item model type for slots: {this.FitsInSlots}");
			return null;
		}
	}
}
