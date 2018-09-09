using System.Collections.Generic;

namespace Gods.Web.Manage {
    public class Coder : IdAndTime {
        public string Name { get; set; }

        public List<Interface> Interfaces { get; set; } = new List<Interface>();
        public List<ActionRecord> ActionRecords { get; set; } = new List<ActionRecord>();
    }
}