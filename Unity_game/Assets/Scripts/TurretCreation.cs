using System;
using Assets.Scripts;
using UnityEngine;

public class TurretCreation : MonoBehaviour
{
    private bool _turretSelected;
    private bool _turretReady;
    [SerializeField] private GameObject _turretPrefab;
    public GameObject UnitsGroup;

    private GameObject _terrainTilesGroup;

    private Bounds _terrainBounds;

    public GameObject TerrainTilesGroup
    {
        set
        {
            _terrainTilesGroup = value;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _terrainBounds = new Bounds(transform.position, Vector3.zero);
            foreach (Renderer renderer in _terrainTilesGroup.GetComponentsInChildren<Renderer>())
            {
                _terrainBounds.Encapsulate(renderer.bounds);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_turretReady && Input.GetMouseButton(0))
        {
            _turretSelected = false;
            _turretReady = false;
            _createTurret();
        }
    }

    void OnMouseDown()
    {
        Debug.Log("UI turret clicked");
        if (!_turretSelected)
        {
            _turretSelected = true;
        }
    }

    void OnMouseUp()
    {
        if (_turretReady)
        {
            return;
        }

        if (_turretSelected)
        {
            _turretReady = true;
            Debug.Log("Turret is ready");
        }
    }

    private void _createTurret()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        if (!_terrainBounds.Contains(mousePosition))
        {
            Debug.Log("Tried to create turret out of map bounds");
            return;
        }

        Vector2 towerPosition = Vector2.negativeInfinity;
        foreach (Transform tile in _terrainTilesGroup.GetComponentInChildren<Transform>())
        {
            if (tile.GetComponent<Renderer>().bounds.Contains(mousePosition))
            {
                towerPosition = tile.position;
                break;
            }
        }

        if (double.IsNegativeInfinity(towerPosition.x))
        {
            Debug.Log("Tried to create turret on non buildable tile");
            return;
        }

        GameObject turret = Instantiate(_turretPrefab);
        turret.transform.position = new Vector3(towerPosition.x, towerPosition.y, -2);
        turret.GetComponent<Turret>().UnitsGroup = UnitsGroup;
        Debug.Log("Turret created at " + towerPosition);
    }
}