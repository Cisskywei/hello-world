using UnityEngine;
using System.Collections.Generic;


namespace PaintCraft{ 
    /// <summary>
    /// this class is a wrapper on top of the standard unity analytics. 
    /// we use this to avoid problems if someone won't use Unity analytics.
    /// </summary>
    public static class AnalyticsWrapper {
        public static void CustomEvent(string key, Dictionary<string, object> data){
            #if UNITY_ANALYTICS
            //UnityEngine.Analytics.Analytics.CustomEvent(key, data);
            #endif
        }
    }
}
    