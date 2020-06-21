using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GoToLoginButton : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        MusicManager.Stop();
        SceneManager.LoadScene("Scenes/LoginMenuScene", LoadSceneMode.Single);
    }
}