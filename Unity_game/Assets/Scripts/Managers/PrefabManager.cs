using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Models;
using Assets.Scripts.Turret;
using Assets.Scripts.Utils;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _turretUiPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _terrainTilePrefab;
    [SerializeField] private GameObject _pathTilePrefab;
    [SerializeField] private GameObject _turretBasePrefab;
    [SerializeField] private GameObject _turretWeaponPrefab;

    [SerializeField] private GameObject _basePrefab;
    public GameObject BasePrefab => _basePrefab;
    [SerializeField] private GameObject _lifePrefab;
    public GameObject LifePrefab => _lifePrefab;

    private Dictionary<string, GameObject> _projectilePrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _turretUiPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _enemyPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> _tilePrefabs = new Dictionary<int, GameObject>();
    private Dictionary<string, GameObject> _turretBasePrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _turretWeaponPrefabs = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    public GameObject GetProjectilePrefab(TurretParams turretParams)
    {
        if (_projectilePrefabs.ContainsKey(turretParams.Name))
        {
            return _projectilePrefabs[turretParams.Name];
        }

        string prefabName = "prefab_projectile " + turretParams.Name;
        GameObject prefab = CopyPrefab(_projectilePrefab, prefabName, turretParams.ProjectileTexture);

        _projectilePrefabs.Add(turretParams.Name, prefab);
        return prefab;
    }

    public GameObject GetTurretUiPrefab(TurretParams turretParams)
    {
        if (_turretUiPrefabs.ContainsKey(turretParams.Name))
        {
            return _turretUiPrefabs[turretParams.Name];
        }

        string prefabName = "prefab_turret_ui " + turretParams.Name;
        GameObject prefab = CopyPrefab(_turretUiPrefab, prefabName, turretParams.UiTexture);

        prefab.GetComponent<CreateTurret>().Params = turretParams;
        _turretUiPrefabs.Add(turretParams.Name, prefab);
        return prefab;
    }

    public GameObject GetTurretWeaponPrefab(TurretParams turretParams)
    {
        if (_turretWeaponPrefabs.ContainsKey(turretParams.Name))
        {
            return _turretWeaponPrefabs[turretParams.Name];
        }

        string prefabName = "prefab_turret_weapon " + turretParams.Name;
        GameObject prefab = CopyPrefab(_turretWeaponPrefab, prefabName, turretParams.WeaponTexture);

        _turretWeaponPrefabs.Add(turretParams.Name, prefab);
        return prefab;
    }

    public GameObject GetTurretBasePrefab(TurretParams turretParams)
    {
        if (_turretBasePrefabs.ContainsKey(turretParams.Name))
        {
            return _turretBasePrefabs[turretParams.Name];
        }

        string prefabName = "prefab_turret_base " + turretParams.Name;
        GameObject prefab = CopyPrefab(_turretBasePrefab, prefabName, turretParams.BaseTexture);

        _turretBasePrefabs.Add(turretParams.Name, prefab);
        return prefab;
    }

    public void SetEnemyPrefab(EnemyModel enemyModel)
    {
        if (_enemyPrefabs.ContainsKey(enemyModel.name))
        {
            return;
        }

        string prefabName = "prefab_enemy " + enemyModel.name;
        GameObject prefab = CopyPrefab(_enemyPrefab, prefabName, enemyModel.texture);

        Unit unit = prefab.GetComponent<Unit>();
        unit.Hp = enemyModel.hitPoints;
        unit.Speed = enemyModel.speed;
        unit.MoneyReward = enemyModel.moneyReward;
        unit.HitSound = ResourceUtil.LoadSound(enemyModel.sound);

        _enemyPrefabs.Add(enemyModel.name, prefab);
    }

    public GameObject GetEnemyPrefab(string enemyName)
    {
        return _enemyPrefabs[enemyName];
    }

    public void SetTilePrefab(int tileNumber, string texturePath)
    {
        if (_tilePrefabs.ContainsKey(tileNumber))
        {
            return;
        }

        string prefabName = "prefab_tile " + tileNumber;
        GameObject prefabSource = tileNumber != 0 ? _terrainTilePrefab : _pathTilePrefab;
        GameObject prefab = CopyPrefab(prefabSource, prefabName, texturePath);

        _tilePrefabs.Add(tileNumber, prefab);
    }

    public GameObject GetTilePrefab(int tileNumber)
    {
        return _tilePrefabs[tileNumber];
    }


    private GameObject CopyPrefab(GameObject prefab, string prefabName, string texturePath)
    {
        GameObject prefabCopy = Instantiate(prefab);
        prefabCopy.SetActive(false);
        prefabCopy.name = prefabName;
        prefabCopy.transform.parent = transform;
        prefabCopy.GetComponent<SpriteRenderer>().sprite =
            ResourceUtil.LoadSprite(texturePath);
        return prefabCopy;
    }

}