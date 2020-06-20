using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Assets.Scripts.Models.Fields;

namespace Assets.Scripts.Models
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LevelModel
    {
        public List<MapRow> map = new List<MapRow>();
        public List<TileRow> tiles = new List<TileRow>();
        public List<string> turrets = new List<string>();
        public List<string> enemies = new List<string>();
        public List<WaveRow> waves;
        public int startingMoney;

        public List<List<int>> MapToList()
        {
            List<List<int>> result = new List<List<int>>();
            foreach (MapRow row in map)
            {
                result.Add(row.row);
            }

            return result;
        }

        public Dictionary<int, string> TilesToDict()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (TileRow row in tiles)
            {
                result.Add(row.key, row.path);
            }

            return result;
        }

        public List<Dictionary<string, EnemyWave>> wavesToDictList()
        {
            List<Dictionary<string, EnemyWave>> result = new List<Dictionary<string, EnemyWave>>();
            foreach (WaveRow waveRow in waves)
            {
                result.Add(new Dictionary<string, EnemyWave>());
                foreach (EnemyWave enemyWave in waveRow.wave)
                {
                    while (true)
                    {
                        if (result.Last().ContainsKey(enemyWave.enemy))
                        {
                            enemyWave.enemy += "_";
                            continue;
                        }

                        result.Last().Add(enemyWave.enemy, enemyWave);
                        break;
                    }
                }
            }

            return result;
        }
    }
}