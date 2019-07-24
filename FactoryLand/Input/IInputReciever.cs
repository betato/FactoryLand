using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    /// <summary>
    /// Can recieve keyboard input from InputManager
    /// </summary>
    interface IInputReciever
    {
        /// <param name="input">The type of input defined in InputType</param>
        /// <param name="state">The state of the button sent as input</param>
        /// <param name="mousePos">The current position of the mouse cursor</param>
        /// <param name="scrollDelta">The difference in scroll wheel position since the last game update</param>
        void RecieveInput(InputType input, InputState state, Point mousePos, int scrollDelta);
    }
}
