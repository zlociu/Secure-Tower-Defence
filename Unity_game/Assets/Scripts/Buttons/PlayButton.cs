using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/MapSelectionMenu", LoadSceneMode.Single);
    }
}
