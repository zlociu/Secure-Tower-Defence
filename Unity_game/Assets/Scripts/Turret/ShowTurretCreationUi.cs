using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Turret;
using Assets.Scripts.Utils;
using UnityEngine;

public class ShowTurretCreationUi : MonoBehaviour
{
    private static List<GameObject> _turretUiList = new List<GameObject>();

    [SerializeField] private GameObject _defaultTurretUiPrefab;
    private static List<GameObject> _defaultTurretUiPrefabs = new List<GameObject>();

    void Start()
    {
        Setup();
    }

    public void OnMouseDown()
    {
        ClearTurretUi();

        if (transform.childCount > 0)
        {
            return;
        }

        float offset = (_defaultTurretUiPrefabs.Count - 1) * 16 / 2.0f;
        for (int i = 0; i < _defaultTurretUiPrefabs.Count; i++)
        {
            GameObject turretUi = Instantiate(_defaultTurretUiPrefabs[i]);
            turretUi.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            turretUi.GetComponent<CreateTurret>().BuildTile = transform;
            turretUi.GetComponent<CreateTurret>().Params = _defaultTurretUiPrefabs[i].GetComponent<CreateTurret>().Params;
            Vector3 uiPosition = transform.position;
            uiPosition.y += 10;
            uiPosition.x += i * 16;
            uiPosition.x -= offset;
            uiPosition.z = -2;
            turretUi.transform.position = uiPosition;
            turretUi.SetActive(true);
            _turretUiList.Add(turretUi);
        }
    }

    public static void ClearTurretUi()
    {
        foreach (GameObject turretUi in _turretUiList)
        {
            Destroy(turretUi);
        }
    }

    public void Setup()
    {
        if (_defaultTurretUiPrefabs.Count > 0)
        {
            return;
        }

        GameObject prefabs = new GameObject("turret_prefabs");

        foreach (TurretParams turretParams in GlobalVariables.DefaultTurretsParams)
        {
            GameObject prefab = Instantiate(_defaultTurretUiPrefab);
            prefab.SetActive(false);
            prefab.name = "prefab_"+turretParams.Name;
            prefab.GetComponent<CreateTurret>().Params = turretParams;
            prefab.transform.parent = prefabs.transform;
            _defaultTurretUiPrefabs.Add(prefab);
        }
    }
}