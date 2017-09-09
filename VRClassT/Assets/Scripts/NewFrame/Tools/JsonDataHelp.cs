using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ko.NetFram
{
    public class JsonDataHelp
    {

        public static JsonDataHelp getInstance()
        {
            return Singleton<JsonDataHelp>.getInstance();
        }

        public T JsonDeserialize<T>(string jsondata)
        {
            T v = JsonUtility.FromJson<T>(jsondata);

            return v;
        }

        public string JsonSerialize<T>(T jsonobjectdata)
        {
            string v = JsonUtility.ToJson(jsonobjectdata);

            return v;
        }

        //编码 string   to  base64   code_type : gb2312 utf-8
        public string EncodeBase64(string code_type, string code)
        {
            // System.Text.Encoding encode = System.Text.Encoding.ASCII
            System.Text.Encoding encodesys = System.Text.Encoding.UTF8;
            if (code_type != null)
            {
                encodesys = Encoding.GetEncoding(code_type);
            }
            string encode = "";
            byte[] bytes = encodesys.GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        //解码 base64  to  string
        public string DecodeBase64(string code_type, string code)
        {
            if(code == null)
            {
                return null;
            }

            // System.Text.Encoding encode = System.Text.Encoding.ASCII
            System.Text.Encoding encodesys = System.Text.Encoding.UTF8;
            if (code_type != null)
            {
                encodesys = Encoding.GetEncoding(code_type);
            }

            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = encodesys.GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
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