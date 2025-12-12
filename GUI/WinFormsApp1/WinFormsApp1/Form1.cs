using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // Internal class to hold P/Invoke definitions
        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        }

        private const int CLICK_COUNT = 100;
        private const int CLICK_INTERVAL_MS = 30; // Milliseconds between clicks
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const string TARGET_PROCESS_NAME = "chrome";

        public Form1()
        {
            InitializeComponent();
            // Optional: You can change the button text here if you want
            this.button1.Text = $"Switch to Chrome and Click {CLICK_COUNT} times";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // 1. Find the Chrome process and window handle
            IntPtr chromeHandle = IntPtr.Zero;
            Process[] chromeProcesses = Process.GetProcessesByName(TARGET_PROCESS_NAME);
            
            if (chromeProcesses.Length == 0)
            {
                MessageBox.Show($"Could not find the process '{TARGET_PROCESS_NAME}'.\nPlease make sure Google Chrome is running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Find the first process that has a main window handle.
            foreach (var process in chromeProcesses)
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    chromeHandle = process.MainWindowHandle;
                    break;
                }
            }

            if (chromeHandle == IntPtr.Zero)
            {
                MessageBox.Show("Google Chrome is running, but could not find its main window.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Activate the Chrome window
            NativeMethods.SetForegroundWindow(chromeHandle);
            await Task.Delay(500); // Wait half a second to ensure the window is focused

            // 3. Get screen center and move the mouse
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Cursor.Position = new Point(screenWidth / 2, screenHeight / 2);

            // 4. Perform the clicks
            for (int i = 0; i < CLICK_COUNT; i++)
            {
                NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                await Task.Delay(CLICK_INTERVAL_MS);
            }

            MessageBox.Show($"{CLICK_COUNT} clicks have been completed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
