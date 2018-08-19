using OpenTK;

namespace AirHockey.Graphics
{
    public class Camera
    {
        public Camera()
        {
            Position = new Vector2(0, 0);
            Angle = 0.0f;
            Width = 1.0f;
            Height = 9.0f / 16.0f;
        }
        public Vector2 Position;
        public float Angle;
        public float Width;
        public float Height;

        public Matrix3 ViewMatrix
        {
            get
            {
                Matrix3 translation = Matrix3.Identity;
                translation.M13 = -Position.X;
                translation.M23 = -Position.Y;
                return translation * Matrix3.CreateRotationZ(-Angle);
                
            }
        }

        public Matrix3 ProjectionMatrix
        {
            get
            {
                return Matrix3.CreateScale(2.0f / Width, 2.0f / Height, 1.0f);
            }
        }
    }

}