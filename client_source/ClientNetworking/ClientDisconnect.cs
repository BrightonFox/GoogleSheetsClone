// Class for ClientDisconnect
// Credits: Glorien Roque, CS3505

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworking
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ClientDisconnect
    {
        [JsonProperty(PropertyName = "messageType")]
        public string messageType { get; set; }

        [JsonProperty(PropertyName = "user")]
        public int user { get; set; }

    }
}
