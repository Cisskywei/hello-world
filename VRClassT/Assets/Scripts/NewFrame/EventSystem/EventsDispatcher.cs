using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class EventsDispatcher
    {
        public static EventsDispatcher getInstance()
        {
            return Singleton<EventsDispatcher>.getInstance();
        }

        // common int event manager.
        private EventsManager<int> eventCommonInt = new EventsManager<int>();
        private EventsManager<string> eventCommonString = new EventsManager<string>();
        // vrclass event manager.
        private EventsManager<int> eventVRClassInt = new EventsManager<int>();
        private EventsManager<string> eventVRClassString = new EventsManager<string>();
        // uiframework event manager.
        private EventsManager<int> eventUIFrameworkInt = new EventsManager<int>();
        private EventsManager<string> eventUIFrameworkString = new EventsManager<string>();

        public EventsManager<int> MainEventInt
        {
            get { return this.eventCommonInt; }
            private set { }
        }

        public EventsManager<string> MainEventString
        {
            get { return this.eventCommonString; }
            private set { }
        }

        public EventsManager<int> VRClassEventInt
        {
            get { return this.eventVRClassInt; }
            private set { }
        }

        public EventsManager<string> VRClassEventString
        {
            get { return this.eventVRClassString; }
            private set { }
        }

        public EventsManager<int> UIFrameWorkEventInt
        {
            get { return this.eventUIFrameworkInt; }
            private set { }
        }

        public EventsManager<string> UIFrameWorkEventString
        {
            get { return this.eventUIFrameworkString; }
            private set { }
        }
    }
}

