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

        public Rectangle GetViewRect()
        {
            var inverseViewMatrix = Matrix.Invert(GetTransform());

            Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            Vector2 tr = Vector2.Transform(new Vector2(Viewport.Bounds.X, 0), inverseViewMatrix);
            Vector2 bl = Vector2.Transform(new Vector2(0, Viewport.Bounds.Y), inverseViewMatrix);
            Vector2 br = Vector2.Transform(new Vector2(Viewport.Bounds.Width, Viewport.Bounds.Height), inverseViewMatrix);

            Vector2 min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));

            Vector2 max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
