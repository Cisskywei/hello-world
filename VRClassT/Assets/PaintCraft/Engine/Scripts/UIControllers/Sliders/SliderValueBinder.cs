using UnityEngine;
using UnityEngine.UI;


namespace PaintCraft.UI.Sliders{
	[RequireComponent(typeof(Slider))]
	public abstract class SliderValueBinder : MonoBehaviour {
		public abstract float Value{ get; set;}

		Slider slider;
		void Start(){
			slider = GetComponent<Slider>();
			slider.onValueChanged.AddListener((value)=>{
				OnValueChanged(value);
			});
		}
		
		
		void OnValueChanged(float newValue){
			Value = newValue;
		}
		
		
		void LateUpdate(){
			if (Value != slider.value){
				slider.value = Value;
			}
		}
	}
}