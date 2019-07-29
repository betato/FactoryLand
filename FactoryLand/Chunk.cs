using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Chunk
    {
        public const int SIZE = 64;
        public const int NUM_TILES = SIZE * SIZE;

        public Point Location { get; set; }
        public Tile[,] Tiles { get; private set; } = new Tile[SIZE, SIZE];
        public int[,] GraphicsTiles { get; set; } = new int[SIZE, SIZE];

        public bool GraphicsLoaded { get; private set; }
        private VertexPositionColorTexture[] graphicsTiles;
        private short vertexIndex;
        private short[] graphicsTileIndices;
        private short indiciesIndex;

        public Chunk(Point location)
        {
            Location = location;
        }

        // To be run when chunk is loaded
        // TODO: selectively update graphics tiles when tile is updated
        public void CalculateAllGraphicsTiles(Chunk down, Chunk left, Chunk corner)
        {
            for (int tileY = 1; tileY < SIZE; tileY++)
            {
                for (int tileX = 1; tileX < SIZE; tileX++)
                {
                    GraphicsTiles[tileX, tileY] =
                        (Tiles[tileX - 1, tileY - 1].Land ? 1 : 0) + (Tiles[tileX, tileY - 1].Land ? 2 : 0) +
                        (Tiles[tileX - 1, tileY].Land ? 4 : 0) + (Tiles[tileX, tileY].Land ? 8 : 0);
                }
            }

            CalculateBottomGraphics(down);
            CalculateSideGraphics(left);
            CalculateCornerGraphics(down, left, corner);
        }

        public void CalculateBottomGraphics(Chunk down)
        {
            if (down != null)
            {
                for (int tileX = 1; tileX < SIZE; tileX++)
                {
                    GraphicsTiles[tileX, 0] =
                        (down.Tiles[tileX - 1, SIZE - 1].Land ? 1 : 0) + (down.Tiles[tileX, SIZE - 1].Land ? 2 : 0) +
                        (Tiles[tileX - 1, 0].Land ? 4 : 0) + (Tiles[tileX, 0].Land ? 8 : 0);
                }
            }
            else
            {
                for (int tileX = 1; tileX < SIZE; tileX++)
                {
                    GraphicsTiles[tileX, 0] = -1;
                }
            }
        }

        public void CalculateSideGraphics(Chunk left)
        {
            if (left != null)
            {
                for (int tileY = 1; tileY < SIZE; tileY++)
                {
                    GraphicsTiles[0, tileY] =
                        (left.Tiles[SIZE - 1, tileY - 1].Land ? 1 : 0) + (Tiles[0, tileY - 1].Land ? 2 : 0) +
                        (left.Tiles[SIZE - 1, tileY].Land ? 4 : 0) + (Tiles[0, tileY].Land ? 8 : 0);
                }
            }
            else
            {
                for (int tileY = 1; tileY < SIZE; tileY++)
                {
                    GraphicsTiles[0, tileY] = -1;
                }
            }
        }

        public void CalculateCornerGraphics(Chunk down, Chunk left, Chunk corner)
        {
            if (down != null && left != null && corner != null)
            {
                GraphicsTiles[0, 0] =
                    (corner.Tiles[SIZE - 1, SIZE - 1].Land ? 1 : 0) + (down.Tiles[0, SIZE - 1].Land ? 2 : 0) +
                    (left.Tiles[SIZE - 1, 0].Land ? 4 : 0) + (Tiles[0, 0].Land ? 8 : 0);
            }
            else
            {
                GraphicsTiles[0, 0] = -1;
            }
        }

        public void LoadGraphics()
        {
            graphicsTiles = new VertexPositionColorTexture[NUM_TILES * 4];
            graphicsTileIndices = new short[NUM_TILES * 6];

            vertexIndex = 0;
            indiciesIndex = 0;
            
            // Corner
            if (GraphicsTiles[0, 0] >= 0)
            {
                AddTextureSquare(0, 0);
            }

            // Bottom
            if (GraphicsTiles[1, 0] >= 0)
            {
                for (int tileX = 1; tileX < SIZE; tileX++)
                {
                    AddTextureSquare(tileX, 0);
                }
            }

            // Left
            if (GraphicsTiles[0, 1] >= 0)
            {
                for (int tileY = 1; tileY < SIZE; tileY++)
                {
                    AddTextureSquare(0, tileY);
                }
            }

            // Main chunk
            for (int tileY = 1; tileY < SIZE; tileY++)
            {
                for (int tileX = 1; tileX < SIZE; tileX++)
                {
                    AddTextureSquare(tileX, tileY);
                }
            }

            GraphicsLoaded = true;
        }

        public void UnloadGraphics()
        {
            GraphicsLoaded = false;
            graphicsTiles = null;
            graphicsTileIndices = null;
            vertexIndex = -1;
            indiciesIndex = -1;
        }

        // Re-loads graphics if graphics are currently loaded
        public void UpdateGraphics()
        {
            if (GraphicsLoaded)
            {
                LoadGraphics();
            }
        }

        private void AddTextureSquare(int x, int y)
        {
            int tileX = x + Location.X * SIZE;
            int tileY = y + Location.Y * SIZE;
            int tileType = GraphicsTiles[x, y];

            // x and y are shifted downwards by half a unit so that they are drawn between world tiles
            // Bottom Left
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX - 0.5f, tileY - 0.5f, 0), Color.White,
                GetTextureMapCoord(tileType, false, false));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            // Bottom Right
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX + 0.5f, tileY - 0.5f, 0), Color.White,
                GetTextureMapCoord(tileType, false, true));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            // Top Left
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX - 0.5f, tileY + 0.5f, 0), Color.White,
                GetTextureMapCoord(tileType, true, false));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            graphicsTileIndices[indiciesIndex++] = (short)(vertexIndex - 2);
            // Top Right
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX + 0.5f, tileY + 0.5f, 0), Color.White,
                GetTextureMapCoord(tileType, true, true));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            graphicsTileIndices[indiciesIndex++] = (short)(vertexIndex - 2);
        }

        private Vector2 GetTextureMapCoord(int tile, bool top, bool right)
        {
            return new Vector2(tile / 16f + (right ? 0.0615234375f : 0.0009765625f), (top ? 1f : 0f));
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), graphicsTileIndices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(graphicsTileIndices);
            graphicsDevice.Indices = indexBuffer;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), graphicsTiles.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorTexture>(graphicsTiles);
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, graphicsTiles.Length / 2);
            }

            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }
    }
}
