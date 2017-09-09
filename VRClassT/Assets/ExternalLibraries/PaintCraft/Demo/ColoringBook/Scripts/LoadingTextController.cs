using UnityEngine;
using UnityEngine.UI;


namespace PaintCraft.Demo.ColoringBook{ 
    public class LoadingTextController : MonoBehaviour
    {
        Text text;
        float startTime;
    	void Start ()
    	{
    	    text = GetComponent<Text>();
    	    startTime = Time.realtimeSinceStartup;
    	}
    		
    	void Update ()
    	{
    	    string dots = new string('.', (int) ((Time.realtimeSinceStartup - startTime)/ 0.5) % 4);
    	    text.text = "Loading" + dots;
    	}
    }
}