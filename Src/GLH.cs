using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Engine3.OpenGL {
	/*
	 * Note for code in this class. See https://github.com/opentk/opentk/issues/1409#issuecomment-1080789084
	 * or "With OpenTK 5 we are introducing types handles which means that instead of working with ints directly you will have something like ShaderHandle instead."
	 * Since the version of OpenTK i am currently using does not support this, these methods are wrappers are for that (with checks). these will probably be deleted once OpenTK adds support.
	 */

	[PublicAPI]
	public static class GLH {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public static ClearBufferMask ClearBufferMask { get; set; } = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BindVertexArray(VertexArrayHandle handle) => GL.BindVertexArray(handle.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DrawElements(int drawSize, int offset = 0) => GL.DrawElements(PrimitiveType.Triangles, drawSize, DrawElementsType.UnsignedInt, offset);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MustUseReturnValue]
		public static BufferHandle CreateBuffer() => new(GL.CreateBuffer());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeleteBuffer(BufferHandle handle) => GL.DeleteBuffer(handle.Handle);

		[MustUseReturnValue]
		public static BufferHandle[] CreateBuffers(int count) {
			int[] handles = new int[count];
			GL.CreateBuffers(handles.Length, handles);
			return handles.Select(static h => new BufferHandle(h)).ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeleteBuffers(BufferHandle[] handles) => GL.DeleteBuffers(handles.Length, handles.Select(static h => h.Handle).ToArray());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MustUseReturnValue]
		public static VertexArrayHandle CreateVertexArray() => new(GL.CreateVertexArray());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeleteVertexArray(VertexArrayHandle handle) => GL.DeleteVertexArray(handle.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NamedBufferData<T>(BufferHandle handle, ReadOnlySpan<T> data, VertexBufferObjectUsage usage) where T : unmanaged, INumber<T> {
			unsafe { GL.NamedBufferData(handle.Handle, data.Length * sizeof(T), data, usage); }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NamedBufferData(BufferHandle handle, int size, VertexBufferObjectUsage usage) => GL.NamedBufferData(handle.Handle, size, IntPtr.Zero, usage);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NamedBufferStorage<T>(BufferHandle handle, ReadOnlySpan<T> data, BufferStorageMask mask) where T : unmanaged, INumber<T> {
			unsafe { GL.NamedBufferStorage(handle.Handle, data.Length * sizeof(T), data, mask); }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NamedBufferStorage(BufferHandle handle, int size, BufferStorageMask mask) => GL.NamedBufferStorage(handle.Handle, size, IntPtr.Zero, mask);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NamedBufferSubData<T>(BufferHandle handle, ReadOnlySpan<T> data, int offset) where T : unmanaged, INumber<T> {
			unsafe { GL.NamedBufferSubData(handle.Handle, offset, data.Length * sizeof(T), data); }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BindBufferBase(BufferTarget bufferTarget, uint index, BufferHandle handle) => GL.BindBufferBase(bufferTarget, index, handle.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MustUseReturnValue]
		public static ProgramPipelineHandle CreateProgramPipeline() => new(GL.CreateProgramPipeline());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BindProgramPipeline(ProgramPipelineHandle handle) => GL.BindProgramPipeline(handle.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UseProgramStages(ProgramPipelineHandle pipelineHandle, UseProgramStageMask stageMask, ShaderHandle shaderHandle) => GL.UseProgramStages(pipelineHandle.Handle, stageMask, shaderHandle.Handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetProgramPipelinei(ProgramPipelineHandle handle, PipelineParameterName parameterName, out int parameters) => GL.GetProgramPipelinei(handle.Handle, parameterName, out parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetProgrami(ShaderHandle handle, ProgramProperty programProperty, out int parameters) => GL.GetProgrami(handle.Handle, programProperty, out parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetUniformLocation(ShaderHandle handle, string name) => GL.GetUniformLocation(handle.Handle, name);
	}
}