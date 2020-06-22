using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GoToRegistrationButton : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        ResourceUtil.Reset();
        SceneManager.LoadScene("Scenes/RegistrationMenuScene", LoadSceneMode.Single);
    }
}