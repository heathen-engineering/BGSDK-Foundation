using System;

namespace HeathenEngineering.Arkane.Engine
{
    /// <summary>
    /// Responce object returned from the ArkaneAPI for application requests
    /// </summary>
    /// <remarks>
    /// <para>Used by <see cref="Arkane.Editor.ListApplications(ArkaneIdentity, Action{ListApplicationsResult})"/> to list available applications.</para>
    /// <para>See <a href="https://docs-staging.arkane.network/pages/token-management.html#_list_applications">https://docs-staging.arkane.network/pages/token-management.html#_list_applications</a> for more information.</para>
    /// </remarks>
    [Serializable]
    public class AppId : System.IEquatable<AppId>, System.IComparable<AppId>
    {
        public static readonly AppId Invalid = new AppId(Guid.Empty);
        public string id;
        public string clientId;
#if UNITY_SERVER || UNITY_EDITOR
        public string clientSecret;
#endif
        public string name;
        public string description;
        public string rootURL;
        public string imageUrl;

        public AppId(string id)
        {
            this.id = id;
            clientId = string.Empty;
#if UNITY_SERVER || UNITY_EDITOR
            clientSecret = string.Empty;
#endif
            name = string.Empty;
            description = string.Empty;
            rootURL = string.Empty;
            imageUrl = string.Empty;
        }

        public AppId(Guid id)
        {
            this.id = id.ToString("D");
            clientId = string.Empty;
#if UNITY_SERVER || UNITY_EDITOR
            clientSecret = string.Empty;
#endif
            name = string.Empty;
            description = string.Empty;
            rootURL = string.Empty;
            imageUrl = string.Empty;
        }

#if UNITY_SERVER || UNITY_EDITOR
        public AppId(Guid id, string secret)
        {
            this.id = id.ToString("D");
            clientId = string.Empty;
            clientSecret = secret;
            name = string.Empty;
            description = string.Empty;
            rootURL = string.Empty;
            imageUrl = string.Empty;
        }

        public AppId(Guid id, Guid secret)
        {
            this.id = id.ToString("D");
            clientId = string.Empty;
            clientSecret = secret.ToString("D");
            name = string.Empty;
            description = string.Empty;
            rootURL = string.Empty;
            imageUrl = string.Empty;
        }
#endif

        public override bool Equals(object other)
        {
            return other is AppId && this == (AppId)other;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(AppId x, AppId y)
        {
            return x.id == y.id;
        }

        public static bool operator !=(AppId x, AppId y)
        {
            return !(x == y);
        }

        public static bool operator ==(AppId x, Guid y)
        {
            Guid result;
            var valid = Guid.TryParse(x.id, out result);
            return valid ? result == y : false;
        }

        public static bool operator !=(AppId x, Guid y)
        {
            return !(x == y);
        }

        public static explicit operator AppId(Guid value)
        {
            return new AppId(value);
        }

        public static explicit operator Guid(AppId that)
        {
            Guid buffer;
            if (Guid.TryParse(that.id, out buffer))
                return buffer;
            else
                return Guid.Empty;
        }

        public bool Equals(AppId other)
        {
            return id == other.id;
        }

        public int CompareTo(AppId other)
        {
            return id.CompareTo(other.id);
        }
    }
}
