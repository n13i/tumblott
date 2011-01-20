using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace M2HQ.Utils
{
    public sealed class VisualStyle
    {
        // via http://social.msdn.microsoft.com/Forums/en/netfxcompact/thread/1bc628d9-1810-4251-acdd-adba529194f9
        //     http://d.hatena.ne.jp/ch3cooh393/20101107
        //     http://d.hatena.ne.jp/iseebi/20100521/p1
        //     http://d.hatena.ne.jp/tmyt/20101106/1289053765
        [DllImport("coredll")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;

        [DllImport("coredll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("coredll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        private const int GW_CHILD = 5;

        [DllImport("coredll.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);


        // TabControl
        private const int TCS_TOOLTIPS = 0x4000;
        public static void EnableVisualStyle(TabControl tabControl)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.WinCE)
            {
                return;
            }

            //get handle of native control
            IntPtr hNativeTab = GetWindow(tabControl.Handle, GW_CHILD);
            //get current style flags
            int style = GetWindowLong(hNativeTab, GWL_STYLE);
            //add tooltips style
            style = SetWindowLong(hNativeTab, GWL_STYLE, style | TCS_TOOLTIPS);
        }

        // ListView
        private const int LVS_EX_THEME = 0x02000000;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1000 + 54;
        private const int LVM_GETEXTENDEDLISTVIEWSTYLE = 0x1000 + 55;
        public static void EnableVisualStyle(ListView listView)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.WinCE)
            {
                return;
            }

            int style = SendMessage(listView.Handle, (uint)LVM_GETEXTENDEDLISTVIEWSTYLE, 0, 0);

            SendMessage(listView.Handle, (uint)LVM_SETEXTENDEDLISTVIEWSTYLE, 0, style | LVS_EX_THEME);
        }

        // RadioBox/CheckBox
        private const int BS_THEME = 0x8000;
        private const int BS_EX_SIZEWITHCONTROL = 0x0002;
        public static void EnableVisualStyle(ButtonBase button)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.WinCE)
            {
                return;
            }

            int style = GetWindowLong(button.Handle, GWL_STYLE);
            SetWindowLong(button.Handle, GWL_STYLE, style | BS_THEME);
            int exstyle = GetWindowLong(button.Handle, GWL_EXSTYLE);
            SetWindowLong(button.Handle, GWL_EXSTYLE, exstyle | BS_EX_SIZEWITHCONTROL);
        }

        // ListBox/ComboBox
        private const int CBS_EX_THEME = 0x0004;
        public static void EnableVisualStyle(ListControl listBox)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.WinCE)
            {
                return;
            }

            int exstyle = GetWindowLong(listBox.Handle, GWL_EXSTYLE);
            SetWindowLong(listBox.Handle, GWL_EXSTYLE, exstyle | CBS_EX_THEME);
        }


        public static void ApplyVisualStyle(Control top)
        {
            //System.Diagnostics.Debug.WriteLine(top);

            if(top is TabControl)
            {
                System.Diagnostics.Debug.WriteLine("-> TabControl");
                EnableVisualStyle(top as TabControl);
            }
            else if(top is ListView)
            {
                System.Diagnostics.Debug.WriteLine("-> ListView");
                EnableVisualStyle(top as ListView);
            }
            else if(top is ButtonBase)
            {
                System.Diagnostics.Debug.WriteLine("-> ButtonBase");
                EnableVisualStyle(top as ButtonBase);
            }
            else if(top is ListControl)
            {
                System.Diagnostics.Debug.WriteLine("-> ListControl");
                EnableVisualStyle(top as ListControl);
            }

            foreach (Control c in top.Controls)
            {
                ApplyVisualStyle(c);
            }

            return;
        }
    }
}
