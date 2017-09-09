using UnityEngine;  
using System.Collections;  
using System;  
using System.IO;  
using ICSharpCode.SharpZipLib.Checksums;  
using ICSharpCode.SharpZipLib.Zip;  

public class ZipHelper : MonoBehaviour  
{  
	/// <summary>  
	/// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）  
	/// </summary>  
	/// <param name="dirPath">被压缩的文件夹夹路径</param>  
	/// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>  
	/// <param name="err">出错信息</param>  
	/// <returns>是否压缩成功</returns>  
	public static bool ZipFile(string dirPath, string zipFilePath, out string err)  
	{  
		err = "";  
		if (dirPath == string.Empty)  
		{  
			err = "要压缩的文件夹不能为空！";  
			return false;  
		}  
		if (!Directory.Exists(dirPath))  
		{  
			err = "要压缩的文件夹不存在！";  
			return false;  
		}  
		//压缩文件名为空时使用文件夹名＋.zip  
		if (zipFilePath == string.Empty)  
		{  
			if (dirPath.EndsWith("//"))  
			{  
				dirPath = dirPath.Substring(0, dirPath.Length - 1);  
			}  
			zipFilePath = dirPath + ".zip";  
		}  

		try  
		{  
			string[] filenames = Directory.GetFiles(dirPath);  
			using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))  
			{  
				s.SetLevel(9);  
				byte[] buffer = new byte[4096];  
				foreach (string file in filenames)  
				{  
					ZipEntry entry = new ZipEntry(Path.GetFileName(file));  
					entry.DateTime = DateTime.Now;  
					s.PutNextEntry(entry);  
					using (FileStream fs = File.OpenRead(file))  
					{  
						int sourceBytes;  
						do  
						{  
							sourceBytes = fs.Read(buffer, 0, buffer.Length);  
							s.Write(buffer, 0, sourceBytes);  
						} while (sourceBytes > 0);  
					}  
				}  
				s.Finish();  
				s.Close();  
			}  
		}  
		catch (Exception ex)  
		{  
			err = ex.Message;  
			return false;  
		}  
		return true;  
	}  

	/// <summary>  
	/// 功能：解压zip格式的文件。  
	/// </summary>  
	/// <param name="zipFilePath">压缩文件路径</param>  
	/// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
	/// <param name="err">出错信息</param>  
	/// <returns>解压是否成功</returns>  
	public static bool UnZipFile(string zipFilePath, string unZipDir, out string err)  
	{  
		err = "";  
		if (zipFilePath == string.Empty)  
		{  
			err = "压缩文件不能为空！";  
			return false;  
		}  
		if (!File.Exists(zipFilePath))  
		{  
			err = "压缩文件不存在！";  
			return false;  
		}  
		//解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
		if (unZipDir == string.Empty)  
			unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));  
		if (!unZipDir.EndsWith("//"))  
			unZipDir += "//";  
		if (!Directory.Exists(unZipDir))  
			Directory.CreateDirectory(unZipDir);  

		try  
		{  
			using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))  
			{  

				ZipEntry theEntry;  
				while ((theEntry = s.GetNextEntry()) != null)  
				{  
					string directoryName = Path.GetDirectoryName(theEntry.Name);  
					string fileName = Path.GetFileName(theEntry.Name);  
					if (directoryName.Length > 0)  
					{  
						Directory.CreateDirectory(unZipDir + directoryName);  
					}  
					if (!directoryName.EndsWith("//"))  
						directoryName += "//";  
					if (fileName != String.Empty)  
					{  
						using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))  
						{  

							int size = 2048;  
							byte[] data = new byte[2048];  
							while (true)  
							{  
								size = s.Read(data, 0, data.Length);  
								if (size > 0)  
								{  
									streamWriter.Write(data, 0, size);  
								}  
								else  
								{  
									break;  
								}  
							}  
						}  
					}  
				}//while  
			}  
		}  
		catch (Exception ex)  
		{  
			err = ex.Message;  
			return false;  
		}  
		return true;  
	}//解压结束 

//	string sourcesPath = "";  
//	string targetPath = "";  
//	string filename = "test.zip";  
//
//	void Start()  
//	{  
//		sourcesPath = Application.dataPath + "/Zip";  
//		targetPath = Application.dataPath + "/CompressZip";  
//		Debug.Log("sourcesPaht is:" + sourcesPath + "   " + targetPath);  
//		StartCoroutine(StartUnZip());  
//	}  
//
//
//	IEnumerator StartUnZip()  
//	{  
//		string uri = "file:///" + sourcesPath + "/" + filename;  
//		WWW www = new WWW(uri);  
//		Debug.Log("uri is:" + uri);  
//		yield return www;  
//
//		if (www.error != null)  
//		{  
//			Debug.Log("loading is failed");  
//		}  
//
//		if (www.isDone)  
//		{  
//			Debug.Log("www is success");  
//		}  
//
//		//如果路径不存在 则创建  
//		if (!Directory.Exists(targetPath))  
//		{  
//			Directory.CreateDirectory(targetPath);  
//		}  
//		//把当前的压缩包写入到指定的路径下  
//		File.WriteAllBytes(targetPath + "/" + filename, www.bytes);  
//		//ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;  
//		StartCoroutine(UnzipWithPath(targetPath + "/" + filename));  
//	}  
//
//
//	private int totalCount;  
//	private int doneCount;  
//	private int indicatorStep = 1;  
//	public IEnumerator UnzipWithPath(string path)  
//	{  
//		Debug.Log("paht is:" + path);  
//		//这是根目录的路径  
//		string dirPath = path.Substring(0, path.LastIndexOf('/'));  
//		Debug.Log("dirpath is:" + dirPath);  
//		//ZipEntry：文件条目 就是该目录下所有的文件列表(也就是所有文件的路径)  
//		ZipEntry zip = null;  
//		//输入的所有的文件流都是存储在这里面的  
//		ZipInputStream zipInStream = null;  
//		//读取文件流到zipInputStream  
//		zipInStream = new ZipInputStream(File.OpenRead(path));  
//		//循环读取Zip目录下的所有文件  
//		while ((zip = zipInStream.GetNextEntry()) != null)  
//		{  
//			Debug.Log("name is:" + zip.Name + " zipStream " + zipInStream);  
//			UnzipFile(zip, zipInStream, dirPath);  
//			doneCount++;  
//			if (doneCount % indicatorStep == 0)  
//			{  
//				yield return new WaitForEndOfFrame();  
//			}  
//		}  
//		try  
//		{  
//			zipInStream.Close();  
//		}  
//		catch (Exception ex)  
//		{  
//			Debug.Log("UnZip Error");  
//			throw ex;  
//		}  
//
//		Debug.Log("解压完成：" + path);  
//	}  
//	static void UnzipFile(ZipEntry zip, ZipInputStream zipInStream, string dirPath)  
//	{  
//		try  
//		{  
//			//文件名不为空  
//			if (!string.IsNullOrEmpty(zip.Name))  
//			{  
//				string filePath = dirPath;  
//				filePath += ("/" + zip.Name);  
//
//				//如果是一个新的文件路径　这里需要创建这个文件路径  
//				if (IsDirectory(filePath))  
//				{  
//					Debug.Log("Create  file paht " + filePath);  
//					if (!Directory.Exists(filePath))  
//					{  
//						Directory.CreateDirectory(filePath);  
//					}  
//				}  
//				else  
//				{  
//					FileStream fs = null;  
//					//当前文件夹下有该文件  删掉  重新创建  
//					if (File.Exists(filePath))  
//					{  
//						File.Delete(filePath);  
//					}  
//					fs = File.Create(filePath);  
//					int size = 2048;  
//					byte[] data = new byte[2048];  
//					//每次读取2MB  直到把这个内容读完  
//					while (true)  
//					{  
//						size = zipInStream.Read(data, 0, data.Length);  
//						//小于0， 也就读完了当前的流  
//						if (size > 0)  
//						{  
//							fs.Write(data, 0, size);  
//						}  
//						else  
//						{  
//							break;  
//						}  
//					}  
//					fs.Close();  
//				}  
//			}  
//		}  
//		catch (Exception e)  
//		{  
//			throw new Exception();  
//		}  
//	}  
//
//
//	/// <summary>  
//	/// 判断是否是目录文件  
//	/// </summary>  
//	/// <param name="path"></param>  
//	/// <returns></returns>  
//	static bool IsDirectory(string path)  
//	{  
//
//		if (path[path.Length - 1] == '/')  
//		{  
//			return true;  
//		}  
//		return false;  
//	}  
}  
