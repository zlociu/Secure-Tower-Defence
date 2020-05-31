using UnityEngine;

public class TurretCreation : MonoBehaviour
{
    [SerializeField] private GameObject _turretBasePrefab;
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
        turret.transform.position = new Vector3(buildTile.position.x, buildTile.position.y, -3);
        turret.GetComponent<Turret>().UnitsGroup = UnitsGroup;
        turret.transform.parent = buildTile;

        GameObject turretBase = Instantiate(_turretBasePrefab);
        turretBase.transform.position = new Vector3(buildTile.position.x, buildTile.position.y, -2);
        turretBase.transform.parent = buildTile;

        Debug.Log("Turret created at " + buildTile.position);
        Destroy(gameObject);
    }
}