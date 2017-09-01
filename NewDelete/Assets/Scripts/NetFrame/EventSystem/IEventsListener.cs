using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    interface IEventsListener
    {
        void RegisterEvent();
        void UnRegisterEvent();
    }
}

