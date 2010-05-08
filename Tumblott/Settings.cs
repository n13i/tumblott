﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace Tumblott
{
    public sealed class Settings : Dictionary<string,string>
    {
        private static readonly Settings instance = new Settings();

        public enum ProxyMode { NoUse = 0, Use, Default };

        public static string Email
        {
            get { return GetValue("email"); }
            set { instance["email"] = value; }
        }
        public static string Password
        {
            get {
                try
                {
                    byte[] src = Convert.FromBase64String(GetValue("password"));
                    return Encoding.Unicode.GetString(src, 0, src.Length);
                }
                catch(Exception e)
                {
                    return null;
                }
            }
            set {
                try
                {
                    byte[] src = Encoding.Unicode.GetBytes(value);
                    instance["password"] = Convert.ToBase64String(src);
                }
                catch (Exception e)
                {
                    instance["password"] = null;
                }
            }
        }
        public static bool IsAutoLogin
        {
            get { return (GetValue("autologin") == "1"); }
            set { instance["autologin"] = (value ? "1" : "0"); }
        }
        public static bool IsConfirmWhenOpenLinks
        {
            get { return (GetValue("confirmwhenopenlinks") == "1"); }
            set { instance["confirmwhenopenlinks"] = (value ? "1" : "0"); }
        }
        public static ProxyMode Proxy
        {
            get
            {
                return (GetValue("proxymode") == "0" ? ProxyMode.NoUse :
                        GetValue("proxymode") == "1" ? ProxyMode.Use :
                        GetValue("proxymode") == "2" ? ProxyMode.Default :
                                                       ProxyMode.Default);
            }
            set { instance["proxymode"] = ((int)value).ToString(); }
        }
        public static string ProxyServer
        {
            get { return GetValue("proxyserver"); }
            set { instance["proxyserver"] = value; }
        }
        public static int ProxyPort
        {
            get { return int.Parse(GetValue("proxyport")); }
            set { instance["proxyport"] = value.ToString(); }
        }
        public static string ProxyUsername
        {
            get { return GetValue("proxyusername"); }
            set { instance["proxyusername"] = value; }
        }
        public static string ProxyPassword
        {
            // FIXME encrypt/decrypt
            get { return GetValue("proxypassword"); }
            set { instance["proxypassword"] = value; }
        }
        public static int HttpTimeout
        {
            get { return 30000; }
        }
        public static bool DebugLog
        {
            get { return (GetValue("debuglog") == "1"); }
            set { instance["debuglog"] = (value ? "1" : "0"); }
        }

        public static string AppDataPath
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();

                object[] prodArray = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                string product = ((AssemblyProductAttribute)prodArray[0]).Product;

                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                return Path.Combine(appData, product);
            }
        }

        public static string ExePath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
            }
        }

        public static string FilePath
        {
            get
            {
                // 設定ファイルのパス
                string iniFileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().ManifestModule.Name, "ini");
                return Path.Combine(Settings.AppDataPath, iniFileName);
            }
        }

        private Settings()
        {
        }

        private static string GetValue(string key)
        {
            if (instance.ContainsKey(key))
            {
                return instance[key];
            }
            else
            {
                return null;
            }
        }

        public static IWebProxy GetProxy()
        {
            if (Proxy == ProxyMode.NoUse)
            {
                return GlobalProxySelection.GetEmptyWebProxy();
            }
            else if (Proxy == ProxyMode.Use)
            {
                if (Settings.ProxyServer == null)
                {
                    return null;
                }

                WebProxy proxy = new WebProxy("http://" + Settings.ProxyServer + ":" + Settings.ProxyPort.ToString() + "/");
                if (Settings.ProxyUsername != null && Settings.ProxyPassword != null)
                {
                    proxy.Credentials = new NetworkCredential(Settings.ProxyUsername, Settings.ProxyPassword);
                }
                return proxy;
            }

            return null;
        }

        // from TwitterWM
        public static void Load()
        {
            // FIXME 他のところで作るべき？
            if (!Directory.Exists(Settings.AppDataPath))
            {
                Directory.CreateDirectory(Settings.AppDataPath);
            }

            // デフォルト値を設定
            Settings.Email = null;
            Settings.Password = null;
            Settings.IsAutoLogin = false;
            Settings.IsConfirmWhenOpenLinks = true;
            Settings.Proxy = ProxyMode.Default;
            Settings.ProxyServer = null;
            Settings.ProxyPort = 8080;
            Settings.ProxyUsername = null;
            Settings.ProxyPassword = null;
            Settings.DebugLog = false;

            Utils.ReadLine(Settings.FilePath, line =>
            {
                if (line.StartsWith(";")) return;

                int pos = line.IndexOf('=');
                if (pos < 0) return;

                var key = line.Substring(0, pos);
                var value = line.Substring(pos + 1);
                instance[key] = value;
            });
        }

        public static bool Save()
        {
            try
            {
                using (var sw = new StreamWriter(Settings.FilePath))
                {
                    foreach (string key in instance.Keys)
                    {
                        sw.Write(key);
                        sw.Write("=");
                        sw.WriteLine(instance[key]);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}