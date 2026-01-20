using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Management
{
    public partial class Transmission : Form
    {
        // DATASOURCE: 実運用では app.config / secrets に接続文字列を入れてください。
        // 空文字列の場合はサンプルデータを返します（ビルド・動作確認用）。
        private const string ConnectionString = ""; // 例: "Server=...;Database=...;User Id=...;Password=...;";

        public Transmission()
        {
            InitializeComponent();

            // デザイナーでイベント未接続のためここで接続
            button1.Click += btnSearch_Click;   // 検索
            button2.Click += btnClear_Click;    // 条件クリア
            button3.Click += btnRefresh_Click;  // 更新
            button4.Click += btnClose_Click;    // 閉じる

            Load += Transmission_Load;
        }

        private void Transmission_Load(object sender, EventArgs e)
        {
            // 期間の初期表示：現在の年月を yyyy/MM 形式で開始・終了に設定
            var current = DateTime.Now;
            txtStartDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);
            txtEndDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);

            // 分類・ステータスは初期で全チェック（UIの全チェックボックス）
            chkCategoryA.Checked = true;
            chkCategoryB.Checked = true;
            chkCategoryC.Checked = true;
            chkCategoryD.Checked = true;
            checkBox1.Checked = true;

            // DataGridView の初期設定（表示上の要件に合わせる）
            _salesRecord.AllowUserToAddRows = false;
            _salesRecord.AllowUserToDeleteRows = false;
            _salesRecord.ReadOnly = true;
            _salesRecord.AutoGenerateColumns = true;
            _salesRecord.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _salesRecord.ScrollBars = ScrollBars.Vertical;
            AdjustGridDisplay();
            
            // 初期は検索前のため更新ボタンは無効
            button3.Enabled = false;
        }

        // -------------------------
        // イベントハンドラ
        // -------------------------

        // 検索ボタン押下
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 入力値取得
            string startText = txtStartDate.Text.Trim();
            string endText = txtEndDate.Text.Trim();
            string productId = ""; // Transmission 画面に商品番号等の入力コントロールがないため空にしています
            string productName = ""; // 同上

            var selectedCategories = GetSelectedCategories();

            // 入力チェック・期間変換
            if (!TryParsePeriod(startText, endText, out DateTime startDate, out DateTime endDate))
            {
                MessageBox.Show("期間は 'yyyy/MM' 形式で入力してください。単月は開始と終了を同じ年月にしてください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedCategories.Count == 0)
            {
                MessageBox.Show("分類/ステータスを一つ以上選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // DB からデータ取得（またはサンプルデータ返却）
                var table = QueryTransmissionLogs(startDate, endDate, selectedCategories, productId, productName);

                // 表示：横スクロールしないようにカラム幅調整（列幅は画面幅に合わせる）
                _salesRecord.DataSource = table;
                _salesRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                AdjustGridDisplay();

                // 更新ボタン活性化（検索後にアクティブ）
                button3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("送受信ログの検索中にエラーが発生しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError(ex);
            }
        }

        // 条件クリアボタン押下
        private void btnClear_Click(object sender, EventArgs e)
        {
            // 期間を現在年月に戻す
            var current = DateTime.Now;
            txtStartDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);
            txtEndDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);

            // 分類・ステータスを全チェック
            chkCategoryA.Checked = true;
            chkCategoryB.Checked = true;
            chkCategoryC.Checked = true;
            chkCategoryD.Checked = true;
            checkBox1.Checked = true;

            // グリッド・メッセージクリア
            _salesRecord.DataSource = null;
        }

        // 更新ボタン押下：検索処理を呼び出す
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // 検索ボタンと同じ処理を呼ぶ
            btnSearch_Click(sender, e);
        }

        // 閉じるボタン押下
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Owner != null)
                {
                    this.Owner.Show();
                }
                else
                {
                    // Top フォームが開いていればそれを再表示
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
                LogError(ex);
            }

            this.Close();
        }

        // -------------------------
        // データ取得・ユーティリティ
        // -------------------------

        /// <summary>
        /// 送受信ログをデータベースから取得して DataTable を返す。
        /// 接続文字列が未設定の場合はサンプルデータを返す（動作確認用）。
        /// </summary>
        private DataTable QueryTransmissionLogs(DateTime startDate, DateTime endDate, List<string> selectedCategories, string productId, string productName)
        {
            // 取得するカラム名は表示要件に合わせる（ファイル名など実際の列名に合わせてください）
            DataTable dt = new DataTable();
            dt.Columns.Add("処理日時", typeof(DateTime));
            dt.Columns.Add("分類", typeof(string));
            dt.Columns.Add("ステータス", typeof(string));
            dt.Columns.Add("ファイル名", typeof(string));
            dt.Columns.Add("商品番号", typeof(string));
            dt.Columns.Add("商品名", typeof(string));
            dt.Columns.Add("件数", typeof(int));

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                // サンプルデータ（接続文字列未設定時）
                for (int i = 0; i < 12; i++)
                {
                    dt.Rows.Add(
                        startDate.AddDays(i),
                        i % 2 == 0 ? "送信" : "受信",
                        (i % 3) == 0 ? "処理済み" : (i % 3) == 1 ? "再送待ち" : "異常",
                        $"file_{i + 1}.txt",
                        (1000 + i).ToString(),
                        $"商品名サンプル{i + 1}",
                        (i + 1) * 10
                    );
                }
                return dt;
            }

            // SQL を構築（パラメータ化して SQL インジェクションを防止）
            string sql = @"
                SELECT LogDate, Classification, Status, FileName, ProductId, ProductName, Count
                FROM TransmissionLog
                WHERE LogDate >= @startDate AND LogDate <= @endDate
            ";

            // 分類・ステータスは selectedCategories に含まれるものを OR でマッチさせる
            // 実運用では分類列とステータス列を分けて適切にフィルタすること
            if (selectedCategories != null && selectedCategories.Count > 0)
            {
                // サンプル的に Classification IN (...) OR Status IN (...)
                sql += " AND (Classification IN ({0}) OR Status IN ({0}))";
            }

            if (!string.IsNullOrWhiteSpace(productId))
            {
                sql += " AND ProductId = @productId";
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                sql += " AND ProductName LIKE @productName";
            }

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@startDate", startDate.Date);
                    cmd.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1).AddTicks(-1)); // end of day

                    // selectedCategories 用のパラメータ名リストを作成
                    if (selectedCategories != null && selectedCategories.Count > 0)
                    {
                        var names = new List<string>();
                        for (int i = 0; i < selectedCategories.Count; i++)
                        {
                            string param = "@cat" + i;
                            names.Add(param);
                            cmd.Parameters.AddWithValue(param, selectedCategories[i]);
                        }
                        string joined = string.Join(",", names);
                        cmd.CommandText = string.Format(cmd.CommandText, joined);
                    }
                    else
                    {
                        cmd.CommandText = string.Format(cmd.CommandText, "NULL");
                    }

                    if (!string.IsNullOrWhiteSpace(productId))
                    {
                        cmd.Parameters.AddWithValue("@productId", productId);
                    }
                    if (!string.IsNullOrWhiteSpace(productName))
                    {
                        cmd.Parameters.AddWithValue("@productName", "%" + productName + "%");
                    }

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dt.Rows.Add(
                                reader.GetDateTime(reader.GetOrdinal("LogDate")),
                                reader["Classification"]?.ToString() ?? string.Empty,
                                reader["Status"]?.ToString() ?? string.Empty,
                                reader["FileName"]?.ToString() ?? string.Empty,
                                reader["ProductId"]?.ToString() ?? string.Empty,
                                reader["ProductName"]?.ToString() ?? string.Empty,
                                reader["Count"] != DBNull.Value ? Convert.ToInt32(reader["Count"]) : 0
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // DB エラーは呼び出し元で表示するのでここではログのみ
                LogError(ex);
                throw;
            }

            return dt;
        }

        /// <summary>
        /// 期間入力（yyyy/MM）を解析して開始日・終了日（当月の初日・末日）を返す。
        /// </summary>
        private bool TryParsePeriod(string startText, string endText, out DateTime startDate, out DateTime endDate)
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            if (!DateTime.TryParseExact(startText, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime s))
            {
                return false;
            }
            if (!DateTime.TryParseExact(endText, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime e))
            {
                return false;
            }

            // 開始日は当月の1日、終了日は当月の最終日
            startDate = new DateTime(s.Year, s.Month, 1);
            endDate = new DateTime(e.Year, e.Month, DateTime.DaysInMonth(e.Year, e.Month), 23, 59, 59);
            return true;
        }

        /// <summary>
        /// チェックされている分類・ステータス名を返す（UI のラベルテキストをそのまま使用）
        /// </summary>
        private List<string> GetSelectedCategories()
        {
            var list = new List<string>();
            if (chkCategoryA.Checked) list.Add(chkCategoryA.Text);
            if (chkCategoryB.Checked) list.Add(chkCategoryB.Text);
            if (chkCategoryC.Checked) list.Add(chkCategoryC.Text);
            if (chkCategoryD.Checked) list.Add(chkCategoryD.Text);
            if (checkBox1.Checked) list.Add(checkBox1.Text);
            return list;
        }

        /// <summary>
        /// DataGridView の表示調整：横スクロールを出さないように列幅調整、最大表示行数を 10 行に揃える
        /// </summary>
        private void AdjustGridDisplay()
        {
            // 横スクロールを避けるために列幅を画面幅に合わせる
            _salesRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 表示行数を最大10行にするために DataGridView の高さを調整
            int visibleRows = 10;
            int rowHeight = _salesRecord.RowTemplate.Height;
            int headerHeight = _salesRecord.ColumnHeadersHeight;
            int border = 4; // マージン
            _salesRecord.Height = headerHeight + rowHeight * visibleRows + border;

            // 縦スクロールが出るようにする（データが多い場合にスクロールで続きが見られる）
            _salesRecord.ScrollBars = ScrollBars.Vertical;
        }

        /// <summary>
        /// 他画面から呼ばれる可能性のあるデータ取得メソッド（SalesData から参照される想定）。
        /// 実行は QueryTransmissionLogs をラップして List&lt;SalesData.SalesRecord&gt; を返す。
        /// </summary>
        public List<SalesData.SalesRecord> GetSalesData(DateTime startDate, DateTime endDate, List<string> selectedCategories, string productId, string productName)
        {
            var result = new List<SalesData.SalesRecord>();

            try
            {
                var table = QueryTransmissionLogs(startDate, endDate, selectedCategories, productId, productName);

                foreach (DataRow r in table.Rows)
                {
                    var rec = new SalesData.SalesRecord
                    {
                        SalesDate = r.Field<DateTime>("処理日時"),
                        CategoryName = r.Field<string>("分類") + (string.IsNullOrEmpty(r.Field<string>("ステータス")) ? "" : " / " + r.Field<string>("ステータス")),
                        ProductId = r.Field<string>("商品番号"),
                        ProductName = r.Field<string>("商品名"),
                        Quantity = r.Field<int>("件数"),
                        DiscountAmount = 0m, // 送受信ログには割引額が無い想定のため 0 を設定
                        SalesAmount = 0m     // 送受信ログには金額が無い想定のため 0 を設定
                    };
                    result.Add(rec);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // 呼び出し側で例外/エラーを扱うため、空リストを返す選択肢もあるがここでは空リストを返す
            }

            return result;
        }

        /// <summary>
        /// エラーログ記録（簡易実装）
        /// </summary>
        private void LogError(Exception ex)
        {
            try
            {
                string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Management", "Logs");
                Directory.CreateDirectory(logDir);
                string logFile = Path.Combine(logDir, $"error_{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.Message}\r\n{ex.StackTrace}\r\n\r\n");
            }
            catch
            {
                // ログ書き込み失敗しても処理を妨げない
            }

            Debug.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}