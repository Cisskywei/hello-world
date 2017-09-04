using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsSystemTester : MonoBehaviour, IEventsListener
{

    void OnEnable()
    {
        RegisterEvent();
    }

    void OnDisable()
    {
        UnRegisterEvent();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventsDispatcher.getInstance().MainEventInt.AddEventListener<string>(0, this.OnUserInput);
        EventsDispatcher.getInstance().MainEventString.AddEventListener<string>("ll", this.OnUserInputString);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventsDispatcher.getInstance().MainEventInt.RemoveEventListener<string>(0, this.OnUserInput);
        EventsDispatcher.getInstance().MainEventString.RemoveEventListener<string>("ll", this.OnUserInputString);
    }

    private void OnUserInput(string msg)
    {
        Debug.Log("[on User input message:" + msg + "]");
    }

    private void OnUserInputString(string msg)
    {
        Debug.Log("[on User input String message:" + msg + "]");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // trigger the event.
            EventsDispatcher.getInstance().MainEventInt.TriggerEvent<string>(0, "Hello World!");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // trigger the event.
            EventsDispatcher.getInstance().MainEventString.TriggerEvent<string>("ll", "Hello World!");
        }
    }
}
