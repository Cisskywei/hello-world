using System;
using UnityEditor;
using UnityEngine;
using UMP;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(UniversalMediaPlayer))]
[CanEditMultipleObjects]
[Serializable]
public class UMPEditor : Editor
{
    private const string DESKTOP_CATEGORY_NAME = "Desktop";
    private const string WEBGL_CATEGORY_NAME = "WebGL";
    private const string ANDROID_CATEGORY_NAME = "Android";
    private const string IPHONE_CATEGORY_NAME = "iOS";

    SerializedProperty _renderingObjectsProp;
    SerializedProperty _pathProp;
    SerializedProperty _autoPlayProp;
    SerializedProperty _loopProp;
    SerializedProperty _loopSmoothProp;
    SerializedProperty _muteProp;
    SerializedProperty _volumeProp;
    SerializedProperty _playRateProp;
    SerializedProperty _positionProp;

    SerializedProperty _useAdvancedProp;
    SerializedProperty _useFixedSizeProp;
    SerializedProperty _chosenPlatformProp;

    SerializedProperty _fixedVideoWidthProp;
    SerializedProperty _fixedVideoHeightProp;

    #region Desktop Options
    SerializedProperty _audioSourcesDesktopProp;
    SerializedProperty _hardwareDecodingDesktopProp;
    SerializedProperty _flipVerticallyDesktopProp;
    SerializedProperty _outputToFileDesktopProp;
    SerializedProperty _displayOutputDesktopProp;
    SerializedProperty _outputFilePathDesktopProp;
    SerializedProperty _rtspOverTcpDesktopProp;
    SerializedProperty _fileCachingDesktopProp;
    SerializedProperty _liveCachingDesktopProp;
    SerializedProperty _diskCachingDesktopProp;
    SerializedProperty _networkCachingDesktopProp;
    #endregion

    #region Android Options
    SerializedProperty _hardwareDecodingAndroidProp;
    SerializedProperty _playInBackgroundAndroidProb;
    SerializedProperty _rtspOverTcpAndroidProp;
    SerializedProperty _fileCachingAndroidProp;
    SerializedProperty _liveCachingAndroidProp;
    SerializedProperty _diskCachingAndroidProp;
    SerializedProperty _networkCachingAndroidProp;
    #endregion

    #region IPhone Options
    SerializedProperty _videoToolboxIPhoneProp;
    SerializedProperty _videoToolboxMaxFrameWidthIPhoneProp;
    SerializedProperty _videoToolboxAsyncIPhoneProp;
    SerializedProperty _videoToolboxWaitAsyncIPhoneProp;
    SerializedProperty _playInBackgroundIPhoneProb;
    SerializedProperty _rtspOverTcpIPhoneProp;
    SerializedProperty _packetBufferingIPhoneProp;
    SerializedProperty _maxBufferSizeIPhoneProp;
    SerializedProperty _minFramesIPhoneProp;
    SerializedProperty _infbufIPhoneProp;
    SerializedProperty _framedropIPhoneProp;
    SerializedProperty _maxFpsIPhoneProp;
    #endregion

    SerializedProperty _logDetailProp;
    SerializedProperty _lastEventMsgProp;
    SerializedProperty _pathPreparedEventProp;
    SerializedProperty _openingEventProp;
    SerializedProperty _bufferingEventProp;
    SerializedProperty _preparedEventProp;
    SerializedProperty _playingEventProp;
    SerializedProperty _pausedEventProp;
    SerializedProperty _stoppedEventProp;
    SerializedProperty _endReachedEventProp;
    SerializedProperty _encounteredErrorEventProp;
    SerializedProperty _timeChangedEventProp;
    SerializedProperty _positionChangedEventProp;
    SerializedProperty _snapshotTakenEventProp;

    private string[] _availablePlatforms;
    private string _externalPath = string.Empty;
    private bool _showEventsListeners;
    private Vector2 _lastMsgScrollPos = Vector2.zero;
    private int _barWidth = 42;
    private FontStyle _cachedFontStyle;
    private Color _cachedTextColor;
    private float _cachedLabelWidth;
	private bool _cachedLabelWordWrap;
    private static GUIStyle _toggleButton = null;

    private string[] AvailablePlatforms
    {
        get
        {
            var availablePlatforms = new List<string>();
            foreach (UMPSettings.Platforms platform in Enum.GetValues(typeof(UMPSettings.Platforms)))
            {
                var libraryPath = UMPSettings.PlatformLibraryPath(platform, false);

                if (!string.IsNullOrEmpty(libraryPath) && Directory.Exists(libraryPath))
                {
                    foreach (var file in Directory.GetFiles(libraryPath))
                    {
                        if (Path.GetFileName(file).Contains(UMPSettings.AssetName))
                        {
                            if ((platform == UMPSettings.Platforms.Win ||
                                platform == UMPSettings.Platforms.Mac ||
                                platform == UMPSettings.Platforms.Linux) &&
                                !availablePlatforms.Contains(DESKTOP_CATEGORY_NAME))
                            {
                                availablePlatforms.Add(DESKTOP_CATEGORY_NAME);
                            }

                            if (platform == UMPSettings.Platforms.WebGL &&
                                !availablePlatforms.Contains(WEBGL_CATEGORY_NAME))
                            {
                                availablePlatforms.Add(WEBGL_CATEGORY_NAME);
                            }

                            if (platform == UMPSettings.Platforms.Android &&
                                !availablePlatforms.Contains(ANDROID_CATEGORY_NAME))
                            {
                                availablePlatforms.Add(ANDROID_CATEGORY_NAME);
                            }

                            if (platform == UMPSettings.Platforms.iOS &&
                                !availablePlatforms.Contains(IPHONE_CATEGORY_NAME))
                            {
                                availablePlatforms.Add(IPHONE_CATEGORY_NAME);
                            }

                            break;
                        }
                    }
                }
            }

            return availablePlatforms.ToArray();
        }
    }

    private void OnEnable()
    {
        // Setup the SerializedProperties
        _renderingObjectsProp = serializedObject.FindProperty("_renderingObjects");
        _pathProp = serializedObject.FindProperty("_path");
        _autoPlayProp = serializedObject.FindProperty("_autoPlay");
        _loopProp = serializedObject.FindProperty("_loop");
        _loopSmoothProp = serializedObject.FindProperty("_loopSmooth");
        _muteProp = serializedObject.FindProperty("_mute");

        _volumeProp = serializedObject.FindProperty("_volume");
        _playRateProp = serializedObject.FindProperty("_playRate");
        _positionProp = serializedObject.FindProperty("_position");

        _useAdvancedProp = serializedObject.FindProperty("_useAdvanced");
        _useFixedSizeProp = serializedObject.FindProperty("_useFixedSize");
        _chosenPlatformProp = serializedObject.FindProperty("_chosenPlatform");

        _fixedVideoWidthProp = serializedObject.FindProperty("_fixedVideoWidth");
        _fixedVideoHeightProp = serializedObject.FindProperty("_fixedVideoHeight");

        #region Desktop Options
        _audioSourcesDesktopProp = serializedObject.FindProperty("_audioSourcesDesktop");
        _hardwareDecodingDesktopProp = serializedObject.FindProperty("_hardwareDecodingDesktop");
        _flipVerticallyDesktopProp = serializedObject.FindProperty("_flipVerticallyDesktop");
        _outputToFileDesktopProp = serializedObject.FindProperty("_outputToFileDesktop");
        _displayOutputDesktopProp = serializedObject.FindProperty("_displayOutputDesktop");
        _outputFilePathDesktopProp = serializedObject.FindProperty("_outputFilePathDesktop");
        _rtspOverTcpDesktopProp = serializedObject.FindProperty("_rtspOverTcpDesktop");
        _fileCachingDesktopProp = serializedObject.FindProperty("_fileCachingDesktop");
        _liveCachingDesktopProp = serializedObject.FindProperty("_liveCachingDesktop");
        _diskCachingDesktopProp = serializedObject.FindProperty("_diskCachingDesktop");
        _networkCachingDesktopProp = serializedObject.FindProperty("_networkCachingDesktop");
        #endregion

        #region Android Options
        _hardwareDecodingAndroidProp = serializedObject.FindProperty("_hardwareDecodingAndroid");
        _playInBackgroundAndroidProb = serializedObject.FindProperty("_playInBackgroundAndroid");
        _rtspOverTcpAndroidProp = serializedObject.FindProperty("_rtspOverTcpAndroid");
        _fileCachingAndroidProp = serializedObject.FindProperty("_fileCachingAndroid");
        _liveCachingAndroidProp = serializedObject.FindProperty("_liveCachingAndroid");
        _diskCachingAndroidProp = serializedObject.FindProperty("_diskCachingAndroid");
        _networkCachingAndroidProp = serializedObject.FindProperty("_networkCachingAndroid");
        #endregion

        #region IPhone Options
        _videoToolboxIPhoneProp = serializedObject.FindProperty("_videoToolboxIPhone");
        _videoToolboxMaxFrameWidthIPhoneProp = serializedObject.FindProperty("_videoToolboxMaxFrameWidthIPhone");
        _videoToolboxAsyncIPhoneProp = serializedObject.FindProperty("_videoToolboxAsyncIPhone");
        _videoToolboxWaitAsyncIPhoneProp = serializedObject.FindProperty("_videoToolboxWaitAsyncIPhone");
        _playInBackgroundIPhoneProb = serializedObject.FindProperty("_playInBackgroundIPhone");
        _rtspOverTcpIPhoneProp = serializedObject.FindProperty("_rtspOverTcpIPhone");
        _packetBufferingIPhoneProp = serializedObject.FindProperty("_packetBufferingIPhone");
        _maxBufferSizeIPhoneProp = serializedObject.FindProperty("_maxBufferSizeIPhone");
        _minFramesIPhoneProp = serializedObject.FindProperty("_minFramesIPhone");
        _infbufIPhoneProp = serializedObject.FindProperty("_infbufIPhone");
        _framedropIPhoneProp = serializedObject.FindProperty("_framedropIPhone");
        _maxFpsIPhoneProp = serializedObject.FindProperty("_maxFpsIPhone");
        #endregion

        _logDetailProp = serializedObject.FindProperty("_logDetail");
        _lastEventMsgProp = serializedObject.FindProperty("_lastEventMsg");
        _pathPreparedEventProp = serializedObject.FindProperty("_pathPreparedEvent");
        _openingEventProp = serializedObject.FindProperty("_openingEvent");
        _bufferingEventProp = serializedObject.FindProperty("_bufferingEvent");
        _preparedEventProp = serializedObject.FindProperty("_preparedEvent");
        _playingEventProp = serializedObject.FindProperty("_playingEvent");
        _pausedEventProp = serializedObject.FindProperty("_pausedEvent");
        _stoppedEventProp = serializedObject.FindProperty("_stoppedEvent");
        _endReachedEventProp = serializedObject.FindProperty("_endReachedEvent");
        _encounteredErrorEventProp = serializedObject.FindProperty("_encounteredErrorEvent");
        _timeChangedEventProp = serializedObject.FindProperty("_timeChangedEvent");
        _positionChangedEventProp = serializedObject.FindProperty("_positionChangedEvent");
        _snapshotTakenEventProp = serializedObject.FindProperty("_snapshotTakenEvent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var umpEditor = (UniversalMediaPlayer)target;

        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;

        EditorGUI.BeginChangeCheck();

        _cachedFontStyle = EditorStyles.label.fontStyle;
        _cachedTextColor = EditorStyles.textField.normal.textColor;
		_cachedLabelWordWrap = EditorStyles.label.wordWrap;

        #region Rendering Field
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_renderingObjectsProp, new GUIContent("Rendering GameObjects:"), true);
        #endregion

        #region Path Field
        EditorGUILayout.Space();

        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Path to video file:");
        EditorStyles.label.fontStyle = _cachedFontStyle;
        EditorStyles.textField.wordWrap = true;
        _pathProp.stringValue = EditorGUILayout.TextField(_pathProp.stringValue, GUILayout.Height(30));
        EditorStyles.textField.wordWrap = false;
        #endregion

        #region Additional Fields
        EditorGUILayout.Space();
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Additional properties:");
        EditorStyles.label.fontStyle = _cachedFontStyle;

        GUILayout.BeginVertical("Box");
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AutoPlay:", GUILayout.MinWidth(60));
        _autoPlayProp.boolValue = EditorGUILayout.Toggle(_autoPlayProp.boolValue, EditorStyles.radioButton);

        if (!_loopSmoothProp.boolValue)
            EditorGUILayout.LabelField("Loop:", GUILayout.MinWidth(36));
        else
            EditorGUILayout.LabelField("Loop(smooth):", GUILayout.MinWidth(90));

        _loopProp.boolValue = EditorGUILayout.Toggle(_loopProp.boolValue, EditorStyles.radioButton, GUILayout.MaxWidth(20));
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        _loopSmoothProp.boolValue = EditorGUILayout.Toggle(_loopSmoothProp.boolValue, EditorStyles.radioButton);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("Mute:", GUILayout.MinWidth(36));
        _muteProp.boolValue = EditorGUILayout.Toggle(_muteProp.boolValue, EditorStyles.radioButton);
        GUILayout.EndHorizontal();

        if (_toggleButton == null)
        {
            _toggleButton = new GUIStyle(EditorStyles.miniButton);
            _toggleButton.normal.background = EditorStyles.miniButton.active.background;
        }

        if (GUILayout.Button("Advanced options", _useAdvancedProp.boolValue ? _toggleButton : EditorStyles.miniButton))
        {
            _useAdvancedProp.boolValue = !_useAdvancedProp.boolValue;
            _availablePlatforms = AvailablePlatforms;
        }

        if (_useAdvancedProp.boolValue)
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            _cachedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 170;

            _useFixedSizeProp.boolValue = EditorGUILayout.Toggle("Use fixed video size:", _useFixedSizeProp.boolValue);

            if (_useFixedSizeProp.boolValue)
            {
                _fixedVideoWidthProp.intValue = EditorGUILayout.IntField(new GUIContent("Width: ", "Fixed video width."), _fixedVideoWidthProp.intValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Fixed video width."));
                _fixedVideoWidthProp.intValue = Mathf.Clamp(_fixedVideoWidthProp.intValue, 1, 7680);

                _fixedVideoHeightProp.intValue = EditorGUILayout.IntField(new GUIContent("Height: ", "Fixed video height."), _fixedVideoHeightProp.intValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Fixed video height."));
                _fixedVideoHeightProp.intValue = Mathf.Clamp(_fixedVideoHeightProp.intValue, 1, 7680);
            }
            else
            {
                _fixedVideoWidthProp.intValue = 0;
                _fixedVideoHeightProp.intValue = 0;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            if (_availablePlatforms == null || _availablePlatforms.Length <= 0)
                _availablePlatforms = AvailablePlatforms;

            if (_availablePlatforms.Length <= 0)
            {
                var warningLabel = new GUIStyle(EditorStyles.textArea);
                warningLabel.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Can't find 'UniversalMediaPlayer' asset folder, please check your Unity UMP preferences.", warningLabel);

                EditorStyles.label.normal.textColor = _cachedTextColor;
                EditorStyles.label.fontStyle = _cachedFontStyle;

                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                return;
            }

            _chosenPlatformProp.intValue = GUILayout.SelectionGrid(_chosenPlatformProp.intValue, _availablePlatforms, _availablePlatforms.Length, EditorStyles.miniButton);
            _chosenPlatformProp.intValue = Mathf.Clamp(_chosenPlatformProp.intValue, 0, _availablePlatforms.Length - 1);

            #region Desktop Options
            if (_availablePlatforms[_chosenPlatformProp.intValue] == DESKTOP_CATEGORY_NAME)
            {
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                #region Audio Option
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(_audioSourcesDesktopProp, new GUIContent("Audio Sources:"), true);
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                #endregion

                #region Decoding Options
                EditorGUILayout.PropertyField(_hardwareDecodingDesktopProp, new GUIContent("Hardware decoding: ", "This allows hardware decoding when available."), false);

                if (_hardwareDecodingDesktopProp.intValue == (int)PlayerOptions.State.Default)
                {
                    var hardwareDecodingName = "DirectX Video Acceleration (DXVA) 2.0";
                    if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Mac)
                        hardwareDecodingName = "Video Decode Acceleration Framework (VDA)";
                    if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
                        hardwareDecodingName = "VA-API video decoder via DRM";

                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", hardwareDecodingName));

                    if (UMPSettings.EditorBitMode == UMPSettings.BitModes.x86)
                    {
                        var warningLabel = new GUIStyle(EditorStyles.textArea);
                        warningLabel.fontStyle = FontStyle.Italic;
                        EditorGUILayout.LabelField("Doesn't support on current bit system and will be ignored and swiched to 'Enable' state.", warningLabel);
                    }
                }

                if (_hardwareDecodingDesktopProp.intValue == (int)PlayerOptions.State.Enable)
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Automatic"));
                EditorGUILayout.Space();
                #endregion

                #region Flip Options
                _flipVerticallyDesktopProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Flip vertically: ", "Flip video frame vertically when we get it from native library (CPU usage cost)."), _flipVerticallyDesktopProp.boolValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Flip video frame vertically when we get it from native library (CPU usage cost)."));
                #endregion

                #region Dublicate Options
                _outputToFileDesktopProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Data to a file: ", "Duplicate the output stream and redirect it to a file (output file must have '.mp4' video file format)."), _outputToFileDesktopProp.boolValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Duplicate the output stream and redirect it to a file (output file must have '.mp4' video file format)."));

                if (_outputToFileDesktopProp.boolValue)
                {
                    _displayOutputDesktopProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Display source video: ", "Display source video when duplicate data to a file."), _displayOutputDesktopProp.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Display source video when duplicate data to a file."));

                    _outputFilePathDesktopProp.stringValue = EditorGUILayout.TextField(new GUIContent("Path to file: ", "Full path to a file where output data will be stored (example: 'C:\\Path\\To\\File\\Name.mp4')."), _outputFilePathDesktopProp.stringValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Full path to a file where output data will be stored (example: 'C:\\Path\\To\\File\\Name.mp4')."));
                }
                EditorGUILayout.Space();
                #endregion

                #region RTP/RTSP/SDP Options
                _rtspOverTcpDesktopProp.boolValue = EditorGUILayout.Toggle(new GUIContent("RTP over RTSP (TCP): ", "Use RTP over RTSP (TCP) (HTTP default)."), _rtspOverTcpDesktopProp.boolValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Use RTP over RTSP (TCP) (HTTP default)."));
                #endregion
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(umpEditor.IsReady);
                #region Caching Options
                _fileCachingDesktopProp.intValue = EditorGUILayout.IntField(new GUIContent("File caching (ms): ", "Caching value for local files, in milliseconds."), _fileCachingDesktopProp.intValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for local files, in milliseconds."));
                _fileCachingDesktopProp.intValue = Mathf.Clamp(_fileCachingDesktopProp.intValue, 0, 60000);

                _liveCachingDesktopProp.intValue = EditorGUILayout.IntField(new GUIContent("Live capture caching (ms): ", "Caching value for cameras and microphones, in milliseconds."), _liveCachingDesktopProp.intValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for cameras and microphones, in milliseconds."));
                _liveCachingDesktopProp.intValue = Mathf.Clamp(_liveCachingDesktopProp.intValue, 0, 60000);

                _diskCachingDesktopProp.intValue = EditorGUILayout.IntField(new GUIContent("Disc caching (ms): ", "Caching value for optical media, in milliseconds."), _diskCachingDesktopProp.intValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for optical media, in milliseconds."));
                _diskCachingDesktopProp.intValue = Mathf.Clamp(_diskCachingDesktopProp.intValue, 0, 60000);

                _networkCachingDesktopProp.intValue = EditorGUILayout.IntField(new GUIContent("Network caching (ms): ", "Caching value for network resources, in milliseconds."), _networkCachingDesktopProp.intValue);
                GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for network resources, in milliseconds."));
                _networkCachingDesktopProp.intValue = Mathf.Clamp(_networkCachingDesktopProp.intValue, 0, 60000);
                #endregion
                EditorGUI.EndDisabledGroup();
            }
            #endregion

            #region WebGL Options
            if (_availablePlatforms[_chosenPlatformProp.intValue] == WEBGL_CATEGORY_NAME)
            {
                var warningLabel = new GUIStyle(EditorStyles.textArea);
                warningLabel.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Doesn't support in current version.", warningLabel);
            }
            #endregion

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            #region Android Options
            if (_availablePlatforms[_chosenPlatformProp.intValue] == ANDROID_CATEGORY_NAME)
            {
                if (!UMPPreference.Instance.UseAndroidNative)
                {
                    #region Decoding Options
                    EditorGUILayout.PropertyField(_hardwareDecodingAndroidProp, new GUIContent("Hardware decoding: ", "This allows hardware decoding when available."), false);
                    if (_hardwareDecodingAndroidProp.intValue == (int)PlayerOptions.State.Default)
                        GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "HW decoder will be used"));

                    if (_hardwareDecodingAndroidProp.intValue == (int)PlayerOptions.State.Enable)
                        GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Force HW acceleration even for unknown devices"));
                    EditorGUILayout.Space();
                    #endregion

                    #region Background Options
                    _playInBackgroundAndroidProb.boolValue = EditorGUILayout.Toggle(new GUIContent("Play in background: ", "Continue play video when application in background."), _playInBackgroundAndroidProb.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Continue play video when application in background."));
                    #endregion

                    #region RTP/RTSP/SDP Options
                    _rtspOverTcpAndroidProp.boolValue = EditorGUILayout.Toggle(new GUIContent("RTP over RTSP (TCP): ", "Use RTP over RTSP (TCP) (HTTP default)."), _rtspOverTcpAndroidProp.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Use RTP over RTSP (TCP) (HTTP default)."));
                    #endregion

                    #region Caching Options
                    _fileCachingAndroidProp.intValue = EditorGUILayout.IntField(new GUIContent("File caching (ms): ", "Caching value for local files, in milliseconds."), _fileCachingAndroidProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for local files, in milliseconds."));
                    _fileCachingAndroidProp.intValue = Mathf.Clamp(_fileCachingAndroidProp.intValue, 0, 60000);

                    _liveCachingAndroidProp.intValue = EditorGUILayout.IntField(new GUIContent("Live capture caching (ms): ", "Caching value for cameras and microphones, in milliseconds."), _liveCachingAndroidProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for cameras and microphones, in milliseconds."));
                    _liveCachingAndroidProp.intValue = Mathf.Clamp(_liveCachingAndroidProp.intValue, 0, 60000);

                    _diskCachingAndroidProp.intValue = EditorGUILayout.IntField(new GUIContent("Disc caching (ms): ", "Caching value for optical media, in milliseconds."), _diskCachingAndroidProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for optical media, in milliseconds."));
                    _diskCachingAndroidProp.intValue = Mathf.Clamp(_diskCachingAndroidProp.intValue, 0, 60000);

                    _networkCachingAndroidProp.intValue = EditorGUILayout.IntField(new GUIContent("Network caching (ms): ", "Caching value for network resources, in milliseconds."), _networkCachingAndroidProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Caching value for network resources, in milliseconds."));
                    _networkCachingAndroidProp.intValue = Mathf.Clamp(_networkCachingAndroidProp.intValue, 0, 60000);
                    #endregion
                }
                else
                {
                    var warningLabel = new GUIStyle(EditorStyles.textArea);
                    warningLabel.fontStyle = FontStyle.Bold;
                    EditorGUILayout.LabelField("Doesn't support in Android native player.", warningLabel);
                }
            }
            #endregion
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            #region IPhone Options
            if (_availablePlatforms[_chosenPlatformProp.intValue] == IPHONE_CATEGORY_NAME)
            {
                if (!UMPPreference.Instance.UseIPhoneNative)
                {
                    #region VideoToolbox Options
                    _videoToolboxIPhoneProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Hardware decoding: ", "This allows hardware decoding when available (enable VideoToolbox decoding)."), _videoToolboxIPhoneProp.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "This allows hardware decoding when available (enable VideoToolbox decoding)."));

                    if (_videoToolboxIPhoneProp.boolValue)
                    {
                        _videoToolboxMaxFrameWidthIPhoneProp.intValue = EditorGUILayout.IntField(new GUIContent("Max width of output frame: ", "Max possible video resolution for hardware decoding."), _videoToolboxMaxFrameWidthIPhoneProp.intValue);
                        GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Max possible video resolution for hardware decoding."));
                        _videoToolboxMaxFrameWidthIPhoneProp.intValue = Mathf.Clamp(_videoToolboxMaxFrameWidthIPhoneProp.intValue, 0, 32768);

                        _videoToolboxAsyncIPhoneProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Async decompression: ", "Use asynchronous decompression for hardware frame decoding."), _videoToolboxAsyncIPhoneProp.boolValue);
                        GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Use asynchronous decompression for hardware frame decoding."));

                        if (_videoToolboxAsyncIPhoneProp.boolValue)
                        {
                            _videoToolboxWaitAsyncIPhoneProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Wait for asynchronous frames: ", "Wait when frames is ready."), _videoToolboxWaitAsyncIPhoneProp.boolValue);
                            GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Wait when frames is ready."));
                        }
                    }
                    EditorGUILayout.Space();
                    #endregion

                    #region Background Options
                    _playInBackgroundIPhoneProb.boolValue = EditorGUILayout.Toggle(new GUIContent("Play in background: ", "Continue play video when application in background."), _playInBackgroundIPhoneProb.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Continue play video when application in background."));
                    #endregion

                    #region RTP/RTSP/SDP Options
                    _rtspOverTcpIPhoneProp.boolValue = EditorGUILayout.Toggle(new GUIContent("RTP over RTSP (TCP): ", "Use RTP over RTSP (TCP) (HTTP default)."), _rtspOverTcpIPhoneProp.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Use RTP over RTSP (TCP) (HTTP default)."));
                    #endregion

                    #region Buffer Options
                    _packetBufferingIPhoneProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Packet buffering: ", "Pause output until enough packets have been read after stalling."), _packetBufferingIPhoneProp.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Pause output until enough packets have been read after stalling."));

                    _maxBufferSizeIPhoneProp.intValue = EditorGUILayout.IntField(new GUIContent("Max buffer size: ", "Max buffer size should be pre-read (in bytes)."), _maxBufferSizeIPhoneProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Max buffer size should be pre-read."));
                    _maxBufferSizeIPhoneProp.intValue = Mathf.Clamp(_maxBufferSizeIPhoneProp.intValue, 0, 15 * 1024 * 1024);

                    _minFramesIPhoneProp.intValue = EditorGUILayout.IntField(new GUIContent("Min frames: ", "Minimal frames to stop pre-reading."), _minFramesIPhoneProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Minimal frames to stop pre-reading."));
                    _minFramesIPhoneProp.intValue = Mathf.Clamp(_minFramesIPhoneProp.intValue, 5, 50000);

                    _infbufIPhoneProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Infbuf: ", "Don't limit the input buffer size (useful with realtime streams)."), _infbufIPhoneProp.boolValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Don't limit the input buffer size (useful with realtime streams)."));

                    EditorGUILayout.Space();
                    #endregion

                    #region Frame Options
                    _framedropIPhoneProp.intValue = EditorGUILayout.IntField(new GUIContent("Framedrop: ", "Drop frames when cpu is too slow."), _framedropIPhoneProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Drop frames when cpu is too slow."));
                    _framedropIPhoneProp.intValue = Mathf.Clamp(_framedropIPhoneProp.intValue, -1, 120);

                    _maxFpsIPhoneProp.intValue = EditorGUILayout.IntField(new GUIContent("Max fps: ", "Drop frames in video whose fps is greater than max-fps."), _maxFpsIPhoneProp.intValue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Drop frames in video whose fps is greater than max-fps."));
                    _maxFpsIPhoneProp.intValue = Mathf.Clamp(_maxFpsIPhoneProp.intValue, -1, 120);
                    #endregion
                }
                else
                {
                    var warningLabel = new GUIStyle(EditorStyles.textArea);
                    warningLabel.fontStyle = FontStyle.Bold;
                    EditorGUILayout.LabelField("Doesn't support in current version.", warningLabel);
                }
            }
            #endregion
            EditorGUI.EndDisabledGroup();

            EditorGUIUtility.labelWidth = _cachedLabelWidth;
        }
        else
        {
            _fileCachingDesktopProp.intValue = PlayerOptions.DEFAULT_CACHING_VALUE;
            _liveCachingDesktopProp.intValue = PlayerOptions.DEFAULT_CACHING_VALUE;
            _diskCachingDesktopProp.intValue = PlayerOptions.DEFAULT_CACHING_VALUE;
            _networkCachingDesktopProp.intValue = PlayerOptions.DEFAULT_CACHING_VALUE;
            _fixedVideoWidthProp.intValue = -1;
            _fixedVideoHeightProp.intValue = -1;
        }

        if (UMPSettings.Instance.UseExternalLibs)
        {
            if (_externalPath.Equals(string.Empty))
                _externalPath = UMPSettings.RuntimePlatformLibraryPath(true);

            if (_externalPath != string.Empty)
            {
                var wrapTextStyle = EditorStyles.textArea;
                wrapTextStyle.wordWrap = true;
                EditorGUILayout.LabelField("Path to external/installed libraries: '" + _externalPath + "'", wrapTextStyle);
            }
        }
        else
        {
            _externalPath = string.Empty;
        }

        GUILayout.EndVertical();
        #endregion

        #region Player Fields
        EditorGUILayout.Space();
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Player properties:");
        EditorStyles.label.fontStyle = _cachedFontStyle;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.MiddleCenter;
		EditorGUILayout.LabelField("Volume", centeredStyle, GUILayout.MinWidth(80));
        if (GUILayout.Button("x", EditorStyles.miniButton))
        {
            _volumeProp.intValue = 50;
        }
        GUILayout.EndHorizontal();

        _volumeProp.intValue = EditorGUILayout.IntSlider(_volumeProp.intValue, 0, 100);
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Play rate", centeredStyle, GUILayout.MinWidth(80));
        if (GUILayout.Button("x", EditorStyles.miniButton))
        {
            _playRateProp.floatValue = 1f;
        }
        GUILayout.EndHorizontal();

        _playRateProp.floatValue = EditorGUILayout.Slider(_playRateProp.floatValue, 0.5f, 5f);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(!umpEditor.IsReady);
        EditorGUILayout.Space();
        GUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Position", centeredStyle, GUILayout.MinWidth(100));
        _positionProp.floatValue = EditorGUILayout.Slider(_positionProp.floatValue, 0f, 1f);
        GUILayout.EndVertical();
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying || !umpEditor.isActiveAndEnabled || umpEditor.IsParsing);
        GUILayout.BeginHorizontal("Box");
		if (GUILayout.Button("LOAD", GUILayout.MinWidth(40)))
        {
            umpEditor.Prepare();
        }
		if (GUILayout.Button("PLAY", GUILayout.MinWidth(40)))
        {
            umpEditor.Play();
        }
		if (GUILayout.Button("PAUSE", GUILayout.MinWidth(40)))
        {
            umpEditor.Pause();
        }
		if (GUILayout.Button("STOP", GUILayout.MinWidth(40)))
        {
            umpEditor.Stop();
        }
		if (GUILayout.Button("SHOT", GUILayout.MinWidth(40)))
        {
            umpEditor.Snapshot(Application.persistentDataPath);
        }
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
        #endregion

        #region Events & Logging Fields
        EditorGUILayout.Space();
        EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("Events & Logging:");
        EditorStyles.label.fontStyle = _cachedFontStyle;

        GUILayout.BeginVertical("Box");
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        EditorGUILayout.PropertyField(_logDetailProp, GUILayout.MinWidth(50));
        EditorGUI.EndDisabledGroup();

        GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Last msg: ", GUILayout.MinWidth(70));
        EditorStyles.label.normal.textColor = Color.black;
        EditorStyles.label.fontStyle = FontStyle.Italic;
		EditorStyles.label.wordWrap = true;
        EditorGUILayout.LabelField(_lastEventMsgProp.stringValue, GUILayout.MaxWidth(100));
        EditorStyles.label.normal.textColor = _cachedTextColor;
        EditorStyles.label.fontStyle = _cachedFontStyle;
		EditorStyles.label.wordWrap = _cachedLabelWordWrap;
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        _showEventsListeners = EditorGUILayout.Foldout(_showEventsListeners, "Event Listeners");

        if (_showEventsListeners)
        {
			EditorGUILayout.PropertyField(_pathPreparedEventProp, new GUIContent("Path Prepared"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_openingEventProp, new GUIContent("Opening"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_bufferingEventProp, new GUIContent("Buffering"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_preparedEventProp, new GUIContent("Prepared"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_playingEventProp, new GUIContent("Playing"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_pausedEventProp, new GUIContent("Paused"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_stoppedEventProp, new GUIContent("Stopped"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_endReachedEventProp, new GUIContent("End Reached"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_encounteredErrorEventProp, new GUIContent("Encountered Error"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_timeChangedEventProp, new GUIContent("Time Changed"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_positionChangedEventProp, new GUIContent("Position Changed"), true, GUILayout.MinWidth(50));
			EditorGUILayout.PropertyField(_snapshotTakenEventProp, new GUIContent("Snapshot"), true, GUILayout.MinWidth(50));
        }
        #endregion

        EditorStyles.label.normal.textColor = _cachedTextColor;
        EditorStyles.label.fontStyle = _cachedFontStyle;

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
