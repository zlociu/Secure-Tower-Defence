using System;
using System.Diagnostics.CodeAnalysis;

namespace Assets.Scripts.Models
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class LoginResponseModel
    {
        public string status;
        public long version;
    }
}
