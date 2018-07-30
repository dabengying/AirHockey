using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace AirHockey.Graphics
{
    class VertexColorEffect : Effect
    {
        public VertexColorEffect() 
            : base(@"..\..\shaders\vertexColor.vsh", @"..\..\shaders\vertexColor.psh")
        {
            worldViewMatrixLocation = GL.GetUniformLocation(ProgramId, "worldViewMatrix");
            WorldViewMatrix = Matrix3.Identity;
        }

        public override void Begin()
        {
            base.Begin();
            GL.UniformMatrix3(worldViewMatrixLocation, true, ref WorldViewMatrix);
        }

        public override void End()
        {
            base.End();
        }

        public Matrix3 WorldViewMatrix;

        int worldViewMatrixLocation;
    }
}
