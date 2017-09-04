using UnityEngine;
using PaintCraft.Utils;
using System;


namespace PaintCraft.Tools{
	[Serializable]
    public class PointColor {

		private PointColor(){
			//close public constructor;
		}

		public static PointColor White{
			get{
				PointColor result = new PointColor();
                result.Color = Color.white;
//				result._alpha = 1.0f;
//				result._h = 0.5f;
//				result._s = 0.97f;
//				result._v = 0.97f;
//				result.UpdateRGBBaseOnHSV();
				return result;
			}
		}


		public Color Color{
			get{
				return new Color(_r, _g, _b,Alpha);
			}
			set {                
				_r = value.r;
				_g = value.g;
				_b = value.b;
				_alpha = value.a;
				UpdateHSVBaseOnRGB();
			}
		}

        [SerializeField]
		float _alpha = 1.0f;
		public float Alpha {
			get {
				return _alpha;
			}
			set {
				_alpha = value;
			}
		}

        [SerializeField]
		float _r = 1.0f;
		public float R {
			get {
				return _r;
			}
			set {
				if (_r != value){
					_r = value;
					UpdateHSVBaseOnRGB();
				}
			}
		}

        [SerializeField]
		float _g = 1.0f;
		public float G {
			get {
				return _g;
			}
			set {
				if (_g != value){
					_g = value;
					UpdateHSVBaseOnRGB();
				}
			}
		}

        [SerializeField]
		float _b = 1.0f;
		public float B {
			get {
				return _b;
			}
			set {
				if (_b != value){
					_b = value;
					UpdateHSVBaseOnRGB();
				}
			}
		}

		float _h = 0.5f;
		public float H {
			get {
				return _h;
			}
			set {
				if (_h != value){
					_h = value;
					UpdateRGBBaseOnHSV();
				}
			}
		}

		float _s = 0.97f;
		public float S {
			get {
				return _s;
			}
			set {
				if (_s != value){
					_s = value;
					UpdateRGBBaseOnHSV();
				}
			}
		}

		float _v = 0.97f;
		public float V {
			get {
				return _v;
			}
			set {
				if (_v != value){
					_v = value;
					UpdateRGBBaseOnHSV();
				}
			}
		}


		void UpdateRGBBaseOnHSV(){
			ColorUtil.HsvToRgb(_h, _s, _v, out _r, out _g, out _b);
		}

		void UpdateHSVBaseOnRGB(){		
			ColorUtil.RgbToHsv(_r,_g,_b, out _h, out _s, out _v);
		}

		public void CopyFrom(PointColor anotherColor){
			_alpha = anotherColor._alpha;
			_r = anotherColor._r;
			_g = anotherColor._g;
			_b = anotherColor._b;

			_h = anotherColor._h;
			_s = anotherColor._s;
			_v = anotherColor._v;
		}


	}
}