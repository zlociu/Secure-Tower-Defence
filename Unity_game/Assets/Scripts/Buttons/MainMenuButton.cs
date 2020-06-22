using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        Time.timeScale = 1;
        MusicManager.PlayMenuMusic();
        SceneManager.LoadScene("Scenes/MainMenuScene", LoadSceneMode.Single);
    }
}