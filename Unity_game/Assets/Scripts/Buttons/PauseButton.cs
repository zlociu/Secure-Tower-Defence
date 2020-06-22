using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        Time.timeScale = 0;
        SceneManager.LoadScene("Scenes/PauseMenuScene", LoadSceneMode.Additive);
    }
}
