// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TrackBallGUI {
    public partial class TrackBall {
        private readonly AxisAngleRotation3D axis_rotation = new(new Vector3D(0, 1, 0), 0);

        private const int sphere_divisions = 16;

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

            AddOctant(group, +1, +1, +1);
            AddOctant(group, -1, +1, +1);
            AddOctant(group, -1, +1, -1);
            AddOctant(group, +1, +1, -1);

            AddOctant(group, +1, -1, +1);
            AddOctant(group, -1, -1, +1);
            AddOctant(group, -1, -1, -1);
            AddOctant(group, +1, -1, -1);

            return group;
        }

        private static void AddOctant(Model3DGroup group, int x_sign, int y_sign, int z_sign) {
            MeshGeometry3D geometry = BuildOctantMesh(sphere_divisions, x_sign, y_sign, z_sign);
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

        private static MeshGeometry3D BuildOctantMesh(int divisions, int x_sign, int y_sign, int z_sign) {
            Debug.Assert(divisions > 1 && divisions <= 256, $"Out of range: {nameof(divisions)}");

            MeshGeometry3D mesh = new();
            Dictionary<(int i, int j, int k), int> index_map = [];
            bool flip_winding = (x_sign * y_sign * z_sign) < 0;

            for (int i = 0; i <= divisions; i++) {
                for (int j = 0; j <= divisions - i; j++) {
                    int k = divisions - i - j;

                    Point3D position = LatticeToOctantPoint(divisions, x_sign, y_sign, z_sign, i, j, k);

                    mesh.Positions.Add(position);

                    Vector3D normal = new(position.X, position.Y, position.Z);
                    normal.Normalize();
                    mesh.Normals.Add(normal);

                    index_map[(i, j, k)] = mesh.Positions.Count - 1;
                }
            }

            for (int i = 0; i < divisions; i++) {
                for (int j = 0; j < divisions - i; j++) {
                    int k = divisions - i - j;

                    int a = index_map[(i, j, k)];
                    int b = index_map[(i + 1, j, k - 1)];
                    int c = index_map[(i, j + 1, k - 1)];
                    AddTriangle(mesh, a, b, c, flip_winding);

                    if (k >= 2) {
                        int d = index_map[(i + 1, j + 1, k - 2)];
                        AddTriangle(mesh, b, d, c, flip_winding);
                    }
                }
            }

            return mesh;
        }

        private static Point3D LatticeToOctantPoint(int divisions, int x_sign, int y_sign, int z_sign, int i, int j, int k) {
            double n = divisions;

            (int ring_ij, int ring_jk, int ring_ki) = (i + j, j + k, k + i);

            double theta_ij = 0.5 * (ring_ij / n);
            double theta_jk = 0.5 * (ring_jk / n);
            double theta_ki = 0.5 * (ring_ki / n);

            double phi_ij = ring_ij > 0 ? 0.5 * (j / (double)ring_ij) : 0.0;
            double phi_jk = ring_jk > 0 ? 0.5 * (k / (double)ring_jk) : 0.0;
            double phi_ki = ring_ki > 0 ? 0.5 * (i / (double)ring_ki) : 0.0;

            (double sin_theta_ij, double cos_theta_ij) = double.SinCosPi(theta_ij);
            (double sin_theta_jk, double cos_theta_jk) = double.SinCosPi(theta_jk);
            (double sin_theta_ki, double cos_theta_ki) = double.SinCosPi(theta_ki);

            (double sin_phi_ij, double cos_phi_ij) = double.SinCosPi(phi_ij);
            (double sin_phi_jk, double cos_phi_jk) = double.SinCosPi(phi_jk);
            (double sin_phi_ki, double cos_phi_ki) = double.SinCosPi(phi_ki);

            double x = (sin_theta_ij * cos_phi_ij + sin_theta_ki * sin_phi_ki + cos_theta_jk) / 3;
            double y = (sin_theta_jk * cos_phi_jk + sin_theta_ij * sin_phi_ij + cos_theta_ki) / 3;
            double z = (sin_theta_ki * cos_phi_ki + sin_theta_jk * sin_phi_jk + cos_theta_ij) / 3;

            return new(x_sign * x, y_sign * y, z_sign * z);
        }

        private static void AddTriangle(MeshGeometry3D mesh, int i0, int i1, int i2, bool flip_winding) {
            if (!flip_winding) {
                mesh.TriangleIndices.Add(i0);
                mesh.TriangleIndices.Add(i1);
                mesh.TriangleIndices.Add(i2);
            }
            else {
                mesh.TriangleIndices.Add(i0);
                mesh.TriangleIndices.Add(i2);
                mesh.TriangleIndices.Add(i1);
            }
        }

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
