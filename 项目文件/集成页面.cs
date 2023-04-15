using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace 文献查找
{
    public partial class 集成页面 : Form
    {
        string userid;
        public 集成页面(string s)
        {

            InitializeComponent();
            userid = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            literature lr = new literature(userid);
            lr.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            translation_tools.textedit f = new translation_tools.textedit(userid);
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            program.Form1 f = new program.Form1(userid);
            f.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("想要加入我们，请联系021-12345678。");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            用户登录界面.f0.Show();
            this.Close();
        }
    }
}
