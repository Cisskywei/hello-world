using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMainLoop : MonoBehaviour {

    public static NetMainLoop _instance;
    public static GameObject _selfgo;
    public static NetMainLoop Instance()
    {
        if(_instance == null)
        {
            if(_selfgo == null)
            {
                _selfgo = GameObject.Find("NetMainLoop");
            }

            if(_selfgo != null)
            {
                _instance = _selfgo.GetComponent<NetMainLoop>();
            }
        }

        return _instance;
    }

    Client _client = new Client();

    FuncLifeCycle flc = new FuncLifeCycle();

    void Awake()
    {
        _instance = this;
        _selfgo = gameObject;

        //_client.InitNet();
    }

    // Use this for initialization
    void Start () {

        _client.InitNet();

        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update() {
        _client.Loop();
        flc.OnUpdate();
    }

    public void AddUpdate(FuncLifeCycle.VoidParam func)
    {
        flc.AddUpdate(func);
    }

    public void RemoveUpdate(FuncLifeCycle.VoidParam func)
    {
        flc.RemoveUpdate(func);
    }

    //游戏退出
    private void OnApplicationQuit()
    {
        NetworkCommunicate.getInstance().PlayerExit((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId);
    }
}
