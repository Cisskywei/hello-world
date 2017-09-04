using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif

public class UMPPostBuilds : MonoBehaviour
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
                BuildWindowsPlayer32(path);
                break;

            case BuildTarget.StandaloneWindows64:
                BuildWindowsPlayer64(path);
                break;

            case BuildTarget.StandaloneOSXIntel64:
                BuildMacPlayer64(path);
                break;

            case BuildTarget.StandaloneLinux:
                BuildLinuxPlayer32(path);
                break;

            case BuildTarget.StandaloneLinux64:
                BuildLinuxPlayer64(path);
                break;

            case BuildTarget.StandaloneLinuxUniversal:
                BuildLinuxPlayerUniversal(path);
                break;

            case BuildTarget.iOS:
                BuildForiOS(path);
                break;
        }
    }

    private static void BuildForiOS(string path)
    {
#if UNITY_IPHONE
        string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        Debug.Log("Build iOS. path: " + projPath);

        PBXProject proj = new PBXProject();
        var file = File.ReadAllText(projPath);
        proj.ReadFromString(file);

        string target = proj.TargetGuidByName("Unity-iPhone");

        if (!UMPPreference.Instance.UseIPhoneNative)
        {
            // Activate Background Mode for Audio
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;
            var buildKey = "UIBackgroundModes";
            rootDict.CreateArray(buildKey).AddString("audio");
            File.WriteAllText(plistPath, plist.WriteToString());

            string fileGuid = proj.AddFile("usr/lib/" + "libc++.dylib", "Frameworks/" + "libc++.dylib", PBXSourceTree.Sdk);
            proj.AddFileToBuild(target, fileGuid);
            fileGuid = proj.AddFile("usr/lib/" + "libz.dylib", "Frameworks/" + "libz.dylib", PBXSourceTree.Sdk);
            proj.AddFileToBuild(target, fileGuid);
            fileGuid = proj.AddFile("usr/lib/" + "libz.tbd", "Frameworks/" + "libz.tbd", PBXSourceTree.Sdk);
            proj.AddFileToBuild(target, fileGuid);

            fileGuid = proj.AddFile("usr/lib/" + "libbz2.dylib", "Frameworks/" + "libbz2.dylib", PBXSourceTree.Sdk);
            proj.AddFileToBuild(target, fileGuid);
            fileGuid = proj.AddFile("usr/lib/" + "libbz2.tbd", "Frameworks/" + "libbz2.tbd", PBXSourceTree.Sdk);
            proj.AddFileToBuild(target, fileGuid);
        }
        else
        {
            proj.AddFrameworkToProject(target, "CoreImage.framework", true);
        }

        File.WriteAllText(projPath, proj.WriteToString());
#endif
    }

    public static void BuildWindowsPlayer32(string path)
    {
        string buildPath = Path.GetDirectoryName(path);
        string dataPath = buildPath + "/" + Path.GetFileNameWithoutExtension(path) + "_Data";

        if (!string.IsNullOrEmpty(buildPath))
        {
            if (!UMPPreference.Instance.UseExternalLibs)
            {
                CopyPlugins(Application.dataPath + "/UniversalMediaPlayer/Plugins/Win/x86/plugins/", dataPath + "/Plugins/plugins/");
            }
            else
            {
                if (File.Exists(dataPath + "/Plugins/libvlc.dll"))
                    File.Delete(dataPath + "/Plugins/libvlc.dll");

                if (File.Exists(dataPath + "/Plugins/libvlccore.dll"))
                    File.Delete(dataPath + "/Plugins/libvlccore.dll");
            }
        }
        Debug.Log("Standalone Windows (x86) build is completed: " + path);
    }

    public static void BuildWindowsPlayer64(string path)
    {
        string buildPath = Path.GetDirectoryName(path);
        string dataPath = buildPath + "/" + Path.GetFileNameWithoutExtension(path) + "_Data";

        if (!string.IsNullOrEmpty(buildPath))
        {
            if (!UMPPreference.Instance.UseExternalLibs)
            {
                CopyPlugins(Application.dataPath + "/UniversalMediaPlayer/Plugins/Win/x86_64/plugins/", dataPath + "/Plugins/plugins/");
            }
            else
            {
                if (File.Exists(dataPath + "/Plugins/libvlc.dll"))
                    File.Delete(dataPath + "/Plugins/libvlc.dll");

                if (File.Exists(dataPath + "/Plugins/libvlccore.dll"))
                    File.Delete(dataPath + "/Plugins/libvlccore.dll");
            }
        }
        Debug.Log("Standalone Windows (x86_x64) build is completed: " + path);
    }

    public static void BuildMacPlayer64(string path)
    {
        string buildPath = Path.GetDirectoryName(path);
        string dataPath = buildPath + "/" + Path.GetFileName(path) + "/Contents";

        if (!string.IsNullOrEmpty(buildPath) && UMPPreference.Instance.UseExternalLibs)
        {
            if (Directory.Exists(dataPath + "/Plugins/libvlc.bundle"))
                Directory.Delete(dataPath + "/Plugins/libvlc.bundle", true);
        }

        Debug.Log("Standalone Mac (x86_x64) build is completed: " + dataPath);
    }

    public static void BuildLinuxPlayer32(string path)
    {
        string buildPath = Path.GetDirectoryName(path);
        string dataPath = buildPath + "/" + Path.GetFileNameWithoutExtension(path) + "_Data";
        string umpLauncherPath = Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/UMPInstaller.sh";
        string umpRemoverPath = Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/UMPRemover.sh";

        if (!string.IsNullOrEmpty(buildPath))
        {
            if (!UMPPreference.Instance.UseExternalLibs)
            {
                string vlcFolderPath32 = dataPath + "/Plugins/x86/vlc";
                if (!Directory.Exists(vlcFolderPath32))
                    Directory.CreateDirectory(vlcFolderPath32);

                CopyPlugins(Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/x86/plugins/", vlcFolderPath32 + "/plugins/");
            }
            else
            {
                if (File.Exists(dataPath + "/Plugins/x86/libvlc.so"))
                    File.Delete(dataPath + "/Plugins/x86/libvlc.so");

                if (File.Exists(dataPath + "/Plugins/x86/libvlccore.so"))
                    File.Delete(dataPath + "/Plugins/x86/libvlccore.so");
            }

            CopyShellScript(umpLauncherPath, buildPath);
            CopyShellScript(umpRemoverPath, buildPath);
        }
        Debug.Log("Standalone Linux (x86) build is completed: " + path);
    }

    public static void BuildLinuxPlayer64(string path)
    {
        string buildPath = Path.GetDirectoryName(path);
        string dataPath = buildPath + "/" + Path.GetFileNameWithoutExtension(path) + "_Data";
        string umpLauncherPath = Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/UMPInstaller.sh";
        string umpRemoverPath = Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/UMPRemover.sh";

        if (!string.IsNullOrEmpty(buildPath))
        {
            if (!UMPPreference.Instance.UseExternalLibs)
            {
                string vlcFolderPath64 = dataPath + "/Plugins/x86_64/vlc";
                if (!Directory.Exists(vlcFolderPath64))
                    Directory.CreateDirectory(vlcFolderPath64);

                CopyPlugins(Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/x86_64/plugins/", vlcFolderPath64 + "/plugins/");
            }
            else
            {
                if (File.Exists(dataPath + "/Plugins/x86_64/libvlc.so"))
                    File.Delete(dataPath + "/Plugins/x86_64/libvlc.so");

                if (File.Exists(dataPath + "/Plugins/x86_64/libvlccore.so"))
                    File.Delete(dataPath + "/Plugins/x86_64/libvlccore.so");
            }

            CopyShellScript(umpLauncherPath, buildPath);
            CopyShellScript(umpRemoverPath, buildPath);
        }
        Debug.Log("Standalone Linux (x86_x64) build is completed: " + path);
    }

    public static void BuildLinuxPlayerUniversal(string path)
    {
        string buildPath = Path.GetDirectoryName(path);
        string dataPath = buildPath + "/" + Path.GetFileNameWithoutExtension(path) + "_Data";
        string umpLauncherPath = Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/UMPInstaller.sh";
        string umpRemoverPath = Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/UMPRemover.sh";

        if (!string.IsNullOrEmpty(buildPath))
        {
            if (!UMPPreference.Instance.UseExternalLibs)
            {
                string vlcFolderPath32 = dataPath + "/Plugins/x86/vlc";
                if (!Directory.Exists(vlcFolderPath32))
                    Directory.CreateDirectory(vlcFolderPath32);

                string vlcFolderPath64 = dataPath + "/Plugins/x86_64/vlc";
                if (!Directory.Exists(vlcFolderPath64))
                    Directory.CreateDirectory(vlcFolderPath64);

                CopyPlugins(Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/x86/plugins/", vlcFolderPath32 + "/plugins/");
                CopyPlugins(Application.dataPath + "/UniversalMediaPlayer/Plugins/Linux/x86_64/plugins/", vlcFolderPath64 + "/plugins/");
            }
            else
            {
                if (File.Exists(dataPath + "/Plugins/x86/libvlc.so"))
                    File.Delete(dataPath + "/Plugins/x86/libvlc.so");

                if (File.Exists(dataPath + "/Plugins/x86/libvlccore.so"))
                    File.Delete(dataPath + "/Plugins/x86/libvlccore.so");

                if (File.Exists(dataPath + "/Plugins/x86_64/libvlc.so"))
                    File.Delete(dataPath + "/Plugins/x86_64/libvlc.so");

                if (File.Exists(dataPath + "/Plugins/x86_64/libvlccore.so"))
                    File.Delete(dataPath + "/Plugins/x86_64/libvlccore.so");
            }

            CopyShellScript(umpLauncherPath, buildPath);
            CopyShellScript(umpRemoverPath, buildPath);
        }
        Debug.Log("Standalone Linux (Universal) build is completed: " + path);
    }

    private static void CopyPlugins(string sourcePath, string targetPath)
    {
        string fileName = string.Empty;
        string destFile = targetPath;

        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        string[] directories = Directory.GetDirectories(sourcePath);

        foreach (var d in directories)
        {
            string[] files = Directory.GetFiles(d);

            if (files.Length > 0)
            {
                destFile = Path.Combine(targetPath, Path.GetFileName(d));
                Directory.CreateDirectory(destFile);
            }

            foreach (var s in files)
            {
                if (Path.GetExtension(s).Equals(".meta"))
                    continue;

                fileName = Path.GetFileName(s);
                File.Copy(s, Path.Combine(destFile, fileName), false);
            }
        }
    }

    private static void CopyShellScript(string scriptPath, string targetPath)
    {
        string scriptName = Path.GetFileName(scriptPath);
        File.Copy(scriptPath, Path.Combine(targetPath, scriptName), false);
    }
}
