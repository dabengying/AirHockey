using System.Collections.Generic;
using AirHockey.Graphics;

namespace AirHockey
{
    //just a play one animation(for now...).
    class Animation
    {

        public Animation()
        {
            frames = new List<Frame>();
            isPlaying = false;
            currentFrame = 0;
            frameTime = 0.0f;
        }
        public void Play()
        {
            currentFrame = 0;
            frameTime = 0.0f;
            isPlaying = true;
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
        }

        public void Stop()
        {
            isPlaying = false;
            currentFrame = 0;
            frameTime = 0.0f;
        }

        public Visual GetGraphicsObject()
        {
            if (currentFrame >= frames.Count)
                return frames[frames.Count - 1].graphicsObject;
            return frames[currentFrame].graphicsObject;
        }

        public void Update(float deltaTime)
        {
            frameTime += deltaTime;
            while (frameTime >= frames[currentFrame].time && currentFrame < frames.Count)
            {

                frameTime = frameTime - frames[currentFrame].time;
                currentFrame++;
                if (currentFrame >= frames.Count)
                {
                    isPlaying = false;
                    break;
                }
            }

        }

        public void AddFrame(Visual graphicsObject, float time, OpenTK.Box2 aabb)
        {
            Frame frame = new Frame();
            frame.graphicsObject = graphicsObject;;
            frame.time = time;
            frame.aabb = aabb;
            frames.Add(frame);
        }

        struct Frame
        {
            public Visual graphicsObject;
            public float time;
            public OpenTK.Box2 aabb;
        }

        bool isPlaying;
        float frameTime;
        int currentFrame;
        List<Frame> frames;

    }

}
