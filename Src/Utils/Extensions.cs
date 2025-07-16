using System.Diagnostics.CodeAnalysis;
using OpenTK.Graphics.OpenGL;

namespace Engine3.OpenGL.Utils {
	[SuppressMessage("Performance", "CA1822:Mark members as static")] // but i can't? is there
	[SuppressMessage("ReSharper", "UnusedType.Global")] // Resharper broken? i'm using it???
	public static class Extensions {
		extension(ShaderType self) {
			public UseProgramStageMask ToUseProgramStageMask =>
					self switch {
							ShaderType.FragmentShader => UseProgramStageMask.FragmentShaderBit,
							ShaderType.VertexShader => UseProgramStageMask.VertexShaderBit,
							ShaderType.GeometryShader => UseProgramStageMask.GeometryShaderBit,
							ShaderType.TessEvaluationShader => UseProgramStageMask.TessEvaluationShaderBit,
							ShaderType.TessControlShader => UseProgramStageMask.TessControlShaderBit,
							ShaderType.ComputeShader => UseProgramStageMask.ComputeShaderBit,
							_ => throw new ArgumentOutOfRangeException(nameof(self), self, null),
					};
			public string ToFileExtension =>
					self switch {
							ShaderType.FragmentShader => "frag",
							ShaderType.VertexShader => "vert",
							ShaderType.GeometryShader => "geom",
							ShaderType.TessEvaluationShader => "tese",
							ShaderType.TessControlShader => "tesc",
							ShaderType.ComputeShader => "comp",
							_ => throw new ArgumentOutOfRangeException(nameof(self), self, null),
					};
		}
	}
}