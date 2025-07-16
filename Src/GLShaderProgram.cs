using System.Reflection;
using Engine3.IO;
using Engine3.OpenGL.Utils;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine3.OpenGL {
	[PublicAPI]
	public class GLShaderProgram {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public ShaderHandle Handle { get; private set; }
		public readonly ShaderType ShaderType;
		public readonly string DebugName;

		private readonly string fileName;
		private readonly Lazy<Assembly> assembly;
		private readonly Dictionary<string, int> uniformLocations = new();

		public bool HasHandle => Handle.Handle != 0;

		public GLShaderProgram(ShaderType shaderType, string debugName, string fileName, Lazy<Assembly>? assembly = null) {
			ShaderType = shaderType;
			DebugName = debugName;
			this.fileName = fileName;
			this.assembly = assembly ?? new(static () => GameEngine.InstanceAssembly ?? throw new NullReferenceException());
		}

		public void CreateShader() {
			if (HasHandle) {
				Logger.Error("Cannot create shader because it already has a handle.");
				return;
			}

			string str;
			using (Stream stream = AssetH.GetAssetStream(assembly.Value, $"Shaders.{fileName}.{ShaderType.ToFileExtension}")) {
				using (StreamReader reader = new(stream)) { str = reader.ReadToEnd(); }
			}

			Handle = new(GL.CreateShaderProgram(ShaderType, str));

			GLH.GetProgrami(Handle, ProgramProperty.ActiveUniforms, out int numberOfUniforms);
			GLH.GetProgrami(Handle, ProgramProperty.ActiveUniformMaxLength, out int maxNameLength);

			for (uint i = 0; i < numberOfUniforms; i++) {
				string name = GL.GetActiveUniform(Handle.Handle, i, maxNameLength, out int _, out int _, out UniformType _);
				uniformLocations.Add(name, GLH.GetUniformLocation(Handle, name));
			}
		}

		private void SetUniform<V>(string name, V data, [RequireStaticDelegate] SetUniformDelegate<V> uniformFunc) where V : struct {
			if (!uniformLocations.TryGetValue(name, out int value)) {
				Logger.Warn($"Tried to set variable named '{name}' in shader '{DebugName}' but it doesn't exist!");
				return;
			}

			uniformFunc(Handle.Handle, value, data);
		}

		private void SetMatrix(string name, bool transpose, Matrix4 data) {
			if (!uniformLocations.TryGetValue(name, out int value)) {
				Logger.Warn($"Tried to set variable named '{name}' in shader '{DebugName}' but it doesn't exist!");
				return;
			}

			GL.ProgramUniformMatrix4f(Handle.Handle, value, 1, transpose, in data); // TODO look into this transpose variable. i think i can fix opentk's row-column thing with this?
		}

		public void SetBool(string name, bool data) => SetUniform(name, data ? 1 : 0, GL.ProgramUniform1i);
		public void SetInt(string name, int data) => SetUniform(name, data, GL.ProgramUniform1i);
		public void SetFloat(string name, float data) => SetUniform(name, data, GL.ProgramUniform1f);
		public void SetVector2(string name, Vector2 data) => SetUniform(name, data, static (program, uniform, data) => GL.ProgramUniform2f(program, uniform, data.X, data.Y));
		public void SetVector3(string name, Vector3 data) => SetUniform(name, data, static (program, uniform, data) => GL.ProgramUniform3f(program, uniform, data.X, data.Y, data.Z));
		public void SetVector4(string name, Vector4 data) => SetUniform(name, data, static (program, uniform, data) => GL.ProgramUniform4f(program, uniform, data.X, data.Y, data.Z, data.W));
		public void SetMatrix4(string name, in Matrix4 data) => SetMatrix(name, true, data);
		public void SetColor<T>(string name, Color4<T> data) where T : IColorSpace4 => SetVector4(name, (Vector4)data);

		protected delegate void SetUniformDelegate<in V>(int handle, int uniform, V value) where V : struct;
	}
}