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
        public Camera(Viewport viewport)
        {
            Viewport = viewport;
        }

        public float Zoom { get; set; } = 1;
        public Vector2 Location { get; set; }
        public Viewport Viewport { get; set; } 

        public Matrix GetTransform()
        {
            return Matrix.CreateTranslation(new Vector3(-Location.X, -Location.Y, 0)) * Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(new Vector3(Viewport.Width * 0.5f, Viewport.Height * 0.5f, 0));
        }
    }
}
