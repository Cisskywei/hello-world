using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class CommondRecive : MonoBehaviour, sync_commond
    {
        private void Awake()
        {
            MsgModule.getInstance().registerSyncCommond(this);
        }

        public void ret_sync_commond(string typ, string commond, string name, string objectname)
        {
            // 消息解析函数  可通过Event 事件机制分发
        }
    }
}
