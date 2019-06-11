using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageLoader
{
    private ImageLoader() { }
    private static ImageLoader _instance;
    public static ImageLoader GetInstance()
    {
        if (_instance == null)
            _instance = new ImageLoader();
        return _instance;
    }

    public List<string> GetImgList(string directory)
    {
        List<string> imgList = new List<string>();
        DirectoryInfo directoryInfo = new DirectoryInfo(directory);
        FileInfo[] files = directoryInfo.GetFiles("*.jpg");
        foreach (var file in files)
        {
            imgList.Add(file.FullName);
        }

        return imgList;
    }

    public Texture2D LoadImg(string imgPath)
    {
        using (FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
        {
            int length = (int)fs.Length;
            byte[] buffer = new byte[length];
            fs.Read(buffer, 0, length);

            Texture2D texture = new Texture2D(1024, 1024);
            texture.LoadImage(buffer);

            return texture;
        }
    }
}
