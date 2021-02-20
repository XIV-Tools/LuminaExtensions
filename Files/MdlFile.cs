// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using Lumina.Data;

	public class MdlFile : FileResource
	{
		public int Size => this.Data.Length;
	}
}
