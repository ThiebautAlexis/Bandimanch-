using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Threading.Tasks;

public class AlexisMappingController 
{
	[MenuItem("Tools/Controller/Get XBOX360 Controller mapping")]
    
    /*
     Open the window where the mapping will be shown to the user 
    */
    static void GetXBOX360Controller()
    {
        ControllerWindow _window = new ControllerWindow();
        _window.titleContent = new GUIContent("Controller Mapping");
        _window.Init(); 
        _window.Show(); 
    }
}
public class ControllerWindow : EditorWindow
{
    bool m_pause; 
    /// <summary>
    /// Insert the URL of the images right here if you want more OS.
    /// </summary>
    string windowsUrl = "http://wiki.unity3d.com/images/thumb/a/a7/X360Controller2.png/600px-X360Controller2.png";
    string linuxUrl = "http://wiki.unity3d.com/images/thumb/7/7f/UnityLinuxMapping.png/600px-UnityLinuxMapping.png";
    string macUrl = "http://wiki.unity3d.com/images/thumb/5/5f/Unity_360controller_mac_layout.png/600px-Unity_360controller_mac_layout.png";

    /// <summary>
    /// Insert an entry for each URL you want to show in the window
    /// </summary>
    public enum OSType
    {
        Windows,
        Linux,
        MAC_OS_X
    }
    public OSType currentOS = OSType.Windows;
    Texture mappingTexture;

    /// <summary>
    /// The downloaded images will be saved in this Folder
    /// </summary>
    public string CachePath
    {
        get
        {
            if (!Directory.Exists(Application.persistentDataPath + "/Cache/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Cache/");
            }
            return Application.persistentDataPath + "/Cache/";
        }
    }
    /// <summary>
    /// Return true if the file is already saved in the entered path 
    /// </summary>
    /// <param path="_path"></param>
    bool FileInCache(string _path)
    {
        return File.Exists(_path);
    }
    /// <summary>
    /// Return the directory of the entered url in the CacheFolder
    /// If this directory doesn't exist, create it
    /// </summary>
    /// <param url="_url"></param>
    /// <returns></returns>
    string GetFileDirectory(string _url)
    {
        if (!Directory.Exists(CachePath + Path.GetFileNameWithoutExtension(_url)))
        {
            Directory.CreateDirectory(CachePath + Path.GetFileNameWithoutExtension(_url));
        }
        return CachePath + Path.GetFileNameWithoutExtension(_url) + "/" + Path.GetFileName(_url);
    }

    /// <summary>
    /// Draw all settings in the window
    /// </summary>
    private void OnGUI()
    {
        // Draw the OSType settings in the window
        // This value can be change by the user
        currentOS = (OSType)EditorGUILayout.EnumPopup("OS Selected", currentOS);
        //The _url is set according to the selected OSType and other urls
        string _url;
        switch (currentOS)
        {
            case OSType.Windows:
                _url = windowsUrl;
                break;
            case OSType.Linux:
                _url = linuxUrl;
                break;
            case OSType.MAC_OS_X:
                _url = macUrl;
                break;
            default:
                _url = string.Empty;
                break;
        }
        if (string.IsNullOrEmpty(_url)) return;
        //Verify if the file is in cache
        if(!FileInCache(GetFileDirectory(_url)))
        {
            //If it is not in cache, create a button to download the image
            if (GUILayout.Button("Download Image"))
            {
                DownloadMapping(_url);
            }
        }
        else
        {
            //If it is in cache, create a button to show and actualise the image
            if (GUILayout.Button("Show Mapping"))
            {
                //Get the byte array from the local file and convert it in a Texture2D
                //Then, this Texture2D is converted in Texture and the mappingImage is set to the newly created texture
                byte[] _datas = new byte[0];
                _datas = File.ReadAllBytes(GetFileDirectory(_url));
                if (_datas.Length == 0) return;
                Texture2D _image = new Texture2D(mappingTexture ? (int)mappingTexture.width : 600, mappingTexture ? (int)mappingTexture.height : 600);
                _image.LoadImage(_datas);
                _image.Apply();
                mappingTexture = _image as Texture;
            }
        }
        if (GUILayout.Button("Open Cache"))
        {
            //Open the folder where the images are saved
            Application.OpenURL(CachePath); 
        }
        //If the mapping texture isn't null, create a rect to fit it in 
        //Draw the mapping texture in the rect previously created
        if (!mappingTexture) return;
        Rect _rect = GUILayoutUtility.GetRect(mappingTexture.width, mappingTexture.height);
        EditorGUI.DrawPreviewTexture(_rect, mappingTexture);
    }

    public void Init()
    {

    }
    /// <summary>
    /// Download an image from an url
    /// </summary>
    /// <param url="_url"></param>
    void DownloadMapping(string _url)
    {
        m_pause = false; 
        {
            HttpWebRequest _request = WebRequest.Create(_url) as HttpWebRequest;
            _request.ServicePoint.CloseConnectionGroup(_request.ConnectionGroupName);
            using (FileStream _filestream = new FileStream(GetFileDirectory(_url), FileMode.Append, FileAccess.Write))
            {
                using (HttpWebResponse _response = _request.GetResponse() as HttpWebResponse)
                {
                    byte[] buffer = new byte[1024];
                    using (Stream _stream = _response.GetResponseStream())
                    {
                        while (true)
                        {
                            if (m_pause) return;
                            int actualByte = _stream.Read(buffer, 0, buffer.Length);
                            if (actualByte <= 0)
                            {
                                _filestream.Close();
                                break;
                            }
                            _filestream.Write(buffer, 0, actualByte);

                        }
                    }
                }
            }
            m_pause = true;
        }        
    }

    
}
