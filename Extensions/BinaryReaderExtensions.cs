// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Numerics;
	using System.Text;
	using LuminaExtensions.Files.Mdl;
	using LuminaExtensions.Types;

	public static class BinaryReaderExtensions
	{
		public static string ReadTerminatedString(this BinaryReader self, int size = -1)
		{
			List<byte> bytes = new List<byte>();

			if (size <= 0)
			{
				bytes.Add(self.ReadByte());
				bytes.Add(self.ReadByte());

				byte a;
				while ((a = self.ReadByte()) != 0)
				{
					bytes.Add(a);
				}
			}
			else
			{
				bytes.AddRange(self.ReadBytes(size));
			}

			if (bytes.Count <= 0)
				return string.Empty;

			return Encoding.ASCII.GetString(bytes.ToArray()).Replace("\0", string.Empty);
		}

		public static Vector2 ReadVector2(this BinaryReader self)
		{
			return new Vector2(self.ReadSingle(), self.ReadSingle());
		}

		public static Vector3 ReadVector3(this BinaryReader self)
		{
			return new Vector3(self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
		}

		public static Vector4 ReadVector4(this BinaryReader self)
		{
			return new Vector4(self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
		}

		public static Half ReadHalf(this BinaryReader self)
		{
			return new Half(self.ReadUInt16());
		}

		public static Vector2 ReadHalf2(this BinaryReader self)
		{
			Half x = new Half(self.ReadUInt16());
			Half y = new Half(self.ReadUInt16());
			return new Vector2(x, y);
		}

		public static Vector3 ReadHalf3(this BinaryReader self)
		{
			Half x = new Half(self.ReadUInt16());
			Half y = new Half(self.ReadUInt16());
			Half z = new Half(self.ReadUInt16());
			return new Vector3(x, y, z);
		}

		public static Vector4 ReadHalf4(this BinaryReader self)
		{
			Half x = new Half(self.ReadUInt16());
			Half y = new Half(self.ReadUInt16());
			Half z = new Half(self.ReadUInt16());
			Half w = new Half(self.ReadUInt16());
			return new Vector4(x, y, z, w);
		}

		public static Vector4 ReadByte4(this BinaryReader self)
		{
			float x = self.ReadByte() / 255f;
			float y = self.ReadByte() / 255f;
			float z = self.ReadByte() / 255f;
			float w = self.ReadByte() / 255f;
			return new Vector4(x, y, z, w);
		}

		public static Color ReadRgbaColor(this BinaryReader self)
		{
			byte r = self.ReadByte();
			byte g = self.ReadByte();
			byte b = self.ReadByte();
			byte a = self.ReadByte();

			return Color.FromArgb(a, r, g, b);
		}

		public static Color ReadRgbColor(this BinaryReader self)
		{
			byte r = self.ReadByte();
			byte g = self.ReadByte();
			byte b = self.ReadByte();

			return Color.FromArgb(1, r, g, b);
		}

		public static Color ReadRgbColorHalf(this BinaryReader self)
		{
			Half r = self.ReadHalf();
			Half g = self.ReadHalf();
			Half b = self.ReadHalf();

			return Color.FromArgb(1, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		public static T ReadVertexData<T>(this BinaryReader self, VertexDataStruct.VertexDataType type)
		{
			object obj = self.ReadVertexData(type);

			// Special case for positions
			if (obj is Vector4 vec4 && typeof(T) == typeof(Vector3))
				obj = new Vector3(vec4.X, vec4.Y, vec4.Z);

			if (obj is T tObj)
				return tObj;

			throw new Exception($"Vertex data type: {type} is not type: {typeof(T)}");
		}

		public static object ReadVertexData(this BinaryReader self, VertexDataStruct.VertexDataType type)
		{
			switch (type)
			{
				case VertexDataStruct.VertexDataType.Float1: return self.ReadSingle();
				case VertexDataStruct.VertexDataType.Float2: return self.ReadVector2();
				case VertexDataStruct.VertexDataType.Float3: return self.ReadVector3();
				case VertexDataStruct.VertexDataType.Float4: return self.ReadVector4();
				case VertexDataStruct.VertexDataType.Half2: return self.ReadHalf2();
				case VertexDataStruct.VertexDataType.Half4: return self.ReadHalf4();

				case VertexDataStruct.VertexDataType.Ubyte4n: return self.ReadByte4();

				default: throw new NotImplementedException($"vertex data type: {type} not implemented");
			}
		}
	}
}
