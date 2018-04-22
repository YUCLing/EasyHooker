using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyHooker.Hooker
{
    public class CustomHooker
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookExA(int idHook, InsiderCallback lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);

        private delegate int InsiderCallback(int nCode, int wParam, int lParam);

        private int hookId = 0;
        private int idHook;

        public delegate bool CustomCallback(int wParam, int lParam);
        private CustomCallback cc;
        private PropertyInfo[] propertyInfos;

        private PropertyInfo[] GetPropertyInfoArray(Type type)
        {
            PropertyInfo[] props = null;
            try
            {
                object obj = Activator.CreateInstance(type);
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            } finally {}
            return props;
        }

        private bool isIdHookVaild(int idHook)
        {
            HookId hi = new HookId();
            foreach (PropertyInfo prop in propertyInfos)
                if (idHook == (int)prop.GetValue(hi, null))
                    return true;
            return false;
        }

        public CustomHooker(int idHook,CustomCallback callback)
        {
            propertyInfos = GetPropertyInfoArray(typeof(HookId));
            if (isIdHookVaild(idHook))
                throw new Exception("Supported hook!");
            this.idHook = idHook;
            cc = callback;
        }

        public void SetupHook()
        {
            hookId = SetWindowsHookExA(idHook, CallOutside, IntPtr.Zero, 0);
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
            cc.Invoke(wParam, lParam);
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}
