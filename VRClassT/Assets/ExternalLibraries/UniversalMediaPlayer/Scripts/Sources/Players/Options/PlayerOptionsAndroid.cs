using System.Collections.Generic;
using UnityEngine;

namespace UMP
{
    public class PlayerOptionsAndroid : PlayerOptions
    {
        private const string FILE_CACHING_KEY = ":file-caching";
        private const string LIVE_CACHING_KEY = ":live-caching";
        private const string DISC_CACHING_KEY = ":disc-caching";
        private const string NETWORK_CACHING_KEY = ":network-caching";
        private const string CR_AVERAGE_KEY = ":cr-average";
        private const string CLOCK_SYNCHRO_KEY = ":clock-synchro";
        private const string CLOCK_JITTER_KEY = ":clock-jitter";

        private const string HARDWARE_DECODING_STATE_KEY = "avcodec-hw-state";
        private const string HARDWARE_DECODING_KEY = ":avcodec-hw";
        private const string PLAY_IN_BACKGROUND_KEY = ":play-in-background";
        private const string RTSP_OVER_TCP_KEY = ":rtsp-tcp";

        private const int DEFAULT_CR_AVERAGE_VALUE = 40;
        private const int DEFAULT_CLOCK_JITTER_VALUE = 5000;

        public PlayerOptionsAndroid(string[] options) : base(options)
        {
            FileCaching = DEFAULT_CACHING_VALUE;
            LiveCaching = DEFAULT_CACHING_VALUE;
            DiscCaching = DEFAULT_CACHING_VALUE;
            NetworkCaching = DEFAULT_CACHING_VALUE;
            CrAverage = DEFAULT_CR_AVERAGE_VALUE;
            ClockSynchro = State.Default;
            ClockJitter = DEFAULT_CLOCK_JITTER_VALUE;
            HardwareDecoding = State.Default;
        }

        /// <summary>
        /// This allows hardware decoding when available.
        /// </summary>
        public State HardwareDecoding
        {
            get
            {
                return (State)GetValue<int>(HARDWARE_DECODING_STATE_KEY);
            }
            set
            {
                SetValue(HARDWARE_DECODING_STATE_KEY, ((int)value).ToString());

                switch (value)
                {
                    case State.Default:
                        SetValue(HARDWARE_DECODING_KEY, "any");
                        break;

                    case State.Disable:
                        RemoveOption(HARDWARE_DECODING_KEY);
                        break;

                     default:
                        SetValue(HARDWARE_DECODING_KEY, "true");
                        break;
                }
            }
        }

        /// <summary>
        /// Continue play video when application in background.
        /// </summary>
        public bool PlayInBackground
        {
            get
            {
                return GetValue<bool>(PLAY_IN_BACKGROUND_KEY);
            }
            set
            {
                if (value)
                    SetValue(PLAY_IN_BACKGROUND_KEY, string.Empty);
                else
                    RemoveOption(PLAY_IN_BACKGROUND_KEY);
            }
        }

        /// <summary>
        /// Use RTP over RTSP (TCP) (default disabled).
        /// </summary>
        public bool UseTCP
        {
            get
            {
                return GetValue<bool>(RTSP_OVER_TCP_KEY);
            }
            set
            {
                if (value)
                    SetValue(RTSP_OVER_TCP_KEY, string.Empty);
                else
                    RemoveOption(RTSP_OVER_TCP_KEY);
            }
        }

        /// <summary>
        /// Caching value for local files, in milliseconds.
        /// </summary>
        public int FileCaching
        {
            get
            {
                return GetValue<int>(FILE_CACHING_KEY);
            }
            set
            {
                SetValue(FILE_CACHING_KEY, value.ToString());
            }
        }

        /// <summary>
        /// Caching value for cameras and microphones, in milliseconds.
        /// </summary>
        public int LiveCaching
        {
            get
            {
                return GetValue<int>(LIVE_CACHING_KEY);
            }
            set
            {
                SetValue(LIVE_CACHING_KEY, value.ToString());
            }
        }

        /// <summary>
        /// Caching value for optical media, in milliseconds.
        /// </summary>
        public int DiscCaching
        {
            get
            {
                return GetValue<int>(DISC_CACHING_KEY);
            }
            set
            {
                SetValue(DISC_CACHING_KEY, value.ToString());
            }
        }

        /// <summary>
        /// Caching value for network resources, in milliseconds.
        /// </summary>
        public int NetworkCaching
        {
            get
            {
                return GetValue<int>(NETWORK_CACHING_KEY);
            }
            set
            {
                SetValue(NETWORK_CACHING_KEY, value.ToString());
            }
        }

        /// <summary>
        /// When using the PVR input (or a very irregular source), you should set this to 10000.
        /// </summary>
        public int CrAverage
        {
            get
            {
                return GetValue<int>(CR_AVERAGE_KEY);
            }
            set
            {
                SetValue(CR_AVERAGE_KEY, value.ToString());
            }
        }
        
        /// <summary>
        /// It is possible to disable the input clock synchronisation for
        /// real-time sources.Use this if you experience jerky playback of
        /// network streams.
        /// </summary>
        public State ClockSynchro
        {
            get
            {
                return (State)GetValue<int>(CLOCK_SYNCHRO_KEY);
            }
            set
            {
                SetValue(CR_AVERAGE_KEY, ((int)value).ToString());
            }
        }

        /// <summary>
        /// This defines the maximum input delay jitter that the synchronization
        /// algorithms should try to compensate(in milliseconds).
        /// </summary>
        public int ClockJitter
        {
            get
            {
                return GetValue<int>(CLOCK_JITTER_KEY);
            }
            set
            {
                SetValue(CLOCK_JITTER_KEY, value.ToString());
            }
        }
    }
}