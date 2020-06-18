using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Models.Fields;

namespace Assets.Scripts.Models
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TurretHierarchyModel
    {
        public string baseLongRangeTurret;
        public string baseAreaTurret;
        public string baseShortRangeTurret;
        public List<TurretRelationship> turretRelationships;
    }
}
