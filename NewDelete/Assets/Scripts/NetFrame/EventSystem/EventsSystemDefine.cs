using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public enum HandleType
    {
        Add = 0,
        Remove = 1,
    }

    public class EventsSystemDefine
    {
        public static Dictionary<int, string> dicHandleType = new Dictionary<int, string>()
        {
            { (int)HandleType.Add, "Add"},
            { (int)HandleType.Remove, "Remove"},
        };
    }
}

