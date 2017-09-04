using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class NetMainLoop : MonoBehaviour
    {
        private void Awake()
        {
            MainThreadClient.getInstance().initNet();

            Debug.Log("MainThreadClient ++ Begin");
        }

        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            MainThreadClient.getInstance().loop();
        }

        //// Update is called once per frame
        //void FixedUpdate()
        //{
        //    MainThreadClient.getInstance().loop();
        //}

        private void OnApplicationQuit()
        {
            Int64 roomid = UserInfor.getInstance().RoomId;

            if (!(UserInfor.getInstance().UserUuid == null || UserInfor.getInstance().UserId < 0 || roomid < 0))
            {
                MainThreadClient._client.call_hub("lobby", "WisdomLogin", "player_exit", UserInfor.getInstance().UserUuid, UserInfor.getInstance().UserId, roomid, "cMsgConnect", "ret_msg");
            }

            Debug.Log("程序退出");
        }

    }

}
