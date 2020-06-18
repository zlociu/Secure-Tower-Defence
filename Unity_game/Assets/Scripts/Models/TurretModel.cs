using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Turret;

namespace Assets.Scripts.Models
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TurretModel
    {
        public string name;
        public string type;
        public int price;

        public int damage;
        public float fireRate;
        public int projectileSpeed;
        public float range;

        public List<string> upgrades;
        public string baseTexture;
        public string weaponTexture;
        public string shootSound;

        public TurretParams ToTurretParams()
        {
            return new TurretParams()
            {
                Name = name,
                Type = (TurretType) Enum.Parse(typeof(TurretType), type),
                Price = price,
                Damage = damage,
                FireRate = fireRate,
                ProjectileSpeed = projectileSpeed,
                Range = range
            };
        }
    }
}