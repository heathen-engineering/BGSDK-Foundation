using HeathenEngineering.BGSDK.DataModel;
using System;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Engine
{
    public abstract class Properties : ScriptableObject
    {
        public abstract Type DataType { get; }
#if UNITY_EDITOR
        public abstract string ToJsonDef(TokenDefinition token);
#endif
    }

    /// <summary>
    /// Used to create custom property objects for Tokens
    /// </summary>
    /// <typeparam name="T">Must be a serializable type for which Unity's JsonUtility can serialize</typeparam>
    /// <remarks>
    /// <para>
    /// It is intended that developers will create custom property types as ScriptableObjects.
    /// These types can then be used by designers to set the property model of each indavidual Token they define.
    /// </para>
    /// <para>
    /// Note that it is not nessisary to provide any body to the resulting custom property type ... this will be handled for you via the <see cref="Properties{T}"/> base class.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    ///    [Serializable]
    ///    public class CharacterDataModel
    ///    {
    ///        public string name;
    ///        public string description;
    ///        public int level;
    ///        public int faction;
    ///    }
    ///
    ///    [CreateAssetMenu(menuName = "My Game/Character Properties")]
    ///    public class CharacterProperties : Properties<CharacterDataModel> { }
    /// </code>
    /// </example>
    public class Properties<T> : Properties
    {
        public override Type DataType => typeof(T);
        public T data;


#if UNITY_EDITOR
        /// <summary>
        /// EDITOR ONLY - For internal use
        /// </summary>
        /// <param name="token">The token this properies object is hosted by</param>
        /// <returns>A JSON string suitable for the Create Token Defintion API call</returns>
        /// <remarks>
        /// <para>
        /// This is an internal helper method to construct a JSON string suitable for use with the Create Token Defition API call
        /// </para>
        /// </remarks>
        public override string ToJsonDef(TokenDefinition token)
        {
            TokenDefinition<T> nDef = token as TokenDefinition<T>;
            nDef.properties = data;
            return nDef.ToJson();
        }
#endif
    }
}
