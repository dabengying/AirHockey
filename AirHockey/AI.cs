using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirHockey;

namespace AirHockey
{
    class AIPlayer  
    {
        enum AIState
        {
            MoveToAttackPosition,
            Attack,
            Defense
        }

        public AIPlayer()
        {
            speed = 8.0f;
            state = AIState.Defense;
        }

        public void Update(GameFrame gameFrame, float deltaTime)
        {
            Body playerPaddle = gameFrame.Paddles[1];
            Body opponentPaddle = gameFrame.Paddles[0];
            Body puck = gameFrame.Puck;
            thinkTimer += deltaTime;
            if (thinkTimer > 0.4)
            {
                thinkTimer = 0.0f;

                if (puck.position.Y < 0) // puck is on player side, move towards defensive position(close to goal).
                    state = AIState.Defense;
                // else if (puck.velocity.Y < 0)
                //     aiState = AIState.Defense;
                // else if ((puck.position - paddles[1].position).Y + Constants.puckRadius + Constants.paddleRadius > 0)
                //     aiState = AIState.Defense;
                else
                    state = AIState.Attack;
            }

            Vector2 trajectory;

            if (state == AIState.Defense)
            {
                target = new Vector2(0, 0.4f * Constants.tableHeight);
                target.X += puck.position.X;
            }
            else
            {
                target = puck.position + new Vector2(0, Constants.puckRadius * 0.6f); // try to hit near the top but a little inside
            }

            trajectory = target - playerPaddle.position;
            trajectory.Normalize();

            playerPaddle.velocity = trajectory * speed;


            // don't overshoot target
            if (speed * deltaTime > (target - playerPaddle.position).GetLength() - Constants.puckRadius - Constants.paddleRadius && state == AIState.Defense)
            { //TODO: this is wrong, the impulse should not necessarily be small when we are really close...
                playerPaddle.velocity = trajectory * ((target - playerPaddle.position).GetLength() - Constants.puckRadius - Constants.paddleRadius) / deltaTime;
            }
        }

        public void PuckCollision()
        {
            if (state == AIState.Attack)
            {
                state = AIState.Defense;
                thinkTimer = 0.0f;
            }
        }

        AIState state;
        float thinkTimer;
        Vector2 target;
        float speed;
    }
}
