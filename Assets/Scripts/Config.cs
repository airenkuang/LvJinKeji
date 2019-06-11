using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Config : MonoBehaviour
{
    private string configFile;
    XmlDocument doc;
    XmlNode root;
    XmlElement hideCursor;
    XmlElement loop;
    XmlElement unused;
    XmlElement scale;

    public static Config _instance;



    private void Awake()
    {
        _instance = this;
        configFile = Application.dataPath + "/StreamingAssets/app.config";
        doc = new XmlDocument();
        doc.Load(configFile);


        root = doc.SelectSingleNode("root");
        hideCursor = (XmlElement)root.SelectSingleNode("cursor");
        loop = (XmlElement)root.SelectSingleNode("finished");
        scale = (XmlElement)root.SelectSingleNode("scale");

        unused = (XmlElement)root.SelectSingleNode("unused");
    }

    public float GetUnusedTime()
    {
        return float.Parse(unused.GetAttribute("time"));
    }

    public bool GetHideCursor()
    {
        return hideCursor.GetAttribute("hide") == "1";
    }

    public float ScaleFactor()
    {
        return float.Parse(scale.GetAttribute("factor"));
    }

    public bool GetLoop()
    {
        return loop.GetAttribute("loop") == "1";
    }
}
