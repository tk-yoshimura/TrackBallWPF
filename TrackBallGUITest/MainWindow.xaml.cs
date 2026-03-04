using System.Windows;
using TrackBallGUI;

namespace TrackBallGUITest {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private const int MaxLogLength = 100;

        public MainWindow() {
            InitializeComponent();
            TrackBallControl1.RotationChanged += TrackBallControl_RotationChanged;
            TrackBallControl1.RotationChangeCompleted += TrackBallControl_RotationChangeCompleted;
        }

        private void TrackBallControl_RotationChanged(object? sender, RotationChangedEventArgs e) {
            AppendLog($"dragging: {FormatQuaternion(e.Quaternion)}");
        }

        private void TrackBallControl_RotationChangeCompleted(object? sender, RotationChangedEventArgs e) {
            AppendLog($"released: {FormatQuaternion(e.Quaternion)}");
        }

        private void AppendLog(string message) {
            var timestamped = $"{DateTime.Now:HH:mm:ss.fff} {message}";
            if (string.IsNullOrWhiteSpace(LogTextBox.Text)) {
                LogTextBox.Text = timestamped;
            }
            else {
                LogTextBox.AppendText(Environment.NewLine + timestamped);
            }

            TrimLogLines();
            LogTextBox.ScrollToEnd();
        }

        private void TrimLogLines() {
            var lines = LogTextBox.Text.Split(Environment.NewLine, StringSplitOptions.None);
            if (lines.Length <= MaxLogLength) {
                return;
            }

            var start = lines.Length - MaxLogLength;
            LogTextBox.Text = string.Join(Environment.NewLine, lines[start..]);
        }

        private static string FormatQuaternion(Quaternion q) {
            return $"W={q.W:F4}, X={q.X:F4}, Y={q.Y:F4}, Z={q.Z:F4}";
        }
    }
}
