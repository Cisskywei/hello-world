using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UMP;

public class UniversalMediaPlayer : MonoBehaviour, IMediaListener, IPathPreparedListener, IPlayerPreparedListener, IPlayerTimeChangedListener, IPlayerPositionChangedListener, IPlayerSnapshotTakenListener
{
    private const float DEFAULT_POSITION_CHANGED_OFFSET = 0.05f;

    #region Editor Visible Properties
    [SerializeField]
    private GameObject[] _renderingObjects;

    [SerializeField]
    private string _path;

    [SerializeField]
    private bool _autoPlay;

    [SerializeField]
    private bool _loop;

    [SerializeField]
    private bool _loopSmooth;

    [SerializeField]
    private bool _mute;

    [SerializeField]
    private int _volume = 50;

    [SerializeField]
    private float _playRate = 1;

    [SerializeField]
    private float _position;

    [SerializeField]
    private int _fixedVideoWidth = -1;

    [SerializeField]
    private int _fixedVideoHeight = -1;

    #region Desktop Options
    [SerializeField]
    private AudioSource[] _audioSourcesDesktop;

    [SerializeField]
    private PlayerOptions.State _hardwareDecodingDesktop = PlayerOptions.State.Default;

    [SerializeField]
    private bool _flipVerticallyDesktop = true;

    [SerializeField]
    private bool _outputToFileDesktop = false;

    [SerializeField]
    private bool _displayOutputDesktop = false;

    [SerializeField]
    private string _outputFilePathDesktop;

    [SerializeField]
    private bool _rtspOverTcpDesktop = false;

    [SerializeField]
    private int _fileCachingDesktop = 300;

    [SerializeField]
    private int _liveCachingDesktop = 300;

    [SerializeField]
    private int _diskCachingDesktop = 300;

    [SerializeField]
    private int _networkCachingDesktop = 300;
    #endregion

    #region Android Options
    [SerializeField]
    private PlayerOptions.State _hardwareDecodingAndroid = PlayerOptions.State.Default;

    [SerializeField]
    private bool _playInBackgroundAndroid = false;

    [SerializeField]
    private bool _rtspOverTcpAndroid = false;

    [SerializeField]
    private int _fileCachingAndroid = 300;

    [SerializeField]
    private int _liveCachingAndroid = 300;

    [SerializeField]
    private int _diskCachingAndroid = 300;

    [SerializeField]
    private int _networkCachingAndroid = 300;
    #endregion

    #region IPhone Options
    [SerializeField]
    private bool _videoToolboxIPhone = true;

    [SerializeField]
    private int _videoToolboxMaxFrameWidthIPhone = 4096;

    [SerializeField]
    private bool _videoToolboxAsyncIPhone = false;

    [SerializeField]
    private bool _videoToolboxWaitAsyncIPhone = true;

    [SerializeField]
    private bool _playInBackgroundIPhone = false;

    [SerializeField]
    private bool _rtspOverTcpIPhone = false;

    [SerializeField]
    private bool _packetBufferingIPhone = true;

    [SerializeField]
    private int _maxBufferSizeIPhone = 15 * 1024 * 1024;

    [SerializeField]
    private int _minFramesIPhone = 50000;

    [SerializeField]
    private bool _infbufIPhone = false;

    [SerializeField]
    private int _framedropIPhone = 0;

    [SerializeField]
    private int _maxFpsIPhone = 31;
    #endregion

    [SerializeField]
    private LogDetail _logDetail = LogDetail.Disable;

#pragma warning disable 0414
    [SerializeField]
    private bool _useAdvanced = false;

    [SerializeField]
    private bool _useFixedSize = false;

    [SerializeField]
    private int _chosenPlatform = 0;

    [SerializeField]
    private string _lastEventMsg = string.Empty;
#pragma warning restore 0414

    [SerializeField]
    private bool _isParsing;

    [Serializable]
    private class EventTextType : UnityEvent<string> { }

    [Serializable]
    private class EventFloatType : UnityEvent<float> { }

    [Serializable]
    private class EventLongType : UnityEvent<long> { }

    [Serializable]
    private class EventTextureType : UnityEvent<Texture2D> { }

    [SerializeField]
    private EventTextType _pathPreparedEvent = new EventTextType();

    [SerializeField]
    private UnityEvent _openingEvent = new UnityEvent();

    [SerializeField]
    private EventFloatType _bufferingEvent = new EventFloatType();

    [SerializeField]
    private EventTextureType _preparedEvent = new EventTextureType();

    [SerializeField]
    private UnityEvent _playingEvent = new UnityEvent();

    [SerializeField]
    private UnityEvent _pausedEvent = new UnityEvent();

    [SerializeField]
    private UnityEvent _stoppedEvent = new UnityEvent();

    [SerializeField]
    private UnityEvent _endReachedEvent = new UnityEvent();

    [SerializeField]
    private UnityEvent _encounteredErrorEvent = new UnityEvent();

    [SerializeField]
    private EventLongType _timeChangedEvent = new EventLongType();

    [SerializeField]
    private EventFloatType _positionChangedEvent = new EventFloatType();

    [SerializeField]
    private EventTextType _snapshotTakenEvent = new EventTextType();
    #endregion

    #region Properties
    public GameObject[] RenderingObjects
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.VideoOutputObjects;
            return null;
        }
        set
        {
            if (_mediaPlayer != null)
                _mediaPlayer.VideoOutputObjects = value;

            _renderingObjects = value;
        }
    }

    public AudioSource[] AudioObjects
    {
        get
        {
            return _audioSourcesDesktop;
        }
    }

    public object PlatformPlayer
    {
        get
        {
            return _mediaPlayer != null ? _mediaPlayer.PlatformPlayer : null;
        }
    }

    public string Path
    {
        set { _path = value; }
        get { return _path; }
    }

    public bool AutoPlay
    {
        set { _autoPlay = value; }
        get { return _autoPlay; }
    }

    public bool Loop
    {
        set { _loop = value; }
        get { return _loop; }
    }

    public bool Mute
    {
        set
        {
            _mediaPlayer.Mute = value;
            _mute = value;
        }
        get { return _mediaPlayer.Mute; }
    }

    public float Volume
    {
        set
        {
            _mediaPlayer.Volume = (int)value;
            _volume = (int)value;
        }
        get { return _mediaPlayer.Volume; }
    }

    public float Position
    {
        set { _mediaPlayer.Position = value; }
        get { return _mediaPlayer.Position; }
    }

    public long Time
    {
        set
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Time = value;
        }
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.Time;
            return -1;
        }
    }

    public float PlayRate
    {
        set
        {
            _mediaPlayer.PlaybackRate = value;
            _playRate = value;
        }
        get { return _mediaPlayer.PlaybackRate; }
    }

    public bool AbleToPlay
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.AbleToPlay;
            return false;
        }
    }

    public bool IsPlaying
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.IsPlaying;
            return false;
        }
    }

    public bool IsReady
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.IsReady;
            return false;
        }
    }

    public bool IsParsing
    {
        get
        {
            return _isParsing;
        }
    }

    public float FrameRate
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.FrameRate;
            return 0;
        }
    }

    public long FramesCounter
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.FramesCounter;
            return 0;
        }
    }

    public byte[] FramePixels
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.FramePixels;
            return null;
        }
    }

    public long Length
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.Length;
            return 0;
        }
    }

    public int VideoWidth
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.VideoWidth;
            return 0;
        }
    }

    public int VideoHeight
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.VideoHeight;
            return 0;
        }
    }

    public Vector2 VideoSize
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.VideoSize;
            return new Vector2(0, 0);
        }
    }

    public MediaTrackInfo AudioTrack
    {
        set
        {
            if (_mediaPlayer != null)
                _mediaPlayer.AudioTrack = value;
        }
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.AudioTrack;
            return null;
        }
    }

    public MediaTrackInfo[] AudioTracks
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.AudioTracks;
            return null;
        }
    }

    public MediaTrackInfo SpuTrack
    {
        set
        {
            if (_mediaPlayer != null)
                _mediaPlayer.SpuTrack = value;
        }
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.SpuTrack;
            return null;
        }
    }

    public MediaTrackInfo[] SpuTracks
    {
        get
        {
            if (_mediaPlayer != null)
                return _mediaPlayer.SpuTracks;
            return null;
        }
    }

    public string LastError
    {
        get
        {
            if (_mediaPlayer.PlatformPlayer is MediaPlayerStandalone)
                return (_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).GetLastError();
            return string.Empty;
        }
    }
    #endregion

    private MediaPlayer _mediaPlayer;
    private MediaPlayer _mediaPlayerLoop;
    private VideoHostingParser _videoHostingParser;
    private PlayerManagerLogs _logManager;
    private string _tmpPath = string.Empty;
    private static Dictionary<string, string> _cachedVideoPaths;
    private bool _cachedPlayState;
    private IEnumerator _videoPathPreparingEnum;

#pragma warning disable 0414
    private bool _isFirstEditorStateChange = true;
#pragma warning restore 0414

    private void Awake()
    {
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged += HandleOnPlayModeChanged;
#endif

        if (UMPSettings.Instance.UseAudioSource && (_audioSourcesDesktop == null || _audioSourcesDesktop.Length <= 0))
        {
            var audioOutput = gameObject.AddComponent<AudioSource>();
            _audioSourcesDesktop = new AudioSource[] { audioOutput };
        }
        
        PlayerOptions options = new PlayerOptions(null);

        switch (UMPSettings.SupportedPlatform)
        {
            case UMPSettings.Platforms.Win:
            case UMPSettings.Platforms.Mac:
            case UMPSettings.Platforms.Linux:
                if (UMPSettings.EditorBitMode == UMPSettings.BitModes.x86 && _hardwareDecodingDesktop == PlayerOptions.State.Default)
                    _hardwareDecodingDesktop = PlayerOptions.State.Enable;

                var winOptions = new PlayerOptionsStandalone(null)
                {
                    FixedVideoSize = _useFixedSize ? new Vector2(_fixedVideoWidth, _fixedVideoHeight) : Vector2.zero,
                    AudioOutputSources = _audioSourcesDesktop,
                    HardwareDecoding = _hardwareDecodingDesktop,
                    FlipVertically = _flipVerticallyDesktop,
                    UseTCP = _rtspOverTcpDesktop,
                    FileCaching = _fileCachingDesktop,
                    LiveCaching = _liveCachingDesktop,
                    DiskCaching = _diskCachingDesktop,
                    NetworkCaching = _networkCachingDesktop
                };

                if (_outputToFileDesktop)
                    winOptions.RedirectToFile(_displayOutputDesktop, _outputFilePathDesktop);

                options = winOptions;
                break;

            case UMPSettings.Platforms.Android:
                var androidOptions = new PlayerOptionsAndroid(null)
                {
                    FixedVideoSize = _useFixedSize ? new Vector2(_fixedVideoWidth, _fixedVideoHeight) : Vector2.zero,
                    HardwareDecoding = _hardwareDecodingAndroid,
                    PlayInBackground = _playInBackgroundAndroid,
                    UseTCP = _rtspOverTcpAndroid,
                    FileCaching = _fileCachingAndroid,
                    LiveCaching = _liveCachingAndroid,
                    DiscCaching = _diskCachingAndroid,
                    NetworkCaching = _networkCachingAndroid
                };

                options = androidOptions;
                break;

            case UMPSettings.Platforms.iOS:
                var iphoneOptions = new PlayerOptionsIPhone(null)
                {
                    FixedVideoSize = _useFixedSize ? new Vector2(_fixedVideoWidth, _fixedVideoHeight) : Vector2.zero,
                    VideoToolbox = _videoToolboxIPhone,
                    VideoToolboxFrameWidth = _videoToolboxMaxFrameWidthIPhone,
                    VideoToolboxAsync = _videoToolboxAsyncIPhone,
                    VideoToolboxWaitAsync = _videoToolboxWaitAsyncIPhone,
                    PlayInBackground = _playInBackgroundIPhone,
                    UseTCP = _rtspOverTcpIPhone,
                    PacketBuffering = _packetBufferingIPhone,
                    MaxBufferSize = _maxBufferSizeIPhone,
                    MinFrames = _minFramesIPhone,
                    Infbuf = _infbufIPhone,
                    Framedrop = _framedropIPhone,
                    MaxFps = _maxFpsIPhone
                };

                options = iphoneOptions;
                break;
        }

        _mediaPlayer = new MediaPlayer(this, _renderingObjects, options);

#if UNITY_EDITOR
        if (_mediaPlayer.PlatformPlayer is MediaPlayerStandalone)
        {
            _logManager = (_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).LogManager;
            if (_logManager != null)
            {
                // Set delegate for LogManager to show native library logging in Unity console
                _logManager.LogMessageListener += UnityConsoleLogging;
                // Set debugging level
                _logManager.LogDetail = _logDetail;
            }
        }
#endif
        if (_cachedVideoPaths == null)
            _cachedVideoPaths = new Dictionary<string, string>();

        // Create scpecial parser to add possibiity of get video link from different video hosting servies (like youtube)
        _videoHostingParser = new VideoHostingParser(this);
        // Attach scecial listeners to MediaPlayer instance
        AddListeners();
        // Create additional media player for add smooth loop possibility
        if (_loopSmooth)
        {
            _mediaPlayerLoop = new MediaPlayer(this, _mediaPlayer);
            _mediaPlayerLoop.VideoOutputObjects = null;
            _mediaPlayerLoop.EventManager.RemoveAllEvents();
        }
    }

    #region Editor Additional Possibility
#if UNITY_EDITOR
    private void HandleOnPlayModeChanged()
    {
        if (_isFirstEditorStateChange)
        {
            _isFirstEditorStateChange = false;
            return;
        }

        if (_mediaPlayer == null)
            return;

        if (EditorApplication.isPaused)
        {
            _cachedPlayState = _mediaPlayer.IsPlaying;
            Pause();
        }
        else
        {
            if (!isActiveAndEnabled)
            {
                Stop();
                return;
            }

            if (_cachedPlayState)
            {
                _mediaPlayer.Play();
            }
            else
            {
                Pause();
            }
        }
    }
#endif
    #endregion

    private void Start()
    {
        if (!_autoPlay)
            return;

        Play();
    }

    private void OnValidate()
    {
        if (_mediaPlayer != null && _mediaPlayer.IsReady)
        {
            if (_mediaPlayer.Mute != _mute)
                _mediaPlayer.Mute = _mute;

            if (_mediaPlayer.Volume != _volume)
                _mediaPlayer.Volume = _volume;

            if (_mediaPlayer.PlaybackRate != _playRate)
                _mediaPlayer.PlaybackRate = _playRate;

#if UNITY_EDITOR
            if (_logManager != null)
                _logManager.LogDetail = _logDetail;
#endif
            if (_position > _mediaPlayer.Position + DEFAULT_POSITION_CHANGED_OFFSET ||
                _position < _mediaPlayer.Position - DEFAULT_POSITION_CHANGED_OFFSET)
            {
                _mediaPlayer.Position = _position;
            }
        }
    }

    public void OnDisable()
    {
        if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
        {
            Stop();
        }
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (EditorApplication.playmodeStateChanged != null)
        {
            EditorApplication.playmodeStateChanged -= HandleOnPlayModeChanged;
            EditorApplication.playmodeStateChanged = null;
        }
#endif

        if (_mediaPlayer != null)
        {
            // Release MediaPlayer
            Release();
        }
    }

    private void AddListeners()
    {
        if (_mediaPlayer == null || _mediaPlayer.EventManager == null)
            return;

        // Add to MediaPlayer new main group of listeners
        _mediaPlayer.AddMediaListener(this);
        // Add to MediaPlayer new "OnPlayerTextureCreated" listener
        _mediaPlayer.EventManager.PlayerPreparedListener += OnPlayerPrepared;
        // Add to MediaPlayer new "OnPlayerTimeChanged" listener
        _mediaPlayer.EventManager.PlayerTimeChangedListener += OnPlayerTimeChanged;
        // Add to MediaPlayer new "OnPlayerPositionChanged" listener
        _mediaPlayer.EventManager.PlayerPositionChangedListener += OnPlayerPositionChanged;
        // Add to MediaPlayer new "OnPlayerSnapshotTaken" listener
        _mediaPlayer.EventManager.PlayerSnapshotTakenListener += OnPlayerSnapshotTaken;
    }

    private void RemoveListeners()
    {
        if (_mediaPlayer == null)
            return;

        // Remove from MediaPlayer the main group of listeners
        _mediaPlayer.RemoveMediaListener(this);
        // Remove from MediaPlayer "OnPlayerTextureCreated" listener
        _mediaPlayer.EventManager.PlayerPreparedListener -= OnPlayerPrepared;
        // Remove from MediaPlayer "OnPlayerTimeChanged" listener
        _mediaPlayer.EventManager.PlayerTimeChangedListener -= OnPlayerTimeChanged;
        // Remove from MediaPlayer "OnPlayerPositionChanged" listener
        _mediaPlayer.EventManager.PlayerPositionChangedListener -= OnPlayerPositionChanged;
        // Remove from MediaPlayer new "OnPlayerSnapshotTaken" listener
        _mediaPlayer.EventManager.PlayerSnapshotTakenListener -= OnPlayerSnapshotTaken;
    }

    private IEnumerator VideoPathPreparing(string path, bool isPreparing, IPathPreparedListener listener)
    {
#if UNITY_EDITOR
        _lastEventMsg = "Path Preparing";
#endif

        if (_cachedVideoPaths.ContainsKey(path))
        {
            listener.OnPathPrepared(_cachedVideoPaths[path], isPreparing);
            yield break;
        }

        if (!UMPSettings.Instance.UseAndroidNative && Application.platform == RuntimePlatform.Android && path.StartsWith("file"))
        {
            var fileName = path.Replace("file:///", "");
            WWW www = new WWW(System.IO.Path.Combine(Application.streamingAssetsPath, fileName));
            yield return www;
            
            if (string.IsNullOrEmpty(www.error))
            {
                var tempFilePath = System.IO.Path.Combine(Application.temporaryCachePath, fileName);
                File.WriteAllBytes(tempFilePath, www.bytes);
                _cachedVideoPaths.Add(path, tempFilePath);
                path = tempFilePath;
            }
            else
            {
                Debug.Log("Can't create temp file from asset folder: " + www.error);
            }
            www.Dispose();
        }

        if (_videoHostingParser.IsVideoHostingUrl(path))
        {
            var videoInfos = _videoHostingParser.GetCachedVideoInfos(path);

            if (videoInfos == null)
            {
                var isError = false;
                _isParsing = true;

                _videoHostingParser.ParseVideoInfos(path, (res) =>
                {
                    _isParsing = false;
                }, (error) =>
                {
                    isError = true;
                    Debug.LogError(error);
                });

                while (_isParsing)
                {
                    if (isError)
                    {
                        _isParsing = false;
                        yield break;
                    }

                    yield return null;
                }

                videoInfos = _videoHostingParser.GetCachedVideoInfos(path);
            }

            var videoInfo = _videoHostingParser.GetBestCompatibleVideo(videoInfos);
            var directLink = videoInfo.DownloadUrl;

            if (videoInfo.RequiresDecryption && !videoInfo.IsDecrypted)
            {
                var isDecrypted = false;
                _videoHostingParser.DecryptVideoUrl(videoInfo, (res) =>
                {
                    directLink = videoInfo.DownloadUrl;
                    isDecrypted = true;
                });

                while (!isDecrypted)
                {
                    yield return null;
                }
            }
            _cachedVideoPaths.Add(path, directLink);
            path = directLink;
        }

        listener.OnPathPrepared(path, isPreparing);
        yield return null;
    }

    public void Prepare()
    {
        if (_videoPathPreparingEnum != null)
            StopCoroutine(_videoPathPreparingEnum);

        _videoPathPreparingEnum = VideoPathPreparing(_path, true, this);
        StartCoroutine(_videoPathPreparingEnum);
    }

    public void Play()
    {
        if (_videoPathPreparingEnum != null)
            StopCoroutine(_videoPathPreparingEnum);

        _videoPathPreparingEnum = VideoPathPreparing(_path, false, this);
        StartCoroutine(_videoPathPreparingEnum);
    }

    public void Pause()
    {
        if (_mediaPlayer == null)
            return;

        if (_mediaPlayer.IsPlaying)
            _mediaPlayer.Pause();
    }

    public void Stop()
    {
        Stop(true);
    }

    public void Stop(bool clearVideoTexture)
    {
#if UNITY_EDITOR
        if (EditorApplication.isPaused)
            return;
#endif
        if (_videoPathPreparingEnum != null)
            StopCoroutine(_videoPathPreparingEnum);

        _position = 0;

        if (_mediaPlayer == null)
            return;

        _mediaPlayer.Stop(clearVideoTexture);

        if (_mediaPlayerLoop != null)
            _mediaPlayerLoop.Stop(clearVideoTexture);
    }

    public void Release()
    {
#if UNITY_EDITOR
        if (EditorApplication.playmodeStateChanged != null)
        {
            EditorApplication.playmodeStateChanged -= HandleOnPlayModeChanged;
            EditorApplication.playmodeStateChanged = null;
        }
#endif
        Stop();

        if (_mediaPlayer != null)
        {
            // Release MediaPlayer
            _mediaPlayer.Release();
            _mediaPlayer = null;

            if (_mediaPlayerLoop != null)
                _mediaPlayerLoop.Release();

            if (_videoHostingParser != null)
                _videoHostingParser.Release();

            RemoveListeners();

            _openingEvent.RemoveAllListeners();
            _bufferingEvent.RemoveAllListeners();
            _preparedEvent.RemoveAllListeners();
            _playingEvent.RemoveAllListeners();
            _pausedEvent.RemoveAllListeners();
            _stoppedEvent.RemoveAllListeners();
            _endReachedEvent.RemoveAllListeners();
            _encounteredErrorEvent.RemoveAllListeners();
            _timeChangedEvent.RemoveAllListeners();
            _positionChangedEvent.RemoveAllListeners();
            _snapshotTakenEvent.RemoveAllListeners();
        }
    }

    public string GetFormattedLength(bool detail)
    {
        if (_mediaPlayer != null)
            return _mediaPlayer.GetFormattedLength(detail);
        return string.Empty;
    }

    public void Snapshot(string path)
    {
#if UNITY_EDITOR
        if (EditorApplication.isPaused)
            return;
#endif

        if (_mediaPlayer == null)
            return;

        if (_mediaPlayer.AbleToPlay)
        {
            if (_mediaPlayer.PlatformPlayer is MediaPlayerStandalone)
                (_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).TakeSnapShot(path);
#if UNITY_EDITOR
            Debug.Log("Snapshot path: " + path);
#endif
        }
    }

    private void UnityConsoleLogging(PlayerManagerLogs.PlayerLog args)
    {
        if (args.Level != _logDetail)
            return;

        Debug.Log(args.Level.ToString() + ": " + args.Message);
    }

    public void OnPathPrepared(string path, bool isPreparing)
    {
#if UNITY_EDITOR
        if (EditorApplication.isPaused)
            return;

        _lastEventMsg = "Path Prepared";
#endif

        _mediaPlayer.Mute = _mute;
        _mediaPlayer.Volume = _volume;
        _mediaPlayer.PlaybackRate = _playRate;

        if (!_path.Equals(_tmpPath))
        {
            if (IsPlaying)
                Stop();
            _tmpPath = _path;

            _mediaPlayer.DataSource = new Uri(path);
        }

        if (isPreparing)
            _mediaPlayer.Prepare();
        else
        {
            _mediaPlayer.Play();

            if (_mediaPlayerLoop != null && !_mediaPlayerLoop.IsReady)
            {
                _mediaPlayerLoop.DataSource = _mediaPlayer.DataSource;
                _mediaPlayerLoop.Prepare();
            }
        }

        if (_pathPreparedEvent != null)
            _pathPreparedEvent.Invoke(path);
    }

    public void AddPathPreparedEvent(UnityAction<string> action)
    {
        _pathPreparedEvent.AddListener(action);
    }

    public void RemovePathPreparedEvent(UnityAction<string> action)
    {
        _pathPreparedEvent.RemoveListener(action);
    }

    public void OnPlayerOpening()
    {
#if UNITY_EDITOR
        _lastEventMsg = "Opening";
#endif
        if (_openingEvent != null)
            _openingEvent.Invoke();
    }

    public void AddOpeningEvent(UnityAction action)
    {
        _openingEvent.AddListener(action);
    }

    public void RemoveOpeningEvent(UnityAction action)
    {
        _openingEvent.RemoveListener(action);
    }

    public void OnPlayerBuffering(float percentage)
    {
#if UNITY_EDITOR
        _lastEventMsg = "Buffering: " + percentage;
#endif
        if (_bufferingEvent != null)
            _bufferingEvent.Invoke(percentage);
    }

    public void AddBufferingEvent(UnityAction<float> action)
    {
        _bufferingEvent.AddListener(action);
    }

    public void RemoveBufferingEvent(UnityAction<float> action)
    {
        _bufferingEvent.RemoveListener(action);
    }

    public void OnPlayerPrepared(Texture2D videoTexture)
    {
#if UNITY_EDITOR
        _lastEventMsg = "Prepared";
#endif
        if (_preparedEvent != null)
            _preparedEvent.Invoke(videoTexture);
    }

    public void AddPreparedEvent(UnityAction<Texture2D> action)
    {
        _preparedEvent.AddListener(action);
    }

    public void RemovePreparedEvent(UnityAction<Texture2D> action)
    {
        _preparedEvent.RemoveListener(action);
    }

    public void OnPlayerPlaying()
    {
#if UNITY_EDITOR
        _lastEventMsg = "Playing";
#endif
        if (_playingEvent != null)
            _playingEvent.Invoke();
    }

    public void AddPlayingEvent(UnityAction action)
    {
        _playingEvent.AddListener(action);
    }

    public void RemovePlayingEvent(UnityAction action)
    {
        _playingEvent.RemoveListener(action);
    }

    public void OnPlayerPaused()
    {
#if UNITY_EDITOR
        _lastEventMsg = "Paused";
#endif
        if (_pausedEvent != null)
            _pausedEvent.Invoke();
    }

    public void AddPausedEvent(UnityAction action)
    {
        _pausedEvent.AddListener(action);
    }

    public void RemovePausedEvent(UnityAction action)
    {
        _pausedEvent.RemoveListener(action);
    }

    public void OnPlayerStopped()
    {
#if UNITY_EDITOR
        if (!_lastEventMsg.Contains("Error"))
            _lastEventMsg = "Stopped";
#endif
        if (_stoppedEvent != null)
            _stoppedEvent.Invoke();
    }

    public void AddStoppedEvent(UnityAction action)
    {
        _stoppedEvent.AddListener(action);
    }

    public void RemoveStoppedEvent(UnityAction action)
    {
        _stoppedEvent.RemoveListener(action);
    }

    public void OnPlayerEndReached()
    {
#if UNITY_EDITOR
        _lastEventMsg = "End";
#endif

        if (_endReachedEvent != null)
            _endReachedEvent.Invoke();

        _position = 0;
        _mediaPlayer.Stop(!_loop);

        if (_loop)
        {
            if (_mediaPlayerLoop != null)
            {
                _mediaPlayerLoop.EventManager.CopyPlayerEvents(_mediaPlayer.EventManager);
                _mediaPlayerLoop.VideoOutputObjects = _mediaPlayer.VideoOutputObjects;
                _mediaPlayer.VideoOutputObjects = null;
                _mediaPlayer.EventManager.RemoveAllEvents();

                var tempPlayer = _mediaPlayer;
                _mediaPlayer = _mediaPlayerLoop;
                _mediaPlayerLoop = tempPlayer;
            }

            if (!string.IsNullOrEmpty(_path))
                Play();
        }
    }

    public void AddEndReachedEvent(UnityAction action)
    {
        _endReachedEvent.AddListener(action);
    }

    public void RemoveEndReachedEvent(UnityAction action)
    {
        _endReachedEvent.RemoveListener(action);
    }

    public void OnPlayerEncounteredError()
    {
#if UNITY_EDITOR
        _lastEventMsg = "Error (" + (_mediaPlayer.PlatformPlayer as MediaPlayerStandalone).GetLastError() + ")";
#endif
        Stop();

        if (_encounteredErrorEvent != null)
            _encounteredErrorEvent.Invoke();
    }

    public void AddEncounteredErrorEvent(UnityAction action)
    {
        _encounteredErrorEvent.AddListener(action);
    }

    public void RemoveEncounteredErrorEvent(UnityAction action)
    {
        _encounteredErrorEvent.RemoveListener(action);
    }

    public void OnPlayerTimeChanged(long time)
    {
#if UNITY_EDITOR
        _lastEventMsg = "TimeChanged";
#endif

        if (_timeChangedEvent != null)
            _timeChangedEvent.Invoke(time);
    }

    public void AddTimeChangedEvent(UnityAction<long> action)
    {
        _timeChangedEvent.AddListener(action);
    }

    public void RemoveTimeChangedEvent(UnityAction<long> action)
    {
        _timeChangedEvent.RemoveListener(action);
    }

    public void OnPlayerPositionChanged(float position)
    {
#if UNITY_EDITOR
        _lastEventMsg = "PositionChanged";
#endif
        _position = _mediaPlayer.Position;

        if (_positionChangedEvent != null)
            _positionChangedEvent.Invoke(position);
    }

    public void AddPositionChangedEvent(UnityAction<float> action)
    {
        _positionChangedEvent.AddListener(action);
    }

    public void RemovePositionChangedEvent(UnityAction<float> action)
    {
        _positionChangedEvent.RemoveListener(action);
    }

    public void OnPlayerSnapshotTaken(string path)
    {
        if (_snapshotTakenEvent != null)
            _snapshotTakenEvent.Invoke(path);
    }

    public void AddSnapshotTakenEvent(UnityAction<string> action)
    {
        _snapshotTakenEvent.AddListener(action);
    }

    public void RemoveSnapshotTakenEvent(UnityAction<string> action)
    {
        _snapshotTakenEvent.RemoveListener(action);
    }
}
