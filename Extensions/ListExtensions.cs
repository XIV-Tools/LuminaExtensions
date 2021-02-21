// © XIV-Tools.
// Licensed under the MIT license.

namespace System.Collections.Generic
{
	using System;
	using System.Text;

	public static class ListExtensions
	{
		public static int CopyTo<T>(this List<T> self, List<T> destination, int startIndex, int length)
		{
			for (int i = startIndex; i < startIndex + length; i++)
			{
				destination.Add(self[i]);
			}

			return startIndex + length;
		}

		public static int CopyTo<T>(this List<T> self, List<T> destination, int startIndex)
		{
			int length = destination.Count - startIndex;
			return self.CopyTo(destination, startIndex, length);
		}
	}
}
