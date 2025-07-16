using Engine3.OpenGL.Exceptions;
using Engine3.OpenGL.Utils;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;

namespace Engine3.OpenGL {
	[PublicAPI]
	public class GLProgramPipeline {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public ProgramPipelineHandle Handle { get; private set; }
		public readonly GLShaderProgram[] ShaderPrograms;
		public readonly string DebugName;

		public bool HasHandle => Handle.Handle != 0;

		public GLProgramPipeline(string debugName, GLShaderProgram[] shaderPrograms) {
			if (shaderPrograms.Length == 0) { throw new ArgumentOutOfRangeException(nameof(shaderPrograms), "GLShaderProgram array cannot be empty"); }

			DebugName = debugName;
			ShaderPrograms = shaderPrograms;
		}

		public void CreateProgram() {
			if (HasHandle) {
				Logger.Error("Cannot create program pipeline because it already has a handle.");
				return;
			}

			Handle = GLH.CreateProgramPipeline();
			foreach (GLShaderProgram shaderProgram in ShaderPrograms) {
				if (!shaderProgram.HasHandle) { throw new OpenGLException($"Program Pipeline: {DebugName}:{Handle} has an invalid shader program. No handle"); }
				GLH.UseProgramStages(Handle, shaderProgram.ShaderType.ToUseProgramStageMask, shaderProgram.Handle);
			}
		}
	}
}