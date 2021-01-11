using HeathenEngineering.BGSDK.DataModel;
using System;

namespace HeathenEngineering.BGSDK.Engine
{
    /// <summary>
    /// Represents an BGSDKIdentity
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used by the <see cref="BGSDK"/> object for authenticaiton, see <see cref="BGSDK.Authenticate(Identity, Action{AuthenticationResult})"/> for more information.
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
