// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace TrackBallGUI {
    public partial class TrackBall {
        private Vector3D last_arc_point;
        private bool is_dragging;

        private void Viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (is_dragging) {
                QuaternionChangeCompleted?.Invoke(this, new QuaternionChangedEventArgs(Rotation, user_operation: true));
            }

            is_dragging = false;
            Viewport.ReleaseMouseCapture();
        }

        private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            is_dragging = true;
            last_arc_point = ProjectToArcball(e.GetPosition(Viewport));
            Viewport.CaptureMouse();
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e) {
            if (!is_dragging) {
                return;
            }

            Vector3D current_arc_point = ProjectToArcball(e.GetPosition(Viewport));
            Quaternion delta = Quaternion.FromVectors(last_arc_point, current_arc_point);

            SetRotation(delta * Rotation, user_operation: true);

            last_arc_point = current_arc_point;

            QuaternionChanged?.Invoke(this, new QuaternionChangedEventArgs(Rotation, user_operation: true));
        }

        private Vector3D ProjectToArcball(Point point) {
            double width = Viewport.ActualWidth;
            double height = Viewport.ActualHeight;

            if (width <= 0 || height <= 0) {
                return new Vector3D(0, 0, 1);
            }

            double radius = double.Min(width, height) * 0.5;
            double x = (point.X - (width * 0.5)) / radius;
            double y = ((height * 0.5) - point.Y) / radius;

            double norm_squa = (x * x) + (y * y);
            if (norm_squa > 1.0) {
                double inv_length = 1.0 / double.Sqrt(norm_squa);
                return new Vector3D(x * inv_length, y * inv_length, 0);
            }

            double z = double.Sqrt(1.0 - norm_squa);
            return new Vector3D(x, y, z);
        }
    }
}
