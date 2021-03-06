// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files.Mtrl
{
	using System;
	using System.Drawing;
	using System.Numerics;
	using LuminaExtensions.Types;

	public class ColorSet
	{
		public readonly Row[] Rows = new Row[16];

		public class Row
		{
			public Color Diffuse;
			public Half SpecularPower;
			public Color Specular;
			public Half Gloss;
			public Color Emissive;
			public byte TileMaterial;
			public Vector2 Tile;
			public Vector2 TileSkew;

			public ushort DyeTemplate;
			public DyeFlags DyeFlag;

			[Flags]
			public enum DyeFlags : short
			{
				None = 0,

				Diffuse = 1,
				Specular = 2,
				Emissive = 4,
				Gloss = 8,
				SpecularPower = 16,

				All = Diffuse | Specular | Emissive | Gloss | SpecularPower,
			}
		}
	}
}
