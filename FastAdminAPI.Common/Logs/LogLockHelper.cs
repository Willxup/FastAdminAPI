using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FastAdminAPI.Common.Logs
{
    /// <summary>
    /// 高效日志
    /// </summary>
    public static class LogLockHelper
    {
        /// <summary>
        /// 读写锁
        /// </summary>
        private static readonly ReaderWriterLockSlim _lock = new();

        /// <summary>
        /// 写入次数
        /// </summary>
        private static int _writedCount = 0;
        /// <summary>
        /// 失败次数
        /// </summary>
        private static int _failedCount = 0;

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataParas"></param>
        public static void WriteLog(string fileName, params string[] dataParas)
        {
            try
            {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                _lock.EnterWriteLock();

                var path = Path.Combine(Directory.GetCurrentDirectory() + "/logs/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (fileName == "RequestResponseLog")
                {
                    fileName = DateTime.Now.ToString("yyyyMMdd") + fileName;
                }
                var name = fileName + ".log";

                string logFilePath = Path.Combine(path + name);

                var now = DateTime.Now;
                var logContent = (
                    "--------------------------------\r\n" +
                    DateTime.Now + "|\r\n" +
                    string.Join("\r\n", dataParas) + "\r\n"
                    );

                File.AppendAllText(logFilePath, logContent);

                _writedCount++;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _failedCount++;
            }
            finally
            {
                //退出写入模式，释放资源占用
                //注意：一次请求对应一次释放
                //若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //若请求处理完成后未释放将会触发异常[此模式下不允许以递归方式获取写入锁定]
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="path">日志路径</param>
        /// <param name="encode">编码(默认UTF8)</param>
        /// <returns></returns>
        public static string ReadLog(string path, Encoding encode = null)
        {
            string log = "";
            try
            {
                _lock.EnterReadLock();

                if (!File.Exists(path))
                {
                    log = null;
                }
                else
                {
                    StreamReader f2 = new(path, encode ?? Encoding.UTF8);
                    log = f2.ReadToEnd();
                    f2.Close();
                    f2.Dispose();
                }
            }
            catch (Exception)
            {
                _failedCount++;
            }
            finally
            {
                _lock.ExitReadLock();
            }
            return log;
        }

        /// <summary>
        /// 获取写入次数
        /// </summary>
        /// <returns></returns>
        public static int GetWritedCount()
        {
            return _writedCount;
        }
        /// <summary>
        /// 获取失败次数
        /// </summary>
        /// <returns></returns>
        public static int GetFailedCount()
        {
            return _failedCount;
        }
    }
}
