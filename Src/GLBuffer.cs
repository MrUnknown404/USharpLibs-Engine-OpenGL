using System.Numerics;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Engine3.OpenGL {
	[PublicAPI]
	public sealed class GLBuffer {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public BufferHandle Handle { get; private set; }
		public bool WasFreed { get; private set; }

		public bool HasHandle => Handle.Handle != 0;

		public void GenBuffer() {
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

		public void NamedBufferData<T>(ReadOnlySpan<T> data, VertexBufferObjectUsage usage) where T : unmanaged, INumber<T> {
			if (WasFreed) {
				Logger.Error("Cannot use any OpenGL methods with a freed object");
				return;
			}

			if (!HasHandle) {
				Logger.Error("Cannot use any OpenGL method because we do not have a handle");
				return;
			}

			GLH.NamedBufferData(Handle, data, usage);
		}

		public void NamedBufferStorage<T>(ReadOnlySpan<T> data, BufferStorageMask mask) where T : unmanaged, INumber<T> {
			if (WasFreed) {
				Logger.Error("Cannot use any OpenGL methods with a freed object");
				return;
			}

			if (!HasHandle) {
				Logger.Error("Cannot use any OpenGL method because we do not have a handle");
				return;
			}

			GLH.NamedBufferStorage(Handle, data, mask);
		}

		public void NamedBufferSubData<T>(ReadOnlySpan<T> data, int offset = 0) where T : unmanaged, INumber<T> {
			if (WasFreed) {
				Logger.Error("Cannot use any OpenGL methods with a freed object");
				return;
			}

			if (!HasHandle) {
				Logger.Error("Cannot use any OpenGL method because we do not have a handle");
				return;
			}

			GLH.NamedBufferSubData(Handle, data, offset);
		}

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