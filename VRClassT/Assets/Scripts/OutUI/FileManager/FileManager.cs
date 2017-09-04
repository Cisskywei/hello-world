using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using UnityEngine;

public class FileManager {

    public static FileManager getInstance()
    {
        return Singleton<FileManager>.getInstance();
    }

    //    public FileManager()
    //    {
    //        #if UNITY_EDITOR
    //                approotpath = Application.persistentDataPath + "/";
    //        Debug.Log(approotpath + " 根目录 ");
    //#else
    //                approotpath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
    //#endif

    //        LogText.getInstance().Log(approotpath);
    //    }

    //string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
    //result: X:\xxx\xxx\ (.exe文件所在的目录+"\")
    private string rootpath = string.Empty;
    public string approotpath
    {
        get
        {
            if(rootpath == string.Empty)
            {
#if UNITY_EDITOR
                rootpath = Application.persistentDataPath + "\\";
                Debug.Log(rootpath + " 根目录 ");
#else
                rootpath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
#endif
            }

            return rootpath;
        }
    }

    // config
    //课程资料目录 相对根目录
    public string CourseMaterial { get { return approotpath + "CourseMaterial"; } }
    //和vr课件传递参数的目录
    public string TransitionCache { get { return approotpath + "TransitionCache"; } }

    // 课程资料
    //ppt 文件
    public string ppt { get { return CourseMaterial + "\\ppt"; } }
    // ppt 转换后的图片
    public string pptimage { get { return ppt + "\\pptimage"; } }
    // 视频缓存
    public string videopath { get { return CourseMaterial + "\\video"; } }
    
    // exe缓存
    public string exepath { get { return CourseMaterial + "\\exepath"; } }

    public string GetCourseMaterialPath()
    {
        return CourseMaterial;
    }

    public string GetPPTImagePath(string pptfilename)
    {
        if(!Directory.Exists(pptimage))
        {
            Directory.CreateDirectory(pptimage);
        }

        if (pptfilename.Contains("."))
        {
            int diff = pptfilename.LastIndexOf('.');
            pptfilename = pptfilename.Substring(0, diff);
        }

        if (!Directory.Exists(pptimage + "\\" + pptfilename))
        {
            Directory.CreateDirectory(pptimage + "\\" + pptfilename);
        }

        return pptimage + "\\" + pptfilename;
    }

    // 根据服务器文件type 和 文件名获取文件路径
    public string GetFileSavePath(string filename, string typ)
    {
        string path = CourseMaterial;

        Enums.ContentDataType cdt = GetFileContenType(filename, typ);

        switch (cdt)
        {
            case Enums.ContentDataType.Exe:
                path = exepath;
                break;
            case Enums.ContentDataType.PanoramicVideo:
                path = videopath + "\\vr";
                break;
            case Enums.ContentDataType.OrdinaryVideo:
                path = videopath + "\\normal";
                break;
            case Enums.ContentDataType.Panorama:
                break;
            case Enums.ContentDataType.Picture:
                break;
            case Enums.ContentDataType.PPt:
                path = ppt;
                break;
            case Enums.ContentDataType.Zip:
                path = exepath;
                break;
            default:
                break;
        }

        return path;
    }

    // 根据后缀名获取文件路径

    // 根据文件类型获取文件路径

    // 文件类型转换
    public string GetFileSuffixName(string filename)
    {
        string suffix = string.Empty;

        if (filename.Contains("."))
        {
            int diff = filename.LastIndexOf('.');
            suffix = filename.Substring(diff);
            //Debug.Log(filename + "文件后缀名 " + suffix);
        }

        return suffix;
    }

    public Enums.ContentDataType GetFileContenType(string filename, string typ)
    {
        Enums.ContentDataType ret = Enums.ContentDataType.None;

        string suffix = GetFileSuffixName(filename);

        if (suffix == string.Empty)
        {
            return ret;
        }

        ret = GetFileContenTypeBySuffix(suffix, typ);

        return ret;
    }

    public Enums.ContentDataType GetFileContenTypeBySuffix(string suffix, string typ)
    {
        Enums.ContentDataType ret = Enums.ContentDataType.None;

        if (suffix == string.Empty)
        {
            return ret;
        }

        switch (suffix)
        {
            case ".mp4":
                if (typ == "vr")
                {
                    ret = Enums.ContentDataType.PanoramicVideo;
                }
                else if (typ == "video")
                {
                    ret = Enums.ContentDataType.OrdinaryVideo;
                }
                else
                {
                    ret = Enums.ContentDataType.OrdinaryVideo;
                }
                break;
            case ".ppt":
            case ".pptx":
                ret = Enums.ContentDataType.PPt;
                break;
            case ".exe":
                ret = Enums.ContentDataType.Exe;
                break;
            case ".zip":
                ret = Enums.ContentDataType.Zip;
                break;
            default:
                break;
        }

        return ret;
    }
/*
    // 修改文件夹权限
    public void ChangeDirSecurity(string dirPath)
    {
        //获取文件夹信息
        DirectoryInfo dir = new DirectoryInfo(dirPath);
        //获得该文件夹的所有访问权限
        System.Security.AccessControl.DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
        //设定文件ACL继承
        InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        //添加ereryone用户组的访问权限规则 完全控制权限
        FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
        //添加Users用户组的访问权限规则 完全控制权限
        FileSystemAccessRule usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
        bool isModified = false;
        dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
        dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
        //设置访问权限
        dir.SetAccessControl(dirSecurity);

    }

    public void ChangeDirSecurity(DirectoryInfo dir)
    {
        //获得该文件夹的所有访问权限
        System.Security.AccessControl.DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
        //设定文件ACL继承
        InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        ////添加ereryone用户组的访问权限规则 完全控制权限
        //FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
        ////添加Users用户组的访问权限规则 完全控制权限
        //FileSystemAccessRule usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
        //bool isModified = false;
        //dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
        //dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
        //设置访问权限
        dir.SetAccessControl(dirSecurity);

    }
*/
}
