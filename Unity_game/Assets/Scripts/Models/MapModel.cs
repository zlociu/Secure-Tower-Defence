using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Models.Fields;

namespace Assets.Scripts.Models
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MapModel
    {
        public List<MapRow> map = new List<MapRow>();
        public List<TileRow> tiles = new List<TileRow>();
        public List<string> turrets = new List<string>();
        public List<string> enemies = new List<string>();

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
    }
}