using UnityEngine;

public class DisableSimulateMouseAsTouch : MonoBehaviour {
	void Start () {
		Input.simulateMouseWithTouches = false;
	}
}
