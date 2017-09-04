using UnityEngine;
using System.Collections;


namespace PaintCraft.Canvas.Configs
{
    public abstract class AdvancedPageConfig : PageConfig {
        public abstract Texture2D OutlineTexture{
            get; 
        }

        public abstract Texture2D RegionTexture{
            get; 
        }
    }
}