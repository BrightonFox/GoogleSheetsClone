// Class for RequestError
// Credits: Glorien Roque, CS3505

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace ClientNetworking
{

    [JsonObject(MemberSerialization.OptIn)]
    public class RequestError
    {
        [JsonProperty(PropertyName = "messageType")]
        public string messageType { get; set; }

        [JsonProperty(PropertyName = "cellName")]
        public string cellName { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string message { get; set; }
    }
}
