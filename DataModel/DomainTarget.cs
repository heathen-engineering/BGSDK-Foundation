using System;

namespace HeathenEngineering.Arkane.DataModel
{
    [Serializable]
    public struct DomainTarget
    {
        public string Staging;
        public string Production;

        public string this[bool useStaging] { get { return useStaging ? Staging : Production; } }

        public DomainTarget(string staging, string production)
        {
            Staging = staging;
            Production = production;
        }
    }
}
