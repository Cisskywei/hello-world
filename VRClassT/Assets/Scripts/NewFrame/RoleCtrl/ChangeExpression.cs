using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeExpression : MonoBehaviour {

    public Material[] expression;
    public Renderer face;

    public float interval = 0.2f;
    private float _timer = 0;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(_timer > interval)
        {
            int x = Random.Range(0, expression.Length);
            changeFace(x);
            interval = Random.Range(0.1f, 0.2f);
        }
	}

    private int _index = 0;
    private void changeFace(int index)
    {
        if(index == _index || index >= expression.Length)
        {
            return;
        }

        face.material = expression[index];
        _index = index;
    }
}
