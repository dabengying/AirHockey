using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;


namespace AirHockey.Graphics
{
    public class Geometry
    {
        public Geometry(PrimitiveType primitiveType, Vector2[] vertices, Color4[] colors, Vector2[] texCoords, int[] indices)
        {
            this.PrimitiveType = primitiveType;

            VertexCount = vertices.Length;
            if(indices != null)
                IndexCount = indices.Length;

            UsesTexCoords = texCoords != null;
            UsesIndices = indices != null;
           
            CreateVertexBufferObjects(vertices, colors, texCoords, indices);
            CreateVertexArrayObject();
        }

        public void Draw()
        {
            GL.BindVertexArray(VertexArrayId);

            if (UsesIndices)
                GL.DrawElements(PrimitiveType, IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            else
                GL.DrawArrays(PrimitiveType, 0, VertexCount);

            GL.BindVertexArray(0);
        }

        void CreateVertexBufferObjects(Vector2[] Vertices, Color4[] colors, Vector2[] texCoords, int[] indices)
        {
            
            GL.GenBuffers(1, out VertexBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(2 * sizeof(float) * Vertices.Length), Vertices, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out ColorBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(4 * sizeof(float) * colors.Length), colors, BufferUsageHint.StaticDraw);

            if (UsesTexCoords)
            {
                GL.GenBuffers(1, out TexCoordsBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, TexCoordsBufferId);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(2 * sizeof(float) * texCoords.Length), texCoords, BufferUsageHint.StaticDraw);
            }

            if (UsesIndices)
            {
                GL.GenBuffers(1, out IndexBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferId);
                GL.BufferData(BufferTarget.ElementArrayBuffer,
                    new IntPtr(sizeof(uint) * indices.Length),
                    indices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        void CreateVertexArrayObject()
        {
            GL.GenVertexArrays(1, out VertexArrayId);
            GL.BindVertexArray(VertexArrayId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferId);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);


            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferId);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, TexCoordsBufferId);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferId);

            GL.BindVertexArray(0);
        }

        PrimitiveType PrimitiveType;

        int VertexArrayId;

        int VertexBufferId;
        int ColorBufferId;
        int TexCoordsBufferId;
        int IndexBufferId;

        bool UsesTexCoords;
        bool UsesIndices;

        int VertexCount;
        int IndexCount;
    }
}
