using Assets.Scripts;
using Assets.Scripts.Turret;
using Assets.Scripts.Utils;
using UnityEngine;

public class CreateTurret : MonoBehaviour
{
    [SerializeField] private GameObject _turretBasePrefab;
    public GameObject TurretWeaponPrefab;
    public static GameObject UnitsGroup;
    public static LevelManager LevelManagerVar;
    public Transform BuildTile;
    public TurretParams Params;
    public AudioClip ShootSound;

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
        GameObject turretWeapon = Instantiate(TurretWeaponPrefab);
        turretWeapon.transform.position = new Vector3(BuildTile.position.x, BuildTile.position.y, -3);
        turretWeapon.GetComponent<Turret>().UnitsGroup = UnitsGroup;
        turretWeapon.GetComponent<Turret>().Params = Params;
        turretWeapon.transform.parent = BuildTile;
        turretWeapon.GetComponent<SpriteRenderer>().sprite =
            ResourceUtil.LoadSprite(Params.WeaponTexture);
        turretWeapon.GetComponent<Turret>().ShotSoundClip =
            ResourceUtil.LoadSound(Params.ShootSound);

        GameObject turretBase = Instantiate(_turretBasePrefab);
        turretBase.transform.position = new Vector3(BuildTile.position.x, BuildTile.position.y, -2);
        turretBase.transform.parent = BuildTile;
        turretBase.GetComponent<SpriteRenderer>().sprite =
            ResourceUtil.LoadSprite(Params.BaseTexture);

        Debug.Log("Turret created at " + BuildTile.position);
        Destroy(BuildTile.GetComponent<Rigidbody2D>());
        Destroy(BuildTile.GetComponent<BoxCollider2D>());
        Destroy(BuildTile.GetComponent<ShowTurretCreationUi>());
        ShowTurretCreationUi.ClearTurretUi();
    }
}