using System.Reflection;
using Engine3.IO;
using Engine3.OpenGL.Utils;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine3.OpenGL.Objects {
	[PublicAPI]
	public class GLShader : OpenGLObject<ShaderHandle> { // TODO implement hot reloading
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public readonly ShaderType ShaderType;

		public override bool HasHandle => Handle.Handle != 0;

		private readonly string debugName;
		private readonly string fileName;
		private readonly Lazy<Assembly> assembly;
		private readonly Dictionary<string, int> uniformLocations = new();

		public GLShader(ShaderType shaderType, string debugName, string fileName, Lazy<Assembly>? assembly = null) {
			ShaderType = shaderType;
			this.debugName = debugName;
			this.fileName = fileName;
			this.assembly = assembly ?? new(static () => GameEngine.InstanceAssembly ?? throw new NullReferenceException());
		}

		public static implicit operator ShaderHandle(GLShader self) => self.Handle;

		public void CreateShader() {
			if (!CheckValidForCreation()) { return; }

			string str;
			using (Stream stream = AssetH.GetAssetStream(assembly.Value, $"Shaders.{fileName}.{ShaderType.ToFileExtension}")) {
				using (StreamReader reader = new(stream)) { str = reader.ReadToEnd(); }
			}

			Handle = GLH.CreateShader(ShaderType, str);

			GLH.GetProgrami(Handle, ProgramProperty.ActiveUniforms, out int numberOfUniforms);
			GLH.GetProgrami(Handle, ProgramProperty.ActiveUniformMaxLength, out int maxNameLength);

			for (uint i = 0; i < numberOfUniforms; i++) {
				string name = GLH.GetActiveUniform(Handle, i, maxNameLength, out int _, out int _, out UniformType _);
				uniformLocations.Add(name, GLH.GetUniformLocation(Handle, name));
			}
		}

		private bool CheckContains(string name, out int uniformLocation) {
			if (!uniformLocations.TryGetValue(name, out uniformLocation)) {
				Logger.Warn($"Tried to set variable named '{name}' in shader '{debugName}' but it doesn't exist!");
				return true;
			}

			return false;
		}

		public void SetUniform(string name, bool value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniform1i(Handle.Handle, uniformLocation, value ? 1 : 0);
		}

		public void SetUniform(string name, int value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniform1i(Handle.Handle, uniformLocation, value);
		}

		public void SetUniform(string name, float value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniform1f(Handle.Handle, uniformLocation, value);
		}

		public void SetUniform(string name, Vector2 value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniform2f(Handle.Handle, uniformLocation, value.X, value.Y);
		}

		public void SetUniform(string name, Vector3 value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniform3f(Handle.Handle, uniformLocation, value.X, value.Y, value.Z);
		}

		public void SetUniform(string name, Vector4 value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniform4f(Handle.Handle, uniformLocation, value.X, value.Y, value.Z, value.W);
		}

		public void SetUniform(string name, Matrix4 value) {
			if (!CheckValidForUse()) { return; }
			if (CheckContains(name, out int uniformLocation)) { return; }
			GL.ProgramUniformMatrix4f(Handle.Handle, uniformLocation, 1, true, in value); // TODO look into this transpose variable. i think i can fix opentk's row-column thing with this?
		}

		protected override void FreeHandle() {
			GLH.DeleteShader(Handle);
			Handle = new();
		}
	}
}