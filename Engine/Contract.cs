using HeathenEngineering.BGSDK.DataModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Engine
{
    [CreateAssetMenu(menuName = "Blockchain Game SDK/Contract")]
    public class Contract : ScriptableObject
    {
        [HideInInspector]
        public bool UpdatedFromServer = false;
        [HideInInspector]
        public long UpdatedOn;
        [HideInInspector]
        public ContractData Data;
        [HideInInspector]
        public List<Token> Tokens = new List<Token>();

        public string id
        { get { return Data.id; } set { Data.id = value; } }
        public string systemName
        { get { return Data.name; } set { Data.name = value; } }
        public string description
        { get { return Data.description; } set { Data.description = value; } }
        public bool confirmed
        { get { return Data.confirmed; } set { Data.confirmed = value; } }
        public string address
        { get { return Data.address; } set { Data.address = value; } }

        public Token this[int tokenIndex] => Tokens[tokenIndex];
        public Token this[string tokenId] => Tokens.FirstOrDefault(p => p.Id == tokenId);
        public Token FindTokenByName(string tokenName) => Tokens.FirstOrDefault(p => p.SystemName == tokenName);
    }
}
