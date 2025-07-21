using System.Numerics;
using System.Runtime.InteropServices;
using Engine3.OpenGL.Exceptions;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Engine3.OpenGL.Objects {
	[PublicAPI]
	public class GLBuffer : OpenGLObject<BufferHandle> {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public int SizeInBytes { get; private set; }
		public override bool HasHandle => Handle.Handle != 0;

		private readonly BufferStorageMask storageMask;
		private readonly VertexBufferObjectUsage usage;

		public GLBuffer(BufferStorageMask storageMask) => this.storageMask = storageMask;
		public GLBuffer(VertexBufferObjectUsage usage) => this.usage = usage;

		public static implicit operator BufferHandle(GLBuffer self) => self.Handle;

		public void CreateBuffer() {
			if (!CheckValidForCreation()) { return; }
			Handle = GLH.CreateBuffer();
		}

		public void SetBuffer<T>(in ReadOnlySpan<T> data) where T : unmanaged, INumber<T> => SetBuffer(MemoryMarshal.AsBytes(data));

		public void SetBuffer(in ReadOnlySpan<byte> data) {
			if (!CheckValidForUse()) { return; }

			if (SizeInBytes != 0 && usage != 0) {
				Logger.Error("Cannot resize a GLBuffer that does not allow it");
				return;
			}

			SizeInBytes = data.Length;

			if (storageMask != 0) {
				GLH.NamedBufferStorage(Handle, data, storageMask); //
			} else if (usage != 0) {
				GLH.NamedBufferData(Handle, data, usage); //
			} else { throw new OpenGLException("Invalid GLBuffer type?"); }
		}

		public void EditBuffer(ReadOnlySpan<byte> data, int offset = 0) {
			if (!CheckValidForUse()) { return; }

			if (SizeInBytes == 0) {
				Logger.Error("Cannot edit buffer because it was never set");
				return;
			} else if (SizeInBytes < data.Length + offset) {
				Logger.Error("Cannot edit buffer because it will cause an overflow");
				return;
			}

			GLH.NamedBufferSubData(Handle, data, offset);
		}

		public void Bind(uint index) {
			if (!CheckValidForUse()) { return; }
			GLH.BindBufferBase(BufferTarget.ShaderStorageBuffer, index, Handle);
		}

		protected override void FreeHandle() {
			GLH.DeleteBuffer(Handle);
			Handle = new();
		}
	}
}