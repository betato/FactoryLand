using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Selector : IDrawableEntity
    {
        public Vector2 Location { get; private set; }
        public Point Tile { get; private set; }

        public void Update(Vector2 location)
        {
            Location = location;
            Tile = location.ToPoint();
        }

        public void GetVertexData(out VertexPositionColorTexture[] verticies, out short[] indicies, out Texture2D texture)
        {
            verticies = new VertexPositionColorTexture[4];
            verticies[0] = new VertexPositionColorTexture(new Vector3(Tile.X, Tile.Y, 0), Color.White, new Vector2(0, 0));
            verticies[1] = new VertexPositionColorTexture(new Vector3(Tile.X + 1, Tile.Y, 0), Color.White, new Vector2(1, 0));
            verticies[2] = new VertexPositionColorTexture(new Vector3(Tile.X, Tile.Y + 1, 0), Color.White, new Vector2(0, 1));
            verticies[3] = new VertexPositionColorTexture(new Vector3(Tile.X + 1, Tile.Y + 1, 0), Color.White, new Vector2(1, 1));
            
            indicies = new short[6];
            indicies[0] = 0;
            indicies[1] = 1;
            indicies[2] = 2;
            indicies[3] = 1;
            indicies[4] = 3;
            indicies[5] = 2;

            texture = TextureManager.GetTexture(TextureId.Selection);
        }
    }
}
