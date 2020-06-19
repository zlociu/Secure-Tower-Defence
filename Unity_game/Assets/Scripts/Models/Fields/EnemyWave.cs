using System;
using System.Diagnostics.CodeAnalysis;

namespace Assets.Scripts.Models.Fields
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class EnemyWave
    {
        public string enemy;
        public int amount;
    }
}
