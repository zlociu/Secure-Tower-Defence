using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMapSelection : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        string text = GetComponentInChildren<Text>().text;
        using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:8000/map-download?path=" + text))
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
                string responseBody = request.downloadHandler.text;
                MapModel map = JsonUtility.FromJson<MapModel>(responseBody);
                GlobalVariables.CurrentMap = map;
                SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
            }
        }
    }
}