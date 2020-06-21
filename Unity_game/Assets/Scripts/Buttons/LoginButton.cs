using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private InputField _usernameInput;
    [SerializeField] private InputField _passwordInput;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_usernameInput.text.Length == 0 || _passwordInput.text.Length == 0)
        {
            return;
        }

        Dictionary<string, string> loginDict = new Dictionary<string, string>
        {
            {"username", _usernameInput.text}, {"password", _passwordInput.text}
        };

        UnityWebRequest request = UnityWebRequest.Post("http://127.0.0.1:8000/login", loginDict);
        request.SendWebRequest();
        while (!request.isDone)
        {
            new WaitForSeconds(0.5f);
        }

        if (request.responseCode == 201)
        {
            DownloadData();
            MusicManager.LoadAllMusicClips();
            MusicManager.PlayMenuMusic();
            SceneManager.LoadScene("Scenes/MainMenuScene", LoadSceneMode.Single);
        }
    }

    private void DownloadData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:8000/request-update"))
        {
            request.SendWebRequest();
            while (!request.isDone)
            {
                new WaitForSeconds(0.5f);
            }

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                byte[] results = request.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    using (var fileStream = new FileStream("data.zip", FileMode.Create, FileAccess.Write))
                    {
                        using (var binaryStream = new BinaryReader(stream))
                        {
                            int len = 1;
                            while (len > 0)
                            {
                                byte[] buffer = new byte[1024];
                                len = binaryStream.Read(buffer, 0, 1024);
                                fileStream.Write(buffer, 0, len);
                            }
                        }
                    }
                }

                if (Directory.Exists("data"))
                {
                    Directory.Delete("data", true);
                }

                ZipFile.ExtractToDirectory("data.zip", "./");
                File.Delete("data.zip");
            }
        }
    }
}