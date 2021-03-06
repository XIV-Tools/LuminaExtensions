// © XIV-Tools.
// Licensed under the MIT license.

// xivModdingFramework
// Copyright © 2018 Rafael Gonzalez - All Rights Reserved
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace LuminaExtensions.Files.Mtrl
{
	using System;

	[Serializable]
	public class TextureUsageStruct
	{
		public enum Types : uint
		{
			Normal = 4113354501,
			Decal = 3531043187,
			Diffuse = 3054951514,
			Specular = 3367837167,
			Skin = 940355280,
			Other = 612525193,
		}

		public Types TextureType { get; set; }
		public uint Unknown { get; set; }
	}
}
