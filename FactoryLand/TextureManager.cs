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
