using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace M2HQ.Utils
{
    // via http://gihyo.jp/dev/serial/01/windows-phone/0014
    public sealed class Win32Helper
    {
        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        public extern static bool GradientFill(
            IntPtr          hdc,
            TRIVERTEX[]     pVertex,
            uint            dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint            dwNumMesh,
            uint            dwMode
        );

        public struct TRIVERTEX
        {
            public int x;
            public int y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;

            public TRIVERTEX(int x, int y, Color color)
                : this(x, y, color.R, color.G, color.B, color.A)
            {
            }

            public TRIVERTEX(
                int x, int y,
                ushort red, ushort green, ushort blue,
                ushort alpha)
            {
                this.x = x;
                this.y = y;
                this.Red = (ushort)(red << 8);
                this.Green = (ushort)(green << 8);
                this.Blue = (ushort)(blue << 8);
                this.Alpha = (ushort)(alpha << 8);
            }

        }
        public struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;
            public GRADIENT_RECT(uint ul, uint lr)
            {
                this.UpperLeft = ul;
                this.LowerRight = lr;
            }
        }

        public enum GRADIENT_FILL : int
        {
            /// <summary>
            /// 水平方向のグラデーション
            /// </summary>
            GRADIENT_FILL_RECT_H = 0x00000000,

            /// <summary>
            /// 縦方向のグラデーション
            /// </summary>
            GRADIENT_FILL_RECT_V = 0x00000001,

            /// <summary>
            /// Windows CEではサポートされていません
            /// </summary>
            GRADIENT_FILL_TRIANGLE = 0x00000002
        }

        // via http://blogs.msdn.com/b/chrislorton/archive/2006/04/07/570649.aspx
        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "AlphaBlend")]
        public extern static Int32 AlphaBlend(
            IntPtr        hdcDest,
            Int32         xDest,
            Int32         yDest,
            Int32         cxDest,
            Int32         cyDest,
            IntPtr        hdcSrc,
            Int32         xSrc,
            Int32         ySrc,
            Int32         cxSrc,
            Int32         cySrc,
            BlendFunction blendFunction
        );

        public struct BlendFunction
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public enum BlendOperation : byte
        {
            AC_SRC_OVER = 0x00
        }

        public enum BlendFlags : byte
        {
            Zero = 0x00
        }

        public enum SourceConstantAlpha : byte
        {
            Transparent = 0x00,
            Opaque = 0xFF
        }

        public enum AlphaFormat : byte
        {
            AC_SRC_ALPHA = 0x01
        }
    }
}
