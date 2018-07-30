using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey.Graphics;

namespace AirHockey
{
    public class Scene
    {
        public Scene()
        {
            Visuals = new List<Visual>();
            Lights = new List<Light>();
            Shadows = new List<Shadow>();
            Ambient = new Color4(1, 1, 1, 1);
        }

        public void AddVisual(Visual visual)
        {
            Visuals.Add(visual);
        }

        public void SetAmbientLight(Color4 ambient)
        {
            Ambient = ambient;
        }
        public void AddLight(Light light)
        {
            Lights.Add(light);
        }
        public void AddShadow(Shadow shadow)
        {
            Shadows.Add(shadow);
        }

        public List<Light> Lights;
        public List<Visual> Visuals;
        public List<Shadow> Shadows;
        public Color4 Ambient;
    }
}
