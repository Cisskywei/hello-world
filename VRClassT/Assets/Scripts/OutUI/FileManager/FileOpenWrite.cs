using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ko.NetFram
{
    /// <summary>
    /// 此类负责文件读写的具体实现 
    /// </summary>
    public class FileOpenWrite
    {
        public static FileOpenWrite getInstance()
        {
            return Singleton<FileOpenWrite>.getInstance();
        }

        //创建文件路径
        public bool createFilePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                return true;
            }

            return false;
        }

        // 指定路径下所有文件
        public FileInfo[] findByFilePath(string fullpath)
        {
            FileInfo[] findfiles = null;

            if (!Directory.Exists(fullpath))
            {
                return findfiles;
            }

            DirectoryInfo direction = new DirectoryInfo(fullpath);
            findfiles = direction.GetFiles("*", SearchOption.AllDirectories);

 //           Debug.Log(files.Length);

            return findfiles;
        }

        // 指定路径下特定后缀文件
        public FileInfo[] findByFileSuffix(string fullpath, string suffix)
        {
            FileInfo[] findfiles = null;

            if (!Directory.Exists(fullpath))
            {
                return findfiles;
            }

            if (!suffix.Contains("."))
            {
                suffix = "." + suffix;
            }

            DirectoryInfo direction = new DirectoryInfo(fullpath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            int count = 0;
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith(suffix))
                {
                    continue;
                }
                findfiles[count++] = files[i];
            }

            return findfiles;
        }

        // 更改文件名
        public bool changeFileName(string src, string dec)
        {
            if (src == null || dec == null)
            {
                return false;
            }

            FileInfo f = new FileInfo(src);

            if (!f.Exists)
            {
                return false;
            }

            f.MoveTo(dec);

            return true;
        }

        // 删除文件
        public bool deleteFileByPath(string path)
        {
            if (path == null)
            {
                return false;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return true;
        }

        // 文件读写
        public void WriteText(string text, string path)
        {
            if (!File.Exists(path))
            {
                // 文件不存在
                File.CreateText(path);
            }

            text = RijndaelEncrypt(text, this.getKey());
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write(text);
            streamWriter.Close();
        }

        public string ReadText(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            StreamReader sr = new StreamReader(path);

            if (sr == null)
            {
                return null;
            }

            string text = sr.ReadToEnd();

            if (text.Length > 0)
            {
                text = RijndaelDecrypt(text, this.getKey());
            }

            sr.Close();

            return text;
        }

        private string getKey()
        {
            string key = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

            // TODO

            return key;
        }

        ///////////////////////////////// 加密解密 //////////////////////////////
        public static string RijndaelEncrypt(string pString, string pKey)
        {
            //密钥
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
            //待加密明文数组
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(pString);

            //Rijndael解密算法
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();

            //返回加密后的密文
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// ijndael解密算法
        /// </summary>
        /// <param name="pString">待解密的密文</param>
        /// <param name="pKey">密钥,长度可以为:64位(byte[8]),128位(byte[16]),192位(byte[24]),256位(byte[32])</param>
        /// <param name="iv">iv向量,长度为128（byte[16])</param>
        /// <returns></returns>
        public static String RijndaelDecrypt(string pString, string pKey)
        {
            //解密密钥
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
            //待解密密文数组
            byte[] toEncryptArray = Convert.FromBase64String(pString);

            //Rijndael解密算法
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();

            //返回解密后的明文
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}

