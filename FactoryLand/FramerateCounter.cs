using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class FramerateCounter
    {
        public double InstantaneousFps { get; private set; }
        public int AverageFps { get; private set; }

        private Queue<double> frameTimeBuffer = new Queue<double>();

        public void Update(GameTime gameTime)
        {
            InstantaneousFps = 1 / gameTime.ElapsedGameTime.TotalSeconds;

            frameTimeBuffer.Enqueue(gameTime.TotalGameTime.TotalSeconds);
            while (gameTime.TotalGameTime.TotalSeconds - frameTimeBuffer.Peek()  >= 1)
            {
                frameTimeBuffer.Dequeue();
            }
            AverageFps = frameTimeBuffer.Count;
        }

        public override string ToString()
        {
            return String.Format("Average: {0} Current: {1}", AverageFps, InstantaneousFps);
        }
    }
}
