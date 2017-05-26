using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    public float MoveSpeed = 1;
    public SceneObject so;

    public bool iscube = false;

    // Use this for initialization
    void Start () {
        so = gameObject.GetComponent<SceneObject>();
	}
	
	// Update is called once per frame
	void Update () {

        if(so == null)
        {
            so = gameObject.GetComponent<SceneObject>();
        }

        if(!(so.sos.isCanOperate))
        {
            return;
        }

        if(iscube)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.up * Time.deltaTime * MoveSpeed);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(Vector3.left * Time.deltaTime * MoveSpeed);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.up * Time.deltaTime * MoveSpeed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.down * Time.deltaTime * MoveSpeed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * Time.deltaTime * MoveSpeed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * Time.deltaTime * MoveSpeed);
            }
        }
        
    }
}
