﻿// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Excel
{
	using System;

	public abstract class ItemModelBase
	{
		public ItemModelBase(ulong dat)
		{
			this.Set = (ushort)dat;
			this.ImcVariant = (ushort)(dat >> 16);

			this.SetKey = this.GetSetKey();
			this.ImcFilePath = this.GetImcFilePath();
		}

		public string ImcFilePath { get; private set; }

		public string Display => $"{this.SetKey}, v{this.ImcVariant}";
		public string SetKey { get; private set; } = string.Empty;
		public ushort Set { get; protected set; }
		public ushort ImcVariant { get; protected set; }

		public abstract string GetModelPath(RaceTribes raceTribe, RaceTypes raceType, ItemSlots slot);

		protected abstract string GetSetKey();
		protected abstract string GetImcFilePath();
	}

	public class EquipmentModel : ItemModelBase
	{
		public EquipmentModel(ulong dat)
			: base(dat)
		{
		}

		public override string GetModelPath(RaceTribes raceTribe, RaceTypes raceType, ItemSlots slot)
		{
			string raceKey = raceTribe.ToKey(raceType);
			string slotKey = slot.ToAbbreviation();
			return $"chara/equipment/{this.SetKey}/model/{raceKey}{this.SetKey}_{slotKey}.mdl";
		}

		protected override string GetImcFilePath()
		{
			return $"chara/equipment/{this.SetKey}/{this.SetKey}.imc";
		}

		protected override string GetSetKey()
		{
			return "e" + this.Set.ToString().PadLeft(4, '0');
		}
	}

	public class AccessoryModel : ItemModelBase
	{
		public AccessoryModel(ulong dat)
			: base(dat)
		{
		}

		public override string GetModelPath(RaceTribes raceTribe, RaceTypes raceType, ItemSlots slot)
		{
			string raceKey = raceTribe.ToKey(raceType);
			string slotKey = slot.ToAbbreviation();
			return $"chara/accessory/{this.SetKey}/model/{raceKey}{this.SetKey}_{slotKey}.mdl";
		}

		protected override string GetImcFilePath()
		{
			return $"chara/accessory/{this.SetKey}/{this.SetKey}.imc";
		}

		protected override string GetSetKey()
		{
			return "a" + this.Set.ToString().PadLeft(4, '0');
		}
	}

	public class WeaponModel : ItemModelBase
	{
		public WeaponModel(ulong dat)
			: base(dat)
		{
			this.Set = (ushort)(dat >> 16);
			this.ImcVariant = (ushort)(dat >> 32);
		}

		public override string GetModelPath(RaceTribes raceTribe, RaceTypes raceType, ItemSlots slot)
		{
			// secondary id might be the model variant?
			ushort secondaryId = 0000; // ?
			string secondaryIdStr4 = secondaryId.ToString().PadLeft(4, '0');

			string raceKey = raceTribe.ToKey(raceType);
			string slotKey = slot.ToAbbreviation();
			return $"chara/weapon/{this.SetKey}/obj/body/b{secondaryIdStr4}/model/{raceKey}b{secondaryIdStr4}_{slotKey}.mdl";
		}

		protected override string GetImcFilePath()
		{
			// secondary id might be the model variant?
			ushort secondaryId = 0000; // ?
			string secondaryIdStr4 = secondaryId.ToString().PadLeft(4, '0');

			return $"chara/weapon/{this.SetKey}/obj/body/b{secondaryIdStr4}/b{secondaryIdStr4}.imc";
		}

		protected override string GetSetKey()
		{
			return "w" + this.Set.ToString().PadLeft(4, '0');
		}
	}
}
