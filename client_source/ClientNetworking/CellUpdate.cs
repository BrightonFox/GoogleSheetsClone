// Class for CellUpdate
// Credits: Clay Ankeny, CS3505

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworking
{

    [JsonObject(MemberSerialization.OptIn)]
    public class CellUpdate
    {
        [JsonProperty(PropertyName = "messageType")]
        public string messageType { get; set; }

        [JsonProperty(PropertyName = "cellName")]
        public string cellName { get; set; }

        [JsonProperty(PropertyName = "contents")]
        public string contents { get; set; }
    }
}
