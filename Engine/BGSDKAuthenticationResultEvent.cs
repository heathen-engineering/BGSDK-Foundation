using HeathenEngineering.BGSDK.DataModel;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.BGSDK.Engine
{
    [Serializable]
    public class BGSDKAuthenticationResultEvent : UnityEvent<AuthenticationResult>
    { }
}
