﻿using Newtonsoft.Json;

namespace CustomFollowerGoal.Models
{
    public class RefreshableUserAccessTokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string[] Scope { get; set; }
    }
}
