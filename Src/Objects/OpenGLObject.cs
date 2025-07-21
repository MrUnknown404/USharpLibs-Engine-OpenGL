using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using NLog;

namespace Engine3.OpenGL.Objects {
	[PublicAPI]
	public abstract class OpenGLObject<THandle> where THandle : struct {
		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public THandle Handle { get; protected set; }
		public bool WasFreed { get; protected set; }

		public abstract bool HasHandle { get; }

		public void Free(bool callOpenGLDeleteFunc = true) {
			if (WasFreed) {
				Logger.Warn("Attempted to free an OpenGL object that was already freed");
				return;
			}

			if (callOpenGLDeleteFunc && HasHandle) { FreeHandle(); }
			WasFreed = true;
		}

		protected abstract void FreeHandle();

		protected bool CheckValidForCreation() {
			if (WasFreed) {
				Logger.Warn("Cannot use OpenGL object when it was freed");
				return false;
			}

			if (HasHandle) {
				Logger.Warn("Cannot call create method because we already have a handle");
				return false;
			}

			return true;
		}

		protected bool CheckValidForUse() {
			if (WasFreed) {
				Logger.Warn("Cannot use OpenGL object when it was freed");
				return false;
			}

			if (!HasHandle) {
				Logger.Warn("Cannot use OpenGL method because we do not have a handle");
				return false;
			}

			return true;
		}
	}
}