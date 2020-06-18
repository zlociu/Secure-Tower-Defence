using System;
using System.Diagnostics.CodeAnalysis;

namespace Assets.Scripts.Models.Fields
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class TileRow
    {
        public int key;
        public string path;
    }
}
