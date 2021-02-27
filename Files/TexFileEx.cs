// © XIV-Tools.
// Licensed under the MIT license.

namespace LuminaExtensions.Files
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using Lumina.Data.Files;
	using Lumina.Data.Parsing.Tex;
	using Lumina.Extensions;
	using LuminaExtensions.Extensions;

	public class TexFileEx : TexFile
	{
		public byte[]? DdsData { get; private set; }
		public byte[]? ArgbData { get; set; }

		public override void LoadFile()
		{
			base.LoadFile();

			this.Reader.BaseStream.Position = 0;
			this.Header = this.Reader.ReadStructure<TexFile.TexHeader>();

			// TODO: replace current lumina image loader with one that deals with lods correctly.
			this.DdsData = this.DataSpan.Slice(Unsafe.SizeOf<TexHeader>()).ToArray();
			this.ArgbData = this.ImageData;
		}

		public override void SaveFile(string path)
		{
			////base.SaveFile(path);

			using FileStream fs = File.OpenWrite(path);
			using BinaryWriter bw = new BinaryWriter(fs);
			bw.WriteStructure(this.Header);
			bw.Write(this.DdsData);
		}

		public void SetDds(byte[] ddsData)
		{
			this.DdsData = ddsData;

			Span<byte> span = new Span<byte>(this.DdsData);
			this.ArgbData = Convert(span, this.Header.Format, this.Header.Width, this.Header.Height);
		}

		// converts various formats to A8R8G8B8
		private static byte[] Convert(Span<byte> src, TextureFormat format, int width, int height)
		{
			byte[] dst = new byte[width * height * 4];

			switch (format)
			{
				case TextureFormat.DXT1:
					ProcessDxt1(src, dst, width, height);
					break;
				case TextureFormat.DXT3:
					ProcessDxt3(src, dst, width, height);
					break;
				case TextureFormat.DXT5:
					ProcessDxt5(src, dst, width, height);
					break;
				case TextureFormat.R16G16B16A16F:
					ProcessA16R16G16B16_Float(src, dst, width, height);
					break;
				case TextureFormat.R5G5B5A1:
					ProcessA1R5G5B5(src, dst, width, height);
					break;
				case TextureFormat.R4G4B4A4:
					ProcessA4R4G4B4(src, dst, width, height);
					break;
				case TextureFormat.L8:
					ProcessR3G3B2(src, dst, width, height);
					break;
				case TextureFormat.A8R8G8B8:
					src.CopyTo(dst);
					break;
				default:
					throw new NotImplementedException($"TextureFormat {format} is not supported for image conversion.");
			}

			return dst;
		}

		// #region shamelessly copied from coinach
		// might be slowed down by src copying when calling squish
		private static void ProcessA16R16G16B16_Float(Span<byte> src, byte[] dst, int width, int height)
		{
			// Clipping can, and will occur since values go outside 0..1
			for (int i = 0; i < width * height; ++i)
			{
				int srcOff = i * 4 * 2;
				int dstOff = i * 4;

				for (int j = 0; j < 4; ++j)
					dst[dstOff + j] = (byte)(src.Unpack(srcOff + (j * 2)) * byte.MaxValue);
			}
		}

		private static void ProcessA1R5G5B5(Span<byte> src, byte[] dst, int width, int height)
		{
			for (int i = 0; (i + 2) <= 2 * width * height; i += 2)
			{
				ushort v = BitConverter.ToUInt16(src.Slice(i, sizeof(ushort)).ToArray(), 0);

				uint a = (uint)(v & 0x8000);
				uint r = (uint)(v & 0x7C00);
				uint g = (uint)(v & 0x03E0);
				uint b = (uint)(v & 0x001F);

				uint rgb = (r << 9) | (g << 6) | (b << 3);
				uint argbValue = a * 0x1FE00 | rgb | ((rgb >> 5) & 0x070707);

				for (int j = 0; j < 4; ++j)
					dst[(i * 2) + j] = (byte)(argbValue >> (8 * j));
			}
		}

		private static void ProcessA4R4G4B4(Span<byte> src, byte[] dst, int width, int height)
		{
			for (int i = 0; (i + 2) <= 2 * width * height; i += 2)
			{
				ushort v = BitConverter.ToUInt16(src.Slice(i, sizeof(ushort)).ToArray(), 0);

				for (int j = 0; j < 4; ++j)
					dst[(i * 2) + j] = (byte)(((v >> (4 * j)) & 0x0F) << 4);
			}
		}

		private static void ProcessA8R8G8B8(Span<byte> src, byte[] dst, int width, int height)
		{
			// Some transparent images have larger dst lengths than their src.
			int length = Math.Min(src.Length, dst.Length);
			src.Slice(0, length).CopyTo(dst.AsSpan());
		}

		private static void ProcessDxt1(Span<byte> src, byte[] dst, int width, int height)
		{
			byte[]? dec = Squish.DecompressImage(src.ToArray(), width, height, SquishOptions.DXT1);
			Array.Copy(dec, dst, dst.Length);
		}

		private static void ProcessDxt3(Span<byte> src, byte[] dst, int width, int height)
		{
			byte[]? dec = Squish.DecompressImage(src.ToArray(), width, height, SquishOptions.DXT3);
			Array.Copy(dec, dst, dst.Length);
		}

		private static void ProcessDxt5(Span<byte> src, byte[] dst, int width, int height)
		{
			byte[]? dec = Squish.DecompressImage(src.ToArray(), width, height, SquishOptions.DXT5);
			Array.Copy(dec, dst, dst.Length);
		}

		private static void ProcessR3G3B2(Span<byte> src, byte[] dst, int width, int height)
		{
			for (int i = 0; i < width * height; ++i)
			{
				uint r = (uint)(src[i] & 0xE0);
				uint g = (uint)(src[i] & 0x1C);
				uint b = (uint)(src[i] & 0x03);

				dst[(i * 4) + 0] = (byte)(b | (b << 2) | (b << 4) | (b << 6));
				dst[(i * 4) + 1] = (byte)(g | (g << 3) | (g << 6));
				dst[(i * 4) + 2] = (byte)(r | (r << 3) | (r << 6));
				dst[(i * 4) + 3] = 0xFF;
			}
		}
	}
}
