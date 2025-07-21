using Engine3.OpenGL.Exceptions;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;

namespace Engine3.OpenGL.Objects {
	[PublicAPI]
	public class GLProgramPipeline : OpenGLObject<ProgramPipelineHandle> { // TODO technically this entire system is to allow hot swapping so i should probably implement that
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public override bool HasHandle => Handle.Handle != 0;

		private readonly GLShader[] shaderPrograms;
		private readonly string debugName;

		public GLProgramPipeline(string debugName, GLShader[] shaderPrograms) {
			if (shaderPrograms.Length == 0) { throw new ArgumentOutOfRangeException(nameof(shaderPrograms), "GLShader array cannot be empty"); }

			this.debugName = debugName;
			this.shaderPrograms = shaderPrograms;
		}

		public static implicit operator ProgramPipelineHandle(GLProgramPipeline self) => self.Handle;

		public void CreatePipeline() {
			if (!CheckValidForCreation()) { return; }

			Handle = GLH.CreateProgramPipeline();
			foreach (GLShader shaderProgram in shaderPrograms) {
				if (!shaderProgram.HasHandle) { throw new OpenGLException($"Program Pipeline: {debugName}:{Handle} has an invalid shader program. No handle"); }

				GLH.UseProgramStages(Handle, shaderProgram.ShaderType, shaderProgram.Handle);
			}
		}

		public void Bind() {
			if (!CheckValidForUse()) { return; }
			GLH.BindProgramPipeline(Handle);
		}

		protected override void FreeHandle() {
			GLH.DeleteProgramPipeline(Handle);
			Handle = new();
		}
	}
}