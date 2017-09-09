using System;
using System.Collections;
using UnityEngine;
using UMP.Wrappers;

namespace UMP
{
    public class MediaPlayerWebGL : IPlayer
    {
        private MonoBehaviour _monoObject;
        private WrapperInternal _wrapper;
        private IntPtr _pluginObj;

        private float _frameRate;
        private float _tmpTime;
        private long _tmpFrameCounter;

        private bool _isFixedSize;
        private bool _isStarted;
        private bool _isPrepare;
        private bool _isReady;
        private bool _isTextureExist;

        private Uri _dataSource;
        private PlayerBufferVideo _videoBuffer;
        private PlayerManagerEvents _eventManager;
#pragma warning disable 0414
        private PlayerOptions _options;
#pragma warning restore 0414

        private Texture2D _videoTexture;
        private GameObject[] _videoOutputObjects;

        private IEnumerator _updateVideoTextureEnum;

        #region Constructors
        /// <summary>
        ///  Create instance of MediaPlayerWebGL object with additional arguments
        /// </summary>
        /// <param name="monoObject">MonoBehaviour instanse</param>
        /// <param name="videoOutputObjects">Objects that will be rendering video output</param>
        /// <param name="options">Additional player options</param>
        public MediaPlayerWebGL(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptions options)
        {
            _monoObject = monoObject;
            _videoOutputObjects = videoOutputObjects;
            _options = options;

            _wrapper = (Wrapper.Instance.PlatformWrapper as WrapperInternal);

            if (!_wrapper.IsValid)
                return;

            _pluginObj = (IntPtr)_wrapper.NativeHelperInit();
            Debug.Log("_pluginObj: " + _pluginObj);
            if (_options != null)
            {
                if (_options.FixedVideoSize.x > 0 && _options.FixedVideoSize.y > 0)
                {
                    _isFixedSize = true;
                    _videoBuffer = new PlayerBufferVideo((int)_options.FixedVideoSize.x, (int)_options.FixedVideoSize.y);
                }
            }

            _eventManager = new PlayerManagerEvents(_monoObject, this);
        }
        #endregion

        private void UpdateFpsCounter()
        {
            float timeDelta = UnityEngine.Time.time;
            timeDelta = (timeDelta > _tmpTime) ? timeDelta - _tmpTime : 0;
            if (timeDelta >= 1f)
            {
                _frameRate = FramesCounter - _tmpFrameCounter;
                _tmpFrameCounter = FramesCounter;
                _tmpTime = UnityEngine.Time.time;
            }
        }

        private IEnumerator UpdateVideoTexture()
        {
            while (true)
            {
                if (FramesCounter > 0)
                {
                    UpdateFpsCounter();

                    if (!_isTextureExist)
                    {
                        if (_videoTexture != null)
                        {
                            UnityEngine.Object.Destroy(_videoTexture);
                            _videoTexture = null;
                        }

                        if (!_isFixedSize)
                        {
                            int width = VideoWidth;
                            int height = VideoHeight;

                            if (_videoBuffer == null ||
                                (_videoBuffer != null &&
                                _videoBuffer.Width != width || _videoBuffer.Height != height))
                            {
                                if (_videoBuffer != null)
                                    _videoBuffer.ClearFramePixels();

                                _videoBuffer = new PlayerBufferVideo(width, height);
                                //_wrapper.NativeHelperSetPixelsBuffer(_pluginObj, _videoBuffer.FramePixelsAddr, _videoBuffer.Width, _videoBuffer.Height);
                            }
                        }

                        _videoTexture = MediaPlayerHelper.GenPluginTexture(_videoBuffer.Width, _videoBuffer.Height);
                        MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
                        _isTextureExist = true;
                    }

                    if (!_isReady)
                    {
                        _isReady = true;

                        if (_isPrepare)
                        {
                            _eventManager.ReplaceEvent(PlayerState.Paused, PlayerState.Prepared, _videoTexture);
                            Pause();
                        }
                        else
                        {
                            _eventManager.SetEvent(PlayerState.Prepared, _videoTexture);
                        }
                    }

                    _wrapper.NativeHelperUpdateTexture(_pluginObj, _videoTexture.GetNativeTexturePtr());
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public GameObject[] VideoOutputObjects
        {
            set
            {
                _videoOutputObjects = value;
                MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
            }

            get { return _videoOutputObjects; }
        }

        public PlayerManagerEvents EventManager
        {
            get { return _eventManager; }
        }

        public PlayerOptions Arguments
        {
            get
            {
                return _options;
            }
        }

        public PlayerState State
        {
            get
            {
                return _wrapper.PlayerGetState(_pluginObj);
            }
        }

        public object StateValue
        {
            get
            {
                return _wrapper.PlayerGetStateValue(_pluginObj);
            }
        }

        public void AddMediaListener(IMediaListener listener)
        {
            if (_eventManager != null)
            {
                _eventManager.PlayerOpeningListener += listener.OnPlayerOpening;
                _eventManager.PlayerBufferingListener += listener.OnPlayerBuffering;
                _eventManager.PlayerPlayingListener += listener.OnPlayerPlaying;
                _eventManager.PlayerPausedListener += listener.OnPlayerPaused;
                _eventManager.PlayerStoppedListener += listener.OnPlayerStopped;
                _eventManager.PlayerEndReachedListener += listener.OnPlayerEndReached;
                _eventManager.PlayerEncounteredErrorListener += listener.OnPlayerEncounteredError;
            }
        }

        public void RemoveMediaListener(IMediaListener listener)
        {
            if (_eventManager != null)
            {
                _eventManager.PlayerOpeningListener -= listener.OnPlayerOpening;
                _eventManager.PlayerBufferingListener -= listener.OnPlayerBuffering;
                _eventManager.PlayerPlayingListener -= listener.OnPlayerPlaying;
                _eventManager.PlayerPausedListener -= listener.OnPlayerPaused;
                _eventManager.PlayerStoppedListener -= listener.OnPlayerStopped;
                _eventManager.PlayerEndReachedListener -= listener.OnPlayerEndReached;
                _eventManager.PlayerEncounteredErrorListener -= listener.OnPlayerEncounteredError;
            }
        }

        public void Prepare()
        {
            _isPrepare = true;
            Play();
        }

        /// <summary>
        /// Play or resume (True if playback started (and was already started), or False on error.
        /// </summary>
        /// <returns></returns>
        public bool Play()
        {
            if (!_isStarted)
            {
                if (_eventManager != null)
                    _eventManager.StartListener();
            }

            if (_updateVideoTextureEnum == null)
            {
                _updateVideoTextureEnum = UpdateVideoTexture();
                _monoObject.StartCoroutine(_updateVideoTextureEnum);
            }

            _isStarted = _wrapper.PlayerPlay(_pluginObj);

            if (!_isStarted)
                Stop();

            return _isStarted;
        }

        public void Pause()
        {
            if (_videoOutputObjects == null && _updateVideoTextureEnum != null)
            {
                _monoObject.StopCoroutine(_updateVideoTextureEnum);
                _updateVideoTextureEnum = null;
            }

            _wrapper.PlayerPause(_pluginObj);
        }

        public void Stop(bool resetTexture)
        {
            _wrapper.PlayerStop(_pluginObj);

            if (_updateVideoTextureEnum != null)
            {
                _monoObject.StopCoroutine(_updateVideoTextureEnum);
                _updateVideoTextureEnum = null;
            }

            _frameRate = 0;
            _tmpFrameCounter = 0;
            _tmpTime = 0;

            _isStarted = false;
            _isPrepare = false;
            _isReady = false;
            _isTextureExist = !resetTexture;

            if (resetTexture)
            {
                if (_videoTexture != null)
                {
                    UnityEngine.Object.Destroy(_videoTexture);
                    _videoTexture = null;
                }
            }

            if (_eventManager != null)
                _eventManager.StopListener();
        }

        public void Stop()
        {
            Stop(true);
        }

        public void Release()
        {
            if (_pluginObj != IntPtr.Zero && _updateVideoTextureEnum != null)
                Stop();

            if (_updateVideoTextureEnum != null)
            {
                _monoObject.StopCoroutine(_updateVideoTextureEnum);
                _updateVideoTextureEnum = null;
            }

            if (_eventManager != null)
            {
                _eventManager.RemoveAllEvents();
                _eventManager = null;
            }

            _wrapper.PlayerRelease(_pluginObj);
        }

        public Uri DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
                if (_dataSource.AbsoluteUri.Contains("file:///"))
                {
                    var newPath = _dataSource.AbsoluteUri.Replace("file:///", "");
                    if (!newPath.Contains(":/"))
                    {
                        string checkAssetsFilePath = Application.streamingAssetsPath + _dataSource.AbsolutePath;
                        _dataSource = new Uri(checkAssetsFilePath);
                    }
                }

                _wrapper.PlayerSetDataSource(_pluginObj, _dataSource.AbsoluteUri);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _wrapper.PlayerIsPlaying(_pluginObj);
            }
        }

        public bool IsReady
        {
            get { return _isReady; }
        }

        public bool AbleToPlay
        {
            get
            {
                return _dataSource != null && !string.IsNullOrEmpty(_dataSource.ToString());
            }
        }

        /// <summary>
        /// Get the current movie length (in ms).
        /// </summary>
        /// <returns></returns>
        public long Length
        {
            get
            {
                return _wrapper.PlayerGetLength(_pluginObj);
            }
        }

        /// <summary>
        /// Get the current movie formatted length (hh:mm:ss[:ms]).
        /// </summary>
        /// <param name="detail">True: formatted length will be with [:ms]</param>
        /// <returns></returns>
        public string GetFormattedLength(bool detail)
        {
            var length = TimeSpan.FromMilliseconds(Length);

            var format = detail ? "{0:D2}:{1:D2}:{2:D2}:{3:D3}" : "{0:D2}:{1:D2}:{2:D2}";

            return string.Format(format,
                length.Hours,
                length.Minutes,
                length.Seconds,
                length.Milliseconds);
        }

        public float FrameRate
        {
            get { return _frameRate; }
        }

        public int FramesCounter
        {
            get { return _wrapper.PlayerVideoFrameCount(_pluginObj); }
        }

        public byte[] FramePixels
        {
            get
            {
                return null;
            }
        }

        public long Time
        {
            get
            {
                return _wrapper.PlayerGetTime(_pluginObj);
            }
            set
            {
                _wrapper.PlayerSetTime(_pluginObj, (int)value);
            }
        }

        public float Position
        {
            get
            {
                return _wrapper.PlayerGetPosition(_pluginObj);
            }
            set
            {
                _wrapper.PlayerSetPosition(_pluginObj, value);
            }
        }

        public float PlaybackRate
        {
            get
            {
                return _wrapper.PlayerGetRate(_pluginObj);
            }
            set
            {
                _wrapper.PlayerSetRate(_pluginObj, value);
            }
        }

        public int Volume
        {
            get
            {
                return _wrapper.PlayerGetVolume(_pluginObj);
            }
            set
            {
                _wrapper.PlayerSetVolume(_pluginObj, value);
            }
        }

        public bool Mute
        {
            get
            {
                return _wrapper.PlayerGetMute(_pluginObj);
            }
            set
            {
                _wrapper.PlayerSetMute(_pluginObj, value);
            }
        }

        public int VideoWidth
        {
            get
            {
                var width = 0;

                if (!_isFixedSize)
                    width = _wrapper.PlayerVideoWidth(_pluginObj);
                else
                    width = _videoBuffer.Width;

                return width;
            }
        }

        public int VideoHeight
        {
            get
            {
                var height = 0;

                if (!_isFixedSize)
                    height = _wrapper.PlayerVideoHeight(_pluginObj);
                else
                    height = _videoBuffer.Height;

                return height;
            }
        }

        public Vector2 VideoSize
        {
            get
            {
                return new Vector2(VideoWidth, VideoHeight);
            }
        }
    }
}
