// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions
{
	using System;
	using System.Collections.Generic;

	public enum Roles
	{
		Tanks,
		Healers,
		Damage,
		Gatherers,
		Crafters,
	}

	#pragma warning disable SA1649 // File name should match first type name
	public static class RolesExtensions
	{
		private static Dictionary<Roles, List<Classes>>? classLookup;

		public static List<Classes> GetClasses(this Roles role)
		{
			if (classLookup == null)
			{
				classLookup = new Dictionary<Roles, List<Classes>>();

				foreach (Classes? job in Enum.GetValues(typeof(Classes)))
				{
					if (job == null || job == Classes.None)
						continue;

					Roles? classRole = ((Classes)job).GetRole();

					if (classRole == null)
						continue;

					if (!classLookup.ContainsKey((Roles)classRole))
						classLookup.Add((Roles)classRole, new List<Classes>());

					classLookup[(Roles)classRole].Add((Classes)job);
				}
			}

			return classLookup[role];
		}

		public static string GetName(this Roles role)
		{
			return role.ToString();
		}
	}
}
