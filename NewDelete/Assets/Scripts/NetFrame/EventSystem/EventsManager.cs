using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class EventsManager<T_Event_Id_Typ>
    {
        private Dictionary<T_Event_Id_Typ, Delegate> _dicEvents = new Dictionary<T_Event_Id_Typ, Delegate>();

        private void LogTypeError(T_Event_Id_Typ eventId, HandleType handleType, Delegate targetEventType, Delegate listener)
        {
            Debug.LogError(string.Format("## Event Id {0}, [{1}] Wrong Listener Type {2}, needed Type {3}.", eventId.ToString(),
                EventsSystemDefine.dicHandleType[(int)handleType],
                targetEventType.GetType(),
                listener.GetType()));
        }

        private bool CheckAddEventListener(T_Event_Id_Typ eventId, Delegate listener)
        {
            if (!this._dicEvents.ContainsKey(eventId))
                this._dicEvents.Add(eventId, null);
            Delegate tmDelegate = this._dicEvents[eventId];
            if (tmDelegate != null && tmDelegate.GetType() != listener.GetType())
            {
                LogTypeError(eventId, HandleType.Add, _dicEvents[eventId], listener);
                return false;
            }
            return true;
        }

        private bool CheckRemoveEventListener(T_Event_Id_Typ eventId, Delegate listener)
        {
            if (!_dicEvents.ContainsKey(eventId))
                return false;

            Delegate tmpDel = _dicEvents[eventId];
            if (tmpDel != null && tmpDel.GetType() != listener.GetType())
            {
                LogTypeError(eventId, HandleType.Remove, _dicEvents[eventId], listener);
                return false;
            }
            return true;
        }

        #region Void
        public void AddEventListener(T_Event_Id_Typ eventId, Action listener)
        {
            if (CheckAddEventListener(eventId, listener))
            {
                Delegate del = this._dicEvents[eventId];
                _dicEvents[eventId] = (Action)Delegate.Combine((Action)del, listener);
            }
        }

        public void RemoveEventListener(T_Event_Id_Typ eventId, Action listener)
        {
            if (CheckRemoveEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = Delegate.Remove(del, listener);
            }
        }

        public void TriggerEvent(T_Event_Id_Typ eventId)
        {
            Delegate del = null;
            if (_dicEvents.TryGetValue(eventId, out del))
            {
                if (del == null)
                    return;

                Delegate[] invocationList = del.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action action = invocationList[i] as Action;
                    if (action == null)
                    {
                        Debug.LogErrorFormat("## Trigger Event {0} Parameters type [void] are not match  target type : {1}.", eventId.ToString(), invocationList[i].GetType());
                        return;
                    }
                    action();
                }
            }
        }

        #endregion

        #region One param
        public void AddEventListener<T>(T_Event_Id_Typ eventId, Action<T> listener)
        {
            if (CheckAddEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = (Action<T>)Delegate.Combine((Action<T>)del, listener);
            }
        }

        public void RemoveEventListener<T>(T_Event_Id_Typ eventId, Action<T> listener)
        {
            if (CheckRemoveEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = Delegate.Remove(del, listener);
            }
        }

        public void TriggerEvent<T>(T_Event_Id_Typ eventId, T p)
        {
            Delegate del = null;
            if (_dicEvents.TryGetValue(eventId, out del))
            {
                if (del == null)
                    return;

                Delegate[] invocationList = del.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T> action = invocationList[i] as Action<T>;
                    if (action == null)
                    {
                        Debug.LogErrorFormat("## Trigger Event {0} Parameters type [ {1} ] are not match  target type : {2}. ",
                            eventId.ToString(),
                            p.GetType(),
                            invocationList[i].GetType());
                        return;
                    }
                    action(p);
                }
            }
        }
        #endregion

        #region Two params
        public void AddEventListener<T0, T1>(T_Event_Id_Typ eventId, Action<T0, T1> listener)
        {
            if (CheckAddEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = (Action<T0, T1>)Delegate.Combine((Action<T0, T1>)del, listener);
            }
        }

        public void RemoveEventListener<T0, T1>(T_Event_Id_Typ eventId, Action<T0, T1> listener)
        {
            if (CheckRemoveEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = Delegate.Remove(del, listener);
            }
        }

        public void TriggerEvent<T0, T1>(T_Event_Id_Typ eventId, T0 p0, T1 p1)
        {
            Delegate del = null;
            if (_dicEvents.TryGetValue(eventId, out del))
            {
                if (del == null)
                    return;

                Delegate[] invocationList = del.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T0, T1> action = invocationList[i] as Action<T0, T1>;
                    if (action == null)
                    {
                        Debug.LogErrorFormat("## Trigger Event {0} Parameters type [ {1}, {2}] are not match  target type : {3}.",
                            eventId.ToString(),
                            p0.GetType(),
                            p1.GetType(),
                            invocationList[i].GetType());
                        return;
                    }
                    action(p0, p1);
                }
            }
        }
        #endregion

        #region Thress params
        public void AddEventListener<T0, T1, T2>(T_Event_Id_Typ eventId, Action<T0, T1, T2> listener)
        {
            if (CheckAddEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = (Action<T0, T1, T2>)Delegate.Combine((Action<T0, T1, T2>)del, listener);
            }
        }

        public void RemoveEventListener<T0, T1, T2>(T_Event_Id_Typ eventId, Action<T0, T1, T2> listener)
        {
            if (CheckRemoveEventListener(eventId, listener))
            {
                Delegate del = _dicEvents[eventId];
                _dicEvents[eventId] = Delegate.Remove(del, listener);
            }
        }

        public void TriggerEvent<T0, T1, T2>(T_Event_Id_Typ eventId, T0 p0, T1 p1, T2 p2)
        {
            Delegate del = null;
            if (_dicEvents.TryGetValue(eventId, out del))
            {
                if (del == null)
                    return;
                Delegate[] invocationList = del.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T0, T1, T2> action = invocationList[i] as Action<T0, T1, T2>;
                    if (action == null)
                    {
                        Debug.LogErrorFormat("## Trigger Event {0} Parameters type [{1}, {2}, {3}] are not match  target type : {4}.",
                            eventId.ToString(),
                            p0.GetType(),
                            p1.GetType(),
                            p2.GetType(),
                            invocationList[i].GetType());
                        return;
                    }
                    action(p0, p1, p2);
                }
            }
        }
        #endregion
    }
}

