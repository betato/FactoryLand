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
        public int Seed { get; private set; }

        // Layer sizes should be even and greater than 0
        private const int FIRST_LAYER_SIZE = 2;
        private const int SECOND_LAYER_SIZE = 6;

        private const float LAND_THRESHOLD = -0.1f;
        private const float MOUNTAIN_THRESHOLD = 0.5f;

        private Vector2[,] gradients;
        private Random rng;
        
        private readonly byte[] p = new byte[byte.MaxValue * 2];
        private static readonly Vector2[] vectorMap = new Vector2[byte.MaxValue];

        static Generator()
        {
            // Create map from byte to vector
            double byteToRadians = 2 * Math.PI / byte.MaxValue;
            double angle;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                angle = i * byteToRadians;
                vectorMap[i] = new Vector2((float)Math.Cos(angle), (float)(Math.Sin(angle)));
            }
        }

        public Generator(int seed)
        {
            Seed = seed;
            rng = new Random(seed);

            // Shuffle permutation array based on seed
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                p[i] = i;
            }
            for (int i = byte.MaxValue - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                byte temp = p[i];
                p[i + byte.MaxValue] = p[i] = p[j];
                p[j + byte.MaxValue] = p[j] = temp;
            }
        }

        public void GenerateChunk(Chunk chunk)
        {
            float[,] elevations = new float[Chunk.SIZE, Chunk.SIZE];
            GenerateLayer(elevations, FIRST_LAYER_SIZE, 2, chunk.Location);
            GenerateLayer(elevations, SECOND_LAYER_SIZE, 1, chunk.Location);
            for (int y = 0; y < Chunk.SIZE; y++)
            {
                for (int x = 0; x < Chunk.SIZE; x++)
                {
                    chunk.Tiles[x, y] = new Tile();
                    chunk.Tiles[x, y].layerTypes[(int)LayerType.Land] = elevations[x, y] >= LAND_THRESHOLD ? true : false;
                    chunk.Tiles[x, y].layerTypes[(int)LayerType.Mountain] = elevations[x, y] >= MOUNTAIN_THRESHOLD ? true : false;
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
                    gradients[x, y] = GetVector(x + gridOffset * chunkLocation.X, y + gridOffset * chunkLocation.Y);
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

        public Vector2 GetVector(int x, int y)
        {
            return vectorMap[p[p[p[p[p[p[p[p[
                ((x >> 24) & 255)] + ((y >> 24) & 255)] +
                ((x >> 16) & 255)] + ((y >> 16) & 255)] +
                ((x >> 8) & 255)] + ((y >> 8) & 255)] +
                (x & 255)] + (y & 255)]];
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
            float sy = Fade(y - y0);

            // Interpolate between grid gradients
            float ix0 = MathHelper.Lerp(GradientDot(x, y, x0, y0), GradientDot(x, y, x1, y0), sx);
            float ix1 = MathHelper.Lerp(GradientDot(x, y, x0, y1), GradientDot(x, y, x1, y1), sx);
            return MathHelper.Lerp(ix0, ix1, sy);
        }
    }
}
