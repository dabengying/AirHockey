using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLib;

namespace AirHockey
{
    class AI
    {
        enum AIState
        {
            Attack,
            Defense
        }

        public AI()
        {
            speed = 10.0f;
            state = AIState.Defense;
        }

        public void Update(float deltaTime, Table table, Vector2 puckPosition, float puckRadius, float paddleRadius)
        {
            thinkTimer += deltaTime;
            if (thinkTimer > 0.4)
            {
                thinkTimer = 0.0f;

                if (puckPosition.Y < 0) // puck is on player side, move towards defensive position(close to goal).
                    state = AIState.Defense;
                // else if (puck.velocity.Y < 0)
                //     aiState = AIState.Defense;
                // else if ((puck.position - paddles[1].position).Y + puckRadius + paddleRadius > 0)
                //     aiState = AIState.Defense;
                else
                    state = AIState.Attack;
            }

            Vector2 trajectory;

            if (state == AIState.Defense)
            {
                target = new Vector2(0, 0.4f * table.height);
                target.X += puckPosition.X;
            }
            else
            {
                target = puckPosition + new Vector2(0, puckRadius * 0.6f); // try to hit near the top but a little inside
            }

            trajectory = target - position;
            trajectory.Normalize();

            velocity = trajectory * speed;


            // don't overshoot target
            if (speed * deltaTime > (target - position).GetLength() - puckRadius - paddleRadius && state == AIState.Defense)
            { //TODO: this is wrong, the impulse should not be small when we are really close...
                velocity = trajectory * ((target - position).GetLength() - puckRadius - paddleRadius) / deltaTime;
            }

            position += velocity * deltaTime;
        }

        public void PuckCollision()
        {
            if (state == AIState.Attack)
            {
                state = AIState.Defense;
                thinkTimer = 0.0f;
            }
        }


        public Vector2 position;
        public Vector2 velocity;

        AIState state;
        float thinkTimer;
        Vector2 target;
        float speed;
    }
}
