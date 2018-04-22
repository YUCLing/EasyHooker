using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyHooker.Hooker
{
    public class AllHooker
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookExA(int idHook, InsiderCallback lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);

        private delegate int InsiderCallback(int nCode, int wParam, int lParam);

        private int hookId = 0;

        public delegate void AllCallback(int wParam, int lParam);
        private AllCallback ac;

        public AllHooker(AllCallback callback)
        {
            ac = callback;
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
            hookId = SetWindowsHookExA(HookId.WH_CALLWNDPROC, CallOutside, IntPtr.Zero, 0);
            if (hookId == 0)
                RemoveHook();
        }

        public void RemoveHook()
        {
            bool retKeyboard = UnhookWindowsHookEx(hookId);
            if (retKeyboard)
                hookId = 0;
        }

        private int CallOutside(int nCode, int wParam, int lParam)
        {
            if (!(nCode >= 0))
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            ac.Invoke(wParam,lParam);
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}
