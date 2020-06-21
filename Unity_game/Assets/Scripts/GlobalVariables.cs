using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Turret;

namespace Assets.Scripts
{
    public static class GlobalVariables
    {
        public static LevelModel CurrentLevel;
        public static List<TurretParams> DefaultTurretsParams = new List<TurretParams>();
        public static Dictionary<string, TurretParams> AllTurretParams = new Dictionary<string, TurretParams>();
        public static List<EnemyModel> EnemyParams = new List<EnemyModel>();
        public static string GameResult = "";

        public static void Reset()
        {
            CurrentLevel = null;
            DefaultTurretsParams = new List<TurretParams>();
            AllTurretParams = new Dictionary<string, TurretParams>();
            EnemyParams = new List<EnemyModel>();
        }
    }
}