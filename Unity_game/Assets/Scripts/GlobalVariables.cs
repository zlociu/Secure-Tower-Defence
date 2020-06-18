﻿using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Turret;

namespace Assets.Scripts
{
    public static class GlobalVariables
    {
        public static MapModel CurrentMap;
        public static TurretHierarchyModel TurretHierarchy;
        public static List<TurretParams> DefaultTurretsParams;
        public static List<EnemyModel> EnemyParams;
    }
}