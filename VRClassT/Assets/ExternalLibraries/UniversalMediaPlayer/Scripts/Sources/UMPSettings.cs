using System;
using System.IO;
using UMP.Wrappers;
using UnityEngine;

public class UMPSettings
{
    public enum Platforms
    {
        None,
        Win,
        Mac,
        Linux,
        WebGL,
        Android,
        iOS
    }

    public enum BitModes
    {
        x86,
        x64
    }

    private const string ASSET_NAME = "UniversalMediaPlayer";

    private const string SETTINGS_FOLDER_PATH = "Resources";
    private const string SETTINGS_FILE_NAME = "UMPSettings";

    private const string WIN_FOLDER_NAME = "Win";
    private const string MAC_FOLDER_NAME = "Mac";
    private const string LIN_FOLDER_NAME = "Linux";
    private const string WEBGL_FOLDER_NAME = "WebGL";
    private const string ANDROID_FOLDER_NAME = "Android";
    private const string IPHONE_FOLDER_NAME = "iOS";

    private const string PLUGINS_FOLDER_NAME = "Plugins";
    private const string X86_FOLDER_NAME = "x86";
    private const string X64_FOLDER_NAME = "x86_64";

    private const string WIN86_REG_KEY = @"SOFTWARE\WOW6432Node\VideoLAN\VLC";
    private const string WIN64_REG_KEY = @"SOFTWARE\VideoLAN\VLC";

    private const string MAC_APPS_FOLDER_NAME = "/Applications";
    private const string MAC_VLC_PACKAGE_NAME = "vlc.app";
    private const string MAC_LIBVLC_PACKAGE_NAME = "libvlc.bundle";
    private const string MAC_PACKAGE_LIB_PATH = @"Contents/MacOS/lib";

    private const string LIN_86_APPS_FOLDER_NAME = "/usr/lib";
    private const string LIN_64_APPS_FOLDER_NAME = "/usr/lib64";

    private const string LIB_WIN_EXT = ".dll";
    private const string LIB_LIN_EXT = ".so";
    private const string LIB_MAC_EXT = ".dylib";

    private static UMPSettings _instance;

    [SerializeField][HideInInspector]
    private bool _useCustomAssetPath;
    [SerializeField][HideInInspector]
    private string _assetPath;
    [SerializeField][HideInInspector]
    private bool _useAudioSource;
    [SerializeField][HideInInspector]
    private bool _useExternalLibs;
    [SerializeField][HideInInspector]
    private string _additionalLibsPath = string.Empty;
    [SerializeField][HideInInspector]
    private bool _useAndroidNative;
    [SerializeField][HideInInspector]
    private bool _useIPhoneNative;

    public static UMPSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                TextAsset asset = Resources.Load(SETTINGS_FILE_NAME) as TextAsset;
                _instance = asset != null ? JsonUtility.FromJson<UMPSettings>(asset.text) : null;

                if (_instance == null)
                {
                    _instance = new UMPSettings();

                    if (Application.isEditor)
                        SaveChanges();
                }
            }

            return _instance;
        }
    }

    private static void SaveChanges()
    {
        if (_instance != null)
        {
            string settings = JsonUtility.ToJson(_instance);
            string settingsFilePath = Path.Combine(_instance.AssetPath, SETTINGS_FOLDER_PATH);

            if (Directory.Exists(settingsFilePath))
            {
                settingsFilePath = Path.Combine(settingsFilePath, SETTINGS_FILE_NAME + ".txt");
                File.WriteAllText(settingsFilePath, settings);
            }
        }
    }

    public bool UseCustomAssetPath
    {
        get { return _useCustomAssetPath; }
        set
        {
            if (!value)
                _assetPath = string.Empty;

            if (_useCustomAssetPath != value)
            {
                _useCustomAssetPath = value;
                SaveChanges();
            }
        }
    }

    public string AssetPath
    {
        get {
            if (string.IsNullOrEmpty(_assetPath))
            {
                _assetPath = Path.Combine("Assets", ASSET_NAME);
                _assetPath = _assetPath.Replace(@"\", "/");
                SaveChanges();
            }

            return _assetPath;
        }
        set
        {
            if (_assetPath != value)
            {
                _assetPath = value;
                SaveChanges();
            }
        }
    }

    public bool IsValidAssetPath
    {
        get
        {
            return Directory.Exists(AssetPath) && Directory.GetFiles(AssetPath).Length > 0;
        }
    }

    public bool UseAudioSource
    {
        get { return _useAudioSource; }
        set
        {
            if (_useAudioSource != value)
            {
                _useAudioSource = value;
                SaveChanges();
            }
        }
    }

    public bool UseExternalLibs
    {
        get { return _useExternalLibs; }
        set
        {
            if (_useExternalLibs != value)
            {
                _useExternalLibs = value;
                SaveChanges();
            }
        }
    }

    public string AdditionalLibsPath
    {
        get { return _additionalLibsPath; }
        set
        {
            if (_additionalLibsPath != value)
            {
                _additionalLibsPath = value;
                SaveChanges();
            }
        }
    }

    public bool UseAndroidNative
    {
        get { return _useAndroidNative; }
        set
        {
            if (_useAndroidNative != value)
            {
                _useAndroidNative = value;
                SaveChanges();
            }
        }
    }

    public bool UseIPhoneNative
    {
        get { return _useIPhoneNative; }
        set
        {
            if (_useIPhoneNative != value)
            {
                _useIPhoneNative = value;
                SaveChanges();
            }
        }
    }

    public static string AssetName
    {
        get
        {
            return ASSET_NAME;
        }
    }

    /// <summary>
    /// Returns the Unity Editor bit mode (Read Only).
    /// </summary>
    public static BitModes EditorBitMode
    {
        get
        {
            return IntPtr.Size == 4 ? BitModes.x86 : BitModes.x64;
        }
    }

    /// <summary>
    /// Returns the UMP supported platform the game is running on (Read Only).
    /// </summary>
    public static Platforms SupportedPlatform
    {
        get
        {
            var supportedPlatform = Platforms.None;
            var platform = Application.platform;

            if (platform == RuntimePlatform.WindowsEditor ||
                        Application.platform == RuntimePlatform.WindowsPlayer)
                supportedPlatform = Platforms.Win;

            if (platform == RuntimePlatform.OSXEditor ||
                        Application.platform == RuntimePlatform.OSXPlayer)
                supportedPlatform = Platforms.Mac;

            if (platform == RuntimePlatform.LinuxPlayer ||
                        (int)Application.platform == 16)
                supportedPlatform = Platforms.Linux;

            if (platform == RuntimePlatform.WebGLPlayer)
                supportedPlatform = Platforms.WebGL;

            if (platform == RuntimePlatform.Android)
                supportedPlatform = Platforms.Android;

            if (platform == RuntimePlatform.IPhonePlayer)
                supportedPlatform = Platforms.iOS;

            return supportedPlatform;
        }
    }

    /// <summary>
    /// Returns the folder name for current Unity Editor bit mode (Read Only).
    /// </summary>
    public static string EditorBitModeFolderName
    {
        get
        {
            return EditorBitMode == BitModes.x86 ? X86_FOLDER_NAME : X64_FOLDER_NAME;
        }
    }

    /// <summary>
    /// Returns the platform folder name for specific platform.
    /// </summary>
    /// <param name="platform">UMP supported platform</param>
    /// <returns></returns>
    public static string PlatformFolderName(Platforms platform)
    {
        switch (platform)
        {
            case Platforms.Win:
                return WIN_FOLDER_NAME;

            case Platforms.Mac:
                return MAC_FOLDER_NAME;

            case Platforms.Linux:
                return LIN_FOLDER_NAME;

            case Platforms.WebGL:
                return WEBGL_FOLDER_NAME;

            case Platforms.Android:
                return ANDROID_FOLDER_NAME;

            case Platforms.iOS:
                return IPHONE_FOLDER_NAME;
        }

        return string.Empty;
    }

    /// <summary>
    /// Returns the folder name for current platform the game is running on (Read Only).
    /// </summary>
    public static string RuntimePlatformFolderName
    {
        get
        {
            return PlatformFolderName(SupportedPlatform);
        }
    }

    /// <summary>
    /// Returns the libraries path for specific platform.
    /// </summary>
    /// <param name="platform">UMP supported platform</param>
    /// <param name="externalSpace">Use external space (for libraries that installed on your system)</param>
    /// <returns></returns>
    public static string PlatformLibraryPath(Platforms platform, bool externalSpace)
    {
        var umpHelper = Instance;
        string libraryPath = string.Empty;

        if (platform != Platforms.None)
        {
            if (!externalSpace)
            {
                if (Application.isEditor)
                {
                    if (umpHelper.IsValidAssetPath)
                    {
                        libraryPath = Path.Combine(umpHelper.AssetPath, PLUGINS_FOLDER_NAME);
                        libraryPath = Path.Combine(libraryPath, PlatformFolderName(platform));

                        if (platform == Platforms.Win || platform == Platforms.Mac || platform == Platforms.Linux)
                            libraryPath = Path.Combine(libraryPath, EditorBitModeFolderName);
                    }
                }
                else
                {
                    libraryPath = Path.Combine(Application.dataPath, PLUGINS_FOLDER_NAME);

                    if (platform == Platforms.Linux)
                        libraryPath = Path.Combine(libraryPath, EditorBitModeFolderName);
                }

                if (platform == Platforms.Mac)
                    libraryPath = Path.Combine(libraryPath, Path.Combine(MAC_LIBVLC_PACKAGE_NAME, MAC_PACKAGE_LIB_PATH));

                if (!Directory.Exists(libraryPath))
                    libraryPath = string.Empty;
            }
            else
            {
                if (platform == Platforms.Win)
                {
                    var registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(EditorBitMode == BitModes.x86 ? WIN86_REG_KEY : WIN64_REG_KEY);

                    if (registryKey != null)
                        libraryPath = registryKey.GetValue("InstallDir").ToString();
                }

                if (platform == Platforms.Mac)
                {
                    var appsFolderInfo = new DirectoryInfo(MAC_APPS_FOLDER_NAME);
                    var packages = appsFolderInfo.GetDirectories();

                    foreach (var package in packages)
                    {
                        if (package.FullName.ToLower().Contains(MAC_VLC_PACKAGE_NAME))
                            libraryPath = Path.Combine(package.FullName, MAC_PACKAGE_LIB_PATH);
                    }
                }

                if (platform == Platforms.Linux)
                {
                    DirectoryInfo appsFolderInfo = null;

                    if (Directory.Exists(LIN_86_APPS_FOLDER_NAME))
                        appsFolderInfo = new DirectoryInfo(LIN_86_APPS_FOLDER_NAME);

                    if (appsFolderInfo != null)
                    {
                        var appsLibs = appsFolderInfo.GetFiles();

                        foreach (var lib in appsLibs)
                        {
                            if (lib.FullName.ToLower().Contains(Wrapper.LibraryVLCName))
                                libraryPath = LIN_86_APPS_FOLDER_NAME;
                        }
                    }

                    if (libraryPath.Equals(string.Empty))
                    {
                        if (Directory.Exists(LIN_64_APPS_FOLDER_NAME))
                            appsFolderInfo = new DirectoryInfo(LIN_64_APPS_FOLDER_NAME);

                        if (appsFolderInfo != null)
                        {
                            var appsLibs = appsFolderInfo.GetFiles();

                            foreach (var lib in appsLibs)
                            {
                                if (lib.FullName.ToLower().Contains(Wrapper.LibraryVLCName))
                                    libraryPath = LIN_64_APPS_FOLDER_NAME;
                            }
                        }
                    }
                }
            } 

            if (!libraryPath.Equals(string.Empty))
                libraryPath = Path.GetFullPath(libraryPath + Path.AltDirectorySeparatorChar);
        }

        return libraryPath;
    }

    /// <summary>
    /// Returns the folder name for current platform the game is running on (Read Only).
    /// </summary>
    public static string RuntimePlatformLibraryPath(bool externalSpace)
    {
        return PlatformLibraryPath(SupportedPlatform, externalSpace);
    }

    public static bool IsLibrariesExists(string directory, params string[] libraryNames)
    {
        var libExt = LIB_WIN_EXT;
        var additionalPath = string.Empty;
        var supportedPlatform = SupportedPlatform;

        if (!directory.Equals(string.Empty))
            additionalPath = Directory.GetParent(directory.TrimEnd(Path.DirectorySeparatorChar)).FullName;

        if (supportedPlatform == Platforms.Mac)
            libExt = LIB_MAC_EXT;

        if (supportedPlatform == Platforms.Linux)
        {
            if (Instance.UseExternalLibs)
                libExt = LIB_LIN_EXT + ".5";
            else
                libExt = LIB_LIN_EXT;
        }

        foreach (var libraryName in libraryNames)
        {
            var libPath = Path.Combine(directory, libraryName + libExt);
            if (!File.Exists(libPath))
                return false;
        }

        return true;
    }
}
