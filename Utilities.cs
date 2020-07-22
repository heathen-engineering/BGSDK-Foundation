#if UNITY_EDITOR
#endif

namespace HeathenEngineering.Arkane
{
    public static class Utilities
    {
        /// <summary>
        /// A utility method for wrapping JSON arrays in a result tag as Unity's JsonUtility expects
        /// </summary>
        /// <param name="JSON">the JSON array to wrap</param>
        /// <returns></returns>
        public static string JSONArrayWrapper(string JSON)
        {
            if (JSON.StartsWith("["))
                return "{\"result\":" + JSON + "}";
            else
                return JSON;
        }
    }
}
