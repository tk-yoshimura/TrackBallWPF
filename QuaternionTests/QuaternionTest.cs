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

        private static void AssertQuaternion(double expectedW, double expectedX, double expectedY, double expectedZ, Quaternion actual) {
            Assert.AreEqual(expectedW, actual.W, tolerance);
            Assert.AreEqual(expectedX, actual.X, tolerance);
            Assert.AreEqual(expectedY, actual.Y, tolerance);
            Assert.AreEqual(expectedZ, actual.Z, tolerance);
        }
    }
}
