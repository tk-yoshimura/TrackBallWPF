// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TrackBallGUI {
    public partial class TrackBall {
        private readonly AxisAngleRotation3D axis_rotation = new(new Vector3D(0, 1, 0), 0);

        private const int theta_steps = 18;
        private const int phi_steps = 18;

        private static readonly Color sphere_white = Color.FromRgb(250, 250, 250);
        private static readonly Color sphere_black = Color.FromRgb(80, 80, 80);

        private void InitializeScene() {
            Model3DGroup model_root = new();
            model_root.Children.Add(new AmbientLight(Colors.White));

            Model3DGroup group = BuildSphere();
            group.Transform = new RotateTransform3D(axis_rotation);
            model_root.Children.Add(group);

            Viewport.Children.Clear();
            Viewport.Children.Add(new ModelVisual3D { Content = model_root });
            Viewport.Camera = new OrthographicCamera {
                Position = new Point3D(0, 0, 1.5),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0)
            };

            ApplyRotation();
        }

        private static Model3DGroup BuildSphere() {
            Model3DGroup group = new();

            AddOctant(group, +1, +1, +1, 0.0, 0.5, 0.0, 0.5);
            AddOctant(group, -1, +1, +1, 0.0, 0.5, 0.5, 1.0);
            AddOctant(group, -1, +1, -1, 0.0, 0.5, 1.0, 1.5);
            AddOctant(group, +1, +1, -1, 0.0, 0.5, 1.5, 2.0);

            AddOctant(group, +1, -1, +1, 0.5, 1.0, 0.0, 0.5);
            AddOctant(group, -1, -1, +1, 0.5, 1.0, 0.5, 1.0);
            AddOctant(group, -1, -1, -1, 0.5, 1.0, 1.0, 1.5);
            AddOctant(group, +1, -1, -1, 0.5, 1.0, 1.5, 2.0);

            return group;
        }

        private static void AddOctant(Model3DGroup group, int x_sign, int y_sign, int z_sign,
            double theta_start, double theta_end, double phi_start, double phi_end) {

            MeshGeometry3D geometry = BuildPatch(theta_start, theta_end, phi_start, phi_end);
            bool white = (x_sign * y_sign * z_sign) > 0;

            Color color = white ? sphere_white : sphere_black;
            MaterialGroup material = new();
            material.Children.Add(new DiffuseMaterial(new SolidColorBrush(color)));
            material.Children.Add(new SpecularMaterial(new SolidColorBrush(Colors.White), 20));

            group.Children.Add(new GeometryModel3D {
                Geometry = geometry,
                Material = material,
                BackMaterial = material
            });
        }

        private static MeshGeometry3D BuildPatch(double theta_start, double theta_end, double phi_start, double phi_end) {
            MeshGeometry3D mesh = new();
            int columns = phi_steps + 1;

            for (int t = 0; t <= theta_steps; t++) {
                double theta = Lerp(theta_start, theta_end, t / (double)theta_steps);
                (double sin_theta, double cos_theta) = double.SinCosPi(theta);

                for (int p = 0; p <= phi_steps; p++) {
                    double phi = Lerp(phi_start, phi_end, p / (double)phi_steps);
                    (double sin_phi, double cos_phi) = double.SinCosPi(phi);

                    double x = sin_theta * cos_phi;
                    double y = cos_theta;
                    double z = sin_theta * sin_phi;

                    Point3D position = new(x, y, z);
                    mesh.Positions.Add(position);

                    Vector3D normal = new(x, y, z);
                    normal.Normalize();
                    mesh.Normals.Add(normal);
                }
            }

            for (int t = 0; t < theta_steps; t++) {
                for (int p = 0; p < phi_steps; p++) {
                    int i0 = (t * columns) + p;
                    int i1 = i0 + 1;
                    int i2 = i0 + columns;
                    int i3 = i2 + 1;

                    mesh.TriangleIndices.Add(i0);
                    mesh.TriangleIndices.Add(i2);
                    mesh.TriangleIndices.Add(i1);

                    mesh.TriangleIndices.Add(i1);
                    mesh.TriangleIndices.Add(i2);
                    mesh.TriangleIndices.Add(i3);
                }
            }

            return mesh;
        }

        private static double Lerp(double a, double b, double t) => a + ((b - a) * t);

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
            UpdateViewportSize();
        }

        private void UpdateViewportSize() {
            double size = double.Min(ActualWidth, ActualHeight);

            if (size <= 0) {
                return;
            }

            ViewportHost.Width = size;
            ViewportHost.Height = size;
        }

        private void ApplyRotation() {
            Quaternion q = Rotation;
            double norm = double.Sqrt((q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z));

            if (norm >= 1e-12) {
                Vector3D axis = new(q.X / norm, q.Y / norm, q.Z / norm);
                axis_rotation.Axis = axis;
                axis_rotation.Angle = double.Atan2Pi(norm, q.W) * 360.0;

                return;
            }

            axis_rotation.Axis = new Vector3D(0, 1, 0);
            axis_rotation.Angle = 0;
        }
    }
}
