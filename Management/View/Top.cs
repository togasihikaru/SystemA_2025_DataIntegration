using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace Management
{
    public partial class Top : Form
    {
        public Top()
        {
            InitializeComponent();
        }

        private void 売り上げ管理_Click(object sender, EventArgs e)
        {
            // 1. 遷移先のSalesDataのインスタンスを作成
            SalesData newForm = new SalesData();

            // Owner を設定しておく（閉じる時に親を再表示するため）
            newForm.Owner = this;

            // 2. SalesDataを表示
            newForm.Show();

            // 3. 現在のTopを非表示
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 1. 遷移先のTransmissionのインスタンスを作成
            Transmission newForm = new Transmission();

            // Owner を設定しておく（閉じる時に親を再表示するため）
            newForm.Owner = this;

            // 2. Transmissionを表示
            newForm.Show();

            // 3. 現在のTopを非表示
            this.Hide();
        }
    }
}
