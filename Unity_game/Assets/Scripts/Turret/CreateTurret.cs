using Assets.Scripts;
using Assets.Scripts.Turret;
using UnityEngine;

public class CreateTurret : MonoBehaviour
{
    [SerializeField] private GameObject _turretBasePrefab;
    public GameObject TurretWeaponPrefab;
    public static GameObject UnitsGroup;
    public static LevelManager LevelManagerVar;
    public Transform BuildTile;
    public TurretParams Params;

    private void OnMouseDown()
    {
        if (LevelManagerVar.Money >= Params.Price)
        {
            LevelManagerVar.DecreaseMoney(Params.Price);
            _createTurret();
        }
    }

    private void _createTurret()
    {
        Debug.Log("Creating turret");
        GameObject turret = Instantiate(TurretWeaponPrefab);
        turret.transform.position = new Vector3(BuildTile.position.x, BuildTile.position.y, -3);
        turret.GetComponent<Turret>().UnitsGroup = UnitsGroup;
        turret.GetComponent<Turret>().Params = Params;
        Debug.Log(turret.GetComponent<Turret>().Params);
        turret.transform.parent = BuildTile;

        GameObject turretBase = Instantiate(_turretBasePrefab);
        turretBase.transform.position = new Vector3(BuildTile.position.x, BuildTile.position.y, -2);
        turretBase.transform.parent = BuildTile;

        Debug.Log("Turret created at " + BuildTile.position);
        Destroy(BuildTile.GetComponent<Rigidbody2D>());
        Destroy(BuildTile.GetComponent<BoxCollider2D>());
        Destroy(BuildTile.GetComponent<ShowTurretCreationUi>());
        ShowTurretCreationUi.ClearTurretUi();
    }
}