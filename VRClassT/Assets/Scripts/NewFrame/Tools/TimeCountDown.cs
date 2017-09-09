using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class TimeCountDown
    {
        public delegate void VoidCallBack();
        public enum NormalState
        {
            NONE = 0,
            START,
            PLAYING,
            PAUSE,
            END,
        }
        private VoidCallBack _timeover;

        private NormalState _state = NormalState.NONE;

        private bool _isStart;
        private bool _isPause;
        private bool _isEnd;

        private float _endtime;
        private float _timetips;

        public TimeCountDown()
        {
            _state = NormalState.START;

            _endtime = 0;
            _timetips = 0;
        }
        ~TimeCountDown() { _timeover = null; }

        /// <summary>
        /// 开始计时循环
        /// </summary>
        public void loop(float deltatime)
        {
            if (_state != NormalState.PLAYING)
            {
                return;
            }

            if (deltatime <= 0)
            {
                return;
            }

            if (_timetips >= _endtime)
            {
                _state = NormalState.END;
                if (_timeover != null)
                {
                    _timeover();

                    _timeover = null;
                }

                _timetips = 0;
                _endtime = 0;
            }
            else
            {
                _timetips += deltatime;
            }
        }

        public void OnTimeOver(VoidCallBack timeover)
        {
            _timeover = timeover;
        }

        public void StartTimer()
        {
            _state = NormalState.PLAYING;

            _timetips = 0;
        }

        public void StartTimer(float endtime)
        {
            _state = NormalState.PLAYING;

            _endtime = endtime;
            _timetips = 0;
        }

        public void PauseTimer()
        {
            _state = NormalState.PAUSE;
        }

        public void EndTimer()
        {
            _state = NormalState.END;
        }

        public bool IsTimeGoing()
        {
            return _state == NormalState.PLAYING;
        }

        // 获取倒计时当前计时时间
        public float GetLeftTime()
        {
            return _endtime - _timetips;
        }
    }
}

