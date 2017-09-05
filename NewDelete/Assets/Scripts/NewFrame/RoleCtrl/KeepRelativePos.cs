using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRelativePos : MonoBehaviour {

    public Transform target;

    private Vector3 _relativedis;

	// Use this for initialization
	void Start () {
        _relativedis = target.position - transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        transform.position = target.position - _relativedis;
    }
}
