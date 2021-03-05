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
	using System.Drawing;
	using System.Numerics;

	/// <summary>
	/// This class contains the properties for the Vertex Data.
	/// </summary>
	public class VertexData
	{
		public readonly List<Vector3> Positions = new List<Vector3>();

		// Each vertex can hold a maximum of 4 bone weights.
		public readonly List<Vector4> BoneWeights = new List<Vector4>();

		// Each vertex can hold a maximum of 4 bone indices.
		public readonly List<byte[]> BoneIndices = new List<byte[]>();

		// The W coordinate is present but has never been noticed to be anything other than 0.
		public readonly List<Vector4> Normals = new List<Vector4>();

		public readonly List<Vector3> BiNormals = new List<Vector3>();
		public readonly List<byte> BiNormalHandedness = new List<byte>();
		public readonly List<Vector3> Tangents= new List<Vector3>();
		public readonly List<Color> Colors = new List<Color>();
		public readonly List<Vector2> UV0 = new List<Vector2>();
		public readonly List<Vector2> UV1 = new List<Vector2>();
		public readonly List<int> Indices = new List<int>();
	}
}