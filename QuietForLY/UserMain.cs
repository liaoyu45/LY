using Quiet.Models;
using System;
using System.Linq;

namespace QuietForLY {
	class UserMain : Quiet.IUserMain {
		private static readonly Quiet.Db d = new Quiet.Db();
		public User Info { get; set; }

		public int Fight(string data) {
			throw new NotImplementedException();
		}

		public bool Login(string name, string password) {
			return (Info = d.Users.FirstOrDefault(e => e.Phone == name && e.Password == password)) == null;
		}

		public Complain[] MyComplains() {
			return Quiet.Db.UseDb(d => d.Complains.Where(e => e.UserId == Info.Id).ToArray());
		}

		public Spam[] RecentlySpams(int page) {
			const int size = 4;
			return d.Spams.OrderByDescending(e => e.Complains.OrderBy(ee => ee.ComplainTime).LastOrDefault()).Skip(page * size).Take(size).ToArray();
		}

		public int Report(string name, string websit, string phone, string data, string description) {
			throw new NotImplementedException();
		}
	}
}
