using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using IrrKlang;
using AirHockey.Graphics;

namespace AirHockey
{
    class ResourceManager
    {
        public ResourceManager(Renderer renderer, ISoundEngine soundEngine)
        {
            loadedTextures = new Dictionary<string, int>();
        }

        public static int GetTexture(String filename)
        {
            int texture = 0;
            if (!loadedTextures.TryGetValue(filename, out texture))
            {

                texture = LoadTexture(MediaPath + filename);
                loadedTextures.Add(filename, texture);
                return texture;
            }
            return loadedTextures[filename];
        }


        public static int LoadTexture(string file)
        {
            Bitmap bitmap = new Bitmap(file);

            int tex = 0;
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return tex;
        }

        public static int LoadVertexShader(string name)
        {
            string VertexShader = "";
            int VertexShaderId = 0;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(name))
                {
                    // Read the stream to a string, and write the string to the console.
                    VertexShader = sr.ReadToEnd();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return -1;
            }


            VertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShaderId, VertexShader);
            GL.CompileShader(VertexShaderId);

            return VertexShaderId;
        }

        public static int LoadFragmentShader(string name)
        {
            int FragmentShaderId = 0;
            string FragmentShader = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(name))
                {
                    // Read the stream to a string, and write the string to the console.
                    FragmentShader = sr.ReadToEnd();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }


            FragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShaderId, FragmentShader);
            GL.CompileShader(FragmentShaderId);

            return FragmentShaderId;

        }


        static Dictionary<String, int> loadedTextures;
        public const String MediaPath = "../../media/";
    }
}
