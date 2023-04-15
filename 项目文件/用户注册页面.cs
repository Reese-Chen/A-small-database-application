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
    public partial class 用户注册页面 : Form
    {
        public 用户注册页面()
        {
            InitializeComponent();
        }

        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mysql = "insert into users values('"
                           + textBox1.Text.Trim() + "','"
                           + textBox2.Text.Trim() + "','"
                           + textBox3.Text.Trim() + "')";
            //MessageBox.Show(mysql);
            SqlCommand mycmd = new SqlCommand(mysql, myconn);
            myconn.Open();
            {
                try
                {
                    mycmd.ExecuteNonQuery();
                    MessageBox.Show("注册成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            myconn.Close();
        }
    }
}
