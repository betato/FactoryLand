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
        public int[][,] GraphicsTiles { get; set; } = new int[Tile.NUM_LAYER_TYPES][,]; // Change this to a byte array?

        public bool GraphicsLoaded { get; private set; }
        private VertexPositionColorTexture[][] graphicsVertexTiles = new VertexPositionColorTexture[Tile.NUM_LAYER_TYPES][];
        private short[] vertexIndex = new short[Tile.NUM_LAYER_TYPES];
        private short[][] graphicsTileIndices = new short[Tile.NUM_LAYER_TYPES][];
        private short[] indiciesIndex = new short[Tile.NUM_LAYER_TYPES];

        public Chunk(Point location)
        {
            Location = location;
            for (int layer = 0; layer < Tile.NUM_LAYER_TYPES; layer++)
            {
                GraphicsTiles[layer] = new int[SIZE, SIZE];
            }
        }

        // To be run when chunk is loaded
        // TODO: selectively update graphics tiles when tile is updated
        public void CalculateAllGraphicsTiles(Chunk down, Chunk left, Chunk corner)
        {
            for (int layer = 0; layer < Tile.NUM_LAYER_TYPES; layer++)
            {
                for (int tileY = 1; tileY < SIZE; tileY++)
                {
                    for (int tileX = 1; tileX < SIZE; tileX++)
                    {
                        GraphicsTiles[layer][tileX, tileY] =
                            (Tiles[tileX - 1, tileY - 1].layerTypes[layer] ? 1 : 0) + (Tiles[tileX, tileY - 1].layerTypes[layer] ? 2 : 0) +
                            (Tiles[tileX - 1, tileY].layerTypes[layer] ? 4 : 0) + (Tiles[tileX, tileY].layerTypes[layer] ? 8 : 0);
                    }
                }
            }

            CalculateBottomGraphics(down);
            CalculateSideGraphics(left);
            CalculateCornerGraphics(down, left, corner);
        }

        public void CalculateBottomGraphics(Chunk down)
        {
            for (int layer = 0; layer < Tile.NUM_LAYER_TYPES; layer++)
            {
                if (down != null)
                {
                    for (int tileX = 1; tileX < SIZE; tileX++)
                    {
                        GraphicsTiles[layer][tileX, 0] =
                            (down.Tiles[tileX - 1, SIZE - 1].layerTypes[layer] ? 1 : 0) + (down.Tiles[tileX, SIZE - 1].layerTypes[layer] ? 2 : 0) +
                            (Tiles[tileX - 1, 0].layerTypes[layer] ? 4 : 0) + (Tiles[tileX, 0].layerTypes[layer] ? 8 : 0);
                    }
                }
                else
                {
                    for (int tileX = 1; tileX < SIZE; tileX++)
                    {
                        GraphicsTiles[layer][tileX, 0] = -1;
                    }
                }
            }
        }

        public void CalculateSideGraphics(Chunk left)
        {
            for (int layer = 0; layer < Tile.NUM_LAYER_TYPES; layer++)
            {
                if (left != null)
                {
                    for (int tileY = 1; tileY < SIZE; tileY++)
                    {
                        GraphicsTiles[layer][0, tileY] =
                            (left.Tiles[SIZE - 1, tileY - 1].layerTypes[layer] ? 1 : 0) + (Tiles[0, tileY - 1].layerTypes[layer] ? 2 : 0) +
                            (left.Tiles[SIZE - 1, tileY].layerTypes[layer] ? 4 : 0) + (Tiles[0, tileY].layerTypes[layer] ? 8 : 0);
                    }
                }
                else
                {
                    for (int tileY = 1; tileY < SIZE; tileY++)
                    {
                        GraphicsTiles[layer][0, tileY] = -1;
                    }
                }
            }
        }

        public void CalculateCornerGraphics(Chunk down, Chunk left, Chunk corner)
        {
            for (int layer = 0; layer < Tile.NUM_LAYER_TYPES; layer++)
            {
                if (down != null && left != null && corner != null)
                {
                    GraphicsTiles[layer][0, 0] =
                        (corner.Tiles[SIZE - 1, SIZE - 1].layerTypes[layer] ? 1 : 0) + (down.Tiles[0, SIZE - 1].layerTypes[layer] ? 2 : 0) +
                        (left.Tiles[SIZE - 1, 0].layerTypes[layer] ? 4 : 0) + (Tiles[0, 0].layerTypes[layer] ? 8 : 0);
                }
                else
                {
                    GraphicsTiles[layer][0, 0] = -1;
                }
            }
        }

        public void LoadGraphics()
        {
            graphicsVertexTiles = new VertexPositionColorTexture[Tile.NUM_LAYER_TYPES][];
            vertexIndex = new short[Tile.NUM_LAYER_TYPES];
            graphicsTileIndices = new short[Tile.NUM_LAYER_TYPES][];
            indiciesIndex = new short[Tile.NUM_LAYER_TYPES];

            for (int layer = 0; layer < Tile.NUM_LAYER_TYPES; layer++)
            {
                graphicsVertexTiles[layer] = new VertexPositionColorTexture[NUM_TILES * 4];
                graphicsTileIndices[layer] = new short[NUM_TILES * 6];

                vertexIndex[layer] = 0;
                indiciesIndex[layer] = 0;

                // Now ignore all 0 index tiles

                // Corner
                if (GraphicsTiles[layer][0, 0] > 0)
                {
                    AddTextureSquare(layer, 0, 0);
                }

                // Bottom
                if (GraphicsTiles[layer][1, 0] >= 0)
                {
                    for (int tileX = 1; tileX < SIZE; tileX++)
                    {
                        if (GraphicsTiles[layer][tileX, 0] > 0)
                        {
                            AddTextureSquare(layer, tileX, 0);
                        }
                    }
                }

                // Left
                if (GraphicsTiles[layer][0, 1] >= 0)
                {
                    for (int tileY = 1; tileY < SIZE; tileY++)
                    {
                        if (GraphicsTiles[layer][0, tileY] > 0)
                        {
                            AddTextureSquare(layer, 0, tileY);
                        }
                    }
                }

                // Main chunk
                for (int tileY = 1; tileY < SIZE; tileY++)
                {
                    for (int tileX = 1; tileX < SIZE; tileX++)
                    {
                        if (GraphicsTiles[layer][tileX, tileY] > 0)
                        {
                            AddTextureSquare(layer, tileX, tileY);
                        }
                    }
                }

                GraphicsLoaded = true;
            }
        }

        public void UnloadGraphics()
        {
            GraphicsLoaded = false;
            graphicsVertexTiles = null;
            graphicsTileIndices = null;
            vertexIndex = null;
            indiciesIndex = null;
        }

        // Re-loads graphics if graphics are currently loaded
        public void UpdateGraphics()
        {
            if (GraphicsLoaded)
            {
                LoadGraphics();
            }
        }

        private void AddTextureSquare(int layer, int x, int y)
        {
            int tileX = x + Location.X * SIZE;
            int tileY = y + Location.Y * SIZE;
            int tileIndex = GraphicsTiles[layer][x, y];
            int visualType = Tiles[x, y].visualType;

            short layerVertexIndex = vertexIndex[layer];
            short layerIndiciesIndex = indiciesIndex[layer];

            // x and y are shifted downwards by half a unit so that they are drawn between world tiles
            // Bottom Left
            graphicsVertexTiles[layer][layerVertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX - 0.5f, tileY - 0.5f, 0), Color.White,
                GetTextureMapCoord(tileIndex, visualType, false, false));
            graphicsTileIndices[layer][layerIndiciesIndex++] = layerVertexIndex++;
            // Bottom Right
            graphicsVertexTiles[layer][layerVertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX + 0.5f, tileY - 0.5f, 0), Color.White,
                GetTextureMapCoord(tileIndex, visualType, false, true));
            graphicsTileIndices[layer][layerIndiciesIndex++] = layerVertexIndex++;
            // Top Left
            graphicsVertexTiles[layer][layerVertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX - 0.5f, tileY + 0.5f, 0), Color.White,
                GetTextureMapCoord(tileIndex, visualType, true, false));
            graphicsTileIndices[layer][layerIndiciesIndex++] = layerVertexIndex++;
            graphicsTileIndices[layer][layerIndiciesIndex++] = (short)(layerVertexIndex - 2);
            // Top Right
            graphicsVertexTiles[layer][layerVertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX + 0.5f, tileY + 0.5f, 0), Color.White,
                GetTextureMapCoord(tileIndex, visualType, true, true));
            graphicsTileIndices[layer][layerIndiciesIndex++] = layerVertexIndex++;
            graphicsTileIndices[layer][layerIndiciesIndex++] = (short)(layerVertexIndex - 2);

            vertexIndex[layer] = layerVertexIndex;
            indiciesIndex[layer] = layerIndiciesIndex;
        }

        private Vector2 GetTextureMapCoord(int tile, int visualType, bool top, bool right)
        {
            return new Vector2(tile / 16f + (right ? 0.0625f : 0f), visualType / 4f + (top ? 0.25f : 0f));
        }

        public void Draw(int layer, GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), graphicsTileIndices[layer].Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(graphicsTileIndices[layer]);
            graphicsDevice.Indices = indexBuffer;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), graphicsVertexTiles[layer].Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorTexture>(graphicsVertexTiles[layer]);
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, graphicsVertexTiles[layer].Length / 2);
            }

            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }
    }
}
