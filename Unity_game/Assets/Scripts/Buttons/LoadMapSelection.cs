using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Models;
using Assets.Scripts.Turret;
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
        using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:8000/level-download?name=" + text))
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
                LevelModel level = JsonUtility.FromJson<LevelModel>(responseBody);
                GlobalVariables.CurrentLevel = level;
                LoadTurretParams(level.turrets);
                LoadEnemyParams(level.enemies);
                SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
            }
        }
    }

    private void LoadTurretParams(List<string> turretNames)
    {
        GlobalVariables.DefaultTurretsParams = new List<TurretParams>();
        foreach (string turretName in turretNames)
        {
            using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:8000/turret-download?name=" + turretName))
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
                    TurretModel model = JsonUtility.FromJson<TurretModel>(responseBody);
                    GlobalVariables.DefaultTurretsParams.Add(model.ToTurretParams());
                }
            }
        }
    }

    private void LoadEnemyParams(List<string> enemyNames)
    {
        GlobalVariables.EnemyParams = new List<EnemyModel>();
        foreach (string enemyName in enemyNames)
        {
            using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:8000/enemy-download?name=" + enemyName))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    new WaitForSeconds(0.5f);
                }

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("http://127.0.0.1:8000/enemy-download?name=" + enemyName);
                    Debug.Log(request.error);
                }
                else
                {
                    string responseBody = request.downloadHandler.text;
                    EnemyModel model = JsonUtility.FromJson<EnemyModel>(responseBody);
                    GlobalVariables.EnemyParams.Add(model);
                }
            }
        }
    }
}