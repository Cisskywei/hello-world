using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePlayer : MonoBehaviour {

    public Transform head;
    public Transform lefthand;
    public Transform righthand;

    public Vector3 headpos;
    public Quaternion headrot;
    public Vector3 headscal;

    public Vector3 lefthandpos;
    public Quaternion lefthandrot;
    public Vector3 lefthandscal;

    public Vector3 righthandpos;
    public Quaternion righthandrot;
    public Vector3 righthandscal;

    public float freq = 0.02f;
    public float diff = 0.04f;
    private float _timer = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(_timer > freq)
        {
            _timer = 0;
            // 更新坐标
            RealHeadTransform();
            RealLeftHandTransform();
            RealRightHandTransform();
        }

        _timer += Time.deltaTime;
	}

    private void RealHeadTransform()
    {
        if(head == null)
        {
            return;
        }

        if (Vector3.Distance(head.position, headpos) < diff)
        {
            head.position = headpos;
        }
        else
        {
            head.position = Vector3.Lerp(head.position, headpos, Time.deltaTime * 16); //Time.deltaTime * 
        }

        if (Quaternion.Angle(head.rotation, headrot) < diff)
        {
            head.rotation = headrot;
        }
        else
        {
            head.rotation = Quaternion.Slerp(head.rotation, headrot, Time.deltaTime * 8);
        }

 //       head.localScale = headscal;
    }

    private void RealLeftHandTransform()
    {
        if (lefthand == null)
        {
            return;
        }

        if (Vector3.Distance(lefthand.position, lefthandpos) < diff)
        {
            lefthand.position = lefthandpos;
        }
        else
        {
            lefthand.position = Vector3.Lerp(lefthand.position, lefthandpos, Time.deltaTime * 16); //Time.deltaTime * 
        }

        if (Quaternion.Angle(lefthand.rotation, lefthandrot) < diff)
        {
            lefthand.rotation = lefthandrot;
        }
        else
        {
            lefthand.rotation = Quaternion.Slerp(lefthand.rotation, lefthandrot, Time.deltaTime * 8);
        }

   //     lefthand.localScale = lefthandscal;
    }

    private void RealRightHandTransform()
    {
        if (righthand == null)
        {
            return;
        }

        if (Vector3.Distance(righthand.position, righthandpos) < diff)
        {
            righthand.position = righthandpos;
        }
        else
        {
            righthand.position = Vector3.Lerp(righthand.position, righthandpos, Time.deltaTime * 16); //Time.deltaTime * 
        }

        if (Quaternion.Angle(righthand.rotation, righthandrot) < diff)
        {
            righthand.rotation = righthandrot;
        }
        else
        {
            righthand.rotation = Quaternion.Slerp(righthand.rotation, righthandrot, Time.deltaTime * 8);
        }

   //     righthand.localScale = righthandscal;
    }

    public void HeadTransform(Hashtable t)
    {
        if (t == null || t.Count <= 0)
        {
            return;
        }

        if (t["headposx"]!=null)
            {
                this.headpos.x = (float)Convert.ToDouble(t["headposx"]);
            }
            if(t["headposy"] !=null)
            {
                this.headpos.y = (float)Convert.ToDouble(t["headposy"]);
            }
            if (t["headposz"] != null)
            {
                this.headpos.z = (float)Convert.ToDouble(t["headposz"]);
            }
            if (t["headrotx"] != null)
            {
                this.lefthandrot.x = (float)Convert.ToDouble(t["headrotx"]);
            }
            if (t["headroty"] != null)
            {
                this.lefthandrot.y = (float)Convert.ToDouble(t["headroty"]);
            }
            if (t["headrotz"] != null)
            {
                this.lefthandrot.z = (float)Convert.ToDouble(t["headrotz"]);
            }
            if (t["headrotw"] != null)
            {
                this.lefthandrot.w = (float)Convert.ToDouble(t["headrotw"]);
            }
            //if (t["headscalx"] != null)
            //{
            //    this.lefthandscal.x = (float)Convert.ToDouble(t["headscalx"]);
            //}
            //if (t["headscaly"] != null)
            //{
            //    this.lefthandscal.y = (float)Convert.ToDouble(t["headscaly"]);
            //}
            //if (t["headscalz"] != null)
            //{
            //    this.lefthandscal.z = (float)Convert.ToDouble(t["headscalz"]);
            //}

    }

    public void LeftHandTransform(Hashtable t)
    {
        if (t == null || t.Count <= 0)
        {
            return;
        }

        if (t["lhandposx"] != null)
        {
            this.lefthandpos.x = (float)Convert.ToDouble(t["lhandposx"]);
        }
        if (t["lhandposy"] != null)
        {
            this.lefthandpos.y = (float)Convert.ToDouble(t["lhandposy"]);
        }
        if (t["lhandposz"] != null)
        {
            this.lefthandpos.z = (float)Convert.ToDouble(t["lhandposz"]);
        }
        if (t["lhandrotx"] != null)
        {
            this.lefthandrot.x = (float)Convert.ToDouble(t["lhandrotx"]);
        }
        if (t["lhandroty"] != null)
        {
            this.lefthandrot.y = (float)Convert.ToDouble(t["lhandroty"]);
        }
        if (t["lhandrotz"] != null)
        {
            this.lefthandrot.z = (float)Convert.ToDouble(t["lhandrotz"]);
        }
        if (t["lhandrotw"] != null)
        {
            this.lefthandrot.w = (float)Convert.ToDouble(t["lhandrotw"]);
        }
        //if (t["lhandscalx"] != null)
        //{
        //    this.lefthandscal.x = (float)Convert.ToDouble(t["lhandscalx"]);
        //}
        //if (t["lhandscaly"] != null)
        //{
        //    this.lefthandscal.y = (float)Convert.ToDouble(t["lhandscaly"]);
        //}
        //if (t["lhandscalz"] != null)
        //{
        //    this.lefthandscal.z = (float)Convert.ToDouble(t["lhandscalz"]);
        //}
    }

    public void RightHandTransform(Hashtable t)
    {
        if(t == null || t.Count <= 0)
        {
            return;
        }

        if (t["rhandposx"] != null)
        {
            this.righthandpos.x = (float)Convert.ToDouble(t["rhandposx"]);
        }
        if (t["rhandposy"] != null)
        {
            this.righthandpos.y = (float)Convert.ToDouble(t["rhandposy"]);
        }
        if (t["rhandposz"] != null)
        {
            this.righthandpos.z = (float)Convert.ToDouble(t["rhandposz"]);
        }
        if (t["rhandrotx"] != null)
        {
            this.righthandrot.x = (float)Convert.ToDouble(t["rhandrotx"]);
        }
        if (t["rhandroty"] != null)
        {
            this.righthandrot.y = (float)Convert.ToDouble(t["rhandroty"]);
        }
        if (t["rhandrotz"] != null)
        {
            this.righthandrot.z = (float)Convert.ToDouble(t["rhandrotz"]);
        }
        if (t["rhandrotw"] != null)
        {
            this.righthandrot.w = (float)Convert.ToDouble(t["rhandrotw"]);
        }
        //if (t["rhandscalx"] != null)
        //{
        //    this.righthandscal.x = (float)Convert.ToDouble(t["rhandscalx"]);
        //}
        //if (t["rhandscaly"] != null)
        //{
        //    this.righthandscal.y = (float)Convert.ToDouble(t["rhandscaly"]);
        //}
        //if (t["rhandscalz"] != null)
        //{
        //    this.righthandscal.z = (float)Convert.ToDouble(t["rhandscalz"]);
        //}
    }

    public void HeadHandTransform(Hashtable t)
    {
        if(t["head"] != null)
        {
            Hashtable thead = (Hashtable)t["head"];
            HeadTransform(thead);
        }

        if (t["lefthand"] != null)
        {
            Hashtable tleft = (Hashtable)t["lefthand"];
            LeftHandTransform(tleft);
        }

        if (t["righthand"] != null)
        {
            Hashtable tright = (Hashtable)t["righthand"];
            RightHandTransform(tright);
        }
    }
}
