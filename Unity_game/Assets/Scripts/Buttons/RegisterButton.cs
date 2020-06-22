using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterButton : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private InputField _usernameInput;
    [SerializeField] private InputField _passwordInput;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_usernameInput.text.Length == 0 || _passwordInput.text.Length == 0)
        {
            return;
        }

        Dictionary<string, string> registerDict = new Dictionary<string, string>
        {
            {"username", _usernameInput.text}, {"password", _passwordInput.text}
        };

        UnityWebRequest request = UnityWebRequest.Post("http://127.0.0.1:8000/register", registerDict);
        request.SendWebRequest();
        while (!request.isDone)
        {
            new WaitForSeconds(0.5f);
        }

        if (request.responseCode == 201)
        {
            SceneManager.LoadScene("Scenes/LoginMenuScene", LoadSceneMode.Single);
        }
    }
}