// Copyright (c) T.Yoshimura 2026
// https://github.com/tk-yoshimura

using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace TrackBallGUI {
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct Quaternion : IEquatable<Quaternion> {
        public double W { get; }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Quaternion(double w, double x, double y, double z) {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public double NormSquared => (W * W) + (X * X) + (Y * Y) + (Z * Z);

        public double Norm => double.Sqrt(NormSquared);

        public static readonly Quaternion Identity = new(1.0, 0.0, 0.0, 0.0);

        public static readonly Quaternion NaN = new(double.NaN, 0.0, 0.0, 0.0);

        public Quaternion Conjugate() => new(W, -X, -Y, -Z);

        public Quaternion Inverse() {
            double norm_squa = NormSquared;
            if (norm_squa == 0.0) {
                return NaN;
            }

            Quaternion c = Conjugate();
            return new Quaternion(c.W / norm_squa, c.X / norm_squa, c.Y / norm_squa, c.Z / norm_squa);
        }

        public static Quaternion FromVectors(Vector3D from, Vector3D to) {
            from.Normalize();
            to.Normalize();

            double dot = Vector3D.DotProduct(from, to);
            if (dot <= -0.999999) {
                Vector3D axis = Vector3D.CrossProduct(new Vector3D(1, 0, 0), from);

                if (axis.LengthSquared < 1e-8) {
                    axis = Vector3D.CrossProduct(new Vector3D(0, 1, 0), from);
                }

                axis.Normalize();
                return new Quaternion(0, axis.X, axis.Y, axis.Z);
            }

            Vector3D cross = Vector3D.CrossProduct(from, to);
            Quaternion q = new(1.0 + dot, cross.X, cross.Y, cross.Z);

            return Normalize(q);
        }

        public static Quaternion Normalize(Quaternion q) {
            double norm = q.Norm;

            if (norm == 0 || double.IsNaN(norm)) {
                return Identity;
            }
            if (double.Abs(norm - 1.0) < 1e-14) {
                return q;
            }

            return new Quaternion(q.W / norm, q.X / norm, q.Y / norm, q.Z / norm);
        }

        public static Quaternion operator +(Quaternion a, Quaternion b) =>
            new(a.W + b.W, a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Quaternion operator -(Quaternion a, Quaternion b) =>
            new(a.W - b.W, a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Quaternion operator -(Quaternion value) =>
            new(-value.W, -value.X, -value.Y, -value.Z);

        public static Quaternion operator *(Quaternion a, Quaternion b) {
            double w = (a.W * b.W) - (a.X * b.X) - (a.Y * b.Y) - (a.Z * b.Z);
            double x = (a.W * b.X) + (a.X * b.W) + (a.Y * b.Z) - (a.Z * b.Y);
            double y = (a.W * b.Y) - (a.X * b.Z) + (a.Y * b.W) + (a.Z * b.X);
            double z = (a.W * b.Z) + (a.X * b.Y) - (a.Y * b.X) + (a.Z * b.W);

            return new Quaternion(w, x, y, z);
        }

        public static Quaternion operator /(Quaternion a, Quaternion b) => a * b.Inverse();

        public static bool operator ==(Quaternion a, Quaternion b) {
            return a.W == b.W && a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Quaternion a, Quaternion b) => !(a == b);

        public bool Equals(Quaternion other) {
            return this == other;
        }

        public override bool Equals(object? obj) => obj is Quaternion other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(W, X, Y, Z);

        public override string ToString() => $"({W}, {X}, {Y}, {Z})";
    }
}
