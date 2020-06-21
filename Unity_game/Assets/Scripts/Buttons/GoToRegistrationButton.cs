using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GoToRegistrationButton : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        SceneManager.LoadScene("Scenes/RegistrationMenuScene", LoadSceneMode.Single);
    }
}