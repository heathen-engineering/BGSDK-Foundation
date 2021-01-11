using HeathenEngineering.BGSDK.Engine;
using System;
using System.Collections.Generic;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class ListApplicationsResult : BGSDKBaseResult
    {
        public List<AppId> result;
    }
}
