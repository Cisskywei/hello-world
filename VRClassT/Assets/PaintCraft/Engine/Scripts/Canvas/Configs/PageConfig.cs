using System.IO;
using UnityEngine;
using PaintCraft.Utils;
using System;

namespace PaintCraft.Canvas.Configs{	
	public abstract class PageConfig : ScriptableObject
	{
	    public string UniqueId;

	    public string IconSavePath
	    {
	        get
	        {
	            string dir = Path.Combine( Application.persistentDataPath , "icons");
	            if (!Directory.Exists(dir))
	            {
	                Directory.CreateDirectory(dir);
	            }
	            return Path.Combine(dir, UniqueId + ".jpg");
	        }
	    }
	    abstract public Vector2 GetSize();


        [TexturePath] public string startImagePath;
        [NonSerialized]
        Texture2D startImageTexture;

        public Texture2D StartImageTexture
        {
            get
            {
                if (startImageTexture == null)
                {                    
                    startImageTexture = Resources.Load<Texture2D>(startImagePath);
                }
                return startImageTexture;
            }
        }
	}
}
