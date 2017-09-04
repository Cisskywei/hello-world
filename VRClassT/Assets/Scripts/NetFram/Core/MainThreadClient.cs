using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using client;
using common;
using System;
using System.Threading;
using TinyFrameWork;

namespace ko.NetFram
{
    public class MainThreadClient
    {
        public static MainThreadClient getInstance()
        {
            return Singleton<MainThreadClient>.getInstance();
        }

        public static client.client _client;
        private bool _isInit = false;

        Int64 tick;
        Int64 tmptick;
        Int64 tickcount;

        public void initNet()
        {
            tick = Environment.TickCount;
            tickcount = 0;

            _client = new client.client();

            //消息接收器
            addModule(NetConfig.msgconnect_module_name, MsgModule.getInstance());

            _client.connect_server(NetConfig.service_ip, NetConfig.service_port, NetConfig.service_ip, 3237, tick);

            _client.onConnectGate += onGeteHandle;
            _client.onConnectHub += onConnectHub;

            

            _isInit = true;
        }


        public void addModule(string modulename, imodule im)
        {
            if (_client == null)
            {
                _client = new client.client();
            }

            _client.modulemanager.add_module(modulename, im);
        }

        private static void onGeteHandle()
        {
            Debug.Log("onGeteHandle");
            _client.connect_hub("lobby");
        }

        private static void onConnectHub(string hub_name)
        {
            Debug.Log("onConnectHub");

            EventDispatcher.GetInstance().MainEventManager.TriggerEvent(EventId.ConnectedHub);
            //_client.call_hub("lobby", "login", "player_login", "1234-1234", "qianqians");
            //           _client.call_hub("lobby", "slogin", "player_login", "1234-1234", "qianqians");
        }

        public void loop()
        {
            if (!_isInit)
            {
                return;
            }

            tmptick = (Environment.TickCount & UInt32.MaxValue);
            if (tmptick < tick)
            {
                tickcount += 1;
                tmptick = tmptick + tickcount * UInt32.MaxValue;
            }
            tick = tmptick;

            try
            {
                _client.poll();
            }
            catch (Exception e)
            {
                Debug.Log("网络异常 " + e);
            }

            ////while (true)
            ////{
            //Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
            //    if (tmptick < tick)
            //    {
            //        tickcount += 1;
            //        tmptick = tmptick + tickcount * UInt32.MaxValue;
            //    }
            //    tick = tmptick;

            //    try
            //    {
            //        _client.poll(tick);
            //    }catch (Exception e)
            //    {
            //        Debug.Log("网络异常 " + e);
            //    }
                

            //    tmptick = (Environment.TickCount & UInt32.MaxValue);
            //    if (tmptick < tick)
            //    {
            //        tickcount += 1;
            //        tmptick = tmptick + tickcount * UInt32.MaxValue;
            //    }
            //    Int64 ticktime = (tmptick - tick);
            //    tick = tmptick;

            //    //if (ticktime < 50)
            //    //{
            //    //    Thread.Sleep(15);
            //    //}
            ////}
        }
    }
}

