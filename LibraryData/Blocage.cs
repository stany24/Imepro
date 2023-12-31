﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LibraryData
{
    /// <summary>
    /// Class to intercept the keyboard strokes to handle them.
    /// </summary>
    public class GlobalKeyboardHook
    {
        #region Constant, Structure, and Delegate Definitions

        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        public struct KeyboardHookStruct
        {
            public int VkCode { get; set; }
            public int ScanCode { get; set; }
            public int Flags { get; set; }
            public int Time { get; set; }
            public int DwExtraInfo { get; set; }
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        #endregion

        #region Instance Variables

        public List<Keys> HookedKeys { get; set; }
        private IntPtr hHook = IntPtr.Zero;
        private static KeyboardHookProc hookProc;

        #endregion

        #region Events

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        #endregion

        #region Constructors and Destructors

        public GlobalKeyboardHook()
        {
            HookedKeys = new();
            hookProc = HookCallback;
            Hook();
        }

        ~GlobalKeyboardHook()
        {
            Unhook();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Démmare l'interception des touches
        /// </summary>
        public void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        /// <summary>
        /// Arrête l'interception des touches
        /// </summary>
        public void Unhook()
        {
            UnhookWindowsHookEx(hHook);
        }

        #endregion

        #region Private Methods

        private int HookCallback(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.VkCode;
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }
            return CallNextHookEx(hHook, code, wParam, ref lParam);
        }

        #endregion

        #region DLL Imports

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion
    }

    /// <summary>
    /// Class to manage the size of other applications
    /// </summary>
    public static class WindowMinimize
    {
        #region Constant

        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOW = 5;

        #endregion

        #region DLL Imports

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        #region Public Methods

        /// <summary>
        /// Function that minimize all unautorised applications
        /// </summary>
        /// <param name="autorisedProcesses"></param>
        public static void MinimizeUnAuthorised(List<string> autorisedProcesses)
        {
            Process thisProcess = Process.GetCurrentProcess();
            List<Process> processes = Process.GetProcesses().ToList();

            foreach (Process process in processes)
            {
                if (process.ProcessName != autorisedProcesses[0] && process.ProcessName != thisProcess.ProcessName)
                {
                    ShowWindow(process.MainWindowHandle, SW_SHOWMINIMIZED);
                }
            }
        }
        /// <summary>
        /// Function that show all application
        /// </summary>
        public static void ShowBack()
        {
            List<Process> processes = Process.GetProcesses().ToList();
            foreach (Process process in processes)
            {
                ShowWindow(process.MainWindowHandle, SW_SHOW);
            }
        }

        #endregion
    }
}
