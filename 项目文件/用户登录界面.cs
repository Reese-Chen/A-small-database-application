using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace 文献查找
{
    public partial class 用户登录界面 : Form
    {
        public static 用户登录界面 f0 = null;
        public 用户登录界面()
        {
            InitializeComponent();
            f0 = this;
            
        }

        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");
        

        private void button1_Click(object sender, EventArgs e)
        {
            string username_read = textBox1.Text.ToString();
            string usercode_read = textBox2.Text.ToString();
            string mysql = "select * from users where username='"
                +username_read+"'and code = '"
                +usercode_read+"'";
            //MessageBox.Show(mysql);
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            DataSet mydataset = new DataSet();
            myadapter.Fill(mydataset, "user1");
            if (mydataset.Tables["user1"].Rows.Count==0)
            {
                MessageBox.Show("用户或密码错误！");
                textBox1.Clear();
                textBox2.Clear();
            }
            else
            {
                //MessageBox.Show("登录成功");
                //登入进入集成页面
                集成页面 f1 = new 集成页面(textBox1.Text);
                this.Hide();
                f1.ShowDialog();
            }
            //集成页面 f0 = new 集成页面(textBox1.Text);
            //f0.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            用户注册页面 f1 = new 用户注册页面();
            f1.ShowDialog();
        }
    }
}
