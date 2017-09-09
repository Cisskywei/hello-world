using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PaintCraft.Tools;

namespace PaintCraft.UI{
	[RequireComponent(typeof(Dropdown))]
	public class BrushDropDownController : MonoBehaviour, ISelectHandler {
        public List<Brush> Brushes;
        public LineConfig LineConfig;
		Dropdown dd;

		// Use this for initialization
		void Start () {
			dd = GetComponent<Dropdown>();
			dd.options.Clear();
            int selectedId=  0;
            int i=0;
            foreach(var brush in Brushes){
				dd.options.Add(new Dropdown.OptionData(){
                    text = brush.name
                });
                if (brush == LineConfig.Brush){
                    selectedId = i;
                }
                i++;
			}
            dd.value = selectedId;
			OnSelect(null);
		}
		
		public void OnSelect(BaseEventData eventData)
		{
            LineConfig.Brush = Brushes[dd.value]; 
		}
	}
}