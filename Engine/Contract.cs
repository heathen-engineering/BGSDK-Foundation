using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeathenEngineering.Arkane.Engine
{
    [CreateAssetMenu(menuName = "Arkane/Contract")]
    public class Contract : ScriptableObject
    {
        [HideInInspector]
        public bool UpdatedFromServer = false;
        [HideInInspector]
        public long UpdatedOn;
        [HideInInspector]
        public DataModel.Contract Data;
        [HideInInspector]
        public List<Token> Tokens = new List<Token>();

        public ulong id
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
        public Token this[ulong tokenId] => Tokens.FirstOrDefault(p => p.id == tokenId);
        public Token this[string tokenName] => Tokens.FirstOrDefault(p => p.systemName == tokenName);
    }
}
