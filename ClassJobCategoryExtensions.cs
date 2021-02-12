// © XIV-Tools.
// Licensed under the MIT license.

namespace Lumina.Excel.GeneratedSheets
{
	using System;
	using System.Reflection;
	using LuminaExtensions;

	public static class ClassJobCategoryExtensions
	{
		public static bool Contains(this ClassJobCategory self, Classes classJob)
		{
			string abr = classJob.GetAbbreviation();
			FieldInfo field = self.GetType().GetField(abr, BindingFlags.Public | BindingFlags.Instance);

			if (field == null)
				throw new Exception($"Unable to find ClassJob: {abr}");

			object val = field.GetValue(self);

			if (val == null)
				throw new Exception($"Unable to find ClassJob Value: {abr}");

			return (bool)val;
		}

		public static Classes ToFlags(this ClassJobCategory self)
		{
			Classes classes = Classes.None;

			foreach (Classes? job in Enum.GetValues(typeof(Classes)))
			{
				if (job == null || job == Classes.None || job == Classes.All)
					continue;

				if (self.Contains((Classes)job))
				{
					classes |= (Classes)job;
				}
			}

			return classes;
		}
	}
}
