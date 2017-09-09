using UnityEngine;


namespace PaintCraft.Utils{
	public class ColorUtil {		
		private static float lastH=0f;
		
		// also read here http://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
		public static Color32 GetRandomColor () {	
			float r = Random.Range(0.2f, 0.8f);
			lastH += r;
			lastH %= 1.0f;		
			return HsvToRgb(lastH , Random.Range(0.8f,1.0f) ,1.0f );	
		}
		
		// h - 0->1
		// s  0->1
		// v  0-> 1

		public static Color HsvToRgb (float h, float s, float v) {
			float r,g,b;
			HsvToRgb(h,s,v, out r, out g, out b);
			return new Color(r,g,b,1.0f);
		}

		public static void HsvToRgb (float h, float s, float v, out float r, out float g, out float b) {
			
			int H = (int)(h * 6.0f);
			
			float f = h * 6.0f - H;
			float p = v * (1.0f - s);
			float q = v * (1.0f - f * s);
			float t = v * (1.0f - (1.0f - f) * s);
			
			r = g = b = 0.0f;

			switch (H) {
			case 0:
				r = v; g = t; b = p;
				break;	
			case 1:
				r = q; g = v; b = p;
				break;
			case 2:
				r = p; g = v; b = t;
				break;
			case 3:
				r = p; g = q; b = v;
				break;
			case 4:
				r = t; g = p; b = v;
				break;
			case 5:
				r = v; g = p; b = q;
				break;
			}
		}



		public static void RgbToHsv(float r, float g, float b, out float h, out float s, out float v){
			float min,max,delta;

			min = Mathf.Min(r,g,b);
			max = Mathf.Max(r,g,b);
			v = max;
			delta = max - min;
			h = s= v = 0.0f;


			if( max != 0.0f ){
				s = delta / max;		// s
			} else {
				// r = g = b = 0		// s = 0, v is undefined
				s = 0.0f;
				h = -1.0f;
				return;
			}
			if( r == max )
				h = ( g - b ) / delta;		// between yellow & magenta
			else if( g == max )
				h = 2.0f + ( b - r ) / delta;	// between cyan & yellow
			else
				h = 4.0f + ( r - g ) / delta;	// between magenta & cyan
			h *= 60.0f;				// degrees
			if( h < 0.0f )
				h += 360.0f;
			h /=360.0f;

		}		
	}
}