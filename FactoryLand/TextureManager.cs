using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    enum TextureId
    {
        Terrain_00 = 0,
        Terrain_01 = 1,
        Terrain_02 = 2,
        Terrain_03 = 3,
        Terrain_04 = 4,
        Terrain_05 = 5,
        Terrain_06 = 6,
        Terrain_07 = 7,
        Terrain_08 = 8,
        Terrain_09 = 9,
        Terrain_10 = 10,
        Terrain_11 = 11,
        Terrain_12 = 12,
        Terrain_13 = 13,
        Terrain_14 = 14,
        Terrain_15 = 15,
    }

    static class TextureManager
    {
        public static ContentManager ContentManager { get; set; }
        private static Dictionary<TextureId, Texture2D> textures = new Dictionary<TextureId, Texture2D>();
        private static Dictionary<int, Texture2D> terrains = new Dictionary<int, Texture2D>();

        public static void Initialize(ContentManager contentManager)
        {
            ContentManager = contentManager;
            LoadTextures();
        }

        private static void LoadTextures()
        {
            for (int i = 0; i <= 15; i++)
            {
                terrains.Add(i, ContentManager.Load<Texture2D>("Terrain//" + i.ToString()));
            }
        }

        public static Texture2D GetTexture(TextureId id)
        {
            return textures[id];
        }

        public static Texture2D GetTerrain(int id)
        {
            return terrains[id];
        }
    }
}
