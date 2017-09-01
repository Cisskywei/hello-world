using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public Dictionary<int, string> kk = new Dictionary<int, string>();

    public int speed = 6;

    public ArrayList a = new ArrayList();

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");    //水平           
        float y = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(x, 0, y) * speed * Time.deltaTime);
    }

    public void InitModel(params object[] args)
    {
        string n = (string)args[0];
        Dictionary<int, string> k = (Dictionary<int, string>)args[1];

        Debug.Log(n);
        Debug.Log(k[1]);
        k[1] = "pp";
        k.Add(2, "opiiiii");
    }

    private Queue<Vector3> poshcache = new Queue<Vector3>();
    private Queue<float> poshtimecache = new Queue<float>();
    private Queue<Quaternion> rothcache = new Queue<Quaternion>();
    public void ReceiveSync()
    {
        Hashtable h = new Hashtable();

        h.Add("px", 189);
        h.Add("py", 190);
        h.Add("pz", 191);
        h.Add("tp", 192);
        h.Add("rx", 193);
        h.Add("ry", 194);
        h.Add("rz", 195);
        h.Add("rw", 196);

        receive(h, poshcache, poshtimecache, rothcache);
        receive(h, poshcache, poshtimecache, rothcache);

        Debug.Log(poshcache.Count + " -- " + poshcache.Dequeue());
        Debug.Log(poshtimecache.Count + " -- " + poshtimecache.Dequeue());
        Debug.Log(rothcache.Count + " -- " + rothcache.Dequeue());
    }

    private void receive(Hashtable data, Queue<Vector3> pos, Queue<float> t, Queue<Quaternion> rot)
    {
        float px = (float)Convert.ToDouble(data["px"]);
        float py = (float)Convert.ToDouble(data["py"]);
        float pz = (float)Convert.ToDouble(data["pz"]);
        Vector3 p = new Vector3(px, py, pz);
        pos.Enqueue(p);

        float tp = (float)Convert.ToDouble(data["tp"]);
        t.Enqueue(tp);

        float rx = (float)Convert.ToDouble(data["rx"]);
        float ry = (float)Convert.ToDouble(data["ry"]);
        float rz = (float)Convert.ToDouble(data["rz"]);
        float rw = (float)Convert.ToDouble(data["rw"]);
        Quaternion r = new Quaternion(rx, ry, rz, rw);
        rot.Enqueue(r);
    }
}
