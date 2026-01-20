using Renci.SshNet;
using System;
using System.Configuration;

namespace Batch.Common
{
    /// <summary>
    /// SFTP操作クラス
    /// </summary>
    public class SFTPUtil
    {
        /// <summary>サーバ名</summary>
        private string SFTPServer;

        /// <summary>ユーザ名</summary>
        private string SFTPUser;

        /// <summary>秘密鍵ファイルパス</summary>
        private string KeyPath;

        /// <summary>パスフレーズ</summary>
        private string PassPhrase;

        /// <summary>店舗コード</summary>
        private string ShopCode;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SFTPUtil()
        {
            //configファイルから接続情報を読み込む
            SFTPServer = ConfigurationManager.AppSettings["SFTP_SERVER"];
            SFTPUser = ConfigurationManager.AppSettings["SFTP_USER"];
            KeyPath = ConfigurationManager.AppSettings["KEY_PATH"];
            PassPhrase = ConfigurationManager.AppSettings["KEY_PASS_PHRASE"];
            ShopCode = ConfigurationManager.AppSettings["SHOP_CODE"];
        }

        /// <summary>
        /// 商品マスタファイル取得
        /// </summary>
        public void GetProductMasterFiles()
        {
            //ダウンロード元(送信)フォルダ
            string sndFolder = $@"D:\SystemA\{ShopCode}\snd";

            //ダウンロード先(受信)フォルダ
            string rcvFolder = @"D:\SystemA\rcv";

            try
            {
                //秘密鍵作成
                PrivateKeyFile keyFile = new PrivateKeyFile(KeyPath, PassPhrase);

                using (var sftp = new SftpClient(SFTPServer, SFTPUser, keyFile))
                {
                    //sftpサーバ接続
                    sftp.Connect();

                    //先頭ディレクトリに移動
                    sftp.ChangeDirectory("/");

                    //送信フォルダ内のファイル・フォルダ一覧を取得
                    var files = sftp.ListDirectory(sndFolder);

                    foreach (var file in files)
                    {
                        //ディレクトリやシンボリックリンクはスキップ
                        if (file.IsDirectory || file.IsSymbolicLink)
                        {
                            continue;
                        }

                        //ダウンロード先ファイルパス設定
                        string rcvFilePath = Path.Combine(rcvFolder, file.Name);

                        //ダウンロード実行
                        using (var stream = new FileStream(rcvFilePath, FileMode.Create))
                        {
                            sftp.DownloadFile(file.FullName, stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 店舗売上データ送信
        /// </summary>
        public void PutSalesFiles()
        {
            //送信元(送信)フォルダ
            string sndFolder = @"D:\SystemA\snd";

            //送信先(受信)フォルダ
            string rcvFolder = $@"D:\SystemA\{ShopCode}\rcv";

            try
            {
                //秘密鍵作成
                PrivateKeyFile keyFile = new PrivateKeyFile(KeyPath, PassPhrase);

                using (var sftp = new SftpClient(SFTPServer, SFTPUser, keyFile))
                {
                    //sftpサーバ接続
                    sftp.Connect();

                    //先頭ディレクトリに移動
                    sftp.ChangeDirectory("/");

                    //送信元フォルダ内のファイル一覧を取得
                    string[] files = Directory.GetFiles(sndFolder);

                    foreach (string file in files)
                    {
                        //送信ファイル名を取得
                        string sndFileName = Path.GetFileName(file);

                        //送信先ファイルパスを設定
                        string sndFilePath = Path.Combine(rcvFolder, sndFileName);

                        //アップロード実行
                        using (var stream = new FileStream(file, FileMode.Open))
                        {
                            sftp.UploadFile(stream, sndFilePath);
                        }

                        //再送待ちのステータスを更新
                        DBUtil db = new DBUtil();
                        db.Update_Transmission(sndFileName);

                        //処理済みフォルダに移動
                        string archivePath = Path.Combine(sndFolder, "archive", sndFileName);
                        File.Move(file, archivePath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
