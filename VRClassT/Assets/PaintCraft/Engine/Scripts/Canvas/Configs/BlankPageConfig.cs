using UnityEngine;

namespace PaintCraft.Canvas.Configs
{
    [CreateAssetMenu(menuName = "PaintCraft/BlankPageConfig")]
    public class BlankPageConfig : PageConfig
    {
        public Vector2 Size;
        public override Vector2 GetSize()
        {
            return Size;
        }
    }
}
