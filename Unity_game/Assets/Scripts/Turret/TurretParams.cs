using System.Collections.Generic;

namespace Assets.Scripts.Turret
{
    public enum TurretType
    {
        SingleShot,
        MultiShot,
        AreaShot
    }

    public class TurretParams
    {
        public string Name;
        public TurretType Type;
        public int Price = 50;

        public int Damage;
        public float FireRate;
        public int ProjectileSpeed;
        public float Range;

        public List<TurretParams> Upgrades;
        public string BaseTexture;
        public string WeaponTexture;
        public string ProjectileTexture;
        public string UiTexture;
        public string ShootSound;
    }
}
