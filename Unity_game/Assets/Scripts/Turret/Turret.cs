using System.Collections.Generic;
using Assets.Scripts.Turret;
using Assets.Scripts.Utils;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject UnitsGroup;
    public GameObject ProjectilePrefab;
    public TurretParams Params;

    [SerializeField] private GameObject _turretRangePrefab;
    public AudioClip ShotSoundClip;
    private GameObject _turretRange;
    private SoundManager _soundManager;

    private float _spawnPeriod;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = FindObjectOfType<SoundManager>();
        _soundManager.AddAudioSource(gameObject.GetInstanceID(), ShotSoundClip);
        SetupProjectile();
    }

    // Update is called once per frame
    void Update()
    {
        if (_spawnPeriod > Params.FireRate)
        {
            if (SpawnProjectile())
            {
                _spawnPeriod = 0f;
            }
        }
        else
        {
            _spawnPeriod += Time.deltaTime;
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("show range");
        _turretRange = SpawnObject(
            Instantiate(_turretRangePrefab),
            "range"
        );
        _turretRange.transform.localScale = new Vector3(Params.Range, Params.Range, -3);
        _turretRange.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        _turretRange.transform.SetParent(transform);
    }

    private void SetupProjectile()
    {
        GameObject prefabs = new GameObject("projectile_prefabs");
        ProjectilePrefab = Instantiate(ProjectilePrefab);
        ProjectilePrefab.GetComponent<SpriteRenderer>().sprite =
            ResourceUtil.LoadSprite(Params.ProjectileTexture);
        ProjectilePrefab.SetActive(false);
        ProjectilePrefab.name = "prefab_" + Params.Name;
        ProjectilePrefab.transform.parent = prefabs.transform;
    }

    private void OnMouseExit()
    {
        Destroy(_turretRange);
    }

    private bool SpawnProjectile()
    {
        if (Params.Type == TurretType.SingleShot)
        {
            Transform closestUnit = GetClosestUnit();
            return SpawnProjectile(closestUnit);
        }
        else if (Params.Type == TurretType.MultiShot)
        {
            List<Transform> closestUnits = GetClosestUnits(3);
            return SpawnProjectile(closestUnits);
        }
        else if (Params.Type == TurretType.AreaShot)
        {
            Transform closestUnit = GetClosestUnit();
            return SpawnAreaProjectile(closestUnit);
        }

        return false;
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
            Instantiate(ProjectilePrefab),
            "projectile"
        );
        projectile.SetActive(true);
        projectile.transform.SetParent(transform);

        Projectile projectileScript = projectile.AddComponent<Projectile>();
        projectileScript.Speed = Params.ProjectileSpeed;
        projectileScript.Damage = Params.Damage;
        projectileScript.TargetUnit = closestUnit;

        Vector2 dir = closestUnit.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _soundManager.PlaySound(gameObject.GetInstanceID());
        return true;
    }

    private bool SpawnAreaProjectile(Transform closestUnit)
    {
        if (closestUnit == null)
        {
            return false;
        }

        GameObject projectile = SpawnObject(
            Instantiate(ProjectilePrefab),
            "projectile"
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

    private Transform GetClosestUnit()
    {
        List<Transform> closestUnits = GetClosestUnits(1);
        if (closestUnits == null || closestUnits.Count == 0)
        {
            return null;
        }
        else
        {
            return closestUnits[0];
        }
    }

    private List<Transform> GetClosestUnits(int amount)
    {
        if (UnitsGroup.transform.childCount == 0)
        {
            return null;
        }

        List<Transform> closestUnits = new List<Transform>();

        for (int childIndex = 0; childIndex < UnitsGroup.transform.childCount; childIndex++)
        {
            Transform unit = UnitsGroup.transform.GetChild(childIndex);
            float unitDistance = Vector2.Distance(transform.position, unit.position);

            if (unitDistance > Params.Range)
            {
                continue;
            }

            if (closestUnits.Count < amount)
            {
                closestUnits.Add(unit);
                continue;
            }

            int indexToReplace = -1;
            float previousDistanceDifference = 0;
            for (int i = 0; i < closestUnits.Count; i++)
            {
                float distanceDifference =
                    unitDistance - Vector2.Distance(transform.position, closestUnits[i].position);
                if (distanceDifference > previousDistanceDifference)
                {
                    indexToReplace = i;
                    previousDistanceDifference = distanceDifference;
                }
            }

            if (indexToReplace != -1)
            {
                closestUnits[indexToReplace] = unit;
            }
        }

        return closestUnits;
    }

    private GameObject SpawnObject(GameObject gameObject1, string name1)
    {
        gameObject1.transform.position = transform.position;
        gameObject1.name = name1;
        return gameObject1;
    }
}