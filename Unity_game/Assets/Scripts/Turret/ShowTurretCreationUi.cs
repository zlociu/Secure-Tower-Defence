using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Turret;
using UnityEngine;

public class ShowTurretCreationUi : MonoBehaviour
{
    private static List<GameObject> _turretUiList = new List<GameObject>();

    private List<GameObject> _defaultTurretUiPrefabs = new List<GameObject>();
    private PrefabManager _prefabManager;
    private SoundManager _soundManager;
    private Transform _buildTile;

    void Start()
    {
        _soundManager = FindObjectOfType<SoundManager>();
        if (_prefabManager != null)
        {
            return;
        }

        _buildTile = transform;
        _prefabManager = FindObjectOfType<PrefabManager>();
        if (_defaultTurretUiPrefabs.Count > 0)
        {
            return;
        }

        foreach (TurretParams turretParams in GlobalVariables.DefaultTurretsParams)
        {
            _defaultTurretUiPrefabs.Add(_prefabManager.GetTurretUiPrefab(turretParams));
        }
    }

    public void OnMouseDown()
    {
        ClearTurretUi();
        _soundManager.PlayButtonSound();

        float offset = (_defaultTurretUiPrefabs.Count - 1) * 16 / 2.0f;
        for (int i = 0; i < _defaultTurretUiPrefabs.Count; i++)
        {
            GameObject turretUi = Instantiate(_defaultTurretUiPrefabs[i]);
            turretUi.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            turretUi.GetComponent<CreateTurret>().BuildTile = _buildTile;
            turretUi.GetComponent<CreateTurret>().Params =
                _defaultTurretUiPrefabs[i].GetComponent<CreateTurret>().Params;
            Vector3 uiPosition = transform.position;
            uiPosition.y += 10;
            uiPosition.x += i * 16;
            uiPosition.x -= offset;
            uiPosition.z = -4;
            turretUi.transform.position = uiPosition;
            turretUi.SetActive(true);
            _turretUiList.Add(turretUi);
        }
    }

    public static bool ClearTurretUi()
    {
        if (_turretUiList.Count == 0)
        {
            return false;
        }

        foreach (GameObject turretUi in _turretUiList)
        {
            Destroy(turretUi);
        }

        _turretUiList.Clear();

        return true;
    }

    public void SetupUpgradePrefabs(TurretParams turretParams, Transform buildTile)
    {
        _defaultTurretUiPrefabs.Clear();
        _prefabManager = FindObjectOfType<PrefabManager>();
        _buildTile = buildTile;
        foreach (string turretName in turretParams.Upgrades)
        {
            TurretParams upgradeParams = GlobalVariables.AllTurretParams[turretName];
            _defaultTurretUiPrefabs.Add(_prefabManager.GetTurretUiPrefab(upgradeParams));
        }
    }
}