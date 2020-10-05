using System.ComponentModel;

namespace TouchKeyboard {
	public class NotifyBase : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		public void Notify(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
