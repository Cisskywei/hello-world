using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLerp : MonoBehaviour {

    public Transform[] path;

    private Queue<Vector3> poscache = new Queue<Vector3>();
    private Queue<Quaternion> rotcache = new Queue<Quaternion>();
    private Vector3 pos;
    private Quaternion rot;

    private bool _issyncpos = true;
    private bool _issyncrot = true;

    // Use this for initialization
    void Start () {
        pos = transform.position;
        rot = transform.rotation;

        Rig = gameObject.GetComponent<Rigidbody>();

        init();
    }
	
	// Update is called once per frame
	void Update () {
        //DoSync();

        DoSyncPhy();
    }

    public void init()
    {
        for (int i = 0; i < path.Length; i++)
        {
            poscache.Enqueue(path[i].position);
        }
    }

    public int syncRateFrames = 8;
    public int lerpTime = 8;
    public int LerpStep = 1;
    public float t = 1;

    float friction;
    float moveSpeed;
    float startTime;
    float distance;

    private Vector3 speed;
    public void calspeed(Vector3 from, Vector3 to, float t)
    {
        float dis = Vector3.Distance(from, to);
        speed = (dis / t) * (to-from);

        distance = dis;
        moveSpeed = dis / t;
        startTime = (Time.time);
    }

    public Rigidbody Rig;
    private void DoSyncPhy()
    {
        if (Vector3.Distance(pos, transform.position) < 0.5f)
        {
            if (poscache.Count > 0)
            {
                pos = poscache.Dequeue();
            }
        }

        if (poscache.Count > 0)
        {
            Rig.velocity = Vector3.Lerp(Rig.velocity, new Vector3(
            pos.x - transform.position.x,
            pos.y - transform.position.y,
            pos.z - transform.position.z
            ).normalized * (10), 0.1f);
        }
        else
        {
            Rig.velocity = Vector2.Lerp(Rig.velocity, Vector2.zero, 0.2f);
        }

    }

    private void DoSync()
    {
        if (_issyncpos)
        {
            if (Vector3.Distance(pos, transform.position) < 0.5f)
            {
                if (poscache.Count > 0)
                {
                    lerpTime = syncRateFrames;
                    pos = poscache.Dequeue();


                    calspeed(transform.position, pos, 4);

                    transform.position = Vector3.Lerp(transform.position, pos, friction);

                }
                else if (Vector3.Distance(pos, transform.position) > 0.005f)
                {
                    friction = (Time.time - startTime) * moveSpeed / distance;
                    transform.position = Vector3.Lerp(transform.position, pos, friction);
                }
                else
                {
                    transform.position = pos;

                    _issyncpos = false;
                }
            }
            else
            {
                friction = (Time.time - startTime) * moveSpeed / distance;
                transform.position = Vector3.Lerp(transform.position, pos, friction);
                //transform.position += Time.deltaTime * speed;
                //lerpTime -= LerpStep;
                //if(lerpTime<0)
                //{

                //}
                //else
                //{
                //    t = (float)(syncRateFrames - lerpTime) / (float)syncRateFrames;
                //    transform.position = Vector3.Lerp(transform.position, pos, t);
                //}
            }
        }

        if (_issyncrot)
        {
            if (Quaternion.Angle(rot, transform.rotation) < 0.1f)
            {
                transform.rotation = rot;

                if (rotcache.Count > 0)
                {
                    rot = rotcache.Dequeue();
                }
                else
                {
                    _issyncrot = false;
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 16 * Time.deltaTime);
            }
        }

    }
}
