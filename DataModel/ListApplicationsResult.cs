using HeathenEngineering.Arkane.Engine;
using System;
using System.Collections.Generic;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public class ListApplicationsResult : ArkaneBaseResult
    {
        public List<AppId> result;
    }
}
