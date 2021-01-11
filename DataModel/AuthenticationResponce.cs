using System;

namespace HeathenEngineering.BGSDK.DataModel
{
    [Serializable]
    public class AuthenticationResponce
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
        public int refresh_expires_in;
        public string token_type;
        public string session_state;
        public string scope;
        public bool not_before_policy;
        public string error;
        public string error_description;

        public bool IsErrorResponce { get { return !string.IsNullOrEmpty(error); } }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get { return CreatedAt.AddSeconds(expires_in); } }
        public DateTime RefreshExpiresAt { get { return CreatedAt.AddSeconds(refresh_expires_in); } }

        public void Create()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
