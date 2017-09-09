using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ko.NetFram
{
    public class PPTToPicture
    {
        public static PPTToPicture getInstance()
        {
            return Singleton<PPTToPicture>.getInstance();
        }

        //pptpath = "C:\\Users\\Administrator\\Desktop\\testfile\\"
        public int ConvertPPT2Image(string pptpath, string imagepath, string imagename = "ppt", int width = 1920, int height = 1080, string format = "JPG")
        {
            int ret = -1;
            try
            {
                if(!File.Exists(pptpath))
                {
                    return -1;
                }

                Microsoft.Office.Interop.PowerPoint.Application pptApplication = new ApplicationClass();

                if(pptApplication == null)
                {
                    return -1;
                }

                Presentations p = pptApplication.Presentations;

                if (p == null)
                {
                    return -1;
                }

                Presentation pptPresentation = p.Open(pptpath, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

                if (pptPresentation == null)
                {
                    pptApplication.Quit();
                    return -1;
                }

                if (!Directory.Exists(imagepath))
                {
                    Directory.CreateDirectory(imagepath);
                }

                string name = imagepath + "\\" + imagename + "_{0}." + format.ToLower();
                int i = 0;
                foreach (Slide pptSlide in pptPresentation.Slides)
                {
                    string imgname = string.Format(name, i.ToString());
                    if (File.Exists(imgname))
                    {
                        File.Create(imgname);
                    }
                    pptSlide.Export(imgname, format, width, height);
                    i++;
                }

                pptPresentation.Close();
                pptApplication.Quit();

                ret = i;
            }
            //如果转换不成功则抛出异常
            catch (UnityException e)
            {
                throw (e);
            }

            return ret;
        }

        private byte[] LoadImageByIO(string imagefullpath)
        {
            if(!File.Exists(imagefullpath))
            {
                return null;
            }

            //创建文件读取流
            FileStream fileStream = new FileStream(imagefullpath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            return bytes;
        }

        public Sprite[] LoadImage2SpriteByIO(string imagepath, int count = 0, string imagename = "ppt", int width = 960, int height = 720, string format = "PNG")
        {
            if (!Directory.Exists(imagepath))
            {
                Debug.Log("文件不存在" + imagepath);
                return null;
            }

            FileInfo[] filelist = FileOpenWrite.getInstance().findByFilePath(imagepath);

            if(filelist == null)
            {
                return null;
            }

            count = filelist.Length;
            if (count <= 0)
            {
                return null;
            }

            Sprite[] ppt = new Sprite[count];

            string name;
            string index;
            int[] indexs = new int[count];
            for (int i = 0; i < count; i++)
            {
                FileInfo f = filelist[i];
                indexs[i] = -1;
                //创建文件读取流
                FileStream fileStream = f.Open(FileMode.Open, FileAccess.Read);
                fileStream.Seek(0, SeekOrigin.Begin);
                //创建文件长度缓冲区
                byte[] bytes = new byte[fileStream.Length];
                //读取文件
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                //释放文件读取流
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;
                if (bytes == null || bytes.Length <= 0)
                {
                    continue;
                }

                name = f.Name;
                int start = name.LastIndexOf('_')+1;
                int end = name.LastIndexOf('.');
                index = name.Substring(start, end-start);
                indexs[i] = Convert.ToInt32(index);

                //创建Texture
                Texture2D texture = new Texture2D(width, height);
                texture.LoadImage(bytes);
                if (texture != null)
                {
                    //创建Sprite
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    ppt[i] = sprite;
                }
            }

            Sprite s;
            for (int j=0;j<count;j++)
            {
                if(indexs[j]<0 || indexs[j] == j)
                {
                    continue;
                }

                s = ppt[j];
                ppt[j] = ppt[indexs[j]];
                ppt[indexs[j]] = s;
            }

            //string imgname = imagepath + "\\" + imagename + "_{0}." + format.ToLower();
            //string path;

            //for(int i=0;i<count;i++)
            //{
            //    path = string.Format(imgname, i.ToString());
            //    byte[] b = LoadImageByIO(path);
            //    if(b == null || b.Length <= 0)
            //    {
            //        continue;
            //    }

            //    //创建Texture
            //    Texture2D texture = new Texture2D(width, height);
            //    texture.LoadImage(b);
            //    if(texture != null)
            //    {
            //        //创建Sprite
            //        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            //        ppt[i] = sprite;
            //    }
            //}

            return ppt;
        }
    }
}
