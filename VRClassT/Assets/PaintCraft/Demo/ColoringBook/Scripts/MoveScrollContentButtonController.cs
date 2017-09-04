using UnityEngine;
using UnityEngine.UI;


namespace PaintCraft.Demo.ColoringBook{ 
    public class MoveScrollContentButtonController : MonoBehaviour
    {
        public ScrollRect ScrollRect;
        public Vector2 VelocityPerClick;
        public Vector2 LockOnPosition;
        Button button;
    	
    	void Start ()
    	{
    	    button = GetComponent<Button>();
    	    button.onClick.AddListener(OnButtonClicked);
    	}

        void OnButtonClicked()
        {
           
            ScrollRect.normalizedPosition = new Vector2(VelocityPerClick.x != 0.0f ?  Mathf.Clamp(ScrollRect.normalizedPosition.x, 0.001f, 0.999f) : 0.0f,
               VelocityPerClick.y != 0.0f ? Mathf.Clamp(ScrollRect.normalizedPosition.y, 0.001f, 0.999f) : 0.0f);   
             ScrollRect.velocity+= VelocityPerClick;
        }

    	void Update ()
    	{
    	    button.interactable = ScrollRect.normalizedPosition != LockOnPosition;
    	}
    }
}