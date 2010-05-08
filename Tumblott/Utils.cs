using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

namespace Tumblott
{
    public class Utils
    {
        // from TwitterWM
        public static void ReadLine(string path, Action<string> delg)
        {
            try
            {
                using (var sr = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        delg(line);
                }
            }
            catch { }
        }

        public static string ParseCookie(string cookie)
        {
            if (cookie == null) { return null; }

            /*
            redirect_to=%2Fiphone;
            expires=Sun, 08-Nov-2009 12:07:47 GMT;
            path=/;
            httponly,
            */
            int pos = 0;
            int index = 0;
            string ret = "";

            while (pos < cookie.Length)
            {
                string key = null, value = null;

                int posEq = cookie.IndexOf('=', pos);
                int posSc = cookie.IndexOf(';', pos);
                int posCm = cookie.IndexOf(',', pos);

                //MessageBox.Show("eq=" + posEq.ToString() + ", sc=" + posSc.ToString() + ", cm=" + posCm.ToString());

                if (posCm == -1)
                {
                    break;
                }

                if (posEq >= 0 && posEq < posSc && posEq < posCm)
                {
                    // KEY=VALUE;
                    // KEY=VALUE,VALUE;
                    key = cookie.Substring(pos, posEq - pos);
                    value = cookie.Substring(posEq + 1, posSc - posEq - 1);
                    pos = posSc + 2;
                    if (index == 0 && key != "redirect_to")
                    {
                        ret += key + "=" + value + "; ";
                    }

                    index++;
                }
                else if (posSc >= 0 && posSc < posEq && posSc < posCm)
                {
                    // KEY;
                    key = cookie.Substring(pos, posSc - pos);
                    value = null;
                    pos = posSc + 2;
                    index++;
                }
                else if (posCm >= 0 && posCm < posEq && posCm < posSc)
                {
                    // KEY,
                    key = cookie.Substring(pos, posCm - pos);
                    value = null;
                    pos = posCm + 1;
                    index = 0;
                }
            }

            return ret;
        }

        public enum ScaleMode { Fit, Full };

        // from http://blogs.wankuma.com/ch3cooh/archive/2008/07/07/147667.aspx
        public static Image GetScaledImage(Image srcImg, int maxW, int maxH, ScaleMode mode)
        {
            int srcW = srcImg.Width;
            int srcH = srcImg.Height;
            Rectangle srcRect = new Rectangle(0, 0, srcW, srcH);

            int dstW, dstH;

            // FIXME 正方形の画像の場合は？
            // fitのときに横方向がはみ出す(縦に合わせられる)場合がある
            if ((srcW > srcH && mode == ScaleMode.Fit) || (srcH > srcW && mode == ScaleMode.Full))
            {
                dstW = maxW;
                dstH = (int)(((float)dstW / (float)srcW) * srcH);
            }
            else
            {
                dstH = maxH;
                dstW = (int)(((float)dstH / (float)srcH) * srcW);
            }

            Image dstImg = new Bitmap(dstW, dstH, PixelFormat.Format16bppRgb565);

            Rectangle dstRect = new Rectangle(0, 0, dstW, dstH);

            Graphics g = Graphics.FromImage(dstImg);
            g.DrawImage(srcImg, dstRect, srcRect, GraphicsUnit.Pixel);
            g.Dispose();

            return dstImg;
        }

        public static string RemoveWhiteSpaces(string str)
        {
            char[] inStr = str.ToCharArray();
            char[] outStr = new char[inStr.Length];

            int opos = 0;

            bool skip = false;

            for (int i = 0; i < inStr.Length; i++)
            {
                if (!skip)
                {
                    outStr[opos] = inStr[i];
                    if (IsWhiteSpace(inStr[i]))
                    {
                        skip = true;
                    }
                    opos++;
                }
                else
                {
                    if (!IsWhiteSpace(inStr[i]))
                    {
                        skip = false;
                        outStr[opos] = inStr[i];
                        opos++;
                    }
                }
            }

            char[] ret = new char[opos];
            Array.Copy(outStr, ret, opos);

            return new string(ret);
        }

        public static bool IsWhiteSpace(char c)
        {
            return (c == ' ' || c == '\n' || c == '\r' || c == '\t');
        }

        // cf. Character entity references in HTML 4 <http://www.w3.org/TR/html4/sgml/entities.html>
        private static int GetCharacterEntity(string ent)
        {
            switch (ent)
            {
                case "nbsp": return 160;
                case "iexcl": return 161;
                case "cent": return 162;
                case "pound": return 163;
                case "curren": return 164;
                case "yen": return 165;
                case "brvbar": return 166;
                case "sect": return 167;
                case "uml": return 168;
                case "copy": return 169;
                case "ordf": return 170;
                case "laquo": return 171;
                case "not": return 172;
                case "shy": return 173;
                case "reg": return 174;
                case "macr": return 175;
                case "deg": return 176;
                case "plusmn": return 177;
                case "sup2": return 178;
                case "sup3": return 179;
                case "acute": return 180;
                case "micro": return 181;
                case "para": return 182;
                case "middot": return 183;
                case "cedil": return 184;
                case "sup1": return 185;
                case "ordm": return 186;
                case "raquo": return 187;
                case "frac14": return 188;
                case "frac12": return 189;
                case "frac34": return 190;
                case "iquest": return 191;
                case "Agrave": return 192;
                case "Aacute": return 193;
                case "Acirc": return 194;
                case "Atilde": return 195;
                case "Auml": return 196;
                case "Aring": return 197;
                case "AElig": return 198;
                case "Ccedil": return 199;
                case "Egrave": return 200;
                case "Eacute": return 201;
                case "Ecirc": return 202;
                case "Euml": return 203;
                case "Igrave": return 204;
                case "Iacute": return 205;
                case "Icirc": return 206;
                case "Iuml": return 207;
                case "ETH": return 208;
                case "Ntilde": return 209;
                case "Ograve": return 210;
                case "Oacute": return 211;
                case "Ocirc": return 212;
                case "Otilde": return 213;
                case "Ouml": return 214;
                case "times": return 215;
                case "Oslash": return 216;
                case "Ugrave": return 217;
                case "Uacute": return 218;
                case "Ucirc": return 219;
                case "Uuml": return 220;
                case "Yacute": return 221;
                case "THORN": return 222;
                case "szlig": return 223;
                case "agrave": return 224;
                case "aacute": return 225;
                case "acirc": return 226;
                case "atilde": return 227;
                case "auml": return 228;
                case "aring": return 229;
                case "aelig": return 230;
                case "ccedil": return 231;
                case "egrave": return 232;
                case "eacute": return 233;
                case "ecirc": return 234;
                case "euml": return 235;
                case "igrave": return 236;
                case "iacute": return 237;
                case "icirc": return 238;
                case "iuml": return 239;
                case "eth": return 240;
                case "ntilde": return 241;
                case "ograve": return 242;
                case "oacute": return 243;
                case "ocirc": return 244;
                case "otilde": return 245;
                case "ouml": return 246;
                case "divide": return 247;
                case "oslash": return 248;
                case "ugrave": return 249;
                case "uacute": return 250;
                case "ucirc": return 251;
                case "uuml": return 252;
                case "yacute": return 253;
                case "thorn": return 254;
                case "yuml": return 255;
                case "fnof": return 402;
                case "Alpha": return 913;
                case "Beta": return 914;
                case "Gamma": return 915;
                case "Delta": return 916;
                case "Epsilon": return 917;
                case "Zeta": return 918;
                case "Eta": return 919;
                case "Theta": return 920;
                case "Iota": return 921;
                case "Kappa": return 922;
                case "Lambda": return 923;
                case "Mu": return 924;
                case "Nu": return 925;
                case "Xi": return 926;
                case "Omicron": return 927;
                case "Pi": return 928;
                case "Rho": return 929;
                case "Sigma": return 931;
                case "Tau": return 932;
                case "Upsilon": return 933;
                case "Phi": return 934;
                case "Chi": return 935;
                case "Psi": return 936;
                case "Omega": return 937;
                case "alpha": return 945;
                case "beta": return 946;
                case "gamma": return 947;
                case "delta": return 948;
                case "epsilon": return 949;
                case "zeta": return 950;
                case "eta": return 951;
                case "theta": return 952;
                case "iota": return 953;
                case "kappa": return 954;
                case "lambda": return 955;
                case "mu": return 956;
                case "nu": return 957;
                case "xi": return 958;
                case "omicron": return 959;
                case "pi": return 960;
                case "rho": return 961;
                case "sigmaf": return 962;
                case "sigma": return 963;
                case "tau": return 964;
                case "upsilon": return 965;
                case "phi": return 966;
                case "chi": return 967;
                case "psi": return 968;
                case "omega": return 969;
                case "thetasym": return 977;
                case "upsih": return 978;
                case "piv": return 982;
                case "bull": return 8226;
                case "hellip": return 8230;
                case "prime": return 8242;
                case "Prime": return 8243;
                case "oline": return 8254;
                case "frasl": return 8260;
                case "weierp": return 8472;
                case "image": return 8465;
                case "real": return 8476;
                case "trade": return 8482;
                case "alefsym": return 8501;
                case "larr": return 8592;
                case "uarr": return 8593;
                case "rarr": return 8594;
                case "darr": return 8595;
                case "harr": return 8596;
                case "crarr": return 8629;
                case "lArr": return 8656;
                case "uArr": return 8657;
                case "rArr": return 8658;
                case "dArr": return 8659;
                case "hArr": return 8660;
                case "forall": return 8704;
                case "part": return 8706;
                case "exist": return 8707;
                case "empty": return 8709;
                case "nabla": return 8711;
                case "isin": return 8712;
                case "notin": return 8713;
                case "ni": return 8715;
                case "prod": return 8719;
                case "sum": return 8721;
                case "minus": return 8722;
                case "lowast": return 8727;
                case "radic": return 8730;
                case "prop": return 8733;
                case "infin": return 8734;
                case "ang": return 8736;
                case "and": return 8743;
                case "or": return 8744;
                case "cap": return 8745;
                case "cup": return 8746;
                case "int": return 8747;
                case "there4": return 8756;
                case "sim": return 8764;
                case "cong": return 8773;
                case "asymp": return 8776;
                case "ne": return 8800;
                case "equiv": return 8801;
                case "le": return 8804;
                case "ge": return 8805;
                case "sub": return 8834;
                case "sup": return 8835;
                case "nsub": return 8836;
                case "sube": return 8838;
                case "supe": return 8839;
                case "oplus": return 8853;
                case "otimes": return 8855;
                case "perp": return 8869;
                case "sdot": return 8901;
                case "lceil": return 8968;
                case "rceil": return 8969;
                case "lfloor": return 8970;
                case "rfloor": return 8971;
                case "lang": return 9001;
                case "rang": return 9002;
                case "loz": return 9674;
                case "spades": return 9824;
                case "clubs": return 9827;
                case "hearts": return 9829;
                case "diams": return 9830;
                case "quot": return 34;
                case "amp": return 38;
                case "lt": return 60;
                case "gt": return 62;
                case "OElig": return 338;
                case "oelig": return 339;
                case "Scaron": return 352;
                case "scaron": return 353;
                case "Yuml": return 376;
                case "circ": return 710;
                case "tilde": return 732;
                case "ensp": return 8194;
                case "emsp": return 8195;
                case "thinsp": return 8201;
                case "zwnj": return 8204;
                case "zwj": return 8205;
                case "lrm": return 8206;
                case "rlm": return 8207;
                case "ndash": return 8211;
                case "mdash": return 8212;
                case "lsquo": return 8216;
                case "rsquo": return 8217;
                case "sbquo": return 8218;
                case "ldquo": return 8220;
                case "rdquo": return 8221;
                case "bdquo": return 8222;
                case "dagger": return 8224;
                case "Dagger": return 8225;
                case "permil": return 8240;
                case "lsaquo": return 8249;
                case "rsaquo": return 8250;
                case "euro": return 8364;
            }

            Utils.DebugLog(ent + " not found");
            return -1;
        }

        public static string ReplaceCharacterEntityReferences(string str)
        {
            int pos = 0;
            StringBuilder sb = new StringBuilder(16384);

            // ex. &raquo; -> &#187;
            while (pos < str.Length)
            {
                int sp = str.IndexOf('&', pos);
                if (sp != -1)
                {
                    int ep = str.IndexOf(';', sp);

                    if (ep == -1)
                    {
                        sb.Append(str.Substring(pos, sp - pos));
                        sb.Append("&amp;");
                        pos = sp + 1;
                    }
                    else if (str.Substring(sp + 1, 1) == "#")
                    {
                        sb.Append(str.Substring(pos, ep - pos + 1));
                        pos = ep + 1;
                    }
                    else
                    {
                        int c = GetCharacterEntity(str.Substring(sp + 1, ep - sp - 1));
                        if (c >= 0)
                        {
                            sb.Append(str.Substring(pos, sp - pos));
                            sb.Append("&#");
                            sb.Append(c.ToString());
                            sb.Append(";");
                            pos = ep + 1;
                        }
                        else
                        {
                            sb.Append(str.Substring(pos, sp - pos));
                            sb.Append("&amp;");
                            pos = sp + 1;
                        }
                    }
                }
                else
                {
                    // FIXME length == 0 のときは？
                    sb.Append(str.Substring(pos, str.Length - pos));
                    break;
                }
            }

            return sb.ToString();
        }

        public static string GetExecutingAssemblyVersion()
        {
            //Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            //return ver.Major.ToString() + "." + ver.Minor.ToString();

            // AssemblyInformationalVersionを使うよう変更(2010/05/01)
            Assembly asm = Assembly.GetExecutingAssembly();
            AssemblyInformationalVersionAttribute ver = (AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(asm, typeof(AssemblyInformationalVersionAttribute));
            return ver.InformationalVersion;
        }

        // from http://blogs.wankuma.com/naka/archive/2004/05/20/2747.aspx
        public static DateTime GetBuiltDateTime()
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            DateTime dt = new DateTime(2000, 1, 1, 0, 0, 0);
            dt = dt.AddDays(ver.Build);
            dt = dt.AddSeconds(ver.Revision * 2);

            return dt;
        }

        // taken from http://d.hatena.ne.jp/kazuv3/20080605/1212656674
        // The codes below are licensed under the NYSL, so I NYS'd
        public static string UrlEncode(string s, Encoding enc)
        {
            StringBuilder rt = new StringBuilder();
            foreach (byte i in enc.GetBytes(s))
                if (i == 0x20)
                    rt.Append('+');
                else if (i >= 0x30 && i <= 0x39 || i >= 0x41 && i <= 0x5a || i >= 0x61 && i <= 0x7a)
                    rt.Append((char)i);
                else
                    rt.Append("%" + i.ToString("X2"));
            return rt.ToString();
        }

        public static string UrlDecode(string s, Encoding enc)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '%')
                    bytes.Add((byte)int.Parse(s[++i].ToString() + s[++i].ToString(), NumberStyles.HexNumber));
                else if (c == '+')
                    bytes.Add((byte)0x20);
                else
                    bytes.Add((byte)c);
            }
            return enc.GetString(bytes.ToArray(), 0, bytes.Count);
        }

        public static void DebugLog(object value)
        {
            if (Settings.DebugLog)
            {
                System.Diagnostics.Debug.WriteLine(value);
            }
        }
    }
}
