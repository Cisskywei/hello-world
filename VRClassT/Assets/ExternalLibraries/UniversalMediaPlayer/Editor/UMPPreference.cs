using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UMP;

public class UMPPreference
{
    private static UMPPreference _instance;
    private static UMPSettings _umpSettings;

    private static bool _preloaded = false;
    private static bool _nativeAndroidLibraryPrepare = false;
    private static bool _nativeIPhoneLibraryPrepare = false;

    public static UMPPreference Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UMPPreference();
            return _instance;
        }
    }

    public bool UseExternalLibs
    {
        get { return _umpSettings.UseExternalLibs; }
    }

    public bool UseAndroidNative
    {
        get { return _umpSettings.UseAndroidNative; }
    }

    public bool UseIPhoneNative
    {
        get { return _umpSettings.UseIPhoneNative; }
    }

    private UMPPreference()
    {
        _umpSettings = UMPSettings.Instance;
    }

    [PreferenceItem("UMP")]
    public static void UMPGUI()
    {
        if (!_preloaded)
        {
            _umpSettings = UMPSettings.Instance;
            _preloaded = true;
        }

        var chachedLabelColor = EditorStyles.label.normal.textColor;
        var cachedFontStyle = EditorStyles.label.fontStyle;

        _umpSettings.UseCustomAssetPath = EditorGUILayout.Toggle(new GUIContent("Use custom asset path", "Will be using cusstom asset path to main 'UniversalMediaPlayer' folder (give you possibility to move asset folder in different space in your project)."), _umpSettings.UseCustomAssetPath);

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(!_umpSettings.UseCustomAssetPath);
        EditorStyles.textField.wordWrap = true;
        _umpSettings.AssetPath = EditorGUILayout.TextField(_umpSettings.AssetPath, GUILayout.Height(30));
        EditorStyles.textField.wordWrap = false;
        EditorGUI.EndDisabledGroup();

        if (!_umpSettings.IsValidAssetPath && _umpSettings.UseCustomAssetPath)
        {
            if (GUILayout.Button("Find asset folder in current project"))
            {
                GUI.FocusControl(null);
                _umpSettings.AssetPath = FindAssetFolder("Assets");
            }
        }

        EditorStyles.label.fontStyle = FontStyle.Italic;
        if (_umpSettings.IsValidAssetPath)
            EditorGUILayout.LabelField("Path is correct.");
        else
        {
            EditorStyles.label.normal.textColor = Color.red;
            if (!_umpSettings.UseCustomAssetPath)
                EditorGUILayout.LabelField("Can't find asset folder, try to use custom asset path.");
            else
                EditorGUILayout.LabelField("Can't find asset folder.");
        }

        EditorStyles.label.normal.textColor = chachedLabelColor;
        EditorStyles.label.fontStyle = cachedFontStyle;

        EditorGUILayout.LabelField("Editor/Desktop platforms:", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        _umpSettings.UseAudioSource = EditorGUILayout.Toggle(new GUIContent("Use Unity 'Audio Source' component", "Will be using Unity 'Audio Source' component for audio output for all UMP instances (global) by default."), _umpSettings.UseAudioSource);

        EditorGUILayout.Space();

        _umpSettings.UseExternalLibs = EditorGUILayout.Toggle(new GUIContent("Use installed VLC libraries", "Will be using external/installed VLC player libraries for all UMP instances (global). Path to install VLC directory will be obtained automatically (you can also setup your custom path)."), _umpSettings.UseExternalLibs);

        EditorStyles.label.wordWrap = true;
        EditorStyles.label.normal.textColor = Color.red;

        bool useExternal = true;
        string librariesPath = UMPSettings.RuntimePlatformLibraryPath(false);
        if (!string.IsNullOrEmpty(librariesPath) && Directory.Exists(librariesPath))
        {
            string[] libraries = Directory.GetFiles(librariesPath);
            int includes = 0;

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Win)
            {
                foreach (var library in libraries)
                {
                    if (Path.GetFileName(library).Contains("libvlc.dll.meta") ||
                        Path.GetFileName(library).Contains("libvlccore.dll.meta") ||
                        Path.GetFileName(library).Contains("plugins.meta"))
                        includes++;
                }

                if (includes >= 3)
                    useExternal = false;
            }

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Mac)
            {
                foreach (var library in libraries)
                {
                    if (Path.GetFileName(library).Contains("libvlc.dylib.meta") ||
                        Path.GetFileName(library).Contains("libvlccore.dylib.meta"))
                        includes++;
                }

                if (includes >= 2)
                    useExternal = false;
            }

            if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Linux)
            {
                foreach (var library in libraries)
                {
                    if (Path.GetFileName(library).Contains("libvlc.so.meta") ||
                        Path.GetFileName(library).Contains("libvlccore.so.meta") ||
                        Path.GetFileName(library).Contains("plugins.meta"))
                        includes++;
                }

                if (includes >= 3)
                    useExternal = false;
            }
        }

        if (useExternal)
        {
            EditorGUILayout.LabelField("Please correctly import UMP (Win, Mac, Linux) package to use internal VLC libraries.");
            _umpSettings.UseExternalLibs = true;
        }

        EditorGUILayout.Space();

        if (_umpSettings.UseExternalLibs)
        {
            string externalLibsPath = UMPSettings.RuntimePlatformLibraryPath(true);
            if (externalLibsPath.Equals(string.Empty))
            {
                EditorGUILayout.LabelField("Did you install VLC player software correctly? Please make sure that:");
                EditorGUILayout.LabelField("1. Your installed VLC player bit application == Unity Editor bit application (VLC player 64-bit == Unity 64-bit Editor);");
                EditorGUILayout.LabelField("2. Use last version installer from official site: ");

                if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Win)
                {
                    EditorGUILayout.LabelField("Windows platform: ");

                    var link86 = "http://get.videolan.org/vlc/2.2.4/win32/vlc-2.2.4-win32.exe";
                    EditorStyles.label.normal.textColor = Color.blue;
                    EditorGUILayout.LabelField(link86 + " (Editor x86)");

                    Rect linkRect86 = GUILayoutUtility.GetLastRect();

                    if (Event.current.type == EventType.MouseUp && linkRect86.Contains(Event.current.mousePosition))
                        Application.OpenURL(link86);

                    var link64 = "http://get.videolan.org/vlc/2.2.4/win64/vlc-2.2.4-win64.exe";
                    EditorStyles.label.normal.textColor = Color.blue;
                    EditorGUILayout.LabelField(link64 + " (Editor x64)");

                    Rect linkRect64 = GUILayoutUtility.GetLastRect();

                    if (Event.current.type == EventType.MouseUp && linkRect64.Contains(Event.current.mousePosition))
                        Application.OpenURL(link64);
                }

                if (UMPSettings.SupportedPlatform == UMPSettings.Platforms.Mac)
                {
                    EditorGUILayout.LabelField("Mac OS platform: ");

                    var link64 = "http://get.videolan.org/vlc/2.2.4/macosx/vlc-2.2.4.dmg";
                    EditorStyles.label.normal.textColor = Color.blue;
                    EditorGUILayout.LabelField(link64 + " (Editor x64)");

                    Rect linkRect64 = GUILayoutUtility.GetLastRect();

                    if (Event.current.type == EventType.MouseUp && linkRect64.Contains(Event.current.mousePosition))
                        Application.OpenURL(link64);
                }

                EditorStyles.label.normal.textColor = Color.red;
                EditorGUILayout.LabelField("Or you can try to use custom additional path to your VLC libraries.");

                EditorGUILayout.Space();
            }

            EditorStyles.label.normal.textColor = chachedLabelColor;

            EditorGUILayout.LabelField(new GUIContent("External/installed VLC libraries path:", "Default path to installed VLC player libraries. Example: '" + @"C:\Program Files\VideoLAN\VLC'."));
            GUIStyle pathLabel = EditorStyles.textField;
            pathLabel.wordWrap = true;
            EditorGUILayout.LabelField(externalLibsPath, pathLabel);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Additional external/installed VLC libraries path:", "Additional path to installed VLC player libraries. Will be used if path to libraries can't be automatically obtained. Example: '" + @"C:\Program Files\VideoLAN\VLC'."));
            GUIStyle additionalLabel = EditorStyles.textField;
            additionalLabel.wordWrap = true;

            _umpSettings.AdditionalLibsPath = EditorGUILayout.TextField(_umpSettings.AdditionalLibsPath);
        }

        EditorStyles.label.normal.textColor = chachedLabelColor;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Mobile platforms:", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        _umpSettings.UseAndroidNative = EditorGUILayout.Toggle(new GUIContent("Use Android native player", "Will be using Android native media player (MediaPlayer) for all UMP instances (global)."), _umpSettings.UseAndroidNative);

        if (_nativeAndroidLibraryPrepare != _umpSettings.UseAndroidNative)
        {
            List<string> libs = new List<string>();
            libs.AddRange(Directory.GetFiles(Application.dataPath + "/UniversalMediaPlayer/Plugins/Android/libs/armeabi-v7a"));
            libs.AddRange(Directory.GetFiles(Application.dataPath + "/UniversalMediaPlayer/Plugins/Android/libs/x86"));

            foreach (var lib in libs)
            {
                if (lib.Contains(".meta") && !lib.Contains("libUniversalMediaPlayer"))
                {
                    File.SetAttributes(lib, FileAttributes.Normal);
                    string metaData = File.ReadAllText(lib);
                    var match = Regex.Match(metaData, @"Android.*\s*enabled:.");

                    if (match.Success)
                    {
                        metaData = Regex.Replace(metaData, @"Android.*\s*enabled:." + (!_nativeAndroidLibraryPrepare ? 1 : 0), match.Value + (_nativeAndroidLibraryPrepare ? 1 : 0));
                        File.WriteAllText(lib, metaData);
                    }
                }
            }
            libs.Clear();
            AssetDatabase.Refresh();
        }

        _nativeAndroidLibraryPrepare = _umpSettings.UseAndroidNative;

        EditorGUILayout.Space();

        _umpSettings.UseIPhoneNative = EditorGUILayout.Toggle(new GUIContent("Use iOS native player", "Will be using iOS native media player (AVPlayer) for all UMP instances (global)."), _umpSettings.UseIPhoneNative);

        if (_nativeIPhoneLibraryPrepare != _umpSettings.UseIPhoneNative)
        {
            List<string> libs = new List<string>();
            libs.AddRange(Directory.GetFiles(Application.dataPath + "/UniversalMediaPlayer/Plugins/iOS"));

            foreach (var lib in libs)
            {
                if (lib.Contains(".meta"))
                {
                    if (!Path.GetFileNameWithoutExtension(lib).Contains("Base"))
                    {
                        File.SetAttributes(lib, FileAttributes.Normal);
                        string metaData = File.ReadAllText(lib);
                        var match = Regex.Match(metaData, @"iOS.*\s*enabled:.");

                        if (match.Success)
                        {
                            var isFFmpeg = (lib.Contains("FFmpeg") || lib.Contains("framework"));
                            var enable = isFFmpeg ? !_umpSettings.UseIPhoneNative : _umpSettings.UseIPhoneNative;
                            metaData = Regex.Replace(metaData, @"iOS.*\s*enabled:." + (!enable ? 1 : 0), match.Value + (enable ? 1 : 0));
                            File.WriteAllText(lib, metaData);
                        }
                    }
                }
            }
            libs.Clear();
            AssetDatabase.Refresh();
        }

        _nativeIPhoneLibraryPrepare = _umpSettings.UseIPhoneNative;
    }

    private static string FindAssetFolder(string rootPath)
    {
        var projectFolders = Directory.GetDirectories(rootPath);

        foreach (var folderPath in projectFolders)
        {
            if (Path.GetFileName(folderPath).Contains(UMPSettings.AssetName) && Directory.GetFiles(folderPath).Length > 0)
                return folderPath.Replace(@"\", "/");

            return FindAssetFolder(folderPath);
        }

        return string.Empty;
    }
}