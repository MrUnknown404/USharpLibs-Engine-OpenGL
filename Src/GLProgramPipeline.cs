using Engine3.OpenGL.Exceptions;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;

namespace Engine3.OpenGL {
	[PublicAPI]
	public class GLProgramPipeline { // TODO technically this entire system is to allow hot swapping so i should probably implement that
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public ProgramPipelineHandle Handle { get; private set; }

		private readonly GLShader[] shaderPrograms;
		private readonly string debugName;

		public bool HasHandle => Handle.Handle != 0;

		public GLProgramPipeline(string debugName, GLShader[] shaderPrograms) {
			if (shaderPrograms.Length == 0) { throw new ArgumentOutOfRangeException(nameof(shaderPrograms), "GLShader array cannot be empty"); }

			this.debugName = debugName;
			this.shaderPrograms = shaderPrograms;
		}

		public static implicit operator ProgramPipelineHandle(GLProgramPipeline self) => self.Handle;

		public void CreatePipeline() {
			if (HasHandle) {
				Logger.Error("Cannot create program pipeline because it already has a handle.");
				return;
			}

			Handle = GLH.CreateProgramPipeline();
			foreach (GLShader shaderProgram in shaderPrograms) {
				if (!shaderProgram.HasHandle) { throw new OpenGLException($"Program Pipeline: {debugName}:{Handle} has an invalid shader program. No handle"); }

				GLH.UseProgramStages(Handle, shaderProgram.ShaderType, shaderProgram.Handle);
			}
		}

		public void Bind() => GLH.BindProgramPipeline(Handle);
	}
}