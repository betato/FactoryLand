using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Generator
    {
        private const float LAND_THRESHOLD = -0.1f;
        private const float MAP_SCALE = 0.1f;
        private const int GRADIENT_MAP_SIZE = (int)(Chunk.SIZE * MAP_SCALE) + 2;

        public int Seed { get; private set; }
        private Random rng = new Random();
        private Vector2[,] gradients;

        public Generator(int seed)
        {
            Seed = seed;
        }

        public void GenerateChunk(Chunk chunk)
        {
            // Generate gradient grid

            gradients = new Vector2[GRADIENT_MAP_SIZE, GRADIENT_MAP_SIZE];
            for (int y = 0; y < GRADIENT_MAP_SIZE; y++)
            {
                for (int x = 0; x < GRADIENT_MAP_SIZE; x++)
                {
                    gradients[x, y] = new Vector2(rng.Next(-100, 100), rng.Next(-100, 100));
                    gradients[x, y].Normalize();
                }
            }
            // Generate tiles
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    chunk.Tiles[x, y] = new Tile(Perlin(x * MAP_SCALE, y * MAP_SCALE) >= LAND_THRESHOLD? true : false);
                }
            }

            //int DebugSquareSize = 1000;
            //UInt32[] pixels = new UInt32[DebugSquareSize * DebugSquareSize];
            //for (int i = 0; i<DebugSquareSize; i++)
            //{
            //    for (int j = 0; j<DebugSquareSize; j++)
            //    {
            //        pixels[i + j * DebugSquareSize] = GrayscaleToColor((byte)((Perlin(i* 0.0064f, j* 0.0064f) + 1) * 128));
            //    }
            //}

            //DebugRenderer.AddPixels(pixels, new Rectangle(50, 50, DebugSquareSize, DebugSquareSize));
        }

        //private uint GrayscaleToColor(byte shade)
        //{
        //    return (uint)((0xFF << 24) | (shade << 16) | (shade << 8) | (shade << 0));
        //}

        UInt32 Hash(UInt32 a)
        {
            a -= (a << 6);
            a ^= (a >> 17);
            a -= (a << 9);
            a ^= (a << 4);
            a -= (a << 3);
            a ^= (a << 10);
            a ^= (a >> 15);
            return a;
        }

        private float GradientDot(float x, float y, int ix, int iy)
        {
            return ((x - ix) * gradients[ix, iy].X + (y - iy) * gradients[ix, iy].Y);
        }

        private float Fade(float f)
        {
            return f * f * f * (f * (f * 6 - 15) + 10);
        }

        private float Perlin(float x, float y)
        {
            // Get grid coordinates
            int x0 = (int)x;
            int x1 = x0 + 1;
            int y0 = (int)y;
            int y1 = y0 + 1;
            
            float sx = Fade(x - x0);
            float sy = Fade(y - (float)y0);

            // Interpolate between grid gradients
            float ix0 = MathHelper.Lerp(GradientDot(x, y, x0, y0), GradientDot(x, y, x1, y0), sx);
            float ix1 = MathHelper.Lerp(GradientDot(x, y, x0, y1), GradientDot(x, y, x1, y1), sx);
            return MathHelper.Lerp(ix0, ix1, sy);
        }
    }
}
