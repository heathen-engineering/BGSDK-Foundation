using HeathenEngineering.BGSDK.DataModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeathenEngineering.BGSDK.Engine
{
    [CreateAssetMenu(menuName = "Blockchain Game SDK/Contract")]
    public class Contract : ScriptableObject
    {
        [FormerlySerializedAs("UpdatedFromServer")]
        [HideInInspector]
        public bool updatedFromServer = false;
        [FormerlySerializedAs("UpdatedOn")]
        [HideInInspector]
        public long updatedOn;
        [FormerlySerializedAs("Data")]
        public ContractData data;
        
        [FormerlySerializedAs("Tokens")]
        public List<Token> tokens = new List<Token>();

        public string Id
        { get { return data.id; } set { data.id = value; } }
        public string SystemName
        { get { return data.name; } set { data.name = value; } }
        public string Description
        { get { return data.description; } set { data.description = value; } }
        public bool Confirmed
        { get { return data.confirmed; } set { data.confirmed = value; } }
        public string Address
        { get { return data.address; } set { data.address = value; } }

        public Token this[int tokenIndex] => tokens[tokenIndex];
        public Token this[string tokenId] => tokens.FirstOrDefault(p => p.Id == tokenId);
        public Token FindTokenByName(string tokenName) => tokens.FirstOrDefault(p => p.SystemName == tokenName);
    }
}
