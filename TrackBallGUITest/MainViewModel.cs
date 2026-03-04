using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrackBallGUI;

namespace TrackBallGUITest {
    public class MainViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Note: NOT USE `System.Numerics.Quaternion`
        private Quaternion selected_rotation = Quaternion.Identity;
        public Quaternion Rotation {
            get => selected_rotation;
            set {
                selected_rotation = value;

                OnPropertyChanged(nameof(Rotation));
            }
        }
    }
}
