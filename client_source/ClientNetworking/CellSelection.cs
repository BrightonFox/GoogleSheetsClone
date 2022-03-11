// Class for CellSelection
// Credits: Clay Ankeny, CS3505

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientNetworking
{
    [JsonObject(MemberSerialization.OptIn)]

    public class CellSelection
    {
        [JsonProperty(PropertyName = "messageType")]
        public string messageType { get; set; }

        [JsonProperty(PropertyName = "cellName")]
        public string cellName { get; set; }

        [JsonProperty(PropertyName = "selector")]
        public int selector { get; set; }

        [JsonProperty(PropertyName = "selectorName")]
        public string selectorName { get; set; }
    }
}
