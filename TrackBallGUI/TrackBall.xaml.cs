// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TrackBallGUI {
    /// <summary>
    /// Interaction logic for TrackBall.xaml
    /// </summary>
    public partial class TrackBall : UserControl, INotifyPropertyChanged {
        public TrackBall() {
            InitializeComponent();
            InitializeScene();
            UpdateViewportSize();
        }

        #region EventHandler
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<RotationChangedEventArgs>? RotationChanged;
        public event EventHandler<RotationChangedEventArgs>? RotationChangeCompleted;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region RotationProperty
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register(
                nameof(Rotation),
                typeof(Quaternion),
                typeof(TrackBall),
                new FrameworkPropertyMetadata(
                    Quaternion.Identity,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRotationPropertyChanged
                )
            );

        private static void OnRotationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            if (obj is TrackBall ctrl) {
                ctrl.SetRotation((Quaternion)e.NewValue, user_operation: false, internal_only: true);
            }
        }
        public Quaternion Rotation {
            get => GetValue(RotationProperty) is Quaternion q ? q : Quaternion.Identity;
            set {
                SetRotation(value, user_operation: false);
            }
        }

        Quaternion current_rotation = Quaternion.Identity;
        protected void SetRotation(Quaternion rotation, bool user_operation, bool internal_only = false) {
            rotation = Quaternion.Normalize(rotation);

            if (current_rotation != rotation) {
                current_rotation = rotation;

                ApplyRotation();

                if (!internal_only) {
                    SetValue(RotationProperty, rotation);

                    RotationChanged?.Invoke(this, new RotationChangedEventArgs(rotation, user_operation));
                }
            }
        }
        #endregion
    }
}
