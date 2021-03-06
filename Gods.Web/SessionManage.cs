﻿using System;
using System.Linq;
using System.Web;

namespace Gods.Web {
	public partial class Him {
		public static T MapSession<T>(object session, Func<T> action) {
			if (HttpContext.Current?.Session == null) {
				return action();
			}
			var t = session.GetType();
			var ps = t.GetProperties().Where(e => e.DeclaringType == t).ToArray();
			foreach (var item in ps) {
				item.SetValue(session, HttpContext.Current.Session[item.Name]);
			}
			var r = action();
			foreach (var item in ps) {
				HttpContext.Current.Session[item.Name] = item.GetValue(session);
			}
			return r;
		}
	}
}
