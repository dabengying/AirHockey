using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirHockey
{
    class GameFrame
    {
        public GameFrame()
        {
            Puck = new Body();
            Paddles = new Body[2];
            Paddles[0] = new Body();
            Paddles[1] = new Body();
            Reset();
        }

        public void Reset()
        {
            Puck.position = new Vector2(0.0f, 0.0f);
            Puck.velocity = new Vector2(0, 0);
            Paddles[0].position = new Vector2(0.0f, -0.7f);
            Paddles[1].position = new Vector2(0.0f, 0.7f);
        }

        public void DropPuck()
        {
            Puck.position = new Vector2(0, 0);
            Puck.velocity = new Vector2(0, 0);
        }

        public Body Puck;
        public Body[] Paddles;

        public Body[] Bodies
        {
            get
            {
                return new Body[3] {Puck, Paddles[0], Paddles[1]};
            }
        }
        
    }
}
