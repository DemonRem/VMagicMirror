﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Baku.VMagicMirror
{
    //NOTE: マウス位置関係の処理は「人が動かしたのかプログラムでマウスが動いたのか」の判定をしようと頑張った残骸で、
    //再利用する可能性があるので残している
    class MouseHook : IDisposable
    {
        private IntPtr _hHook;
        //明示的に参照保持しないとデリゲートがGCされてしまうのでわざわざ参照を持つ(アンマネージ感がすごい)
        private readonly WindowsAPI.HOOKPROC _hookProc;

        public int FilteredX => X + DX;
        public int FilteredY => Y + DY;

        private readonly object _posXLock = new object();
        private int _x = 0;
        public int X
        {
            get { lock (_posXLock) return _x; }
            private set { lock (_posXLock) _x = value; }
        }

        private readonly object _posYLock = new object();
        private int _y = 0;
        public int Y
        {
            get { lock (_posYLock) return _y; }
            private set { lock (_posYLock) _y = value; }
        }

        private readonly object _posDXLock = new object();
        private int _dx = 0;
        public int DX
        {
            get { lock (_posDXLock) return _dx; }
            private set { lock (_posDXLock) _dx = value; }
        }

        private readonly object _posDYLock = new object();
        private int _dy = 0;
        public int DY
        {
            get { lock (_posDYLock) return _dy; }
            private set { lock (_posDYLock) _dy = value; }
        }

        private readonly object _mouseMessagesLock = new object();
        private readonly Queue<int> _mouseMessages = new Queue<int>();
        private void EnqueueMouseMessage(int msg)
        {
            lock (_mouseMessagesLock)
            {
                _mouseMessages.Enqueue(msg);
            }
        }

        public int DequeueMouseMessage()
        {
            lock (_mouseMessagesLock)
            {
                if (_mouseMessages.Count > 0)
                {
                    return _mouseMessages.Dequeue();
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool HasMouseMessage()
        {
            lock (_mouseMessagesLock)
            {
                return (_mouseMessages.Count > 0);
            }
        }

        public MouseHook()
        {
            _hookProc = HookProc;
        }

        public bool SetHook()
        {
            string moduleName = Process.GetCurrentProcess().MainModule?.ModuleName ?? "";
            IntPtr hModule = string.IsNullOrEmpty(moduleName)
                ? IntPtr.Zero
                : WindowsAPI.GetModuleHandle(moduleName);
            
            _hHook = WindowsAPI.SetWindowsHookEx(
                (int)WindowsAPI.HookType.WH_MOUSE_LL,
                _hookProc, 
                hModule,
                IntPtr.Zero
                );

            return (_hHook != IntPtr.Zero);
        }

        private IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode != WindowsAPI.HC_ACTION)
            {
                return WindowsAPI.CallNextHookEx(_hHook, nCode, wParam, lParam);
            }
            
            try
            {
                //TODO: ここ文字列経由するのパフォーマンス的に勿体ないような
                int wParamVal = wParam.ToInt32();
                string info =
                    (wParamVal == WindowsAPI.MouseMessages.WM_LBUTTONDOWN) ? "LDown" :
                    (wParamVal == WindowsAPI.MouseMessages.WM_LBUTTONUP) ? "LUp" :
                    (wParamVal == WindowsAPI.MouseMessages.WM_RBUTTONDOWN) ? "RDown" :
                    (wParamVal == WindowsAPI.MouseMessages.WM_RBUTTONUP) ? "RUp" :
                    (wParamVal == WindowsAPI.MouseMessages.WM_MBUTTONDOWN) ? "MDown" :
                    (wParamVal == WindowsAPI.MouseMessages.WM_MBUTTONUP) ? "MUp" :
                    "";

                if (!string.IsNullOrEmpty(info))
                {
                    MouseButton?.Invoke(this, new MouseButtonEventArgs(info));
                }
                
//                CheckMousePositionEvent(wParamVal, lParam, info);
            }
            catch (Exception)
            {
                //ここはLogOutputに流さない: キーボード叩くたびにファイルI/Oは流石にまずい
            }

            return WindowsAPI.CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        public void RemoveHook() => WindowsAPI.UnhookWindowsHookEx(_hHook);

        public void Dispose() => RemoveHook();

        public event EventHandler<MouseButtonEventArgs> MouseButton;

        private void CheckMousePositionEvent(int wParamVal, IntPtr lParam, string info)
        {
            var mouseHook = Marshal.PtrToStructure<WindowsAPI.MSLLHOOKSTRUCT>(lParam);

            if (wParamVal == WindowsAPI.MouseMessages.WM_MOUSEMOVE)
            {
                //ソフトが動かした場合、勝手に動かした分を差分として保存することで
                //「手だけで動かしてたらここにマウスがあったはず」という情報が残るようにしたい
                if ((mouseHook.flags & WindowsAPI.MouseFlags.LLMHF_INJECTED) != 0)
                {
                    //動きが怪しいのでDX,DYに非ゼロ値が入らないようにしておく
                    //DX += X - mouseHook.pt.x;
                    //DY += Y - mouseHook.pt.y;
                }
                X = mouseHook.pt.x;
                Y = mouseHook.pt.y;
            }
            
            if (!string.IsNullOrEmpty(info))
            {
                //クリック時にDXとDYをリセットする = クリック時点で一旦マウスやタッチパッドから手を離したものと扱う
                DX = 0;
                DY = 0;
                X = mouseHook.pt.x;
                Y = mouseHook.pt.y;
            }
        }
    }

    class MouseButtonEventArgs : EventArgs
    {
        public MouseButtonEventArgs(string info)
        {
            Info = info;
        }
        public string Info { get; }
    }

}