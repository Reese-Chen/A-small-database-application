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
    public partial class 新词汇插入 : Form
    {
        public 新词汇插入()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");

        private void button1_Click(object sender, EventArgs e)
        {
            string en = textBox1.Text.Trim();
            //获得英文内容
            if (en == "") { MessageBox.Show("英文不能为空"); return; }
            //若英文为空，则提示错误信息，不执行后续片段
            string ch = textBox2.Text.Trim();
            string types = comboBox1.Text.Trim();
            string sql = "insert into enword values ('" + en + "','" + ch + "')";
            string sql0 = "insert into wordtypes values ('" +en + "','"+types+"')";
            try
            {
                SqlCommand mycmd = new SqlCommand(sql, myconn);
                myconn.Open();
                mycmd.ExecuteNonQuery();
                myconn.Close();
                //添加至词汇储存表
                mycmd = new SqlCommand(sql0, myconn);
                myconn.Open();
                mycmd.ExecuteNonQuery();
                myconn.Close();
                //添加至词汇类型表
                MessageBox.Show("插入成功!");
            }
            catch(Exception ex)
            {
                //若单词已存在表中，则插入失败，提示错误信息
                
                MessageBox.Show(ex.ToString());
                myconn.Close();
            }
       
        }

        private void wordinsert_Load(object sender, EventArgs e)
        {
            DataSet mydataset = new DataSet();
            string mysql = "select entypes from wordtypes group by entypes";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            myadapter.Fill(mydataset, "types");
            foreach (DataRow dr in mydataset.Tables[0].Rows)
            {
                comboBox1.Items.Add( dr[0].ToString());
            }
            //将当前所有的词汇类型输入至combobox框中
        }
    }
}
