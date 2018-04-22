# EasyHooker
让你的程序hook得更简单!

## 使用示例（C#）
### 鼠标
```
using EasyHooker.Hooker;
using EasyHooker.Mouse;
public bool Handle(MouseHooker.Point pt,Int32 message) {
  if (message == Messages.WM_MOUSEMOVE)
    System.Diagnostics.Debug.WriteLine(String.Format("Mouse position:{0},{1}",pt.x,pt.y));
  //如果你想拦截这个信息，那就return true
  return false;
}

MouseHooker mh = new MouseHooker(Handle);
if (!mh.IsHooked)
  mh.SetupHook();
//然后是移除hook...
mh.RemoveHook();
```

### 键盘
```
using System.Windows.Forms;
using EasyHooker.Hooker;
public bool Handle(Keys key) {
  if (key == Keys.F1)
    System.Diagnostics.Debug.WriteLine("Hello!");
  //如果你想拦截这个信息，那就return true
  return false;
}

KeyHooker kh = new KeyHooker(Handle);
if (!kh.IsHooked)
  kh.SetupHook();
//然后是移除hook...
kh.RemoveHook();
```
