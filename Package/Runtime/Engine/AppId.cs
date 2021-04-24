using System;

namespace HeathenEngineering.BGSDK.Engine
{
    /// <summary>
    /// Responce object returned from the BGSDKAPI for application requests
    /// </summary>
    /// <remarks>
    /// <para>Used by <see cref="BGSDK.Editor.ListApplications(BGSDKIdentity, Action{ListApplicationsResult})"/> to list available applications.</para>
    /// <para>See <a href="https://docs-staging.arkane.network/pages/token-management.html#_list_applications">https://docs-staging.arkane.network/pages/token-management.html#_list_applications</a> for more information.</para>
    /// </remarks>
    [Serializable]
    public class AppId : System.IEquatable<AppId>, System.IComparable<AppId>
    {
        public static readonly AppId Invalid = new AppId(Guid.Empty);
        public string applicationId;
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
            this.applicationId = id;
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
            this.applicationId = id.ToString("D");
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
            this.applicationId = id.ToString("D");
            clientId = string.Empty;
            clientSecret = secret;
            name = string.Empty;
            description = string.Empty;
            rootURL = string.Empty;
            imageUrl = string.Empty;
        }

        public AppId(Guid id, Guid secret)
        {
            this.applicationId = id.ToString("D");
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
            return applicationId.GetHashCode();
        }

        public static bool operator ==(AppId x, AppId y)
        {
            return x.applicationId == y.applicationId;
        }

        public static bool operator !=(AppId x, AppId y)
        {
            return !(x == y);
        }

        public static bool operator ==(AppId x, Guid y)
        {
            Guid result;
            var valid = Guid.TryParse(x.applicationId, out result);
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
            if (Guid.TryParse(that.applicationId, out buffer))
                return buffer;
            else
                return Guid.Empty;
        }

        public bool Equals(AppId other)
        {
            return applicationId == other.applicationId;
        }

        public int CompareTo(AppId other)
        {
            return applicationId.CompareTo(other.applicationId);
        }
    }
}
