// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Extensions
{
	using System;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;

	public static class BinaryWriterExtensions
	{
		/// <summary>
		/// Writes a structure from the current stream position.
		/// </summary>
		/// <typeparam name="T">The structure to read in to</typeparam>
		public static void WriteStructure<T>(this BinaryWriter self, T value)
			where T : struct
		{
			int size = Unsafe.SizeOf<T>();
			byte[] newbuffer = new byte[size];
			IntPtr mem = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr<T>(value, mem, false);
			Marshal.Copy(mem, newbuffer, 0, size);
			Marshal.FreeHGlobal(mem);

			self.Write(newbuffer);
		}
	}
}
