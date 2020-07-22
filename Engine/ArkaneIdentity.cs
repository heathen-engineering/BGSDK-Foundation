using HeathenEngineering.Arkane.DataModel;
using System;

namespace HeathenEngineering.Arkane.Engine
{
    /// <summary>
    /// Represents an ArkaneIdentity
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used by the <see cref="Arkane"/> object for authenticaiton, see <see cref="Arkane.Authenticate(Identity, Action{AuthenticationResult})"/> for more information.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Identity
    {
        public string username;
        public string password;
        public AuthenticationResponce authentication;
    }
}
