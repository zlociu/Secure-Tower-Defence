using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Assets.Scripts.Models
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class LevelListModel
    {
        public List<string> maps;
    }
}
