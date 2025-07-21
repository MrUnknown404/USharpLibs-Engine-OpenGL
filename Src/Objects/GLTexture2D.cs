using System.Reflection;
using Engine3.IO;
using JetBrains.Annotations;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Engine3.OpenGL.Objects {
	[PublicAPI]
	public class GLTexture2D : OpenGLObject<TextureHandle> { // TODO mipmaps
		public override bool HasHandle => Handle.Handle != 0;

		public static implicit operator TextureHandle(GLTexture2D self) => self.Handle;

		public void CreateTexture(string name, Assembly assembly, string fileType = "png", SizedInternalFormat sizeFormat = SizedInternalFormat.Rgba8, PixelFormat pixelFormat = PixelFormat.Rgba,
				ColorComponents colorComponents = ColorComponents.RedGreenBlueAlpha, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear,
				TextureWrapMode wrapModeU = TextureWrapMode.Repeat, TextureWrapMode wrapModeV = TextureWrapMode.Repeat) {
			if (!CheckValidForCreation()) { return; }

			ushort width, height;
			byte[] data;

			using (Stream stream = AssetH.GetAssetStream($"Textures.{name}.{fileType}", assembly)) {
				ImageResult image = ImageResult.FromStream(stream, colorComponents);
				data = image.Data;
				width = (ushort)image.Width;
				height = (ushort)image.Height;
			}

			CreateTexture(data, width, height, sizeFormat, pixelFormat, minFilter, magFilter, wrapModeU, wrapModeV);
		}

		public void CreateTexture(ReadOnlySpan<byte> data, ushort width, ushort height, SizedInternalFormat sizeFormat = SizedInternalFormat.Rgba8, PixelFormat pixelFormat = PixelFormat.Rgba,
				TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear, TextureWrapMode wrapModeU = TextureWrapMode.Repeat, TextureWrapMode wrapModeV = TextureWrapMode.Repeat) {
			if (!CheckValidForCreation()) { return; }

			Handle = GLH.CreateTexture(TextureTarget.Texture2d);

			SetParameteri(TextureParameterName.TextureMinFilter, minFilter);
			SetParameteri(TextureParameterName.TextureMagFilter, magFilter);
			SetParameteri(TextureParameterName.TextureWrapS, wrapModeU);
			SetParameteri(TextureParameterName.TextureWrapT, wrapModeV);

			GLH.TextureStorage2D(Handle, 1, sizeFormat, width, height);
			GLH.TextureSubImage2D(Handle, 0, 0, 0, width, height, pixelFormat, PixelType.UnsignedByte, data);
		}

		public void SetParameteri(TextureParameterName parameterName, Enum value) {
			if (!CheckValidForUse()) { return; }
			GL.TextureParameteri(Handle.Handle, parameterName, Convert.ToInt32(value)); // Enum implements IConvertible so this should be safe?
		}

		public void Bind(uint index) {
			if (!CheckValidForUse()) { return; }
			GLH.BindTextureUnit(index, Handle);
		}

		protected override void FreeHandle() {
			GLH.DeleteTexture(Handle);
			Handle = new();
		}
	}
}