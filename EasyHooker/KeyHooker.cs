using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EasyHooker.Hooker
{
    public class KeyHooker
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookExA(int idHook, InsiderCallback lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, KeyStruct lParam);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(int idHook);

        private int hookId = 0;

        private delegate int InsiderCallback(int nCode, int wParam, KeyStruct lParam);

        public delegate bool KeyCallback(Keys key,int message);
        private KeyCallback kc;

        public bool IsHooked
        {
            get
            {
                if (hookId != 0)
                    return true;
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KeyStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public KeyHooker(KeyCallback callback)
        {
            kc = callback;
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

        private int CallOutside(int nCode, int wParam, KeyStruct lParam)
        {
            if (!(nCode >= 0))
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            bool shouldBlock = kc.Invoke((Keys)lParam.vkCode, wParam);
            if (!shouldBlock)
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            return 1;
        }
    }
}
