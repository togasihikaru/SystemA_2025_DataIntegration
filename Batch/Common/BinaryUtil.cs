using Batch.Data;

namespace Batch.Common
{
    /// <summary>
    /// バイナリファイル操作クラス
    /// </summary>
    public class BinaryUtil
    {
        /// <summary>
        /// 商品マスタ読込み
        /// </summary>
        /// <param name="_FilePath">読み込み対象のファイルパス</param>
        /// <returns>商品マスタリスト</returns>
        public List<ProductMasterFile> ReadProductMaster(string _FilePath)
        {
            //戻り値
            List <ProductMasterFile> rtnData = new List<ProductMasterFile>();

            //受信フォルダ
            string rcvFolder = @"D:\SystemA\rcv";

            try
            {
                using (FileStream stream = new FileStream(_FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        //1レコード(20バイト)
                        byte[] record = new byte[20];

                        //20バイトずつ読込み
                        while (reader.Read(record, 0, 20) > 0)
                        {
                            //店舗コード(BCD:4バイト)
                            string shopCode = BcdToString(record.AsSpan(0, 4).ToArray());

                            //ファイルバージョン(BCD:2バイト)
                            string fileVersion = BcdToString(record.AsSpan(4, 2).ToArray());

                            //商品コード(BCD:4バイト)
                            string itemCode = BcdToString(record.AsSpan(6, 4).ToArray());

                            //販売単価(BIN:4バイト)
                            string saleUnit = BinToString(record.AsSpan(10, 4).ToArray());

                            //仕入単価(BIN:4バイト)
                            string purchaseUnit = BinToString(record.AsSpan(14, 4).ToArray());

                            //販売フラグ(BIN:2バイト)
                            string saleFlg = BinToString(record.AsSpan(18, 2).ToArray());

                            //レコード追加
                            rtnData.Add(
                                new ProductMasterFile(
                                    shopCode, 
                                    fileVersion, 
                                    itemCode, 
                                    saleUnit, 
                                    purchaseUnit, 
                                    saleFlg
                                )
                            );
                        }
                    }
                }

                return rtnData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 店舗売上データ作成
        /// </summary>
        public void MakeSales(List<SalesFile> _Data, string _FilePath)
        {
            try
            {
                using (FileStream stream = new FileStream(_FilePath, FileMode.Append, FileAccess.Write))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        foreach (SalesFile data in _Data)
                        {
                            //レコードを構成するバイト配列のリスト
                            List<byte[]> record = new List<byte[]>
                            {
                                StringToBcd(data.ShopCode),                 //店舗コード(BCD:4バイト)
                                StringToBcd(data.SummaryTarget),            //集計年月(BCD:3バイト)
                                StringToBcd(data.ItemCode),                 //商品コード(BCD:4バイト)
                                IntToBin(data.SaleUnit),                    //販売単価(BIN:4バイト)
                                IntToBin(data.TotalQuantity),               //売上総数量(BIN:4バイト)
                                IntToBin(data.TotalDiscount),               //割引適用総額(BIN:4バイト)
                                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 } //予約領域(BIN:5バイト)
                            };

                            //各項目を順番に書き込み
                            foreach (byte[] value in record)
                            {
                                writer.Write(value.ToArray());
                            }
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
        /// BCD形式→文字列変換
        /// </summary>
        private string BcdToString(byte[] _Value)
        {
            try
            {
                return BitConverter.ToString(_Value).Replace("-", "");

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// BIN形式→文字列変換
        /// </summary>
        private string BinToString(byte[] _Value)
        {
            try
            {
                string temp = BitConverter.ToString(_Value).Replace("-", "");
                return Convert.ToInt32(temp, 16).ToString();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 文字列→BCD形式変換
        /// </summary>
        private byte[] StringToBcd(string _Value)
        {
            List<byte> rtnData = new List<byte>();

            try
            {
                for (int j = 0; j < _Value.Length; j += 2)
                {
                    rtnData.Add(Convert.ToByte(_Value.Substring(j, 2), 16));
                }

                return rtnData.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 数値(int)型→BIN形式変換
        /// </summary>
        private byte[] IntToBin(int _Value)
        {
            List<byte> rtnValue = new List<byte>();

            try
            {
                byte[] temp = BitConverter.GetBytes(_Value);
                foreach (byte value in temp.Reverse())
                {
                    rtnValue.Add(value);
                }
                return rtnValue.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
