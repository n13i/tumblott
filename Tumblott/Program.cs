using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Tumblott
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 二重起動チェック
            // 参考: http://dobon.net/vb/dotnet/process/checkprevinstance.html
            Mutex mutex = new Mutex(false);
            if (!mutex.WaitOne(0, false))
            {
                return;
            }

            // 参考: http://blogs.msdn.com/nakama/default.aspx?p=2
            // マニュアルスレッド用の集約例外ハンドラ
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.Run(new MainForm());

            mutex.ReleaseMutex();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            throw new ApplicationException("exception occured on manual thread");
        }
    }
}
