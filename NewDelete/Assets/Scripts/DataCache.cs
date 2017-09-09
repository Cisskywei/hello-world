using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCache {

    public static DataCache getInstance()
    {
        return Singleton<DataCache>.getInstance();
    }

    public int classid = -1;
    public int userid = -1;
}
