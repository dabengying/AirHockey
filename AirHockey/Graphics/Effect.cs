using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace AirHockey.Graphics
{
    public class Effect
    {

        public Effect(String vertexShaderFile, String fragmentShaderFile)
        {
            VertexShaderId = ResourceManager.LoadVertexShader(vertexShaderFile);
            FragmentShaderId = ResourceManager.LoadFragmentShader(fragmentShaderFile);


            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, VertexShaderId);
            GL.AttachShader(ProgramId, FragmentShaderId);
            GL.LinkProgram(ProgramId);

        }

        public virtual void Begin()
        {

            GL.UseProgram(ProgramId);
        }

        public virtual void End()
        {
            GL.UseProgram(0);
        }

        protected int VertexShaderId;
        protected int FragmentShaderId;
        protected int ProgramId;

    }
}
