using System.Collections.Generic;
using Assets.Scripts.Turret;
using Assets.Scripts.Utils;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject UnitsGroup;
    public TurretParams Params;

    [SerializeField] private GameObject _turretRangePrefab;
    private GameObject _projectilePrefab;
    private GameObject _turretRange;
    private SoundManager _soundManager;
    private PrefabManager _prefabManager;

    private float _spawnPeriod;

    // Start is called before the first frame update
    void Start()
    {
        _prefabManager = FindObjectOfType<PrefabManager>();
        _projectilePrefab = _prefabManager.GetProjectilePrefab(Params);
        _soundManager = FindObjectOfType<SoundManager>();
        AudioClip shotSoundClip = ResourceUtil.LoadSound(Params.ShootSound);
        _soundManager.AddAudioSource(gameObject.GetInstanceID(), shotSoundClip);
    }

    // Update is called once per frame
    void Update()
    {
        Transform closestEnemy = GetClosestEnemy();
        if (closestEnemy == null)
        {
            return;
        }

        Vector2 dir = closestEnemy.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (_spawnPeriod > Params.FireRate)
        {
            if (SpawnProjectile())
            {
                _spawnPeriod = 0f;
                _soundManager.PlaySound(gameObject.GetInstanceID());
            }
        }
        else
        {
            _spawnPeriod += Time.deltaTime;
        }
    }

    private void OnMouseEnter()
    {
        _turretRange = SpawnObject(
            Instantiate(_turretRangePrefab),
            "range"
        );
        _turretRange.transform.localScale = new Vector3(Params.Range, Params.Range, -3);
        _turretRange.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        _turretRange.transform.SetParent(transform);
    }

    private void OnMouseExit()
    {
        Destroy(_turretRange);
    }

    private bool SpawnProjectile()
    {
        switch (Params.Type)
        {
            case TurretType.SingleShot:
            {
                Transform closestUnit = GetClosestEnemy();
                return SpawnProjectile(closestUnit);
            }
            case TurretType.MultiShot:
            {
                List<Transform> closestUnits = GetClosestEnemies(3);
                return SpawnProjectile(closestUnits);
            }
            case TurretType.AreaShot:
            {
                Transform closestUnit = GetClosestEnemy();
                return SpawnAreaProjectile(closestUnit);
            }
            default:
                return false;
        }
    }

    private bool SpawnProjectile(List<Transform> closestUnits)
    {
        if (closestUnits == null || closestUnits.Count == 0)
        {
            return false;
        }

        foreach (Transform closestUnit in closestUnits)
        {
            SpawnProjectile(closestUnit);
        }

        return true;
    }

    private bool SpawnProjectile(Transform closestUnit)
    {
        if (closestUnit == null)
        {
            return false;
        }

        GameObject projectile = SpawnObject(
            Instantiate(_projectilePrefab),
            "projectile " + Params.Name
        );
        projectile.SetActive(true);

        Projectile projectileScript = projectile.AddComponent<Projectile>();
        projectileScript.Speed = Params.ProjectileSpeed;
        projectileScript.Damage = Params.Damage;
        projectileScript.TargetUnit = closestUnit;

        return true;
    }

    private bool SpawnAreaProjectile(Transform closestUnit)
    {
        if (closestUnit == null)
        {
            return false;
        }

        GameObject projectile = SpawnObject(
            Instantiate(_projectilePrefab),
            "projectile " + Params.Name
        );
        projectile.SetActive(true);
        projectile.transform.SetParent(transform);

        AreaProjectile projectileScript = projectile.AddComponent<AreaProjectile>();
        projectileScript.Range = 100;
        projectileScript.Speed = Params.ProjectileSpeed;
        projectileScript.Damage = Params.Damage;
        projectileScript.UnitsGroup = UnitsGroup;
        projectileScript.TargetUnit = closestUnit;

        Vector2 dir = closestUnit.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _soundManager.PlaySound(gameObject.GetInstanceID());
        return true;
    }

    private Transform GetClosestEnemy()
    {
        Transform closestEnemy = null;
        float closestEnemyDistance = float.MaxValue;
        for (int childIndex = 0; childIndex < UnitsGroup.transform.childCount; childIndex++)
        {
            Transform enemy = UnitsGroup.transform.GetChild(childIndex);
            float enemyDistance = Vector2.Distance(transform.position, enemy.position);
            if (enemyDistance > Params.Range)
            {
                continue;
            }

            if (enemyDistance < closestEnemyDistance)
            {
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private List<Transform> GetClosestEnemies(int amount)
    {
        if (UnitsGroup.transform.childCount == 0)
        {
            return null;
        }

        List<Transform> closestEnemies = new List<Transform>();

        for (int childIndex = 0; childIndex < UnitsGroup.transform.childCount; childIndex++)
        {
            Transform enemy = UnitsGroup.transform.GetChild(childIndex);
            float enemyDistance = Vector2.Distance(transform.position, enemy.position);

            if (enemyDistance > Params.Range)
            {
                continue;
            }

            if (closestEnemies.Count < amount)
            {
                closestEnemies.Add(enemy);
                continue;
            }

            int indexToReplace = -1;
            float previousDistanceDifference = 0;
            for (int i = 0; i < closestEnemies.Count; i++)
            {
                float distanceDifference =
                    enemyDistance - Vector2.Distance(transform.position, closestEnemies[i].position);
                if (distanceDifference > previousDistanceDifference)
                {
                    indexToReplace = i;
                    previousDistanceDifference = distanceDifference;
                }
            }

            if (indexToReplace != -1)
            {
                closestEnemies[indexToReplace] = enemy;
            }
        }

        return closestEnemies;
    }

    private GameObject SpawnObject(GameObject gameObject1, string name1)
    {
        gameObject1.transform.position = transform.position;
        gameObject1.name = name1;
        return gameObject1;
    }
}