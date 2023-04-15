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
using 文献查找;

namespace translation_tools
{
    public partial class wordlr : Form
    {
        string userid;
        public wordlr(string s)
        {
            InitializeComponent();
            userid = s;

        }

        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");
        DataSet mydataset = new DataSet();
        BindingSource mybind = new BindingSource();
        string types;
        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            string mysql = "select en,ch from enword";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            myadapter.Fill(mydataset, "word");
            dataGridView1.DataSource = mydataset.Tables[0];
            //窗体首次加载时将所有单词默认展示在datagridview1中
            mybind.DataSource = mydataset;
            mybind.DataMember = "word";
            label1.DataBindings.Add(new Binding("Text", mybind, "en", true));
            label2.DataBindings.Add(new Binding("text", mybind, "ch", true));
            //将单词绑定至label控件中展示
            mysql = "select entypes from wordtypes group by entypes";
            myadapter = new SqlDataAdapter(mysql, myconn);
            myadapter.Fill(mydataset, "types");
            foreach (DataRow dr in mydataset.Tables[1].Rows)
            {
                types += dr[0].ToString();
                comboBox1.Items.Add(dr[0].ToString());
            }
            //获得词汇表中所有的词汇类型数据，保存在types变量中
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mybind.MoveNext();
            //切换至下一个
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mybind.MovePrevious();
            //切换至上一个
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            mydataset.Clear();
            label1.DataBindings.Clear();
            label2.DataBindings.Clear();
            //清除当前绑定关系
            string type = comboBox1.Text;
            //获得选择的单词类型
            if (types.IndexOf(type) == -1)
            {
                //单词类型表中未设定复习类型
                //若用户选定复习类型，则无法在types变量中检索到，从而判断选定复习类型
                string mysql = "select wordrecord.en,ch from enword,wordrecord where wordrecord.en=enword.en and wordrecord.entimes>0 and username='"+userid+"'order by wordrecord.entimes DESC ";
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                myadapter.Fill(mydataset, "word");
                dataGridView1.DataSource = mydataset.Tables[0];
                mybind.DataSource = mydataset;
                mybind.DataMember = "word";
                label1.DataBindings.Add(new Binding("Text", mybind, "en", true));
                label2.DataBindings.Add(new Binding("text", mybind, "ch", true));
                //加载单词学习记录表数据
            }
            else
            {
                
                string mysql = "select enword.en,enword.ch from enword,wordtypes where wordtypes.en=enword.en and wordtypes.entypes='"+type+"'";
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                myadapter.Fill(mydataset, "word");
                dataGridView1.DataSource = mydataset.Tables[0];
                mybind.DataSource = mydataset;
                mybind.DataMember = "word";
                label1.DataBindings.Add(new Binding("Text", mybind, "en", true));
                label2.DataBindings.Add(new Binding("text", mybind, "ch", true));
                //根据用户选定的类型获取指定单词数据并完成加载
            }
            Random r = new Random();
            int j = r.Next(1, 4500);
            for (int i = 0; i < j; i++)
            {
                mybind.MoveNext();
            }
            //生成随机数，使得用户从随机位置开始学习
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //用户单击不认识按钮
            string sql = "insert into wordrecord values ('" + label1.Text + "',1,'"+userid+"')";
            string sql0 = "update wordrecord set entimes=entimes+1 where en='" + label1.Text + "'and username='"+userid+"'";
            try
            {
                SqlCommand mycmd = new SqlCommand(sql, myconn);
                myconn.Open();
                mycmd.ExecuteNonQuery();
                myconn.Close();
                //将当前单词插入单词记录表
            }
            catch
            {
                //如该单词已经存在于表中，则进入该部分代码
                //将该单词对应的频次加1
                myconn.Close();
                SqlCommand mycmd = new SqlCommand(sql0, myconn);
                myconn.Open();
                mycmd.ExecuteNonQuery();
                myconn.Close();
            }
            mybind.MoveNext();
            //完成数据更新后切换至下一个单词
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //用户点击认识按钮
            string sql = "update wordrecord set entimes=Floor(entimes/2) where en='" + label1.Text + "'and username='" + userid + "'";
           
            try
            {
                //如该单词已存在单词记录表中，则频次减半
                SqlCommand mycmd = new SqlCommand(sql, myconn);
                myconn.Open();
                mycmd.ExecuteNonQuery();
                myconn.Close();
            }
            catch
            {
                //如该单词不存在记录表，无额外操作
                myconn.Close();
            }
            mybind.MoveNext();
            //完成上述过程，切换至下一个单词

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            新词汇插入 f1 = new 新词汇插入();
            f1.Show();
        }
    }
}
