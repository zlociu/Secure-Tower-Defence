using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject UnitsGroup;

    public GameObject Projectile;
    public float Range = 2.5f;
    public float SpawnSpeed = 0.5f;

    [SerializeField] private GameObject _turretRangePrefab;
    private GameObject _turretRange;

    private float _spawnPeriod;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_spawnPeriod > SpawnSpeed)
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


    void OnMouseEnter()
    {
        Debug.Log("show range");
        _turretRange = SpawnObject(
            Instantiate(_turretRangePrefab),
            "range"
        );
        _turretRange.transform.localScale = new Vector3(Range, Range, -3);
        _turretRange.transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        _turretRange.transform.SetParent(transform);
    }

    void OnMouseExit()
    {
        Destroy(_turretRange);
    }

    private bool SpawnProjectile()
    {
        Transform closestUnit = GetClosestUnit();
        if (closestUnit == null)
        {
            return false;
        }

        GameObject projectile = SpawnObject(
            Instantiate(Projectile),
            "projectile"
        );
        projectile.transform.SetParent(transform);
        Projectile projectileScript = projectile.AddComponent<Projectile>();
        projectileScript.Unit = closestUnit;
        return true;
    }

    private Transform GetClosestUnit()
    {
        if (UnitsGroup.transform.childCount == 0)
        {
            return null;
        }

        Transform closestUnit = null;

        for (int childIndex = 0; childIndex < UnitsGroup.transform.childCount; childIndex++)
        {
            Transform unit = UnitsGroup.transform.GetChild(childIndex);
            float unitDistance = Vector2.Distance(transform.position, unit.position);

            if (unitDistance > Range)
            {
                continue;
            }

            if (closestUnit == null)
            {
                closestUnit = unit;
                continue;
            }

            float closestUnitDistance = Vector2.Distance(transform.position, closestUnit.position);
            if (unitDistance < closestUnitDistance)
            {
                closestUnit = unit;
            }
        }

        return closestUnit;
    }

    private GameObject SpawnObject(GameObject gameObject1, string name1)
    {
        gameObject1.transform.position = transform.position;
        gameObject1.name = name1;
        return gameObject1;
    }
}