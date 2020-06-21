using Assets.Scripts;
using Assets.Scripts.Turret;
using UnityEngine;

public class CreateTurret : MonoBehaviour
{
    private PrefabManager _prefabManager;
    public static GameObject UnitsGroup;
    public static LevelManager LevelManagerVar;
    public Transform BuildTile;
    public TurretParams Params;

    private void Start()
    {
        _prefabManager = FindObjectOfType<PrefabManager>();
    }

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
        if (BuildTile.childCount > 0)
        {
            Debug.Log("Destroying existing turret");
            for (int i = BuildTile.childCount; i > 0; i--)
            {
                Destroy(BuildTile.GetChild(0).gameObject);
            }
        }
        FindObjectOfType<SoundManager>().PlayButtonSound();

        Debug.Log("Creating turret");
        GameObject turretWeapon = Instantiate(_prefabManager.GetTurretWeaponPrefab(Params));
        turretWeapon.transform.position = new Vector3(BuildTile.position.x, BuildTile.position.y, -3);
        turretWeapon.GetComponent<Turret>().UnitsGroup = UnitsGroup;
        turretWeapon.transform.parent = BuildTile;
        turretWeapon.GetComponent<Turret>().Params = Params;
        turretWeapon.SetActive(true);

        GameObject turretBase = Instantiate(_prefabManager.GetTurretBasePrefab(Params));
        turretBase.transform.position = new Vector3(BuildTile.position.x, BuildTile.position.y, -2);
        turretBase.transform.parent = BuildTile;
        turretBase.SetActive(true);

        Debug.Log("Turret created at " + BuildTile.position);
        Destroy(BuildTile.GetComponent<Rigidbody2D>());
        Destroy(BuildTile.GetComponent<BoxCollider2D>());
        Destroy(BuildTile.GetComponent<ShowTurretCreationUi>());
        ShowTurretCreationUi.ClearTurretUi();

        if (Params.Upgrades.Count == 0)
        {
            return;
        }

        ShowTurretCreationUi showUi = turretWeapon.AddComponent<ShowTurretCreationUi>();
        showUi.SetupUpgradePrefabs(Params, BuildTile);
    }
}