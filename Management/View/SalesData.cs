using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            public decimal DiscountAmount { get; set; } // 割引適用額
            public decimal SalesAmount { get; set; }    // 売上額
        }

        // データを扱うクラスやヘルパー（Transmission を使用）
        private Transmission _accessor = new Transmission();

        // 商品分類チェックボックスをグループ化するためのリスト
        private readonly List<CheckBox> _categoryCheckBoxes = new List<CheckBox>();

        public SalesData()
        {
            InitializeComponent();

            // DataGridView の基本設定（デザイナ配置の _salesRecord を想定）
            _salesRecord.AllowUserToAddRows = false;
            _salesRecord.AllowUserToDeleteRows = false;
            _salesRecord.ReadOnly = true;
            _salesRecord.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 画面幅に追従するようアンカーをセット（ボタンと重ならない高さ計算を行う）
            _salesRecord.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _salesRecord.ScrollBars = ScrollBars.Vertical;

            // 商品分類チェックボックスをリストに追加（Designer 上の名前に合わせる）
            _categoryCheckBoxes.Add(chkCategoryA);
            _categoryCheckBoxes.Add(chkCategoryB);
            _categoryCheckBoxes.Add(chkCategoryC);
            _categoryCheckBoxes.Add(chkCategoryD);

            // 商品番号は半角数字のみ許可するイベント
            txtProductId.KeyPress += TxtProductId_KeyPress;

            // イベント接続（デザイナで未接続の場合に備える）
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnSendSalesData.Click += btnSendSalesData_Click;
            btnRefresh.Click += btnRefresh_Click;
            button2.Click += btnClose_Click;

            Load += SalesData_Load;
        }

        private void SalesData_Load(object sender, EventArgs e)
        {
            // 期間の初期設定 (現在年月)
            var current = DateTime.Now;
            txtStartDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);
            txtEndDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);

            // 商品分類を全チェックに
            CheckAllCategories();

            // ボタン初期状態
            btnSendSalesData.Enabled = false;
            btnRefresh.Enabled = false;

            // DataGridView 列をプログラム的に作成して DataPropertyName を設定（ヘッダ名固定）
            CreateGridColumnsIfNeeded();

            // 初期は枠内表示用の空行を表示（最大15行）
            ClearGridToEmptyRows();

            // グリッド表示調整（幅・高さ・スクロール）
            AdjustGridDisplay();
        }

        // 検索ボタン押下
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 入力値取得
            string productId = txtProductId.Text.Trim();
            string productName = txtProductName.Text.Trim();
            var selectedCategories = GetSelectedCategories();

            // 入力チェックおよび期間変換
            if (!ValidateInput(productId, productName, selectedCategories, out DateTime startDate, out DateTime endDate))
            {
                return;
            }

            try
            {
                // データ取得（Transmission のラッパーを利用）
                List<SalesRecord> salesList = _accessor.GetSalesData(startDate, endDate, selectedCategories, productId, productName) ?? new List<SalesRecord>();

                // DataSource にバインド（列は DataPropertyName でマップ済み）
                _salesRecord.DataSource = null;
                _salesRecord.Rows.Clear();
                _salesRecord.AutoGenerateColumns = false;
                _salesRecord.DataSource = new BindingList<SalesRecord>(salesList);

                AdjustGridDisplay();

                // ボタン活性化
                btnSendSalesData.Enabled = true;
                btnRefresh.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("売上データの検索中に予期せぬエラーが発生しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError(ex);
            }
        }

        // 条件クリア
        private void btnClear_Click(object sender, EventArgs e)
        {
            var current = DateTime.Now;
            txtStartDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);
            txtEndDate.Text = current.ToString("yyyy/MM", CultureInfo.InvariantCulture);

            CheckAllCategories();

            txtProductId.Clear();
            txtProductName.Clear();

            _salesRecord.DataSource = null;
            _salesRecord.Rows.Clear();
            ClearGridToEmptyRows();

            btnSendSalesData.Enabled = false;
            btnRefresh.Enabled = false;
        }

        // 売上データ送信（バッチ起動）
        private void btnSendSalesData_Click(object sender, EventArgs e)
        {
            string confirmMsg = "選択されている期間の売上データを、期間以外の検索条件は反映せず、月別に全店舗管理サーバへ送信します。よろしいですか？";
            DialogResult result = MessageBox.Show(confirmMsg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            try
            {
                // 期間は UI の入力をそのまま渡す
                string startDate = txtStartDate.Text.Trim();
                string endDate = txtEndDate.Text.Trim();

                int returnCode = StartBatchProcess(startDate, endDate);

                if (returnCode == 0)
                {
                    MessageBox.Show("売上データの送信が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (returnCode == -2)
                {
                    MessageBox.Show("売上データ送信バッチがタイムアウトしました。ログを確認してください。", "タイムアウト", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"売上データの送信中にエラーが発生しました。\nリターンコード: {returnCode}", "異常終了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("売上データ送信バッチの起動中に予期せぬエラーが発生しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError(ex);
            }
        }

        // 更新ボタン：現在の条件で再検索
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            btnSearch_Click(sender, e);
        }

        // 閉じる
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

        // 入力チェック（期間変換含む）
        private bool ValidateInput(string productId, string productName, List<string> categories, out DateTime startDate, out DateTime endDate)
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            if (!DateTime.TryParseExact(txtStartDate.Text.Trim(), "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime s) ||
                !DateTime.TryParseExact(txtEndDate.Text.Trim(), "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime e))
            {
                MessageBox.Show("期間は 'yyyy/MM' 形式で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            startDate = new DateTime(s.Year, s.Month, 1);
            endDate = new DateTime(e.Year, e.Month, DateTime.DaysInMonth(e.Year, e.Month), 23, 59, 59);

            if (startDate > endDate)
            {
                MessageBox.Show("開始月は終了月以下で指定してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (categories == null || categories.Count == 0)
            {
                MessageBox.Show("商品分類を一つ以上選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrEmpty(productId))
            {
                if (productId.Length > 5 || !System.Text.RegularExpressions.Regex.IsMatch(productId, @"^\d+$"))
                {
                    MessageBox.Show("商品番号は半角数値で5桁以下で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(productName) && productName.Length > 30)
            {
                MessageBox.Show("商品名は30文字以下で入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private List<string> GetSelectedCategories()
        {
            var list = new List<string>();
            foreach (var chk in _categoryCheckBoxes)
            {
                if (chk.Checked) list.Add(chk.Text);
            }
            return list;
        }

        private void CheckAllCategories()
        {
            foreach (var chk in _categoryCheckBoxes)
            {
                chk.Checked = true;
            }
        }

        private void AdjustGridDisplay()
        {
            // 列幅をフォーム幅に合わせる（横スクロールを出さない）
            _salesRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // DataGridView の表示可能領域を、下部ボタン群と重ならないように計算して高さを決定する。
            // ボタンのトップ位置のうち最小値を取得（最も上にあるボタン）してそこまでを利用領域とする。
            int gridTop = _salesRecord.Top;
            int buttonsTop = int.MaxValue;

            if (btnSendSalesData != null) buttonsTop = Math.Min(buttonsTop, btnSendSalesData.Top);
            if (btnRefresh != null) buttonsTop = Math.Min(buttonsTop, btnRefresh.Top);
            if (button2 != null) buttonsTop = Math.Min(buttonsTop, button2.Top);

            // マージンを少し入れて重ならないようにする
            int margin = 8;
            int availableHeight = (buttonsTop == int.MaxValue) ? _salesRecord.Height : Math.Max(0, buttonsTop - gridTop - margin);

            int rowHeight = _salesRecord.RowTemplate.Height;
            int headerHeight = _salesRecord.ColumnHeadersHeight;

            // 最低でも 3 行は表示するようにフォールバック
            int minRows = 3;

            int visibleRows;
            if (availableHeight <= headerHeight + rowHeight * minRows)
            {
                visibleRows = minRows;
            }
            else
            {
                visibleRows = Math.Max(minRows, (availableHeight - headerHeight) / rowHeight);
            }

            // 高さを設定（ヘッダ + 行数 * 行高 + 微小マージン）
            _salesRecord.Height = headerHeight + visibleRows * rowHeight + 4;

            // データがはみ出す場合は縦スクロールで見られるようにする
            _salesRecord.ScrollBars = ScrollBars.Vertical;
        }

        // 外部バッチを起動して終了コードを返す（実装）
        // 戻り値:
        //  - 正常終了: バッチの ExitCode をそのまま返す
        //  - 起動に失敗/前処理エラー: -1
        //  - タイムアウトで強制終了: -2
        private int StartBatchProcess(string startDate, string endDate)
        {
            try
            {
                // UI の yyyy/MM 文字列をバッチが期待する yyyyMM に変換する
                if (!DateTime.TryParseExact(startDate, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime s) ||
                    !DateTime.TryParseExact(endDate, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime e))
                {
                    LogError(new ArgumentException("Start/End の期間書式が不正です。yyyy/MM 形式を指定してください。"));
                    return -1;
                }

                string startArg = s.ToString("yyyyMM", CultureInfo.InvariantCulture);
                string endArg = e.ToString("yyyyMM", CultureInfo.InvariantCulture);

                // バッチ実行ファイルの探索（柔軟にいくつかの候補を探す）
                string exePath = FindBatchExecutable();
                if (string.IsNullOrEmpty(exePath))
                {
                    LogError(new FileNotFoundException("バッチ実行ファイルが見つかりません。Batch.exe / BatchSender.exe / Transmission.bat 等を検索しました。"));
                    return -1;
                }

                bool isBatchScript = exePath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || exePath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase);

                // バッチは第1引数が処理区分（"1" = 送信）、続いて yyyyMM 形式の開始/終了
                string arguments = isBatchScript
                    ? $"1 {startArg} {endArg}"
                    : $"1 \"{startArg}\" \"{endArg}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(exePath) ?? Application.StartupPath
                };

                var outputBuilder = new StringBuilder();

                using (var proc = new Process { StartInfo = psi, EnableRaisingEvents = true })
                {
                    proc.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            outputBuilder.AppendLine(e.Data);
                            Debug.WriteLine(e.Data);
                        }
                    };
                    proc.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            outputBuilder.AppendLine(e.Data);
                            Debug.WriteLine(e.Data);
                        }
                    };

                    if (!proc.Start())
                    {
                        LogError(new InvalidOperationException("バッチプロセスの起動に失敗しました。"));
                        return -1;
                    }

                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    // タイムアウト（ミリ秒）：5 分をデフォルトに設定。必要なら調整可能。
                    const int timeoutMs = 5 * 60 * 1000;

                    bool exited = proc.WaitForExit(timeoutMs);
                    if (!exited)
                    {
                        try
                        {
                            proc.Kill(entireProcessTree: true);
                        }
                        catch (Exception killEx)
                        {
                            LogError(killEx);
                        }

                        LogError(new TimeoutException($"バッチプロセスがタイムアウトしました（{timeoutMs / 1000}秒）。"));
                        // 出力をログに残す
                        WriteBatchLog(outputBuilder.ToString());
                        return -2;
                    }

                    // プロセスが終了した後、リダイレクトの読み取り完了を待つ
                    proc.WaitForExit();

                    // 出力ログを保存
                    WriteBatchLog(outputBuilder.ToString());

                    return proc.ExitCode;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return -1;
            }
        }

        // バッチ実行ファイルを探索してパスを返す。見つからなければ null を返す。
        private string? FindBatchExecutable()
        {
            try
            {
                var candidates = new[]
                {
                    "Batch.exe",
                    "BatchSender.exe",
                    "BatchSender.bat",
                    "Batch.bat",
                    "Transmission.bat"
                };

                var searchFolders = new List<string>
                {
                    Application.StartupPath,
                    AppDomain.CurrentDomain.BaseDirectory,
                    Environment.CurrentDirectory,
                    Path.Combine(Application.StartupPath, "Batch", "bin", "Debug", "net8.0"),
                    Path.Combine(Application.StartupPath, "Batch", "bin", "Release", "net8.0"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Batch", "bin", "Debug", "net8.0"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Batch", "bin", "Release", "net8.0")
                };

                var tried = new StringBuilder();

                foreach (var folder in searchFolders.Where(f => !string.IsNullOrEmpty(f)).Select(f => Path.GetFullPath(f)).Distinct())
                {
                    foreach (var cand in candidates)
                    {
                        try
                        {
                            var full = Path.Combine(folder, cand);
                            tried.AppendLine(full);
                            if (File.Exists(full))
                            {
                                // 見つかったパスをログにも残す
                                Debug.WriteLine($"FindBatchExecutable: found {full}");
                                return full;
                            }
                        }
                        catch
                        {
                            // 無視して次へ
                        }
                    }
                }

                // 見つからなかった場合は試したパス一覧を含めてログ出力
                LogError(new FileNotFoundException("バッチ実行ファイルが見つかりません。試したパス:\r\n" + tried.ToString()));
                return null;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        // バッチ出力をログファイルに書く（失敗しても例外を投げない）
        private void WriteBatchLog(string content)
        {
            try
            {
                string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Management", "BatchLogs");
                Directory.CreateDirectory(logDir);
                string file = Path.Combine(logDir, $"batch_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                File.WriteAllText(file, content ?? string.Empty, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        // エラーログ記録（簡易）
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
                // ログ失敗は無視
            }

            Debug.WriteLine($"[ERROR] {DateTime.Now}: {ex.Message}\n{ex.StackTrace}");
        }

        // 商品番号入力は半角数字のみ許可
        private void TxtProductId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // DataGridView 列をプログラム的に作成（DataPropertyName を SalesRecord のプロパティに合わせる）
        private void CreateGridColumnsIfNeeded()
        {
            if (_salesRecord.Columns.Count > 0) return;

            _salesRecord.AutoGenerateColumns = false;
            _salesRecord.Columns.Clear();

            var colSalesDate = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.SalesDate),
                HeaderText = "販売日時",
                Name = "colSalesDate",
                DefaultCellStyle = { Format = "yyyy/MM/dd HH:mm:ss" },
                ReadOnly = true
            };
            _salesRecord.Columns.Add(colSalesDate);

            var colCategory = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.CategoryName),
                HeaderText = "分類",
                Name = "colCategory",
                ReadOnly = true
            };
            _salesRecord.Columns.Add(colCategory);

            var colProductId = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.ProductId),
                HeaderText = "商品番号",
                Name = "colProductId",
                ReadOnly = true
            };
            _salesRecord.Columns.Add(colProductId);

            var colProductName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.ProductName),
                HeaderText = "商品名",
                Name = "colProductName",
                ReadOnly = true
            };
            _salesRecord.Columns.Add(colProductName);

            var colQuantity = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.Quantity),
                HeaderText = "売上数量",
                Name = "colQuantity",
                ReadOnly = true
            };
            _salesRecord.Columns.Add(colQuantity);

            var colDiscount = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.DiscountAmount),
                HeaderText = "割引適用額",
                Name = "colDiscount",
                ReadOnly = true,
                DefaultCellStyle = { Format = "N2" }
            };
            _salesRecord.Columns.Add(colDiscount);

            var colSalesAmount = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SalesRecord.SalesAmount),
                HeaderText = "売上額",
                Name = "colSalesAmount",
                ReadOnly = true,
                DefaultCellStyle = { Format = "N2" }
            };
            _salesRecord.Columns.Add(colSalesAmount);
        }

        // DataGridView を空行で埋める（初期表示用）
        private void ClearGridToEmptyRows()
        {
            // Columns が存在することを前提
            if (_salesRecord.Columns.Count == 0) CreateGridColumnsIfNeeded();

            _salesRecord.Rows.Clear();
            for (int i = 0; i < 15; i++)
            {
                int idx = _salesRecord.Rows.Add();
                // 空行なのでセルは空のまま
            }

            // 縦スクロールを有効にする
            _salesRecord.ScrollBars = ScrollBars.Vertical;
        }
    }
}