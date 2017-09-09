using System;
using UnityEngine;

namespace PaintCraft.Tools{
	[Serializable]
	public class LineConfig : MonoBehaviour {
        public Brush      Brush;
        public PointColor Color = PointColor.White;	
        public float      Spacing = 1.0f;
        [Range(0.0f,1.0f)]
		public float	  Scale  = 1.0f;
        public Texture    Texture;

        void Start(){
            if (Brush == null){
                Debug.LogError("you must provide default Brush tool here ", gameObject);
            }
        }
	}
}
