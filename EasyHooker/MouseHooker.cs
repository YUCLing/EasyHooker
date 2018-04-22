using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyHooker.Hooker
{
    public class MouseHooker
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookExA(int idHook, InsiderCallback lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, MouseStruct lParam);

        private delegate int InsiderCallback(int nCode, int wParam, MouseStruct lParam);

        private int hookId = 0;
        private MouseCallback mc;

        [StructLayout(LayoutKind.Sequential)]
        public class Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class MouseStruct
        {
            public Point pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public delegate bool MouseCallback(Point pt, int message);

        public MouseHooker(MouseCallback callback)
        {
            mc = callback;
        }

        public bool IsHooked
        {
            get
            {
                if (hookId != 0)
                    return true;
                return false;
            }
        }

        public void SetupHook()
        {
            hookId = SetWindowsHookExA(HookId.WH_KEYBOARD_LL, CallOutside, IntPtr.Zero, 0);
            if (hookId == 0)
                RemoveHook();
        }

        public void RemoveHook()
        {
            bool retKeyboard = UnhookWindowsHookEx(hookId);
            if (retKeyboard)
                hookId = 0;
        }

        private int CallOutside(int nCode, int wParam, MouseStruct lParam)
        {
            if (!(nCode >= 0))
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            bool shouldBlock = mc.Invoke(lParam.pt, wParam);
            if (!shouldBlock)
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            return 1;
        }
    }
}
