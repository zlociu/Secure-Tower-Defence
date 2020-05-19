using System;
using Assets.Scripts;
using UnityEngine;

public class TurretCreation : MonoBehaviour
{
    [SerializeField] private GameObject _turretPrefab;
    public static GameObject UnitsGroup;
    public Transform buildTile;


    void OnMouseDown()
    {
        _createTurret();
    }

    private void _createTurret()
    {
        Debug.Log("Creating turret");
        GameObject turret = Instantiate(_turretPrefab);
        turret.transform.position = new Vector3(buildTile.position.x, buildTile.position.y, -2);
        turret.GetComponent<Turret>().UnitsGroup = UnitsGroup;
        turret.transform.parent = buildTile;
        Debug.Log("Turret created at " + buildTile.position);
        Destroy(gameObject);
    }
}