using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using common;

public class Client {

    public static client.client _client;

    public void InitNet()
    {
        _client = new client.client();

        add_necessary_model();

        _client.onDisConnect += () => {
            log.log.trace(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "disconnect");
        };

        Int64 tick = service.timerservice.Tick;
        _client.connect_server(NetConfig.service_ip, NetConfig.service_port, NetConfig.udp_ip, NetConfig.udp_port, tick);

        _client.onConnectGate += onGeteHandle;
        _client.onConnectHub += onConnectHub;
    }

    public void add_Client_Model(string modelname, imodule model)
    {
        _client.modulemanager.add_module(modelname, model);
    }

    public void clear_Client_Model(string modelname, imodule model)
    {
        //_client.modulemanager.remove_module(modelname);
    }

    /// <summary>
    /// 添加必要的model
    /// </summary>
    private void add_necessary_model()
    {
        add_Client_Model(NetworkCommunicate.selfmodelname, NetworkCommunicate.getInstance());
    }

    public void Loop()
    {
        _client.poll();
    }

    private void onGeteHandle()
    {
        Debug.Log("onGeteHandle");
        _client.connect_hub("lobby");
    }

    private void onConnectHub(string hub_name)
    {
        Debug.Log("onConnectHub");

        EventsDispatcher.getInstance().MainEventInt.TriggerEvent(0);
    }
}
