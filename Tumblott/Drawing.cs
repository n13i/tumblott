using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using M2HQ.Utils;

namespace M2HQ.Drawing
{
    // via http://gihyo.jp/dev/serial/01/windows-phone/0014
    public sealed class Gradient
    {
        public enum FillDirection
        {
            // 水平にグラデーションで塗りつぶします
            LeftToRight = Win32Helper.GRADIENT_FILL.GRADIENT_FILL_RECT_H,

            // 垂直にグラデーションで塗りつぶします
            TopToBottom = Win32Helper.GRADIENT_FILL.GRADIENT_FILL_RECT_V
        }

        public sealed class GradientFill
        {
            public static bool Fill(
                Graphics gr, Rectangle rc,
                Color startColor, Color endColor,
                FillDirection fillDir)
            {
                if (System.Environment.OSVersion.Platform != PlatformID.WinCE)
                {
                    using (SolidBrush br = new SolidBrush(startColor))
                    {
                        gr.FillRectangle(br, rc);
                    }
                    return true;
                }

                // 頂点の座標と色を指定
                Win32Helper.TRIVERTEX[] tva = new Win32Helper.TRIVERTEX[2];
                tva[0] = new Win32Helper.TRIVERTEX(rc.X, rc.Y, startColor);
                tva[1] = new Win32Helper.TRIVERTEX(rc.Right, rc.Bottom, endColor);

                // どのTRIVERTEXの値を使用するかインデックスを指定
                Win32Helper.GRADIENT_RECT[] gra
                    = new Win32Helper.GRADIENT_RECT[] { new Win32Helper.GRADIENT_RECT(0, 1) };

                // GradientFill関数の呼び出し
                IntPtr hdc = gr.GetHdc();
                bool b = Win32Helper.GradientFill(
                    hdc, tva, (uint)tva.Length,
                    gra, (uint)gra.Length, (uint)fillDir);
                gr.ReleaseHdc(hdc);
                return b;
            }
        }
    }

    public sealed class AlphaBlend
    {
        public static void DrawImage(Graphics gxDst, Image imgSrc, int dstX, int dstY, int alpha)
        {
            if (System.Environment.OSVersion.Platform != PlatformID.WinCE)
            {
                gxDst.DrawImage(imgSrc, dstX, dstY);
                return;
            }

            Graphics gxSrc = Graphics.FromImage(imgSrc);
            IntPtr hdcDst = gxDst.GetHdc();
            IntPtr hdcSrc = gxSrc.GetHdc();
            Win32Helper.BlendFunction blendFunction = new Win32Helper.BlendFunction();
            blendFunction.BlendOp = (byte)Win32Helper.BlendOperation.AC_SRC_OVER;
            blendFunction.BlendFlags = (byte)Win32Helper.BlendFlags.Zero;
            blendFunction.SourceConstantAlpha = (byte)alpha;
            blendFunction.AlphaFormat = (byte)0;
            Win32Helper.AlphaBlend(hdcDst, dstX, dstY, imgSrc.Width, imgSrc.Height, hdcSrc, 0, 0, imgSrc.Width, imgSrc.Height, blendFunction);
            gxSrc.ReleaseHdc(hdcSrc);
            gxDst.ReleaseHdc(hdcDst);
            gxSrc.Dispose();
        }
    }
}
