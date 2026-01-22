using Batch.Common;
using Batch.Data;
using log4net;
using Renci.SshNet;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace Batch
{

    internal class Program
    {
        /// <summary>送信フォルダ</summary>
        private const string SendFolder = @"D:\SystemA\23180047\snd";

        /// <summary>受信フォルダ</summary>
        private const string RcvFolder = @"D:\SystemA\23180047\rcv";

        /// <summary>処理済み受信フォルダ</summary>
        private const string ArchiveRcvFolder = @"D:\SystemA\23180047\rcv\archive";

        /// <summary>エラー受信フォルダ</summary>
        private const string ErrorRcvFolder = @"D:\SystemA\23180047\rcv\error";

        /// <summary>
        /// エントリーポイント
        /// </summary>
        private static void Main(string[] args)
        {
            // log4netの初期化
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            try
            {
                //パラメータ設定チェック
                if (args.Length == 0)
                {
                    Log.WriteLog("処理区分のパラメータが設定されていません。");
                    Environment.Exit(1);
                }

                //処理区分
                string mode = args[0];

                switch (mode)
                {
                    case "1":

                        //パラメータ設定チェック
                        if (args.Length != 3)
                        {
                            Log.WriteLog("開始年月と終了年月のパラメータが設定されていません。");
                            Environment.Exit(1);
                        }

                        //パラメータ妥当性チェック
                        DateTime start = DateTime.MinValue;
                        DateTime end = DateTime.MinValue;

                        if (DateTime.TryParseExact(args[1], "yyyyMM", null, System.Globalization.DateTimeStyles.None, out start) == false ||
                            DateTime.TryParseExact(args[2], "yyyyMM", null, System.Globalization.DateTimeStyles.None, out end) == false)
                        {
                            Log.WriteLog($"開始年月と終了年月のパラメータ設定値が不正です。({args[1]},{args[2]})");
                            Environment.Exit(1);
                        }

                        //相関チェック
                        if (start > end)
                        {
                            Log.WriteLog($"開始年月が終了年月を超えています。({args[1]},{args[2]})");
                            Environment.Exit(1);
                        }

                        //実行
                        SendStoreSales(start, end);

                        break;

                    case "2":

                        //実行
                        ReceiveProductMaster();

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// 売上データ送信処理
        /// </summary>
        private static void SendStoreSales(DateTime _Start, DateTime _End)
        {
            //各クラスのインスタンスを作成
            DBUtil db = new DBUtil();
            BinaryUtil binary = new BinaryUtil();
            SFTPUtil sftp = new SFTPUtil();

            //店舗コードを取得
            string shopCode = ConfigurationManager.AppSettings["SHOP_CODE"];

            //集計年月設定
            DateTime target = _Start;

            //単月処理を繰り返す
            while (target <= _End)
            {
                //出力ファイルパス
                string filePath = string.Empty;

                //売上データ
                List<SalesFile> sales = new List<SalesFile>();

                //処理日次
                DateTime systemDate = DateTime.Now;

                try
                {
                    //出力ファイルパス設定
                    filePath = Path.Combine(SendFolder, $"{shopCode}{target:yyyyMM}{systemDate:yyyyMMddHHmmss}");

                    //売上データ取得
                    sales = db.Select_Sales(target);

                    //データが0件なら処理終了
                    if (sales.Count == 0)
                    {
                        goto Fin;
                    }

                    //売上データファイル作成
                    binary.MakeSales(sales, filePath);
                }
                catch (Exception ex)
                {
                    // 履歴登録(異常)
                    RecordTransmission(
                            systemDate,
                            Category.Send,
                            filePath,
                            Status.Error,
                            $"売上データ作成失敗({ex.Message})"
                        );

                    throw;
                }

                try
                {
                    //送信
                    sftp.PutSalesFiles();

                    // 履歴登録(処理済み)
                    RecordTransmission(
                            systemDate,
                            Category.Send,
                            filePath,
                            Status.Processed,
                            string.Empty
                        );
                }
                catch (Exception ex)
                {
                    // 履歴登録(再送待ち)
                    RecordTransmission(
                            systemDate,
                            Category.Send,
                            filePath,
                            Status.Retry,
                            $"売上データ送信失敗({ex.Message})"
                        );

                    throw;
                }

            Fin:
                //集計年月インクリメント
                target = target.AddMonths(1);
            }
        }

        /// <summary>
        /// 商品マスタ受信
        /// </summary
        private static void ReceiveProductMaster()
        {
            //各クラスのインスタンスを作成
            DBUtil db = new DBUtil();
            BinaryUtil binary = new BinaryUtil();
            SFTPUtil sftp = new SFTPUtil();

            //商品マスタをダウンロード
            sftp.GetProductMasterFiles();

            foreach (string rcvFile in Directory.GetFiles(RcvFolder))
            {
                //処理日時
                DateTime systemDate = DateTime.Now;

                try
                {
                    //商品マスタファイル読込み
                    List<ProductMasterFile> products = binary.ReadProductMaster(rcvFile);

                    //商品マスタテーブル登録
                    db.Insert_product_master(products);

                    //履歴登録(処理済み)
                    RecordTransmission(
                            systemDate,
                            Category.Receive,
                            rcvFile,
                            Status.Processed,
                            string.Empty
                        );

                    //処理済みファイル移動(上書き移動)
                    string destPath = Path.Combine(ArchiveRcvFolder, Path.GetFileName(rcvFile));
                    File.Move(rcvFile, destPath, true);
                }
                catch (Exception ex)
                {
                    // 履歴登録(異常)
                    RecordTransmission(
                            systemDate,
                            Category.Receive,
                            rcvFile,
                            Status.Error,
                            $"商品マスタ登録失敗({ex.Message})"
                        );

                    //エラーファイル移動(上書き移動)
                    string destPath = Path.Combine(ErrorRcvFolder, Path.GetFileName(rcvFile));
                    File.Move(rcvFile, destPath, true);

                    throw;
                }
            }
        }

        /// <summary>
        /// 送受信テーブル記録
        /// </summary>
        private static void RecordTransmission(DateTime _SystemDate, Category _Category, string _FilePath, Status _Status, string _Message)
        {
            //インスタンスを作成
            DBUtil db = new DBUtil();

            try
            {
                //データ登録
                db.Insert_Transmission(
                    new TransmissionTable(
                        _SystemDate,
                        _Category,
                        Path.GetFileName(_FilePath),
                        _Status,
                        _Message
                    )
                );
            }
            catch (Exception ex)
            {
                Log.WriteLog($"送受信テーブル記録失敗({ex.Message})");
            }
        }
    }
}