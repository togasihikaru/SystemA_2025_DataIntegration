using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Windows Formsを使用

namespace Management
{

    public partial class SalesData : Form
    {

        // 売上データレコードを表すクラス
        public class SalesRecord
        {
            public DateTime SalesDate { get; set; }
            public string CategoryName { get; set; }
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            // ... その他の項目
        }

        // データを扱うクラスやヘルパー（前回の例で定義）をインスタンス化
        private Transmission _accessor = new Transmission();

        // 商品分類チェックボックスをグループ化するためのリスト (必要に応じて実際のコントロール名に修正)
        private List<CheckBox> _categoryCheckBoxes = new List<CheckBox>();

        // ※ デザイナーで定義された `_salesRecord` を使用するため、追加の DataGridView フィールドは不要

        public SalesData()
        {
            InitializeComponent();

            // デザイナーで配置した _salesRecord を初期設定
            _salesRecord.AllowUserToAddRows = false;
            _salesRecord.AllowUserToDeleteRows = false;
            _salesRecord.ReadOnly = true;
            _salesRecord.AutoGenerateColumns = false; // 明示的にカラムを設定する場合は false

            // デザイナーで配置したチェックボックスをリストに追加
            _categoryCheckBoxes.Add(chkCategoryA);
            _categoryCheckBoxes.Add(chkCategoryB);
            _categoryCheckBoxes.Add(chkCategoryC);
            _categoryCheckBoxes.Add(chkCategoryD);

            // デザイナでイベントが未接続の場合があるため、ここでイベントを接続
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnSendSalesData.Click += btnSendSalesData_Click;
            btnRefresh.Click += btnRefresh_Click;
            // Designer の閉じるボタン名は `button2` なので、既存の `btnClose_Click` を接続
            button2.Click += btnClose_Click;
        }

        private void SalesData_Load(object sender, EventArgs e)
        {
            // --- 元の DataGridView 初期設定 （デザイナーの _salesRecord を使用） ---
            // カラム数を指定
            _salesRecord.ColumnCount = 7;

            // カラム名を指定
            _salesRecord.Columns[0].HeaderText = "販売日時";
            _salesRecord.Columns[1].HeaderText = "分類";
            _salesRecord.Columns[2].HeaderText = "商品番号";
            _salesRecord.Columns[3].HeaderText = "商品名";
            _salesRecord.Columns[4].HeaderText = "売上数量";
            _salesRecord.Columns[5].HeaderText = "割引適用額";
            _salesRecord.Columns[6].HeaderText = "売上額";

            // 表示用に枠内の行を確保（設計に合わせ最大15行）
            _salesRecord.Rows.Clear();
            for (int i = 0; i < 15; i++)
            {
                _salesRecord.Rows.Add();
            }
            // --- ここまで元のコード ---

            // **1. 初期表示処理の追加**

            // 期間の初期設定 (現在日時から年月を取得して開始・終了に設定)
            string currentMonth = DateTime.Now.ToString("yyyy/MM");
            txtStartDate.Text = currentMonth;
            txtEndDate.Text = currentMonth;

            // 商品分類の初期設定 (全項目にチェック)
            CheckAllCategories();

            // ボタンの初期状態設定
            btnSendSalesData.Enabled = false; // 検索前は非活性
            btnRefresh.Enabled = false;       // 検索前は非活性

            // DataGridView の初期表示設定（幅調整、スクロールバー設定など）
            AdjustGridDisplay();
        }

        // ===============================================
        //           イベントハンドラの実装
        // ===============================================

        /// <summary>
        /// 検索ボタン押下時の処理
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 1. 変数の初期化と入力値の取得
            DateTime startDate;
            DateTime endDate;
            string productId = txtProductId.Text.Trim();
            string productName = txtProductName.Text.Trim();
            List<string> selectedCategories = GetSelectedCategories();

            // 2. 入力チェック
            if (!ValidateInput(productId, productName, selectedCategories, out startDate, out endDate))
            {
                return; // チェック失敗時はエラーメッセージ表示済みのため終了
            }

            try
            {
                // 3. データ取得
                List<SalesRecord> salesList = _accessor.GetSalesData(
                    startDate, endDate, selectedCategories, productId, productName);

                // 4. データ一覧の表示
                // デザイナーの _salesRecord にデータソースを設定
                _salesRecord.DataSource = null;
                _salesRecord.Rows.Clear();
                _salesRecord.AutoGenerateColumns = true; // DataSource で自動カラムにする場合
                _salesRecord.DataSource = salesList;
                AdjustGridDisplay(); // グリッドの幅調整や行数制限を行う

                // 5. ボタンの制御
                btnSendSalesData.Enabled = true;
                btnRefresh.Enabled = true;

            }
            catch (Exception ex)
            {
                // 6. 例外エラー処理
                MessageBox.Show("売上データの検索中に予期せぬエラーが発生しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // エラーログ記録（実装必須）
                LogError(ex);
            }
        }

        /// <summary>
        /// 条件クリアボタン押下時の処理
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            // 期間の初期化 (現在日時の年月を設定)
            string currentMonth = DateTime.Now.ToString("yyyy/MM");
            txtStartDate.Text = currentMonth;
            txtEndDate.Text = currentMonth;

            // 商品分類の初期化 (全てチェック)
            CheckAllCategories();

            // その他の項目の初期化
            txtProductId.Clear();
            txtProductName.Clear();
        }

        /// <summary>
        /// 売上データ送信ボタン押下時の処理
        /// </summary>
        private void btnSendSalesData_Click(object sender, EventArgs e)
        {
            // 1. 確認メッセージの表示
            string confirmMsg = "選択されている期間の売上データを、期間以外の検索条件は反映せず、月別に全店舗管理サーバへ送信します。よろしいですか？";
            DialogResult result = MessageBox.Show(confirmMsg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            // 2. 送信処理の実行（バッチ起動）
            try
            {
                string startDate = txtStartDate.Text;
                string endDate = txtEndDate.Text;

                // バッチ処理を起動し、リターンコードを取得
                int returnCode = StartBatchProcess(startDate, endDate);

                // 3. リターンコードによる成否判定とメッセージ表示
                if (returnCode == 0) // 0を正常終了と仮定
                {
                    MessageBox.Show("売上データの送信が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"売上データの送信中にエラーが発生しました。\nリターンコード: {returnCode}", "異常終了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // バッチ起動自体に失敗した場合の処理
                MessageBox.Show("売上データ送信バッチの起動中に予期せぬエラーが発生しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError(ex);
            }
        }

        /// <summary>
        /// 更新ボタン押下時の処理
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // 検索ボタン押下のイベント処理を呼び出す
            btnSearch_Click(sender, e);
        }

        /// <summary>
        /// 閉じるボタン押下時の処理（ボタン名に応じてイベント名を設定）
        /// 親フォーム（Top）が Owner に設定されていればそれを再表示してから閉じる
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Owner が設定されていれば再表示
            try
            {
                if (this.Owner != null)
                {
                    this.Owner.Show();
                }
                else
                {
                    // フォールバック: Application.OpenForms から Top を探して再表示
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f is Top)
                        {
                            f.Show();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 念のため例外はログに出力して処理を継続
                LogError(ex);
            }

            this.Close();
        }

        // ===============================================
        //           ヘルパーメソッドの実装（前回の設計内容）
        // ===============================================

        /// <summary>
        /// 検索条件の入力チェックを実施する
        /// </summary>
        private bool ValidateInput(string productId, string productName, List<string> categories, out DateTime startDate, out DateTime endDate)
        {
            // ... (前回の回答のValidateInputメソッドの内容を実装) ...
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            // 期間チェック
            if (!DateTime.TryParseExact(txtStartDate.Text, "yyyy/MM", null, System.Globalization.DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(txtEndDate.Text, "yyyy/MM", null, System.Globalization.DateTimeStyles.None, out endDate))
            {
                MessageBox.Show("期間は 'yyyy/MM' 形式で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 商品分類チェック
            if (categories.Count == 0)
            {
                MessageBox.Show("商品分類を一つ以上選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 商品番号チェック
            if (!string.IsNullOrEmpty(productId))
            {
                if (productId.Length > 5 || !System.Text.RegularExpressions.Regex.IsMatch(productId, @"^\d+$"))
                {
                    MessageBox.Show("商品番号は半角数値で5桁以下で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // 商品名チェック
            if (productName.Length > 30)
            {
                MessageBox.Show("商品名は30文字以下で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 選択されている商品分類名を取得する
        /// </summary>
        private List<string> GetSelectedCategories()
        {
            var list = new List<string>();
            foreach (var chk in _categoryCheckBoxes)
            {
                if (chk.Checked)
                {
                    list.Add(chk.Text);
                }
            }
            return list;
        }

        /// <summary>
        /// 全ての商品分類にチェックを入れる
        /// </summary>
        private void CheckAllCategories()
        {
            foreach (var chk in _categoryCheckBoxes)
            {
                chk.Checked = true;
            }
        }

        /// <summary>
        /// DataGridViewの表示設定（幅調整、行数制限など）を行う
        /// </summary>
        private void AdjustGridDisplay()
        {
            // 横スクロールせずに全ての項目が表示できるよう幅を調整
            _salesRecord.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // 枠内に表示するデータ行数は最大15行とし、縦スクロールで続きが表示できること
            // (これは Rows.Add() で行を予約しているか、データバインドの表示領域に依存します)

            // 商品分類は検索条件の文言に合わせて表示すること（データバインド時に実施）
        }

        /// <summary>
        /// 外部のバッチ処理を起動し、終了コードを返す（仮実装）
        /// </summary>
        private int StartBatchProcess(string startDate, string endDate)
        {
            // 実際は System.Diagnostics.Process を使用して外部プロセスを実行
            // 例: Process.Start("BatchSender.exe", $"{startDate} {endDate}");
            return 0;
        }

        /// <summary>
        /// エラーログを記録する（仮実装）
        /// </summary>
        private void LogError(Exception ex)
        {
            // 例: ファイル、データベース、イベントログなどに例外情報を記録
            System.Diagnostics.Debug.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}