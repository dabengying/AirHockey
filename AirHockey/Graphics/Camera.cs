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

        public Matrix3 Transform
        {
            get
            {
                Matrix3 M = Matrix3.Identity;
                M.M13 = -Position.X;
                M.M23 = -Position.Y;
                M = M * Matrix3.CreateRotationZ(-Angle) * Matrix3.CreateScale(2.0f / Width, 2.0f / Height, 1.0f);
                return M;
            }
        }
    }

}