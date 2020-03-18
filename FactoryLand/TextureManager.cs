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
        Test = 0,
        Terrain = 1,
        Selection = 2,
        Water = 3,
        Land = 4,
        Mountain = 5
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
            // Single Textures
            textures.Add(TextureId.Test, ContentManager.Load<Texture2D>("Test"));
            textures.Add(TextureId.Terrain, ContentManager.Load<Texture2D>("Terrain//TerrainAtlas"));
            textures.Add(TextureId.Water, ContentManager.Load<Texture2D>("Terrain//WaterAtlas"));
            textures.Add(TextureId.Land, ContentManager.Load<Texture2D>("Terrain//LandAtlas"));
            textures.Add(TextureId.Mountain, ContentManager.Load<Texture2D>("Terrain//MountainAtlas"));
            textures.Add(TextureId.Selection, ContentManager.Load<Texture2D>("Selection"));
            // Terrains
            for (int i = 0; i <= 15; i++)
            {
                //terrains.Add(i, ContentManager.Load<Texture2D>("Terrain//" + i.ToString()));
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
