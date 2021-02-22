// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions
{
	using System;
	using System.Text.RegularExpressions;

	public enum RaceTribes : ushort
	{
		HyurMidlanderMale = 1,
		HyurMidlanderFemale = 2,
		HyurHighlanderMale = 3,
		HyurHighlanderFemale = 4,
		ElezenMale = 5,
		ElezenFemale = 6,
		MiqoteMale = 7,
		MiqoteFemale = 8,
		RoegadynMale = 9,
		RoegadynFemale = 10,
		LalafellMale = 11,
		LalafellFemale = 12,
		AuRaMale = 13,
		AuRaFemale = 14,
		HrothgarMale = 15,
		HrothgarFemale = 16, // Asumption
		VieraMale = 17, // Assumption
		VieraFemale = 18,
		NpcMale = 91,
		NpcFemale = 92,
	}

	// Wonder what 02 and 03 would be?
	public enum RaceTypes : ushort
	{
		Player = 1,
		Npc = 4,
	}

	#pragma warning disable SA1649
	public static class RaceTribeExtensions
	{
		public static string ToKey(this RaceTribes self, RaceTypes type)
		{
			ushort id = self.ToId(type);
			return "c" + id.ToString().PadLeft(4, '0');
		}

		public static string ToDisplayName(this RaceTribes self)
		{
			// TODO: actually look this up from the game data to get localised versions
			// Insert spaces before caps
			return Regex.Replace(self.ToString(), "(\\B[A-Z])", " $1");
		}

		public static ushort ToId(this RaceTribes self, RaceTypes type)
		{
			return (ushort)(((ushort)self * 100) + (ushort)type);
		}
	}
}
