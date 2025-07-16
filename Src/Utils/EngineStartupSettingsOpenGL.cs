using Engine3.Api;
using Engine3.Api.Graphics;
using JetBrains.Annotations;
using OpenTK.Platform;

namespace Engine3.OpenGL.Utils {
	[PublicAPI]
	public class EngineStartupSettingsOpenGL : IEngineStartupSettings {
		public const string DefaultTitle = "Default Title - OpenGL";

		public IRenderContext RenderContext { get; } = new OpenGLRenderContext();
		public GraphicsApiHints? GraphicsApiHints { get; init; } = new OpenGLGraphicsApiHints {
				Version = new(4, 6),
				Profile = OpenGLProfile.Core,
#if DEBUG
				DebugFlag = true,
#endif
		};
		public ToolkitOptions? ToolkitOptions { get; init; } = new() { ApplicationName = DefaultTitle, };
		public string WindowTitle { get; init; } = DefaultTitle;
		public bool CenterWindow { get; init; }
	}
}