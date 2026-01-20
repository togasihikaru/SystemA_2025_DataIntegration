using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Batch.Data
{
    /// <summary>
    /// 店舗売上データファイル
    /// </summary>
    public class SalesFile
    {
        /// <summary>店舗コード</summary>
        public string ShopCode;

        /// <summary>集計年月</summary>
        public string SummaryTarget;

        /// <summary>商品コード</summary>
        public string ItemCode;

        /// <summary>販売単価</summary>
        public int SaleUnit;

        /// <summary>売上総数量</summary>
        public int TotalQuantity;

        /// <summary>割引適用総額</summary>
        public int TotalDiscount;

        /// <summary>予約領域</summary>
        public int Padding = 0;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        public SalesFile(string _ShopCode, string _SummaryTarget, string _ItemCode, int _SaleUnit, int _TotalPurchase, int _TotalDiscount)
        {
            try
            {
                ShopCode = _ShopCode;
                SummaryTarget = _SummaryTarget;
                ItemCode = _ItemCode;
                SaleUnit = _SaleUnit;
                TotalQuantity = _TotalPurchase;
                TotalDiscount = _TotalDiscount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
