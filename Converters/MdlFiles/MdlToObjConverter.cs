// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Converters.MdlFiles
{
	using System;
	using System.IO;
	using LuminaExtensions.Files;
	using LuminaExtensions.Files.Mdl;

	public class MdlToObjConverter : ConverterBase<MdlFile>
	{
		public override string Name => "Wavefront Obj";
		public override string FileExtension => ".obj";

		public override void Convert(MdlFile source, Stream destination)
		{
			using StreamWriter writer = new StreamWriter(destination);

			if (source.Lods.Count <= 0)
				throw new Exception("Model has no LODs");

			LevelOfDetail lod = source.Lods[0];

			if (lod.MeshDataList.Count <= 0)
				throw new Exception("Model LOD has no parts");

			int indexOffset = 0;

			foreach (MeshData part in lod.MeshDataList)
			{
				VertexData? vertexData = part.VertexData;

				if (vertexData == null)
					throw new Exception("Model part has no vertex data");

				foreach (System.Numerics.Vector3 vertexDataPosition in vertexData.Positions)
				{
					writer.WriteLine($"v {vertexDataPosition.X:N5} {vertexDataPosition.Y:N5} {vertexDataPosition.Z:N5}");
				}

				foreach (System.Numerics.Vector2 texCoord in vertexData.UV0)
				{
					double ox = texCoord.X - Math.Truncate(texCoord.X);
					double oy = texCoord.Y - Math.Truncate(texCoord.Y);
					writer.WriteLine($"vt {ox:N5} {1 - oy:N5}");
				}

				foreach (System.Numerics.Vector4 vertexDataNormal in vertexData.Normals)
				{
					writer.WriteLine($"vn {vertexDataNormal.X:N5} {vertexDataNormal.Y:N5} {vertexDataNormal.Z:N5}");
				}

				for (int i = 0; i < vertexData.Indices.Count; i += 3)
				{
					int index1 = indexOffset + vertexData.Indices[i] + 1;
					int index2 = indexOffset + vertexData.Indices[i + 1] + 1;
					int index3 = indexOffset + vertexData.Indices[i + 2] + 1;
					writer.WriteLine($"f {index1}/{index1}/{index1} {index2}/{index2}/{index2} {index3}/{index3}/{index3}");
				}

				indexOffset += vertexData.Positions.Count;
			}
		}

		public override void ConvertBack(Stream source, MdlFile destination)
		{
			throw new NotImplementedException();
		}
	}
}
