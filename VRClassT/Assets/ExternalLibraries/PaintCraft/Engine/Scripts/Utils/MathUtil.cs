using UnityEngine;


namespace PaintCraft.Utils {

	public class MathUtil {

		public static int IncrementIntLoop(int valueToIncrement, int minValueInclusive, int maxValueExclusive){
			int result = valueToIncrement+ 1;
			if (result >= maxValueExclusive){
				result = minValueInclusive;
			}
			return result;
		}

		public static int DecrementIntLoop(int valueToDecrement, int minValueInclusive, int maxValueExclusive){
			int result = valueToDecrement- 1;
			if (result < minValueInclusive){
				result = maxValueExclusive - 1;
			}
			return result;
		}


		/// <summary>
		/// Loops the value.
		/// if value > max reduce it until it less or equal max
		/// the same for min
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="value">Value.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static float LoopValue(float value, float min, float max){
			float diff = max - min;
			if (diff < 0){
				Debug.LogError("max must be greater than min");
			}

			if (value > max){
				int d = (int)( (value - max) / diff);
				value -= (d+ 1) * diff;
			} else if ( value < min){
				int d = (int)((value - max) / diff);
				value -= (d  - 1) * diff;
			}
			return value;
		}

	}


}