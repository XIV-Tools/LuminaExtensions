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
namespace LuminaExtensions.Files.Mdl
{
	using System.Collections.Generic;

	/// <summary>
	/// This class contains properties for the Level of Detail data.
	/// </summary>
	public class LevelOfDetail
	{
		/// <summary>
		/// The list of MeshData for the LoD.
		/// </summary>
		public readonly List<MeshData> MeshDataList = new List<MeshData>();

		public ushort MeshOffset;
		public int VertexDataOffset;
		public int IndexDataOffset;

		public short MeshCount;
		public short MeshEnd;
		public short ExtraMeshCount;
		public short MeshSum;
		public int IndexDataStart;
		public int VertexDataSize;
		public int IndexDataSize;

		public int Unknown0;
		public int Unknown1;
		public short Unknown2;
		public int Unknown3;
		public int Unknown4;
		public int Unknown5;
		public int Unknown6;
		public int Unknown7;
	}
}