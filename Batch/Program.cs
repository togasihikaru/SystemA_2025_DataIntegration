using Renci.SshNet;

internal class Program
{
    

    private static void Main(string[] args)
    {
        DownloadAndUpload();
    }

    private static void upload(string sendFileName)
    {
        //秘密鍵のオブジェクト
        var keyFile = new PrivateKeyFile(keyPath, passphrase);

        using (var sftp = new SftpClient(server, user, keyFile))
        {
            try
            {
                //SFTP接続
                sftp.Connect();

                //アップロードファイルパス
                string localFile = Path.Combine(@"D:\SystemA\rsv", sendFileName);

                //コピーするファイル
                string copyFile = Path.Combine(@"D:\SystemA\snd", sendFileName);

                //送信フォルダにコピー
                System.IO.File.Copy(localFile, copyFile, true);

                //アップロード先フォルダパス
                string remoteDir = @"D:\SystemA\23180047\rsv";

                //アップロードファイル名
                string uploadFileName = sendFileName;

                //先頭ディレクトリに戻る
                sftp.ChangeDirectory("/");

                using (var fileStream = new FileStream(localFile, FileMode.Open))
                {
                    //ファイルをアップロード
                    sftp.UploadFile(fileStream, Path.Combine(remoteDir, uploadFileName));
                }

                //成功メッセージ
                Console.WriteLine("アップロード成功");
            }
            catch (Exception ex)
            {
                //失敗メッセージ
                Console.WriteLine("アップロード失敗");
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static void DownloadAndUpload()
    {
        //ダウンロード対象ファイル名
        string remoteFileName = "0000000000000000000000";

        //秘密鍵のオブジェクト
        var keyFile = new PrivateKeyFile(keyPath, passphrase);

        using (var sftp = new SftpClient(server, user, keyFile))
        {
            try
            {
                //SFTP接続
                sftp.Connect();

                //先頭ディレクトリに戻る
                sftp.ChangeDirectory("/");

                //ダウンロードフォルダ
                string remoteDir = @"D:\SystemA\23180047\snd";

                //指定ディレクトリ内のファイルパスを全て取得
                var files = sftp.ListDirectory(remoteDir);

                foreach (var file in files)
                {
                    string temp = file.Name;

                    //現在処理してるファイル名が最新の場合は更新
                    if (long.Parse(remoteFileName.Substring(8, 14)) < long.Parse(temp.Substring(8, 14)))
                    {
                        remoteFileName = temp;
                    }
                }

                //ダウンロード対象ファイルパス
                string remoteFile = Path.Combine(remoteDir, remoteFileName);

                //ダウンロードロード先フォルダパス
                string localDir = @"D:\SystemA\rsv";

                //ダウンロードファイル名
                string fileName = remoteFileName;

                using (var fileStream = new FileStream(Path.Combine(localDir, fileName), FileMode.Create))
                {
                    //ファイルをダウンロード
                    sftp.DownloadFile(remoteFile, fileStream);
                }

                //成功メッセージ
                Console.WriteLine("ダウンロード成功");
            }
            catch (Exception ex)
            {
                //失敗メッセージ
                Console.WriteLine("ダウンロード失敗");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }
        }

        upload(remoteFileName);
    }
}