// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TrackBallGUI {
    public partial class TrackBall {
        private readonly AxisAngleRotation3D rotation_rotation = new(new Vector3D(0, 1, 0), 0);

        private const int theta_steps = 18;
        private const int phi_steps = 18;

        private void InitializeScene() {
            Model3DGroup model_root = new();
            model_root.Children.Add(new AmbientLight(Colors.White));

            Model3DGroup group = BuildOctantSphere();
            group.Transform = new RotateTransform3D(rotation_rotation);
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

        private static Model3DGroup BuildOctantSphere() {
            Model3DGroup group = new();

            AddHemisphereOctants(group, yPositive: true, thetaStart: 0.0, thetaEnd: double.Pi / 2.0);
            AddHemisphereOctants(group, yPositive: false, thetaStart: double.Pi / 2.0, thetaEnd: double.Pi);

            return group;
        }

        private static void AddHemisphereOctants(Model3DGroup group, bool yPositive, double thetaStart, double thetaEnd) {
            AddOctant(group, 1, yPositive ? 1 : -1, 1, thetaStart, thetaEnd, 0.0, double.Pi / 2.0);
            AddOctant(group, -1, yPositive ? 1 : -1, 1, thetaStart, thetaEnd, double.Pi / 2.0, double.Pi);
            AddOctant(group, -1, yPositive ? 1 : -1, -1, thetaStart, thetaEnd, double.Pi, double.Pi * 1.5);
            AddOctant(group, 1, yPositive ? 1 : -1, -1, thetaStart, thetaEnd, double.Pi * 1.5, double.Pi * 2.0);
        }

        private static void AddOctant(Model3DGroup group, int x_sign, int y_sign, int z_sign,
            double theta_start, double theta_end, double phi_start, double phi_end) {

            MeshGeometry3D geometry = BuildPatch(theta_start, theta_end, phi_start, phi_end);
            bool white = (x_sign * y_sign * z_sign) > 0;

            Color fillColor = white ? Color.FromRgb(240, 240, 240) : Color.FromRgb(30, 30, 30);
            MaterialGroup material = new();
            material.Children.Add(new DiffuseMaterial(new SolidColorBrush(fillColor)));
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
                double sinTheta = double.Sin(theta);
                double cosTheta = double.Cos(theta);

                for (int p = 0; p <= phi_steps; p++) {
                    double phi = Lerp(phi_start, phi_end, p / (double)phi_steps);
                    double cosPhi = double.Cos(phi);
                    double sinPhi = double.Sin(phi);

                    double x = sinTheta * cosPhi;
                    double y = cosTheta;
                    double z = sinTheta * sinPhi;

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

            if (norm < 1e-12) {
                rotation_rotation.Axis = new Vector3D(0, 1, 0);
                rotation_rotation.Angle = 0;
                return;
            }

            Vector3D axis = new(q.X / norm, q.Y / norm, q.Z / norm);
            double angleRadians = 2.0 * double.Atan2(norm, q.W);

            rotation_rotation.Axis = axis;
            rotation_rotation.Angle = angleRadians * (180.0 / double.Pi);
        }
    }
}
