using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace AirHockey.Graphics
{
    class TextureLightEffect : Effect
    {
        public TextureLightEffect(string textureFile) 
            : base(@"..\..\shaders\textureLight.vsh", @"..\..\shaders\textureLight.psh")
        {
            worldViewMatrixLocation = GL.GetUniformLocation(ProgramId, "worldViewMatrix");
            WorldViewMatrix = Matrix3.Identity;
            worldMatrixLocation = GL.GetUniformLocation(ProgramId, "worldMatrix");
            WorldMatrix = Matrix3.Identity;
            lightPositionsLocation = GL.GetUniformLocation(ProgramId, "lightPositions");
            lightKsLocation = GL.GetUniformLocation(ProgramId, "lightKs");
            lightPsLocation = GL.GetUniformLocation(ProgramId, "lightPs");
            lightColorsLocation = GL.GetUniformLocation(ProgramId, "lightColors");
            numLightsLocation = GL.GetUniformLocation(ProgramId, "numLights");
            Lights = new Light[0];
            TextureId = ResourceManager.GetTexture(textureFile);
            textureLocation = GL.GetUniformLocation(ProgramId, "tex");
            ambientLocation = GL.GetUniformLocation(ProgramId, "ambient");
        }


        public override void Begin()
        {
            base.Begin();
            GL.Uniform1(textureLocation, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.UniformMatrix3(worldViewMatrixLocation, true, ref WorldViewMatrix);
            GL.UniformMatrix3(worldMatrixLocation, true, ref WorldMatrix);

            float[] lightViewPositions = new float[3 * Lights.Length];
            float[] lightColors = new float[4 * Lights.Length];
            float[] lightKs = new float[Lights.Length];
            float[] lightPs = new float[Lights.Length];

            for (int i = 0; i < Lights.Length; i++)
            {
                OpenTK.Vector3 lightViewPosition = new OpenTK.Vector3(Lights[i].Position.X, Lights[i].Position.Y, 1);

                lightViewPositions[i*3 + 0] = lightViewPosition.X;
                lightViewPositions[i*3 + 1] = lightViewPosition.Y;
                lightViewPositions[i*3 + 2] = lightViewPosition.Z;

                lightColors[i*4 + 0] = Lights[i].Color.R;
                lightColors[i*4 + 1] = Lights[i].Color.G;
                lightColors[i*4 + 2] = Lights[i].Color.B;
                lightColors[i*4 + 3] = Lights[i].Color.A;

                lightKs[i] = Lights[i].K;
                lightPs[i] = Lights[i].P;

            }
            GL.Uniform3(lightPositionsLocation, Lights.Length, lightViewPositions);
            GL.Uniform4(lightColorsLocation, Lights.Length, lightColors);
            GL.Uniform1(lightKsLocation, Lights.Length, lightKs);
            GL.Uniform1(lightPsLocation, Lights.Length, lightPs);
            GL.Uniform1(numLightsLocation, Lights.Length);
            Vector4 vAmbient = new Vector4(Ambient.R, Ambient.G, Ambient.B, Ambient.A);
            GL.Uniform4(ambientLocation, ref vAmbient);

        }

        public override void End()
        {
            base.End();
        }

        public Matrix3 WorldViewMatrix;
        public Matrix3 WorldMatrix;
        public Light[] Lights;
        public Color4 Ambient;
        public int TextureId;

        int worldViewMatrixLocation;
        int worldMatrixLocation;
        int lightPositionsLocation;
        int lightKsLocation;
        int lightPsLocation;
        int lightColorsLocation;
        int numLightsLocation;
        int textureLocation;
        int ambientLocation;
    }
}
