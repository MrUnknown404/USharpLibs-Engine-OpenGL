using System.Runtime.InteropServices;
using Engine3.Api.Graphics;
using Engine3.Graphics;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine3.OpenGL.Utils {
	public class OpenGLRenderContext : IRenderContext {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public RenderSystem RenderSystem => RenderSystem.OpenGL;

		private static OpenGLContextHandle? openGLContextHandle;

		public void Setup(WindowHandle windowHandle) {
			Logger.Debug("Creating and setting OpenGL context...");
			openGLContextHandle = Toolkit.OpenGL.CreateFromWindow(windowHandle);
			Toolkit.OpenGL.SetCurrentContext(openGLContextHandle);
			GLLoader.LoadBindings(Toolkit.OpenGL.GetBindingsContext(openGLContextHandle));

			Logger.Debug($"- OpenGL version: {GL.GetString(StringName.Version)}");
			Logger.Debug($"- GLFW Version: {GLFW.GetVersionString()}");

#if DEBUG
			Logger.Debug("- OpenGL Callbacks are enabled");

			GL.Enable(EnableCap.DebugOutput);
			GL.Enable(EnableCap.DebugOutputSynchronous);

			uint[] defaultIds = [
					131185, // Nvidia static buffer notification
			];

			GL.DebugMessageControl(DebugSource.DebugSourceApi, DebugType.DebugTypeOther, DebugSeverity.DontCare, defaultIds.Length, defaultIds, false);

			GL.DebugMessageCallback(static (source, type, id, severity, length, message, _) => {
				switch (severity) {
					case DebugSeverity.DontCare: return;
					case DebugSeverity.DebugSeverityNotification:
						Logger.Debug($"OpenGL Notification: {id}. Source: {source.ToString()[11..]}, Type: {type.ToString()[9..]}");
						Logger.Debug($"- {Marshal.PtrToStringAnsi(message, length)}");
						break;
					case DebugSeverity.DebugSeverityHigh:
						Logger.Fatal($"OpenGL Fatal Error: {id}. Source: {source.ToString()[11..]}, Type: {type.ToString()[9..]}");
						Logger.Fatal($"- {Marshal.PtrToStringAnsi(message, length)}");
						break;
					case DebugSeverity.DebugSeverityMedium:
						Logger.Error($"OpenGL Error: {id}. Source: {source.ToString()[11..]}, Type: {type.ToString()[9..]}");
						Logger.Error($"- {Marshal.PtrToStringAnsi(message, length)}");
						break;
					case DebugSeverity.DebugSeverityLow:
						Logger.Warn($"OpenGL Warning: {id}. Source: {source.ToString()[11..]}, Type: {type.ToString()[9..]}");
						Logger.Warn($"- {Marshal.PtrToStringAnsi(message, length)}");
						break;
					default: throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
				}
			}, IntPtr.Zero);
#endif

			GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		}

		public void PrepareFrame() => GL.Clear(GLH.ClearBufferMask);
		public void SwapBuffers() => Toolkit.OpenGL.SwapBuffers(openGLContextHandle!);
	}
}