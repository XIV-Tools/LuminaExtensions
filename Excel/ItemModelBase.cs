// © XIV-Tools.
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

		protected ItemModelBase()
		{
			this.ImcFilePath = string.Empty;
		}

		public string ImcFilePath { get; protected set; }

		public virtual string Display => $"{this.SetKey}, v{this.ImcVariant}";
		public string SetKey { get; protected set; } = string.Empty;
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
		public ushort SetVaraint = 0;

		public WeaponModel(ulong dat)
			: base()
		{
			this.Set = (ushort)dat;
			this.SetVaraint = (ushort)(dat >> 16);
			this.ImcVariant = (ushort)(dat >> 32);

			this.SetKey = this.GetSetKey();
			this.SetVaraintKey = "b" + this.SetVaraint.ToString().PadLeft(4, '0');

			this.ImcFilePath = this.GetImcFilePath();
		}

		public string SetVaraintKey { get; protected set; } = string.Empty;

		public override string Display => $"{this.SetKey}, {this.SetVaraintKey}, v{this.ImcVariant}";

		public override string GetModelPath(RaceTribes raceTribe, RaceTypes raceType, ItemSlots slot)
		{
			string raceKey = raceTribe.ToKey(raceType);
			return $"chara/weapon/{this.SetKey}/obj/body/{this.SetVaraintKey}/model/{this.SetKey}{this.SetVaraintKey}.mdl";
		}

		protected override string GetImcFilePath()
		{
			return $"chara/weapon/{this.SetKey}/obj/body/{this.SetVaraintKey}/{this.SetVaraintKey}.imc";
		}

		protected override string GetSetKey()
		{
			return "w" + this.Set.ToString().PadLeft(4, '0');
		}
	}
}
