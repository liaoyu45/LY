namespace Gods.Web.Manage {
	class BugRecord : IdAndTime {
		public int ActionId { get; set; }
		public string Description { get; set; }
		public bool Done { get; set; }

		public ActionRecord ActionRecord { get; set; }
	}
}
