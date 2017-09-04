using UnityEngine;
using System.Collections;
using PaintCraft.Canvas.Configs;
using System.IO;

namespace PaintCraft.Canvas.Configs{

    [CreateAssetMenu(menuName = "PaintCraft/StreamingColoringPageConfig")]
    public class StreamingColoringPageConfig : AdvancedPageConfig {
        public string OutlinePngPath;
        public string RegionPngPath;

        Texture2D outlineTexture;
        Texture2D regionTexture;

        #region implemented abstract members of PageConfig
        public override Vector2 GetSize()
        {
            return new Vector2(OutlineTexture.width, OutlineTexture.height);
        }
        #endregion



        #region implemented abstract members of AdvancedPageConfig
        public override Texture2D OutlineTexture
        {
            get
            {
                if (outlineTexture == null){
                    outlineTexture = GetStreamingTexture(OutlinePngPath);
                }
                return outlineTexture;
            }
        }

        public override Texture2D RegionTexture
        {
            get
            {
                if (regionTexture == null){
                    regionTexture = GetStreamingTexture(RegionPngPath);
                }
                return regionTexture;
            }
        }
        #endregion

        Texture2D GetStreamingTexture(string texturePath){
            string path = Path.Combine(Application.streamingAssetsPath, texturePath);
            //path = "file://"+path;
            Texture2D result = new Texture2D(1,1, TextureFormat.Alpha8, false);
            result.LoadImage(File.ReadAllBytes(path), true);
            return result;
        }
    }
}