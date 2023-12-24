using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiLauncher
{
    public partial class Form1 : Form
    {

        private Migemo m_migemo;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Migemoオブジェクトを作る
            m_migemo = new Migemo("./Dict/migemo-dict");

            // 行またがり検索をするときはこれを設定する
            m_migemo.OperatorNewLine = @"\s*";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 反転表示のクリア
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = richTextBox1.BackColor;

            // 正規表現オブジェクトの生成
            Regex regex = m_migemo.GetRegex(textBox1.Text);
            Debug.WriteLine(regex.ToString());  //生成された正規表現をデバッグコンソールに出力

            // テキストの検索と反転表示
            MatchCollection matches = regex.Matches(richTextBox1.Text, 0);

            foreach (Match match in matches)
            {
                richTextBox1.Select(match.Index, 0);
                richTextBox1.SelectionLength = match.Length;
                richTextBox1.SelectionBackColor = Color.Yellow;
            }
            richTextBox1.Select(0, 0);
        }
    }
}
