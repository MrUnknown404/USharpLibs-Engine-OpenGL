using Engine3.Api.Graphics;
using Engine3.OpenGL.Exceptions;
using JetBrains.Annotations;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Engine3.OpenGL.Objects {
	[PublicAPI]
	public class GLProgramPipeline : OpenGLObject<ProgramPipelineHandle>, IProgramPipeline {
		public override bool HasHandle => Handle.Handle != 0;

		private readonly string debugName;

		public IShaderAccess? VertexShader { get; }
		public IShaderAccess? FragmentShader { get; }
		public IShaderAccess? GeometryShader { get; }
		public IShaderAccess? TessEvaluationShader { get; }
		public IShaderAccess? TessControlShader { get; }
		// TODO compute?
		// TODO shader hot-swapping

		public GLProgramPipeline(string debugName, GLShader? vert, GLShader? frag, GLShader? geom = null, GLShader? tessEval = null, GLShader? tessCtrl = null) {
			this.debugName = debugName;
			VertexShader = vert;
			FragmentShader = frag;
			GeometryShader = geom;
			TessEvaluationShader = tessEval;
			TessControlShader = tessCtrl;
		}

		public void CreatePipeline() {
			void TryAddStage(IShaderAccess? shaderAccess, ShaderType shaderType) {
				if (shaderAccess == null) { return; }
				if (!shaderAccess.HasHandle) { throw new OpenGLException($"Program Pipeline: {debugName}:{Handle} has an invalid shader program. No handle"); }

				GLH.UseProgramStages(Handle, shaderType, shaderAccess.Handle);
			}

			if (!CheckValidForCreation()) { return; }

			Handle = GLH.CreateProgramPipeline();

			TryAddStage(VertexShader, ShaderType.VertexShader);
			TryAddStage(FragmentShader, ShaderType.FragmentShader);
			TryAddStage(GeometryShader, ShaderType.GeometryShader);
			TryAddStage(TessEvaluationShader, ShaderType.TessEvaluationShader);
			TryAddStage(TessControlShader, ShaderType.TessControlShader);
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