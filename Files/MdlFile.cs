// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Numerics;
	using System.Text;
	using Lumina.Data;
	using LuminaExtensions.Extensions;
	using LuminaExtensions.Files.Mdl;

	public class MdlFile : FileResource
	{
		public readonly List<string> Attributes = new List<string>();
		public readonly List<string> Bones = new List<string>();
		public readonly List<string> Materials = new List<string>();
		public readonly List<string> Shapes = new List<string>();
		public readonly List<string> ExtraPaths = new List<string>();

		public readonly List<LevelOfDetail> Lods = new List<LevelOfDetail>();
		public readonly List<BoneSet> BoneSets = new List<BoneSet>();
		public readonly List<Vector4> BoundingBox = new List<Vector4>(8);
		public readonly List<(Vector4, Vector4)> BoneTransformData = new List<(Vector4, Vector4)>();
		public ShapeData? MeshShapeData;
		public BoneSet? PartBoneSets;

		// See XivModdingFramework Mdl.cs GetRawMdlData method for reference
		// https://github.com/TexTools/xivModdingFramework/blob/master/xivModdingFramework/Models/FileTypes/Mdl.cs#L417
		public override void LoadFile()
		{
			base.LoadFile();

			// not sure what the first 12 bytes are for
			this.Reader.BaseStream.Seek(12, SeekOrigin.Begin);

			ushort meshCount = this.Reader.ReadUInt16();
			ushort materialCount = this.Reader.ReadUInt16();

			// We skip the Vertex Data Structures for now
			// This is done so that we can get the correct number of meshes per LoD first
			int pathDataOffset = 64 + (136 * meshCount) + 4;
			this.Reader.BaseStream.Seek(pathDataOffset, SeekOrigin.Begin);
			int pathCount = this.Reader.ReadInt32();
			int pathBlockSize = this.Reader.ReadInt32();

			// Read all paths and attributes
			this.Reader.BaseStream.Seek(pathDataOffset + 8, SeekOrigin.Begin);
			List<string> paths = new List<string>();
			for (int i = 0; i < pathCount; i++)
			{
				byte a;
				List<byte> bytes = new List<byte>();
				while ((a = this.Reader.ReadByte()) != 0)
				{
					bytes.Add(a);
				}

				paths.Add(Encoding.ASCII.GetString(bytes.ToArray()).Replace("\0", string.Empty));
			}

			// Ensure we are at the end of the path block
			int modelDataOffset = pathDataOffset + pathBlockSize + 8;
			this.Reader.BaseStream.Seek(modelDataOffset, SeekOrigin.Begin);
			int unknown0 = this.Reader.ReadInt32();
			short meshCount2 = this.Reader.ReadInt16();
			short attributeCount = this.Reader.ReadInt16();
			short meshPartCount = this.Reader.ReadInt16();
			short materialCount2 = this.Reader.ReadInt16();
			short boneCount = this.Reader.ReadInt16();
			short boneListCount = this.Reader.ReadInt16();
			short shapeCount = this.Reader.ReadInt16();
			short shapePartCount = this.Reader.ReadInt16();
			ushort shapeDataCount = this.Reader.ReadUInt16();
			short unknown1 = this.Reader.ReadInt16();
			short unknown2 = this.Reader.ReadInt16();
			short unknown3 = this.Reader.ReadInt16();
			short unknown4 = this.Reader.ReadInt16();
			short unknown5 = this.Reader.ReadInt16();
			short unknown6 = this.Reader.ReadInt16();
			short unknown7 = this.Reader.ReadInt16();
			short furnatureTransformCount = this.Reader.ReadInt16();
			short unknown9 = this.Reader.ReadInt16();
			byte unknown10a = this.Reader.ReadByte();
			byte unknown10b = this.Reader.ReadByte();
			short unknown11 = this.Reader.ReadInt16();
			short unknown12 = this.Reader.ReadInt16();
			short unknown13 = this.Reader.ReadInt16();
			short unknown14 = this.Reader.ReadInt16();
			short unknown15 = this.Reader.ReadInt16();
			short unknown16 = this.Reader.ReadInt16();
			short unknown17 = this.Reader.ReadInt16();

			// Now that we have the paths and the data, unpack the paths in order
			int index = 0;
			index = paths.CopyTo(this.Attributes, index, attributeCount);
			index = paths.CopyTo(this.Bones, index, boneCount);
			index = paths.CopyTo(this.Materials, index, materialCount);
			index = paths.CopyTo(this.Shapes, index, shapeCount);
			index = paths.CopyTo(this.ExtraPaths, index);

			// Currently Unknown Data
			this.Reader.ReadBytes(unknown2 * 32);

			// TODO: XivModdingFramework has a hack to fix something with some furnature items such as picture frames and easel
			// that has not been replicated here. Investigate what that hack was doing.

			// Note: There is always 3 LoD
			for (int i = 0; i < 3; i++)
			{
				LevelOfDetail lod = new LevelOfDetail();

				lod.MeshOffset = this.Reader.ReadUInt16();
				lod.MeshCount = this.Reader.ReadInt16();
				lod.Unknown0 = this.Reader.ReadInt32();
				lod.Unknown1 = this.Reader.ReadInt32();
				lod.MeshEnd = this.Reader.ReadInt16();
				lod.ExtraMeshCount = this.Reader.ReadInt16();
				lod.MeshSum = this.Reader.ReadInt16();
				lod.Unknown2 = this.Reader.ReadInt16();
				lod.Unknown3 = this.Reader.ReadInt32();
				lod.Unknown4 = this.Reader.ReadInt32();
				lod.Unknown5 = this.Reader.ReadInt32();
				lod.IndexDataStart = this.Reader.ReadInt32();
				lod.Unknown6 = this.Reader.ReadInt32();
				lod.Unknown7 = this.Reader.ReadInt32();
				lod.VertexDataSize = this.Reader.ReadInt32();
				lod.IndexDataSize = this.Reader.ReadInt32();
				lod.VertexDataOffset = this.Reader.ReadInt32();
				lod.IndexDataOffset = this.Reader.ReadInt32();

				this.Lods.Add(lod);
			}

			// Now that we have the LoD data, we can go back and read the Vertex Data Structures
			// First we save our current position
			long savePosition = this.Reader.BaseStream.Position;

			int loDStructPos = 68;

			foreach (LevelOfDetail lod in this.Lods)
			{
				int totalMeshCount = lod.MeshCount + lod.ExtraMeshCount;
				for (int j = 0; j < totalMeshCount; j++)
				{
					MeshData meshData = new MeshData();

					// LoD Index * Vertex Data Structure size + Header
					this.Reader.BaseStream.Seek((j * 136) + loDStructPos, SeekOrigin.Begin);

					// If the first byte is 255, we reached the end of the Vertex Data Structs
					byte dataBlockNum = this.Reader.ReadByte();
					while (dataBlockNum != 255)
					{
						VertexDataStruct vertexDataStruct = new VertexDataStruct();
						vertexDataStruct.DataBlock = dataBlockNum;
						vertexDataStruct.DataOffset = this.Reader.ReadByte();
						vertexDataStruct.DataType = (VertexDataStruct.VertexDataType)this.Reader.ReadByte();
						vertexDataStruct.DataUsage = (VertexDataStruct.VertexUsageType)this.Reader.ReadByte();
						meshData.VertexDataStructList.Add(vertexDataStruct);

						// padding between Vertex Data Structs
						this.Reader.ReadBytes(4);

						dataBlockNum = this.Reader.ReadByte();
					}

					lod.MeshDataList.Add(meshData);
				}

				loDStructPos += 136 * lod.MeshCount;
			}

			// Now that we finished reading the Vertex Data Structures, we can go back to our saved position
			this.Reader.BaseStream.Seek(savePosition, SeekOrigin.Begin);

			// Mesh Data Information
			foreach (LevelOfDetail lod in this.Lods)
			{
				int totalMeshCount = lod.MeshCount + lod.ExtraMeshCount;

				for (int i = 0; i < totalMeshCount; i++)
				{
					MeshData md = lod.MeshDataList[i];

					md.VertexCount = this.Reader.ReadInt32();
					md.IndexCount = this.Reader.ReadInt32();
					md.MaterialIndex = this.Reader.ReadInt16();
					md.MeshPartIndex = this.Reader.ReadInt16();
					md.MeshPartCount = this.Reader.ReadInt16();
					md.BoneSetIndex = this.Reader.ReadInt16();
					md.IndexDataOffset = this.Reader.ReadInt32();
					md.VertexDataOffset0 = this.Reader.ReadInt32();
					md.VertexDataOffset1 = this.Reader.ReadInt32();
					md.VertexDataOffset2 = this.Reader.ReadInt32();
					md.VertexDataEntrySize0 = this.Reader.ReadByte();
					md.VertexDataEntrySize1 = this.Reader.ReadByte();
					md.VertexDataEntrySize2 = this.Reader.ReadByte();
					md.VertexDataBlockCount = this.Reader.ReadByte();
				}
			}

			// Data block for attributes offset paths
			// Not sure what attribute offsets are used for since TexTools never reads or writes them
			for (int i = 0; i < this.Attributes.Count; i++)
			{
				this.Reader.ReadInt32();
			}

			// Unknown data block
			// No idea how this would work, since commenting it out should change the position in the read stream, but its disabled
			// in XivModdingFramework so I'm guessing its not used at all.
			// XivModdingFramework Comment:
			//    This is commented out to allow housing items to display, the data does not exist for housing items
			//    more investigation needed as to what this data is
			////byte[] unknownData1 = br.ReadBytes(xivMdl.ModelData.Unknown3 * 20)

			// Mesh Parts
			foreach (LevelOfDetail lod in this.Lods)
			{
				foreach (MeshData meshData in lod.MeshDataList)
				{
					for (int i = 0; i < meshData.MeshPartCount; i++)
					{
						MeshPart meshPart = new MeshPart();
						meshPart.IndexOffset = this.Reader.ReadInt32();
						meshPart.IndexCount = this.Reader.ReadInt32();
						meshPart.AttributeBitmask = this.Reader.ReadUInt32();
						meshPart.BoneStartOffset = this.Reader.ReadInt16();
						meshPart.BoneCount = this.Reader.ReadInt16();
						meshData.MeshPartList.Add(meshPart);
					}
				}
			}

			// Unknown data block
			byte[] unknownData2 = this.Reader.ReadBytes(unknown9 * 12);

			// Data block for material offset paths
			// Not sure what material offsets are used for since TexTools never reads or writes them
			for (int i = 0; i < this.Materials.Count; i++)
			{
				this.Reader.ReadInt32();
			}

			// Data block for bone offset paths
			// Not sure what bone offsets are used for since TexTools never reads or writes them
			for (int i = 0; i < this.Bones.Count; i++)
			{
				this.Reader.ReadInt32();
			}

			// Bone Lists
			for (int i = 0; i < boneListCount; i++)
			{
				BoneSet boneIndexMesh = new BoneSet();

				for (int j = 0; j < 64; j++)
				{
					boneIndexMesh.BoneIndices.Add(this.Reader.ReadInt16());
				}

				boneIndexMesh.Count = this.Reader.ReadInt32();
				this.BoneSets.Add(boneIndexMesh);
			}

			this.MeshShapeData = new ShapeData();

			// Shape Info

			// Each shape has a header entry, then a per-lod entry.
			for (int i = 0; i < shapeCount; i++)
			{
				// Header - Offset to the shape name.
				ShapeData.ShapeInfo shapeInfo = new ShapeData.ShapeInfo();
				shapeInfo.ShapeNameOffset = this.Reader.ReadInt32();
				shapeInfo.Name = this.Shapes[i];

				// Per LoD entry (offset to this shape's parts in the shape set)
				List<ushort> dataInfoIndexList = new List<ushort>();
				for (int j = 0; j < this.Lods.Count; j++)
				{
					dataInfoIndexList.Add(this.Reader.ReadUInt16());
				}

				// Per LoD entry (number of parts in the shape set)
				List<short> infoPartCountList = new List<short>();
				for (int j = 0; j < this.Lods.Count; j++)
				{
					infoPartCountList.Add(this.Reader.ReadInt16());
				}

				for (int j = 0; j < this.Lods.Count; j++)
				{
					ShapeData.ShapeLodInfo shapeIndexPart = new ShapeData.ShapeLodInfo
					{
						PartOffset = dataInfoIndexList[j],
						PartCount = infoPartCountList[j],
					};
					shapeInfo.ShapeLods.Add(shapeIndexPart);
				}

				this.MeshShapeData.ShapeInfoList.Add(shapeInfo);
			}

			// Shape Index Info
			for (int i = 0; i < shapePartCount; i++)
			{
				ShapeData.ShapePart shapeIndexInfo = new ShapeData.ShapePart();

				// The offset to the index block this Shape Data should be replacing in. -- This is how Shape Data is tied to each mesh.
				shapeIndexInfo.MeshIndexOffset = this.Reader.ReadInt32();

				// # of triangle indices to replace.
				shapeIndexInfo.IndexCount = this.Reader.ReadInt32();

				// The offset where this part should start reading in the Shape Data list.
				shapeIndexInfo.ShapeDataOffset = this.Reader.ReadInt32();

				this.MeshShapeData.ShapeParts.Add(shapeIndexInfo);
			}

			// Shape data
			for (int i = 0; i < shapeDataCount; i++)
			{
				ShapeData.ShapeDataEntry entry = new ShapeData.ShapeDataEntry();

				// Base Triangle Index we're replacing
				entry.BaseIndex = this.Reader.ReadUInt16();

				// The Vertex that Triangle Index should now point to instead.
				entry.ShapeVertex = this.Reader.ReadUInt16();
				this.MeshShapeData.ShapeDataList.Add(entry);
			}

			// Build the list of offsets so we can match it for shape data.
			List<List<int>> indexOffsets = new List<List<int>>();
			for (int l = 0; l < this.Lods.Count; l++)
			{
				indexOffsets.Add(new List<int>());
				for (int m = 0; m < this.Lods[l].MeshDataList.Count; m++)
				{
					indexOffsets[l].Add(this.Lods[l].MeshDataList[m].IndexDataOffset);
				}
			}

			this.MeshShapeData.AssignMeshAndLodNumbers(indexOffsets);

			// Bone index for Parts
			BoneSet partBoneSet = new BoneSet();
			partBoneSet.Count = this.Reader.ReadInt32();

			for (int i = 0; i < partBoneSet.Count / 2; i++)
			{
				partBoneSet.BoneIndices.Add(this.Reader.ReadInt16());
			}

			this.PartBoneSets = partBoneSet;

			// Padding
			byte paddingSize = this.Reader.ReadByte();
			byte[] paddedBytes = this.Reader.ReadBytes(paddingSize);

			// Bounding box
			for (int i = 0; i < 8; i++)
			{
				this.BoundingBox.Add(this.Reader.ReadVector4());
			}

			// Bone Transform Data
			short transformCount = boneCount;

			if (transformCount == 0)
				transformCount = furnatureTransformCount;

			for (int i = 0; i < transformCount; i++)
			{
				Vector4 transform0 = this.Reader.ReadVector4();
				Vector4 transform1 = this.Reader.ReadVector4();
				this.BoneTransformData.Add((transform0, transform1));
			}

			// Load lod data
			int totalLodMeshCount = 0;
			int lodNum = 0;
			foreach (LevelOfDetail lod in this.Lods)
			{
				if (lod.MeshCount == 0)
					continue;

				List<MeshData> meshDataList = lod.MeshDataList;

				if (lod.MeshOffset != totalLodMeshCount)
					meshDataList = this.Lods[lodNum + 1].MeshDataList;

				foreach (MeshData meshData in meshDataList)
				{
					VertexData vertexData = this.ReadLod(lod, meshData);

					// region Indices
					int indexOffset = lod.IndexDataOffset + (meshData.IndexDataOffset * 2);

					this.Reader.BaseStream.Seek(indexOffset, SeekOrigin.Begin);

					for (int i = 0; i < meshData.IndexCount; i++)
					{
						vertexData.Indices.Add(this.Reader.ReadUInt16());
					}

					meshData.VertexData = vertexData;
					totalLodMeshCount++;
				}

				// If the model contains Shape Data, parse the data for each mesh
				if (shapeCount > 0)
				{
					// Dictionary containing <index data offset, mesh number>
					Dictionary<int, int> indexMeshNum = new Dictionary<int, int>();

					List<ShapeData.ShapeDataEntry> shapeData = this.MeshShapeData.ShapeDataList;

					// Get the index data offsets in each mesh
					for (int i = 0; i < lod.MeshCount; i++)
					{
						int indexDataOffset = lod.MeshDataList[i].IndexDataOffset;

						if (!indexMeshNum.ContainsKey(indexDataOffset))
						{
							indexMeshNum.Add(indexDataOffset, i);
						}
					}

					for (int i = 0; i < lod.MeshCount; i++)
					{
						Dictionary<int, Vector3> referencePositionsDictionary = new Dictionary<int, Vector3>();
						SortedDictionary<int, Vector3> meshShapePositionsDictionary = new SortedDictionary<int, Vector3>();
						Dictionary<int, Dictionary<ushort, ushort>> shapeIndexOffsetDictionary = new Dictionary<int, Dictionary<ushort, ushort>>();

						// Shape info list
						List<ShapeData.ShapeInfo> shapeInfoList = this.MeshShapeData.ShapeInfoList;

						// Number of shape info in each mesh
						short perMeshCount = shapeCount;

						for (int j = 0; j < perMeshCount; j++)
						{
							ShapeData.ShapeInfo shapeInfo = shapeInfoList[j];

							if (shapeInfo.Name == null)
								throw new Exception("Shape has no name");

							ShapeData.ShapeLodInfo indexPart = shapeInfo.ShapeLods[lodNum];

							// The part count
							short infoPartCount = indexPart.PartCount;

							for (int k = 0; k < infoPartCount; k++)
							{
								// Gets the data info for the part
								ShapeData.ShapePart shapeDataInfo = this.MeshShapeData.ShapeParts[indexPart.PartOffset + k];

								// The offset in the shape data
								int indexDataOffset = shapeDataInfo.MeshIndexOffset;

								int indexMeshLocation = 0;

								// Determine which mesh the info belongs to
								if (indexMeshNum.ContainsKey(indexDataOffset))
								{
									indexMeshLocation = indexMeshNum[indexDataOffset];

									// Move to the next part if it is not the current mesh
									if (indexMeshLocation != i)
									{
										continue;
									}
								}

								// Get the mesh data
								MeshData mesh = lod.MeshDataList[indexMeshLocation];

								if (mesh.VertexData == null)
									throw new Exception("Mesh has no shape data");

								// Get the shape data for the current mesh
								List<ShapeData.ShapeDataEntry> shapeDataForMesh = shapeData.GetRange(shapeDataInfo.ShapeDataOffset, shapeDataInfo.IndexCount);

								// Fill shape data dictionaries
								////ushort dataIndex = ushort.MaxValue;
								foreach (ShapeData.ShapeDataEntry data in shapeDataForMesh)
								{
									int referenceIndex = 0;

									if (data.BaseIndex < mesh.VertexData.Indices.Count)
									{
										// Gets the index to which the data is referencing
										referenceIndex = mesh.VertexData.Indices[data.BaseIndex];

										////throw new Exception($"Reference Index is larger than the index count. Reference Index: {data.ReferenceIndexOffset}  Index Count: {mesh.VertexData.Indices.Count}");
									}

									if (!referencePositionsDictionary.ContainsKey(data.BaseIndex))
									{
										if (mesh.VertexData.Positions.Count > referenceIndex)
										{
											referencePositionsDictionary.Add(data.BaseIndex, mesh.VertexData.Positions[referenceIndex]);
										}
									}

									if (!meshShapePositionsDictionary.ContainsKey(data.ShapeVertex))
									{
										if (data.ShapeVertex >= mesh.VertexData.Positions.Count)
										{
											meshShapePositionsDictionary.Add(data.ShapeVertex, new Vector3(0));
										}
										else
										{
											meshShapePositionsDictionary.Add(data.ShapeVertex, mesh.VertexData.Positions[data.ShapeVertex]);
										}
									}
								}

								mesh.ShapePathList.Add(shapeInfo.Name);
							}
						}
					}
				}
			}
		}

		public override void SaveFile(string path)
		{
			throw new NotImplementedException();
		}

		private VertexData ReadLod(LevelOfDetail lod, MeshData meshData)
		{
			VertexData vertexData = new VertexData();

			foreach (VertexDataStruct data in meshData.VertexDataStructList)
			{
				int vertexDataOffset = data.DataBlock switch
				{
					0 => meshData.VertexDataOffset0,
					1 => meshData.VertexDataOffset1,
					2 => meshData.VertexDataOffset2,
					_ => throw new Exception($"Invalid data block id: {data.DataBlock}"),
				};

				int vertexDataSize = data.DataBlock switch
				{
					0 => meshData.VertexDataEntrySize0,
					1 => meshData.VertexDataEntrySize1,
					2 => meshData.VertexDataEntrySize2,
					_ => throw new Exception($"Invalid data block id: {data.DataBlock}"),
				};

				for (int i = 0; i < meshData.VertexCount; i++)
				{
					int offset = lod.VertexDataOffset + vertexDataOffset + data.DataOffset + (vertexDataSize * i);
					this.Reader.BaseStream.Seek(offset, SeekOrigin.Begin);

					if (data.DataUsage == VertexDataStruct.VertexUsageType.Position)
					{
						vertexData.Positions.Add(this.Reader.ReadVertexData<Vector3>(data.DataType));
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.BoneWeight)
					{
						vertexData.BoneWeights.Add(this.Reader.ReadVertexData<Vector4>(data.DataType));
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.BoneIndex)
					{
						vertexData.BoneIndices.Add(this.Reader.ReadBytes(4));
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.Normal)
					{
						vertexData.Normals.Add(this.Reader.ReadVertexData<Vector4>(data.DataType));
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.Binormal)
					{
						float x = (this.Reader.ReadByte() * 2 / 255f) - 1f;
						float y = (this.Reader.ReadByte() * 2 / 255f) - 1f;
						float z = (this.Reader.ReadByte() * 2 / 255f) - 1f;
						byte w = this.Reader.ReadByte();

						vertexData.BiNormals.Add(new Vector3(x, y, z));
						vertexData.BiNormalHandedness.Add(w);
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.Tangent)
					{
						float x = (this.Reader.ReadByte() * 2 / 255f) - 1f;
						float y = (this.Reader.ReadByte() * 2 / 255f) - 1f;
						float z = (this.Reader.ReadByte() * 2 / 255f) - 1f;
						byte w = this.Reader.ReadByte();

						vertexData.Tangents.Add(new Vector3(x, y, z));
						////vertexData.TangentHandedness.Add(w);
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.Color)
					{
						vertexData.Colors.Add(this.Reader.ReadColor());
					}
					else if (data.DataUsage == VertexDataStruct.VertexUsageType.TextureCoordinate)
					{
						object value = this.Reader.ReadVertexData(data.DataType);
						if (value is Vector2 vec2)
						{
							vertexData.UV0.Add(vec2);
						}
						else if (value is Vector4 vec4)
						{
							vertexData.UV0.Add(new Vector2(vec4.X, vec4.Y));
							vertexData.UV1.Add(new Vector2(vec4.Z, vec4.W));
						}
					}
				}
			}

			return vertexData;
		}
	}
}
