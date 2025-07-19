using System.Numerics;
using System.Runtime.InteropServices;
using Engine3.OpenGL.Exceptions;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Engine3.OpenGL {
	[PublicAPI]
	public class GLBuffer {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public BufferHandle Handle { get; private set; }
		public int Size { get; private set; }
		public bool WasFreed { get; private set; }

		private readonly BufferStorageMask storageMask;
		private readonly VertexBufferObjectUsage usage;

		public bool HasHandle => Handle.Handle != 0;

		public static implicit operator BufferHandle(GLBuffer self) => self.Handle;

		public GLBuffer(BufferStorageMask storageMask) => this.storageMask = storageMask;
		public GLBuffer(VertexBufferObjectUsage usage) => this.usage = usage;

		public void CreateBuffer() {
			if (WasFreed) {
				Logger.Error("Cannot create buffer because it was already freed");
				return;
			}

			if (HasHandle) {
				Logger.Error("Cannot create buffer because it already has a handle.");
				return;
			}

			Handle = GLH.CreateBuffer();
		}

		public void SetBuffer<T>(in ReadOnlySpan<T> data) where T : unmanaged, INumber<T> => SetBuffer(MemoryMarshal.AsBytes(data));

		public void SetBuffer(in ReadOnlySpan<byte> data) {
			if (WasFreed) {
				Logger.Error("Cannot use any OpenGL methods with a freed object");
				return;
			}

			if (!HasHandle) {
				Logger.Error("Cannot use any OpenGL method because we do not have a handle");
				return;
			}

			if (Size != 0 && usage != 0) {
				Logger.Error("Cannot resize a GLBuffer that does not allow it");
				return;
			}

			Size = data.Length;

			if (storageMask != 0) {
				GLH.NamedBufferStorage(Handle, data, storageMask); //
			} else if (usage != 0) {
				GLH.NamedBufferData(Handle, data, usage); //
			} else { throw new OpenGLException("Invalid GLBuffer type"); }
		}

		public void EditBuffer(ReadOnlySpan<byte> data, int offset = 0) {
			if (WasFreed) {
				Logger.Error("Cannot use any OpenGL methods with a freed object");
				return;
			}

			if (!HasHandle) {
				Logger.Error("Cannot use any OpenGL method because we do not have a handle");
				return;
			}

			if (Size == 0) {
				Logger.Error("Cannot edit buffer because it was never set");
				return;
			} else if (Size < data.Length + offset) {
				Logger.Error("Cannot edit buffer because it will cause an overflow");
				return;
			}

			GLH.NamedBufferSubData(Handle, data, offset);
		}

		public void Bind(uint index) => GLH.BindBufferBase(BufferTarget.ShaderStorageBuffer, index, Handle);

		public void Free(bool deleteBuffer = true) {
			if (WasFreed) {
				Logger.Warn("Cannot free an already freed object");
				return;
			}

			if (deleteBuffer && HasHandle) {
				GLH.DeleteBuffer(Handle);
				Handle = new();
			}

			WasFreed = true;
		}
	}
}