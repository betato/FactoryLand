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
        // Layer sizes should be even and greater than 0
        private const int FIRST_LAYER_SIZE = 2;
        private const int SECOND_LAYER_SIZE = 6;

        public int Seed { get; private set; }
        private Random rng = new Random();
        private Vector2[,] gradients;

        public Generator(int seed)
        {
            Seed = seed;
        }

        public int Random(int x, int y)
        {
            //return ((x * 735632791) % 180 + (y * 694847539) % 180);
            //return x * 4 + 4 * y;
            return rng.Next(0, 360);
        }

        public void GenerateChunk(Chunk chunk)
        {
            float[,] elevations = new float[Chunk.SIZE, Chunk.SIZE];
            GenerateLayer(elevations, FIRST_LAYER_SIZE, 1, chunk.Location);
            GenerateLayer(elevations, SECOND_LAYER_SIZE, 1, chunk.Location);
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    chunk.Tiles[x, y] = new Tile(elevations[x, y] >= LAND_THRESHOLD ? true : false);
                }
            }
        }

        private void GenerateLayer(float[,] elevations, int layerSize, float weight, Point chunkLocation)
        {
            // Generate gradient grid
            int gridSize = layerSize + 1;
            int gridOffset = layerSize - 1;
            gradients = new Vector2[gridSize, gridSize];
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    int random = Random(x + gridOffset * chunkLocation.X, y + gridOffset * chunkLocation.Y);
                    gradients[x, y] = new Vector2((float)Math.Cos(MathHelper.ToRadians(random)), (float)Math.Sin(MathHelper.ToRadians(random)));
                }
            }
            // Generate tiles
            float scale = gridOffset / (Chunk.SIZE - 1f);
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    elevations[x, y] += Perlin(0.5f + x * scale, 0.5f + y * scale) * weight;
                }
            }
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
