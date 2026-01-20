using System;

namespace Batch.Data
{
    /// <summary>
    /// 商品マスタファイル
    /// </summary>
    public class ProductMasterFile
    {
        /// <summary>店舗コード</summary>
        public string ShopCode;

        /// <summary>ファイルバージョン</summary>
        public string FileVersion;

        /// <summary>商品コード</summary>
        public string ItemCode;

        /// <summary>販売単価</summary>
        public string SaleUnit;

        /// <summary>仕入単価</summary>
        public string PurchaseUnit;

        /// <summary>販売フラグ</summary>
        public string SaleFlg;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public ProductMasterFile(string _ShopCode, string _FileVersion, string _ItemCode, string _SaleUnit, string _PurchaseUnit, string _SaleFlg)
        {
            try
            {
                ShopCode = _ShopCode;
                FileVersion = _FileVersion;
                ItemCode = _ItemCode;
                SaleUnit = _SaleUnit;
                PurchaseUnit = _PurchaseUnit;
                SaleFlg = _SaleFlg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
