using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batch.Data
{
    /// <summary>分類</summary>
    public enum Category
    {
        Send = 0,       
        Receive = 1
    }

    /// <summary>ステータス</summary>
    public enum Status
    {
        Processed = 0,
        Retry = 1,
        Error = 2
    }

    /// <summary>
    /// 送受信テーブル
    /// </summary>
    public class TransmissionTable
    {
        /// <summary>ID</summary>
        public string Id;

        /// <summary>処理日時</summary>
        public DateTime ProcessedAt;

        /// <summary>分類</summary>
        public Category Category;

        /// <summary>ファイル名</summary>
        public string FileName;

        /// <summary>ステータス</summary>
        public Status Status;

        /// <summary>出力メッセージ</summary>
        public string OutputMessage;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public TransmissionTable(DateTime _ProcessedAt, Category _Category, string _FileName, Status _Status, string _OutputMessage, string _Id = "0")                                                                                                                                                                                                                         /// </summary>)
        {
            Id = _Id;
            ProcessedAt = _ProcessedAt;
            Category = _Category;
            FileName = _FileName;
            Status = _Status;
            OutputMessage = _OutputMessage;
        }
    }
}
