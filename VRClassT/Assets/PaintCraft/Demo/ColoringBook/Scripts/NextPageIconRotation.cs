using UnityEngine;

namespace PaintCraft.Demo.ColoringBook{ 
    public class NextPageIconRotation : MonoBehaviour {
    	public RectTransform ContentRect;

    	public float MiddleYPosition = 1030.0f;

    	RectTransform selfRectTransform;
    	void Start(){
    		selfRectTransform = GetComponent<RectTransform>();
    	}


    	// Update is called once per frame
    	void Update () {
    		float rotation = (ContentRect.anchoredPosition.y / MiddleYPosition) * 180.0f;
    		selfRectTransform.localRotation = Quaternion.Euler(0,0,rotation);
    	}
    }
}