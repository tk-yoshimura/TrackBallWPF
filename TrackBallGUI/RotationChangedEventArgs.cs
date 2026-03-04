// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

namespace TrackBallGUI {
    public sealed class RotationChangedEventArgs : EventArgs {
        public Quaternion Quaternion { get; }

        public bool UserOperation { private set; get; }

        public RotationChangedEventArgs(Quaternion quaternion, bool user_operation) {
            Quaternion = quaternion;
            UserOperation = user_operation;
        }
    }
}
