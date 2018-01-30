using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivingDB {
	class DB {
		public DbContext D { get; set; }
		public MainTableData[] Ts { get; set; }
		public void Recreate() {

		}
	}
}
