﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

        public class SystemSleepManagement
        {
            [DllImport("kernel32.dll")]
            static extern uint SetThreadExecutionState(ExecutionFlag flags);

            [Flags]
            enum ExecutionFlag : uint
            {
                System = 0x00000001,
                Display = 0x00000002,
                Continus = 0x80000000,
            }

            public static void PreventSleep(bool includeDisplay = false)
            {
                if (includeDisplay)
                    SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Display | ExecutionFlag.Continus);
                else
                    SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Continus);
            }

            public static void ResotreSleep()
            {
                SetThreadExecutionState(ExecutionFlag.Continus);
            }

            public static void ResetSleepTimer(bool includeDisplay = false)
            {
                if (includeDisplay)
                    SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Display);
                else
                    SetThreadExecutionState(ExecutionFlag.System);
            }
        }

        #region --- ElevatedDragDrop ---
        public class ElevatedDragDropManager : IMessageFilter
        {
            #region "P/Invoke"
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, ChangeWindowMessageFilterExAction action, ref CHANGEFILTERSTRUCT changeInfo);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ChangeWindowMessageFilter(uint msg, ChangeWindowMessageFilterFlags flags);

            [DllImport("shell32.dll")]
            private static extern void DragAcceptFiles(IntPtr hwnd, bool fAccept);

            [DllImport("shell32.dll")]
            private static extern uint DragQueryFileW(IntPtr hDrop, uint iFile, [MarshalAs(UnmanagedType.LPWStr)][Out()] StringBuilder lpszFile, uint cch);

            [DllImport("shell32.dll")]
            private static extern bool DragQueryPoint(IntPtr hDrop, ref POINT lppt);

            [DllImport("shell32.dll")]
            private static extern void DragFinish(IntPtr hDrop);

            [StructLayout(LayoutKind.Sequential)]
            private struct POINT
            {
                public int X;

                public int Y;
                public POINT(int newX, int newY)
                {
                    X = newX;
                    Y = newY;
                }

                public static implicit operator System.Drawing.Point(POINT p)
                {
                    return new System.Drawing.Point(p.X, p.Y);
                }

                public static implicit operator POINT(System.Drawing.Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }

            private enum MessageFilterInfo : uint
            {
                None,
                AlreadyAllowed,
                AlreadyDisAllowed,
                AllowedHigher
            }

            private enum ChangeWindowMessageFilterExAction : uint
            {
                Reset,
                Allow,
                Disallow
            }

            private enum ChangeWindowMessageFilterFlags : uint
            {
                Add = 1,
                Remove = 2
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct CHANGEFILTERSTRUCT
            {
                public uint cbSize;
                public MessageFilterInfo ExtStatus;
            }
            #endregion

            public static ElevatedDragDropManager Instance = new ElevatedDragDropManager();
            public event EventHandler<ElevatedDragDropArgs> ElevatedDragDrop;

            private const uint WM_DROPFILES = 0x233;
            private const uint WM_COPYDATA = 0x4a;

            private const uint WM_COPYGLOBALDATA = 0x49;
            private readonly bool IsVistaOrHigher = Environment.OSVersion.Version.Major >= 6;

            private readonly bool Is7OrHigher = (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major > 6;
            protected ElevatedDragDropManager()
            {
                Application.AddMessageFilter(this);
            }

            public void EnableDragDrop(IntPtr hWnd)
            {
                if (Is7OrHigher)
                {
                    CHANGEFILTERSTRUCT changeStruct = new CHANGEFILTERSTRUCT()
                    {
                        cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT)))
                    };
                    ChangeWindowMessageFilterEx(hWnd, WM_DROPFILES, ChangeWindowMessageFilterExAction.Allow, ref changeStruct);
                    ChangeWindowMessageFilterEx(hWnd, WM_COPYDATA, ChangeWindowMessageFilterExAction.Allow, ref changeStruct);
                    ChangeWindowMessageFilterEx(hWnd, WM_COPYGLOBALDATA, ChangeWindowMessageFilterExAction.Allow, ref changeStruct);
                }
                else if (IsVistaOrHigher)
                {
                    ChangeWindowMessageFilter(WM_DROPFILES, ChangeWindowMessageFilterFlags.Add);
                    ChangeWindowMessageFilter(WM_COPYDATA, ChangeWindowMessageFilterFlags.Add);
                    ChangeWindowMessageFilter(WM_COPYGLOBALDATA, ChangeWindowMessageFilterFlags.Add);
                }

                DragAcceptFiles(hWnd, true);
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_DROPFILES)
                {
                    HandleDragDropMessage(m);
                    return true;
                }

                return false;
            }

            private void HandleDragDropMessage(Message m)
            {
                uint numFiles = DragQueryFileW(m.WParam, 0xFFFFFFFF, null, 0);
                dynamic list = new List<string>();

                for (uint i = 0; i <= numFiles - 1; i++)
                {
                    uint size = DragQueryFileW(m.WParam, i, null, 0);
                    StringBuilder sb = new StringBuilder((int)size);
                    if (DragQueryFileW(m.WParam, i, sb, (uint)sb.Capacity + 1) > 0)
                    {
                        list.Add(sb.ToString());
                    }
                }

                POINT p = default;
                DragQueryPoint(m.WParam, ref p);
                DragFinish(m.WParam);

                dynamic args = new ElevatedDragDropArgs()
                {
                    HWnd = m.HWnd,
                    Files = list,
                    X = p.X,
                    Y = p.Y
                };

                ElevatedDragDrop?.Invoke(this, args);
            }
        }

        public class ElevatedDragDropArgs : EventArgs
        {
            public IntPtr HWnd
            {
                get { return m_HWnd; }
                set { m_HWnd = value; }
            }
            private IntPtr m_HWnd;
            public List<string> Files
            {
                get { return m_Files; }
                set { m_Files = value; }
            }
            private List<string> m_Files;
            public int X
            {
                get { return m_X; }
                set { m_X = value; }
            }
            private int m_X;
            public int Y
            {
                get { return m_Y; }
                set { m_Y = value; }
            }

            private int m_Y;
            public ElevatedDragDropArgs()
            {
                Files = new List<string>();
            }
        }
        #endregion

    }



}
