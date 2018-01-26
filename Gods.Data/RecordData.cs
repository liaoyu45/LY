using System;
using System.Collections.Generic;

namespace LivingDB {
	class RecordData {
		public string Main { get; set; }
		public string Detail { get; set; }
		public int Count { get; set; }
	}
	class MainTableData {
		public MainTableData(Type type, int max) {
			FullName = type.FullName;
			Name = type.Name;
			Max = max;
		}
		public int Max { get; }
		public string FullName { get; }
		public string Name { get; }
		public string TableName { get; set; }
		public List<DetailTable> Tables { get; set; }
	}
	class DetailTable {
		public bool IsMain { get; set; }
		public string Name { get; set; }
		public int Count { get; set; }
	}
}
