using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class SceneConfig
    {

        /// <summary>
        /// 智慧教室的模式
        /// </summary>
        public enum ModelEnums
        {
            None = 0,
            Separate = 2,     // IndependentMode
            SynchronousOne = 4,   // WatchAndLearnModel
            SynchronousMultiple = 8,   // GuidanceMode
            Collaboration = 16,   // CollaborativeModel
        }
    }
}
