// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions
{
	using System;
	using System.Collections.Generic;

	public enum RaceTribes : ushort
	{
		HyurMidlanderMale = 0100,
		HyurMidlanderFemale = 0200,
		HyurHighlanderMale = 0300,
		HyurHighlanderFemale = 0400,
		ElezenMale = 0500,
		ElezenFemale = 0600,
		MiqoteMale = 0700,
		MiqoteFemale = 0800,
		RoegadynMale = 0900,
		RoegadynFemale = 1000,
		LalafellMale = 1100,
		LalafellFemale = 1200,
		AuRaMale = 1300,
		AuRaFemale = 1400,
		Hrothgar = 1500,
		Viera = 1800,
		NpcMale = 9100,
		NpcFemale = 9200,
	}

	public enum RaceTypes : ushort
	{
		Player = 01,
		Npc = 04,
	}

	#pragma warning disable SA1649
	public static class RaceTribeExtensions
	{
		public static string ToKey(this RaceTribes self, RaceTypes type)
		{
			ushort id = self.ToId(type);
			return "c" + id.ToString().PadLeft(4, '0');
		}

		public static ushort ToId(this RaceTribes self, RaceTypes type)
		{
			if ((self == RaceTribes.NpcMale || self == RaceTribes.NpcFemale) && type == RaceTypes.Player)
				throw new Exception("Npc race can not be player");

			return (ushort)((ushort)self + (ushort)type);
		}
	}
}
