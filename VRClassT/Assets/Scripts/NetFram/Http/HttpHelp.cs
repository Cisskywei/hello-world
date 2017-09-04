using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class HttpHelp
    {
        public delegate void LoadSpriteCallBack(Sprite s);

        public class LoadImageSprite: MonoBehaviour
        {
            public LoadSpriteCallBack spriteCallBack;

            public void StartLoadImage(string path)
            {
                StartCoroutine(LoadIamge(path));
            }

            public void StartLoadImage(string path, LoadSpriteCallBack callbackfunc)
            {
                this.spriteCallBack = callbackfunc;

                StartCoroutine(LoadIamge(path));
            }

            IEnumerator LoadIamge(string path)
            {
                WWW www = new WWW(path);
                yield return www;
                Texture2D texture = www.texture;
                Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                if (this.spriteCallBack != null)
                {
                    this.spriteCallBack.Invoke(sp);
                }
            }
        }
    }
}

