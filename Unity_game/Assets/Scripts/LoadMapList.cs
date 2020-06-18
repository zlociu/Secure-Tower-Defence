using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadMapList : MonoBehaviour
{
    [SerializeField] private GameObject _scrollViewContent;
    [SerializeField] private GameObject _viewItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:8000/list-maps"))
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
                MapListModel mapList = JsonUtility.FromJson<MapListModel>(responseBody);
                foreach (string mapName in mapList.maps)
                {
                    Debug.Log(mapName);
                    GameObject viewItem = Instantiate(_viewItemPrefab);
                    viewItem.transform.SetParent(_scrollViewContent.transform);
                    viewItem.GetComponentInChildren<Text>().text = mapName;
                }
            }
        }
    }
}