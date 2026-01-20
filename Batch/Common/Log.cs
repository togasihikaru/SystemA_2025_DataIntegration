namespace Batch.Common
{
    /// <summary>
    /// ログクラス
    /// </summary>
    public static class Log
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// エラーログ出力
        /// </summary>
        public static void WriteLog(string argMsg)
        {
            logger.Error(argMsg);
        }
    }
}
