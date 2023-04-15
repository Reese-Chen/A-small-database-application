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
using System.Drawing.Imaging;
using System.IO;

namespace 文献查找
{
    public partial class Form2 : Form
    {
        string userid;
        public Form2(string s)
        {
            InitializeComponent();
            userid = s;
        }


        //建立数据库连接
        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");

        private void button2_Click(object sender, EventArgs e)
        {
            //若网站名称为空，则不执行
            if (textBox2.Text.ToString() == "")
            {
                MessageBox.Show("请输入有效字段!");
            }

            //向literature_website表中插入新记录，定义sql语句
            string mysql = "insert into literature_website values('"
                           + textBox1.Text.Trim() + "','"
                           + userid+"','"
                           + textBox2.Text.Trim() + "','"
                           + textBox4.Text.Trim() + "','"
                           + textBox3.Text.Trim() + "',"
                           + "@blobdata,"
                           + 0 +")";
            //MessageBox.Show(mysql);
            SqlCommand mycmd = new SqlCommand(mysql, myconn);

            string picturePath = textBox5.Text.Trim();
            //如果用户没有提供图片则使用默认图片
            if (picturePath == "")
            {
                picturePath= "D:\\数据库基础\\icon\\none.png";
            }
            //创建FileStream对象
            FileStream fs = new FileStream(picturePath, FileMode.Open, FileAccess.Read);
            //声明Byte数组
            Byte[] mybyte = new byte[fs.Length];
            //读取数据
            fs.Read(mybyte, 0, mybyte.Length);
            fs.Close();
            //转换成二进制数据
            SqlParameter prm = new SqlParameter
            ("@blobdata", SqlDbType.VarBinary, mybyte.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte);
            mycmd.Parameters.Add(prm);
            //创建连接插入数据库
            myconn.Open();
            {
                try
                {                mycmd.ExecuteNonQuery();
                    MessageBox.Show("插入成功");
    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            myconn.Close(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
