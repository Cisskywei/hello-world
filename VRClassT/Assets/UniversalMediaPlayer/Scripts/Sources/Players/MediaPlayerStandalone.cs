using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using UnityEngine;
using UMP.Wrappers;

namespace UMP
{
    public class MediaPlayerStandalone : IPlayer, IPlayerAudio, IPlayerSpu
    {
        #region LibVLC Delegates
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr VideoLockHandler(IntPtr opaque, ref IntPtr pixels);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VideoDisplayHandler(IntPtr opaque, IntPtr picture);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint VideoFormatHandler(ref IntPtr opaque, ref uint chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void AudioPlayHandler(IntPtr data, IntPtr samples, uint count, long pts);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int AudioFormatHandler(ref IntPtr data, string format, ref int rate, ref int channels);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EventHandler(ref IntPtr e, IntPtr data);
        #endregion

        private MonoBehaviour _monoObject;
        private WrapperStandalone _wrapper;
        private int _playerIndex;

        private VideoLockHandler _lockHandler;
        private VideoDisplayHandler _displayHandler;
        private VideoFormatHandler _videoFormatHandler;
        private EventHandler _eventHandler;

        private AudioPlayHandler _audioPlayHandler;
        private AudioFormatHandler _audioFormatHandler;

        /// Native VLC library callback pointers
        private IntPtr _lockPtr;
        private IntPtr _displayPtr;
        private IntPtr _videoFormatPtr;

        private IntPtr _audioPlayPtr;
        private IntPtr _audioFormatPtr;

        private IntPtr _eventHandlerPtr;
        private IntPtr _eventManagerPtr;

        private IntPtr _vlcObj;
        private IntPtr _mediaObj;
        private IntPtr _playerObj;

        private float _frameRate;
        private float _tmpTime;
        private long _tmpFrameCounter;

        private bool _isFixedSize;
        private bool _isStarted;
        private bool _isPrepare;
        private bool _isReady;
        private bool _isAudioDataReady;
        private bool _isTextureExist;

        private Uri _dataSource;
        private PlayerBufferVideo _videoBuffer;
        private PlayerBufferSound _soundBuffer;
        private PlayerManagerLogs _logManager;
        private PlayerManagerEvents _eventManager;
        private PlayerOptionsStandalone _options;
        private string[] _playerArguments;

        private Texture2D _videoTexture;
        private GameObject[] _videoOutputObjects;
        private PlayerManagerAudios _audioManager;
        private int _audioSamplesOffset;
        private AudioClip _mainAudioClip;

        private IEnumerator _updateVideoTextureEnum;
        private IEnumerator _updateAudioSourceEnum;

        private Thread _mainThread;
        private Thread _releaseThread;

        #region Constructors
        /// <summary>
        ///  Create instance of MediaPlayerStandalone object with additional arguments
        /// </summary>
        /// <param name="monoObject">MonoBehaviour instanse</param>
        /// <param name="videoOutputObjects">Objects that will be rendering video output</param>
        /// <param name="options">Additional player options</param>
        public MediaPlayerStandalone(MonoBehaviour monoObject, GameObject[] videoOutputObjects, PlayerOptionsStandalone options)
        {
            _monoObject = monoObject;
            _videoOutputObjects = videoOutputObjects;
            _options = options;

            _wrapper = (Wrapper.Instance.PlatformWrapper as WrapperStandalone);

            if (!_wrapper.IsValid)
                return;

            _playerIndex = _wrapper.NativeHelperInit();

            if (_playerIndex < 0)
            {
                Debug.LogError("Don't support video playback on current platform or you use incorrect UMP libraries!");
                throw new Exception();
            }

            if (_options != null)
            {
                _playerArguments = _options.GetOptions('\n').Split('\n');
                _wrapper.NativeHelperSetPixelsVerticalFlip((IntPtr)_playerIndex, _options.FlipVertically);

                if (_options.FixedVideoSize.x > 0 && _options.FixedVideoSize.y > 0)
                {
                    _isFixedSize = true;
                    _videoBuffer = new PlayerBufferVideo((int)_options.FixedVideoSize.x, (int)_options.FixedVideoSize.y);
                    _wrapper.NativeHelperSetPixelsBuffer((IntPtr)_playerIndex, _videoBuffer.FramePixelsAddr, _videoBuffer.Width, _videoBuffer.Height);
                }

                if (_options.AudioOutputSources != null)
                    _audioManager = new PlayerManagerAudios(_options.AudioOutputSources);
            }

            MediaPlayerInit();
        }
        #endregion

        private void MediaPlayerInit()
        {
            _vlcObj = _wrapper.PlayerExpandedLibVLCNew(_playerArguments);

            if (_vlcObj == IntPtr.Zero)
                throw new Exception("Can't create new libVLC object instance");

            _playerObj = _wrapper.PlayerExpandedMediaPlayerNew(_vlcObj);

            if (_playerObj == IntPtr.Zero)
                throw new Exception("Can't create new media player object instance");

            _eventManagerPtr = _wrapper.PlayerExpandedEventManager(_playerObj);
            _eventHandler = (EventHandler)Marshal.GetDelegateForFunctionPointer(_wrapper.NativeHelperMediaPlayerEventCallback(), typeof(EventHandler));
            _eventHandlerPtr = Marshal.GetFunctionPointerForDelegate(_eventHandler);
            EventsAttach(_eventManagerPtr, _eventHandlerPtr);

            _eventManager = new PlayerManagerEvents(_monoObject, this);

            _wrapper.PlayerExpandedLogSet(_vlcObj, _wrapper.NativeHelperGetLogMessageCallback(), new IntPtr(_playerIndex));
            _logManager = new PlayerManagerLogs(_monoObject, this);

            _audioPlayHandler = AudioPlay;
            _audioFormatHandler = AudioFormat;

            _lockHandler = (VideoLockHandler)Marshal.GetDelegateForFunctionPointer(_wrapper.NativeHelperGetVideoLockCallback(), typeof(VideoLockHandler));
            _displayHandler = (VideoDisplayHandler)Marshal.GetDelegateForFunctionPointer(_wrapper.NativeHelperGetVideoDisplayCallback(), typeof(VideoDisplayHandler));

            _lockPtr = Marshal.GetFunctionPointerForDelegate(_lockHandler);
            _displayPtr = Marshal.GetFunctionPointerForDelegate(_displayHandler);

            _videoFormatHandler = (VideoFormatHandler)Marshal.GetDelegateForFunctionPointer(_wrapper.NativeHelperGetVideoFormatCallback(), typeof(VideoFormatHandler));
            _videoFormatPtr = Marshal.GetFunctionPointerForDelegate(_videoFormatHandler);

            _audioFormatPtr = Marshal.GetFunctionPointerForDelegate(_audioFormatHandler);
            _audioPlayPtr = Marshal.GetFunctionPointerForDelegate(_audioPlayHandler);

            _wrapper.PlayerExpandedVideoSetCallbacks(_playerObj, _lockPtr, IntPtr.Zero, _displayPtr, new IntPtr(_playerIndex));
            
            if (_videoBuffer == null)
                _wrapper.PlayerExpandedVideoSetFormatCallbacks(_playerObj, _videoFormatPtr, IntPtr.Zero);
            else
                _wrapper.PlayerExpandedVideoSetFormat(_playerObj, _videoBuffer.Chroma, (uint)_videoBuffer.Width, (uint)_videoBuffer.Height, (uint)_videoBuffer.Stride);

            if (_audioManager != null && _audioManager.IsValid)
            {
                _wrapper.PlayerExpandedAudioSetFormatCallbacks(_playerObj, _audioFormatPtr, IntPtr.Zero);
                _wrapper.PlayerExpandedAudioSetCallbacks(_playerObj, _audioPlayPtr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, new IntPtr(_playerIndex));
            }

            _mainThread = Thread.CurrentThread;
            _releaseThread = new Thread(OnRelease);
            _releaseThread.Start();
        }

        private void OnRelease()
        {
            bool threadDone = false;

            while (!threadDone)
            {
                threadDone = _monoObject == null;
                Thread.Sleep(1000);
            }

            Release();
        }

        #region Private callbacks
        private int AudioFormat(ref IntPtr data, string format, ref int rate, ref int channels)
        {
            _soundBuffer = new PlayerBufferSound(PlayerBufferSound.GetSoundType(format), rate, channels);
            rate = _soundBuffer.Rate;
            channels = _soundBuffer.Channels;
            return 0;
        }

        private void AudioPlay(IntPtr data, IntPtr samples, uint count, long pts)
        {
            _soundBuffer.UpdateSamplesData(samples, count);
            _soundBuffer.Pts = pts;
            _isAudioDataReady = true;
        }
        #endregion

        #region Private methods
        private void UpdateFpsCounter()
        {
            float currentTime = UnityEngine.Time.time;
            currentTime = (currentTime > _tmpTime) ? currentTime - _tmpTime : 0;
            if (currentTime >= 1f)
            {
                _frameRate = FramesCounter - _tmpFrameCounter;
                _tmpFrameCounter = FramesCounter;
                _tmpTime = UnityEngine.Time.time;
            }
        }

        private MediaTrackInfo[] GetTracks(int tracksCount, IntPtr tracksList)
        {
            var result = new List<MediaTrackInfo>();
            IntPtr list = tracksList;

            for (int i = 0; i < tracksCount; i++)
            {
                if (list != IntPtr.Zero)
                {
                    var track = (TrackDescription)Marshal.PtrToStructure(list, typeof(TrackDescription));
                    result.Add(new MediaTrackInfo(track.Id, track.Name));
                    list = track.NextDescription;
                }
            }

            return result.ToArray();
        }

        private void EventsAttach(IntPtr eventManager, IntPtr enentHandlerPtr)
        {
            string exceptionMsg = "Failed to subscribe to event notification";

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerOpening, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (Opening)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerBuffering, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (Buffering)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerPlaying, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (Playing)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerPaused, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (Paused)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerStopped, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (Stopped)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerEndReached, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (EndReached)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerEncounteredError, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (EncounteredError)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerTimeChanged, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (TimeChanged)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerPositionChanged, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (PositionChanged)");

            if (_wrapper.PlayerExpandedEventAttach(eventManager, EventTypes.MediaPlayerSnapshotTaken, enentHandlerPtr, (IntPtr)_playerIndex) != 0)
                throw new OutOfMemoryException(exceptionMsg + " (SnapshotTaken)");
        }

        private void EventsDettach(IntPtr eventManager, IntPtr enentHandlerPtr)
        {
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerOpening, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerBuffering, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerPlaying, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerPaused, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerStopped, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerEndReached, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerEncounteredError, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerTimeChanged, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerPositionChanged, enentHandlerPtr, (IntPtr)_playerIndex);
            _wrapper.PlayerExpandedEventDetach(eventManager, EventTypes.MediaPlayerSnapshotTaken, enentHandlerPtr, (IntPtr)_playerIndex);
        }

        private IEnumerator UpdateAudioSource()
        {
            while (true)
            {
                if (_isAudioDataReady)
                {
                    if (_mainAudioClip == null)
                    {
                        _mainAudioClip = AudioClip.Create(AudioTrack.Name, _soundBuffer.SamplesData.Count * 2, _soundBuffer.Channels, _soundBuffer.Rate, false);
                        _audioManager.Clip = _mainAudioClip;
                        _audioManager.Loop = true;
                    }

                    if (!_audioManager.IsPlaying)
                        _audioManager.Play();

                    if (_soundBuffer.SamplesData.Count > 0)
                    {
                        lock (_soundBuffer.SamplesData)
                        {
                            int samplesCount = _soundBuffer.SamplesData.Count >> 1;
                            float[] audioData = null;

                            if (_audioSamplesOffset == _mainAudioClip.samples)
                                _audioSamplesOffset = 0;

                            if (_audioSamplesOffset + samplesCount > _mainAudioClip.samples)
                            {
                                var arrayOffset = (_audioSamplesOffset + samplesCount) - _mainAudioClip.samples;
                                arrayOffset = _soundBuffer.SamplesData.Count - (arrayOffset << 1);
                                audioData = _soundBuffer.SamplesData.GetRange(0, arrayOffset).ToArray();
                                _soundBuffer.SamplesData.RemoveRange(0, arrayOffset);
                                samplesCount = audioData.Length >> 1;
                            }
                            else
                            {
                                audioData = _soundBuffer.SamplesData.ToArray();
                                _soundBuffer.SamplesData.Clear();
                            }

                            _mainAudioClip.SetData(audioData, _audioSamplesOffset);
                            _audioSamplesOffset += samplesCount;
                        }
                    }

                    _isAudioDataReady = false;
                }
                yield return null;
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
                            int width = _wrapper.NativeHelperGetPixelsBufferWidth((IntPtr)_playerIndex);
                            int height = _wrapper.NativeHelperGetPixelsBufferHeight((IntPtr)_playerIndex);

                            if (_videoBuffer == null ||
                                (_videoBuffer != null &&
                                _videoBuffer.Width != width || _videoBuffer.Height != height))
                            {
                                _videoBuffer = new PlayerBufferVideo(width, height);
                                _wrapper.NativeHelperSetPixelsBuffer((IntPtr)_playerIndex, _videoBuffer.FramePixelsAddr, _videoBuffer.Width, _videoBuffer.Height);
                            }
                        }

                        _videoTexture = MediaPlayerHelper.GenPluginTexture(_videoBuffer.Width, _videoBuffer.Height);
                        MediaPlayerHelper.ApplyTextureToRenderingObjects(_videoTexture, _videoOutputObjects);
                        _wrapper.NativeHelperSetTexture((IntPtr)_playerIndex, _videoTexture.GetNativeTexturePtr());

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
                    
                    GL.IssuePluginEvent(_wrapper.NativeHelperGetUnityRenderCallback(), _playerIndex);
                }

                yield return new WaitForEndOfFrame();
            }
        }
        #endregion

        #region Public methods 
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
                if (_eventManagerPtr != IntPtr.Zero)
                    return _wrapper.PlayerGetState((IntPtr)_playerIndex);

                return PlayerState.Empty;
            }
        }

        public object StateValue
        {
            get
            {
                if (_eventManagerPtr != IntPtr.Zero)
                    return _wrapper.PlayerGetStateValue((IntPtr)_playerIndex);

                return null;
            }
        }

        public PlayerManagerLogs LogManager
        {
            get { return _logManager; }
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
            if (_playerObj != IntPtr.Zero)
            {
                if (!_isStarted)
                {
                    if (_eventManager != null)
                        _eventManager.StartListener();

                    if (_logManager != null)
                        _logManager.StartListener();

                    _wrapper.NativeHelperUpdateFramesCounter((IntPtr)_playerIndex, 0);

                    if (_audioManager != null && _audioManager.IsValid)
                    {
                        _updateAudioSourceEnum = UpdateAudioSource();
                        _monoObject.StartCoroutine(_updateAudioSourceEnum);
                    }
                }

                if (_updateVideoTextureEnum == null)
                {
                    _updateVideoTextureEnum = UpdateVideoTexture();
                    _monoObject.StartCoroutine(_updateVideoTextureEnum);
                }

                _isStarted = _wrapper.PlayerPlay(_playerObj);

                if (!_isStarted)
                    Stop();
            }

            return _isStarted;
        }
        
        /// <summary>
        /// Pause current video playback
        /// </summary>
        public void Pause()
        {
            if (_playerObj != IntPtr.Zero)
            {
                if (_videoOutputObjects == null && _updateVideoTextureEnum != null)
                {
                    _monoObject.StopCoroutine(_updateVideoTextureEnum);
                    _updateVideoTextureEnum = null;
                }

                if (_updateAudioSourceEnum != null)
                {
                    _monoObject.StopCoroutine(_updateAudioSourceEnum);
                    _updateAudioSourceEnum = null;
                }

                if (_audioManager != null)
                    _audioManager.Pause();
                _wrapper.PlayerPause(_playerObj);

                _isAudioDataReady = false;
                if (_soundBuffer != null)
                    _soundBuffer.SamplesData.Clear();
            }
        }

        /// <summary>
        /// Stop current video playback
        /// </summary>
        /// <param name="resetTexture">Clear previous playback texture</param>
        public void Stop(bool resetTexture)
        {
            if (_playerObj != IntPtr.Zero && _isStarted)
            {
                _wrapper.PlayerStop(_playerObj);

                if (_updateVideoTextureEnum != null)
                {
                    _monoObject.StopCoroutine(_updateVideoTextureEnum);
                    _updateVideoTextureEnum = null;
                }

                if (_updateAudioSourceEnum != null)
                {
                    _monoObject.StopCoroutine(_updateAudioSourceEnum);
                    _updateVideoTextureEnum = null;
                }

                if (_audioManager != null)
                    _audioManager.Stop();

                _frameRate = 0;
                _tmpFrameCounter = 0;
                _tmpTime = 0;
                _audioSamplesOffset = 0;

                _isStarted = false;
                _isPrepare = false;
                _isReady = false;
                _isAudioDataReady = false;

                _wrapper.NativeHelperUpdateFramesCounter((IntPtr)_playerIndex, 0);

                if (_soundBuffer != null)
                    _soundBuffer.SamplesData.Clear();

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

                if (_logManager != null)
                    _logManager.StopListener();
            }
        }

        /// <summary>
        /// Stop current video playback
        /// </summary>
        public void Stop()
        {
            Stop(true);
        }

        /// <summary>
        /// Release current video player
        /// </summary>
        public void Release()
        {
            if (Thread.CurrentThread == _mainThread)
            {
                if (_playerObj != IntPtr.Zero)
                    Stop();
            }

            if (_eventManager != null)
            {
                _eventManager.RemoveAllEvents();
                _eventManager = null;

                if (_eventHandlerPtr != IntPtr.Zero)
                    EventsDettach(_eventManagerPtr, _eventHandlerPtr);
            }

            if (_logManager != null)
            {
                _logManager.RemoveAllEvents();

                if (_vlcObj != IntPtr.Zero)
                    _wrapper.PlayerExpandedLogUnset(_vlcObj);
            }

            if (_videoBuffer != null)
            {
                _videoBuffer.ClearFramePixels();
                _videoBuffer = null;
            }

            if (_playerObj != IntPtr.Zero)
                _wrapper.PlayerRelease(_playerObj);
            _playerObj = IntPtr.Zero;

            if (_vlcObj != IntPtr.Zero)
                _wrapper.PlayerExpandedLibVLCRelease(_vlcObj);
            _vlcObj = IntPtr.Zero;
        }

        public Uri DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                {
                    _dataSource = value;
                    if (File.Exists(Application.streamingAssetsPath + _dataSource.AbsolutePath))
                        _dataSource = new Uri(Application.streamingAssetsPath + _dataSource.AbsolutePath);

                    if (_mediaObj != IntPtr.Zero)
                        _wrapper.PlayerExpandedMediaRelease(_mediaObj);

                    _mediaObj = _wrapper.PlayerExpandedMediaNewLocation(_vlcObj, _dataSource.AbsoluteUri);

                    if (_playerArguments != null)
                    {
                        foreach (string option in _playerArguments)
                        {
                            if (option.Contains(":"))
                                _wrapper.PlayerExpandedAddOption(_mediaObj, option);
                        }
                    }

                    _wrapper.PlayerExpandedSetMedia(_playerObj, _mediaObj);
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerIsPlaying(_playerObj);

                return false;
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
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerWillPlay(_playerObj);

                return false;
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
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerGetLength(_playerObj);

                return 0;
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

        public byte[] FramePixels
        {
            get
            {
                if (_videoBuffer != null)
                    return _videoBuffer.FramePixels;

                return new byte[] { };
            }
        }

        public int FramesCounter
        {
            get { return _wrapper.NativeHelperGetFramesCounter((IntPtr)_playerIndex); }
        }

        public long Time
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerGetTime(_playerObj);

                return 0;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                    _wrapper.PlayerSetTime(_playerObj, value);

                _isAudioDataReady = false;

                if (_soundBuffer != null)
                    _soundBuffer.SamplesData.Clear();

                _audioSamplesOffset = 0;
            }
        }

        public float Position
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerGetPosition(_playerObj);

                return 0;
            }
            set
            {
                if (_audioManager != null)
                    _audioManager.Stop();

                if (_playerObj != IntPtr.Zero)
                    _wrapper.PlayerSetPosition(_playerObj, value);

                _isAudioDataReady = false;

                if (_soundBuffer != null)
                    _soundBuffer.SamplesData.Clear();

                _audioSamplesOffset = 0;
                _wrapper.NativeHelperUpdateFramesCounter((IntPtr)_playerIndex, FramesAmount > 0 ? (int)(value * FramesAmount) : 0);
            }
        }

        public float PlaybackRate
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerGetRate(_playerObj);

                return 0;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                {
                    if (value > 4)
                        _audioManager.Stop();

                    bool res = _wrapper.PlayerSetRate(_playerObj, value);
                    if (!res)
                    {
                        throw new Exception("Native library problem: can't change playback rate");
                    }
                }
            }
        }

        public int Volume
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                {
                    if (_audioManager != null && _audioManager.IsValid)
                        return (int)(_audioManager.Volume * 100f);

                    return _wrapper.PlayerGetVolume(_playerObj);
                }

                return 0;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                {
                    if (_audioManager != null && _audioManager.IsValid)
                        _audioManager.Volume = value / 100f;
                    else
                        _wrapper.PlayerSetVolume(_playerObj, value);
                }
            }
        }

        public bool Mute
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                {
                    if (_audioManager != null && _audioManager.IsValid)
                        return _audioManager.Mute;

                    return _wrapper.PlayerGetMute(_playerObj);
                }

                return false;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                {
                    if (_audioManager != null && _audioManager.IsValid)
                        _audioManager.Mute = value;
                    else
                        _wrapper.PlayerSetMute(_playerObj, value);
                }
            }
        }

        public int VideoWidth
        {
            get
            {
                var width = 0;
                if (_playerObj != IntPtr.Zero)
                {
                    width = _wrapper.PlayerVideoWidth(_playerObj);

                    if (width <= 0 || _isFixedSize)
                        width = _videoBuffer.Width;
                }

                return width;
            }
        }

        public int VideoHeight
        {
            get
            {
                var height = 0;

                if (_playerObj != IntPtr.Zero)
                {
                    height = _wrapper.PlayerVideoHeight(_playerObj);

                    if (height <= 0 || _isFixedSize)
                        height = _videoBuffer.Height;
                }

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

        /// <summary>
        /// Get available audio tracks.
        /// </summary>
        public MediaTrackInfo[] AudioTracks
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                {
                    int traksCount = _wrapper.PlayerAudioGetTrackCount(_playerObj);
                    IntPtr tracksList = _wrapper.PlayerAudioGetTrackDescription(_playerObj);

                    var resultTracks = GetTracks(traksCount, tracksList);

                    //if (tracksList != IntPtr.Zero)
                    //    _libVLC.Libvlc_track_description_release(tracksList);

                    return resultTracks;
                }

                return null;
            }
        }

        /// <summary>
        /// Get/Set current audio track.
        /// </summary>
        public MediaTrackInfo AudioTrack
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                {
                    int id = _wrapper.PlayerAudioGetTrack(_playerObj);

                    if (id == -1)
                        return null;

                    return AudioTracks.Where(t => t.Id == id).SingleOrDefault();
                }

                return null;
            }
            set
            {
                int status = _wrapper.PlayerAudioSetTrack(_playerObj, value.Id);
                if (status == -1)
                    throw new Exception("Native library problem: can't set new audio track");
            }
        }

        /// <summary>
        /// Get available spu tracks.
        /// </summary>
        public MediaTrackInfo[] SpuTracks
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                {
                    int traksCount = _wrapper.PlayerSpuGetCount(_playerObj);
                    IntPtr tracksList = _wrapper.PlayerSpuGetDescription(_playerObj);

                    var resultTracks = GetTracks(traksCount, tracksList);

                    //if (tracksList != IntPtr.Zero)
                    //    _libVLC.Libvlc_track_description_release(tracksList);

                    return resultTracks;
                }

                return null;
            }
        }

        /// <summary>
        /// Get/Set current spu track.
        /// </summary>
        public MediaTrackInfo SpuTrack
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                {
                    int id = _wrapper.PlayerSpuGet(_playerObj);

                    if (id == -1)
                        return null;

                    return AudioTracks.Where(t => t.Id == id).SingleOrDefault();
                }

                return null;
            }
            set
            {
                int status = _wrapper.PlayerSpuSet(_playerObj, value.Id);
                if (status == -1)
                    throw new Exception("Native library problem: can't set new spu track");
            }
        }

        /// <summary>
        /// Set new video subtitle file
        /// </summary>
        /// <param name="path">Path to the new video subtitle file</param>
        /// <returns></returns>
        public bool SetSubtitleFile(Uri path)
        {
            if (_playerObj != IntPtr.Zero)
                return _wrapper.PlayerSpuSetFile(_playerObj, path.AbsolutePath) == 1;

            return false;
        }

        public MediaTrackInfoExpanded[] TracksInfo
        {
            get
            {
                if (_mediaObj != IntPtr.Zero)
                {
                    var tracks = _wrapper.PlayerExpandedMediaGetTracksInfo(_mediaObj);

                    if (tracks != null && tracks.Length > 0)
                    {
                        var result = new List<MediaTrackInfoExpanded>(tracks.Length);
                        foreach (var trackInfo in tracks)
                        {
                            switch (trackInfo.Type)
                            {
                                case TrackTypes.Unknown:
                                    result.Add(new MediaTrackInfoUnknown(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level));
                                    break;

                                case TrackTypes.Video:
                                    result.Add(new MediaTrackInfoVideo(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level, trackInfo.Video.Width, trackInfo.Video.Height));
                                    break;

                                case TrackTypes.Audio:
                                    result.Add(new MediaTrackInfoAudio(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level, trackInfo.Audio.Channels, trackInfo.Audio.Rate));
                                    break;

                                case TrackTypes.Text:
                                    result.Add(new MediaTrackInfoSpu(trackInfo.Codec, trackInfo.Id, trackInfo.Profile, trackInfo.Level));
                                    break;
                            }
                        }
                        return result.ToArray();
                    }
                }

                return null;
            }
        }

        public long AudioDelay
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerExpandedGetAudioDelay(_playerObj);

                return 0;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                    _wrapper.PlayerExpandedSetAudioDelay(_playerObj, value);
            }
        }

        public float VideoScale
        {
            get
            {
                if (_playerObj != IntPtr.Zero)
                    return _wrapper.PlayerVideoGetScale(_playerObj);

                return 0;
            }
            set
            {
                if (_playerObj != IntPtr.Zero)
                    _wrapper.PlayerVideoSetScale(_playerObj, value);
            }
        }

        #region Platform dependent functionality
        public string LogMessage
        {
            get
            {
                if (_vlcObj != IntPtr.Zero)
                    return _wrapper.NativeHelperGetLogMessage((IntPtr)_playerIndex);

                return null;
            }
        }

        public int LogLevel
        {
            get
            {
                if (_vlcObj != IntPtr.Zero)
                    return _wrapper.NativeHelperGetLogLevel((IntPtr)_playerIndex);

                return -1;
            }
        }

        public float FrameRateStable
        {
            get
            {
                if (_playerObj != IntPtr.Zero && IsReady)
                    return _wrapper.PlayerVideoFrameRate(_playerObj);

                return 0;
            }
        }

        public int FramesAmount
        {
            get
            {
                if (_playerObj != IntPtr.Zero && IsReady)
                    return (int)(Length * FrameRateStable * 0.001f);

                return 0;
            }
        }

        public void TakeSnapShot(string path)
        {
            if (_playerObj != IntPtr.Zero)
                _wrapper.PlayerVideoTakeSnapshot(_playerObj, 0, path, 0, 0);
        }

        public string GetLastError()
        {
            if (_logManager != null)
                return _logManager.LastError;

            return string.Empty;
        }
        #endregion
        #endregion
    }
}