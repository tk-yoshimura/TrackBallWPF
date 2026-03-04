
using TrackBallGUI;

namespace QuaternionTests {
    [TestClass]
    public sealed class QuaternionTest {
        private const double tolerance = 1e-10;

        [TestMethod]
        public void AddTest() {
            Quaternion q1 = new(1, 2, 3, 4);
            Quaternion q2 = new(5, 6, 7, 8);

            Quaternion result = q1 + q2;

            AssertQuaternion(6, 8, 10, 12, result);
        }

        [TestMethod]
        public void SubTest() {
            Quaternion q1 = new(1, 2, 3, 4);
            Quaternion q2 = new(5, 6, 7, 8);

            Quaternion result = q1 - q2;

            AssertQuaternion(-4, -4, -4, -4, result);
        }

        [TestMethod]
        public void UnaryMinusTest() {
            Quaternion q = new(1, -2, 3, -4);

            Quaternion result = -q;

            AssertQuaternion(-1, 2, -3, 4, result);
        }

        [TestMethod]
        public void MulTest() {
            Quaternion q1 = new(1, 2, 3, 4);
            Quaternion q2 = new(5, 6, 7, 8);

            Quaternion result = q1 * q2;

            AssertQuaternion(-60, 12, 30, 24, result);
        }

        [TestMethod]
        public void DivTest() {
            Quaternion q1 = new(1, 2, 3, 4);
            Quaternion q2 = new(5, 6, 7, 8);

            Quaternion result = (q1 / q2) * q2;

            AssertQuaternion(q1.W, q1.X, q1.Y, q1.Z, result);
        }

        [TestMethod]
        public void ConjugateTest() {
            Quaternion q = new(1, 2, -3, 4);

            Quaternion result = q.Conjugate();

            AssertQuaternion(1, -2, 3, -4, result);
        }

        [TestMethod]
        public void InverseTest() {
            Quaternion q = new(1, 2, 3, 4);

            Quaternion result = q * q.Inverse();

            AssertQuaternion(1, 0, 0, 0, result);
        }

        [TestMethod]
        public void InverseZeroReturnsNaNTest() {
            Quaternion result = new Quaternion(0, 0, 0, 0).Inverse();

            Assert.IsTrue(double.IsNaN(result.W));
            Assert.AreEqual(0.0, result.X, tolerance);
            Assert.AreEqual(0.0, result.Y, tolerance);
            Assert.AreEqual(0.0, result.Z, tolerance);
        }

        [TestMethod]
        public void NormalizeZeroReturnsIdentityTest() {
            Quaternion result = Quaternion.Normalize(new Quaternion(0, 0, 0, 0));

            AssertQuaternion(1, 0, 0, 0, result);
        }

        [TestMethod]
        public void NormalizeNaNReturnsIdentityTest() {
            Quaternion result = Quaternion.Normalize(Quaternion.NaN);

            AssertQuaternion(1, 0, 0, 0, result);
        }

        [TestMethod]
        public void NormalizeUnitQuaternionReturnsSameValueTest() {
            Quaternion unit = new(1, 0, 0, 0);

            Quaternion result = Quaternion.Normalize(unit);

            AssertQuaternion(unit.W, unit.X, unit.Y, unit.Z, result);
        }

        [TestMethod]
        public void FromVectorsSameDirectionReturnsIdentityTest() {
            System.Windows.Media.Media3D.Vector3D from = new(1, 0, 0);
            System.Windows.Media.Media3D.Vector3D to = new(2, 0, 0);

            Quaternion result = Quaternion.FromVectors(from, to);

            AssertQuaternion(1, 0, 0, 0, result);
        }

        [TestMethod]
        public void FromVectorsOppositeDirectionReturnsHalfTurnQuaternionTest() {
            System.Windows.Media.Media3D.Vector3D from = new(1, 0, 0);
            System.Windows.Media.Media3D.Vector3D to = new(-1, 0, 0);

            Quaternion result = Quaternion.FromVectors(from, to);

            Assert.AreEqual(0.0, result.W, tolerance);
            Assert.AreEqual(0.0, result.X, tolerance);
            Assert.AreEqual(0.0, result.Y, tolerance);
            Assert.AreEqual(-1.0, result.Z, tolerance);
        }

        [TestMethod]
        public void EqualsAndOperatorTest() {
            Quaternion a = new(1, 2, 3, 4);
            Quaternion b = new(1, 2, 3, 4);
            Quaternion c = new(1, 2, 3, 5);

            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);
            Assert.IsFalse(a.Equals(c));
        }

        [TestMethod]
        public void CastToSystemNumericsQuaternionTest() {
            Quaternion q = new(1.5, 2.5, 3.5, 4.5);

            global::System.Numerics.Quaternion result = (global::System.Numerics.Quaternion)q;

            Assert.AreEqual(2.5f, result.X, 1e-6f);
            Assert.AreEqual(3.5f, result.Y, 1e-6f);
            Assert.AreEqual(4.5f, result.Z, 1e-6f);
            Assert.AreEqual(1.5f, result.W, 1e-6f);
        }

        [TestMethod]
        public void CastFromSystemNumericsQuaternionTest() {
            global::System.Numerics.Quaternion q = new(2.5f, 3.5f, 4.5f, 1.5f);

            Quaternion result = q;

            AssertQuaternion(1.5, 2.5, 3.5, 4.5, result);
        }

        private static void AssertQuaternion(double expectedW, double expectedX, double expectedY, double expectedZ, Quaternion actual) {
            Assert.AreEqual(expectedW, actual.W, tolerance);
            Assert.AreEqual(expectedX, actual.X, tolerance);
            Assert.AreEqual(expectedY, actual.Y, tolerance);
            Assert.AreEqual(expectedZ, actual.Z, tolerance);
        }
    }
}
