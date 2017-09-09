using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMP
{
    public enum LogDetail
    {
        Disable,
        Debug,
        Error,
        Warning
    }

    public class PlayerManagerLogs
    {
        public class PlayerLog
        {
            public PlayerLog(LogDetail level, string msg)
            {
                Message = msg;
                Level = level;
            }

            public string Message { get; private set; }
            public LogDetail Level { get; private set; }
        }

        private MonoBehaviour _monoObject;
        private MediaPlayerStandalone _player;
        private Queue<PlayerLog> _playerLogs;
        private IEnumerator _eventListenerEnum;
        private LogDetail _logDetail;
        private string _errorMessage = string.Empty;

        internal PlayerManagerLogs(MonoBehaviour monoObject, MediaPlayerStandalone player)
        {
            _monoObject = monoObject;
            _player = player;
            _playerLogs = new Queue<PlayerLog>();
        }

        private PlayerLog Message
        {
            get
            {
                string message = _player.LogMessage;

                if (!string.IsNullOrEmpty(message))
                {
                    var logLevel = _player.LogLevel;
                    var logDetail = LogDetail.Debug;

                    switch(logLevel)
                    {
                        case 3:
                            logDetail = LogDetail.Error;
                            break;

                        case 4:
                            logDetail = LogDetail.Warning;
                            break;
                    }

                    return new PlayerLog(logDetail, message);
                }

                return null;
            }
        }

        private IEnumerator LogManager()
        {
            while (true)
            {
                var currentMessage = Message;

                if (currentMessage != null)
                    _playerLogs.Enqueue(currentMessage);

                if (_playerLogs.Count <= 0)
                {
                    yield return null;
                    continue;
                }

                CallLog();
            }
        }


        private void CallLog()
        {
            var logValue = _playerLogs.Dequeue();

            if (logValue != null && logValue.Level == _logDetail)
            {
                if (_logMessageListener != null)
                    _logMessageListener(logValue);
            }
        }

        internal void SetLog(LogDetail detail, string message)
        {
            _playerLogs.Enqueue(new PlayerLog(detail, message));
        }

        public void StartListener()
        {
            _playerLogs.Clear();
            if (_eventListenerEnum != null)
                _monoObject.StopCoroutine(_eventListenerEnum);

            _eventListenerEnum = LogManager();
            _monoObject.StartCoroutine(_eventListenerEnum);
        }

        public void StopListener()
        {
            if (_eventListenerEnum != null)
                _monoObject.StopCoroutine(_eventListenerEnum);

            while (_playerLogs.Count > 0)
            {
                CallLog();
            }
        }

        public void RemoveAllEvents()
        {
            if (_logMessageListener != null)
            {
                foreach (Action<PlayerLog> eh in _logMessageListener.GetInvocationList())
                {
                    _logMessageListener -= eh;
                }
            }
        }

        public LogDetail LogDetail
        {
            get { return _logDetail; }
            set { _logDetail = value; }
        }

        public string LastError
        {
            get
            {
                return _errorMessage;
            }
        }

        #region Actions
        private event Action<PlayerLog> _logMessageListener;

        public event Action<PlayerLog> LogMessageListener
        {
            add
            {
                _logMessageListener += value;
            }
            remove
            {
                if (_logMessageListener != null)
                {
                    _logMessageListener -= value;
                }
            }
        }
        #endregion
    }
}
