using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    /// <summary>
    /// A movable and zoomable camera 
    /// </summary>
    class Camera
    {
        public float Zoom { get; set; } = 1;
        public Vector2 Location { get; set; }

        public Matrix View
        {
            get
            {
                return Matrix.CreateTranslation(-Location.X, -Location.Y, -1) * Matrix.CreateScale(Zoom);
            }
        }
    }
}
