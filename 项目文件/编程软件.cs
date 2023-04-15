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
using System.IO;
using System.Diagnostics;

namespace program
{
    public partial class Form1 : Form
    {
        string userid;
        public Form1(string s)
        {
            InitializeComponent();
            userid = s;
        }
        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            label2.Text = "当前时间为:" + DateTime.Now.ToString();
            DataSet mydataset = new DataSet();
            SqlDataAdapter myadapter = new SqlDataAdapter("select count(*) from saveexe where username='" + userid + "'", myconn);
            myadapter.Fill(mydataset, "count");
            int n = Convert.ToInt32(mydataset.Tables[0].Rows[0].ItemArray[0]);
            if (n == 0)
                label1.Visible = true;
            else
                label1.Visible = false;
            comboBox1.Items.Clear();
            myadapter = new SqlDataAdapter("select distinct category as 类别 from saveexe where username='" + userid + "'", myconn);
            myadapter.Fill(mydataset, "catergory");
            int clenth = mydataset.Tables[1].Rows.Count;
            for (int j = 0; j < clenth; j++)
            {
                comboBox1.Items.Add(mydataset.Tables[1].Rows[j].ItemArray[0].ToString());
            }
            //在listview1和imageList1中清空原有的内容
            listView1.Items.Clear();
            imageList1.Images.Clear();

            string mystr = "select rename as 名称,pic as 图标,times from saveexe where username='" + userid + "'" + " order by times DESC";
            myadapter = new SqlDataAdapter(mystr, myconn);
            myadapter.Fill(mydataset, "icon");
            int a = mydataset.Tables[2].Rows.Count;
            //提取其中图片
            MemoryStream myPic = null;
            byte[] mydata;
            //暂时未考虑排名，将所有数据库中记录全部显示出来
            for (int i = 0; i < a; i++)
            {
                mydata = (byte[])(mydataset.Tables[2].Rows[i].ItemArray[1]);
                string rename = mydataset.Tables[2].Rows[i].ItemArray[0].ToString();
                myPic = new MemoryStream(mydata);
                imageList1.Images.Add(Image.FromStream(myPic));
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = rename;
                listView1.Items.Add(listViewItem);
                listView1.Items[i].ImageIndex = i;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //浮雕文字
            Brush backBrush = Brushes.White;
            Brush foreBrush = Brushes.Black;
            Font font = new Font("宋体", Convert.ToInt16(40), FontStyle.Regular);
            Graphics g = this.CreateGraphics();
            string text = "欢迎来到编程软件！";
            SizeF size = g.MeasureString(text, font);
            Single posX = (this.Width - Convert.ToInt16(size.Width)) / 2;
            Single posY = (this.Height - Convert.ToInt16(size.Height)) / 20;
            g.DrawString(text, font, backBrush, posX + 1, posY + 1);
            g.DrawString(text, font, foreBrush, posX, posY);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2(userid);
            fm2.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            label2.Text = "当前时间为:" + DateTime.Now.ToString();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            int n = listView1.Items.Count;
            for (int i = 0; i < n; i = i + 1)
            {
                if (listView1.Items[i].Selected == true)
                {
                    //获取选中的网页名称
                    string rename = listView1.Items[i].Text;
                    //提取数据库中对应的网址
                    string mystr = "select spath,times from saveexe where rename='"
                    + rename + "' and username='" + userid + "'";
                    SqlDataAdapter myadapter = new SqlDataAdapter(mystr, myconn);
                    DataSet mydataset = new DataSet();
                    myadapter.Fill(mydataset, "open");
                    string spath = mydataset.Tables["open"].Rows[0].ItemArray[0].ToString();
                    int times = Convert.ToInt32(mydataset.Tables["open"].Rows[0].ItemArray[1]) + 1;
                    Process.Start(@spath);
                    string cmd = "update saveexe set times= "
                    + times
                    + "where rename='" + rename
                    + "'and username='" + userid + "'";
                    //MessageBox.Show(cmd);
                    SqlCommand mycmd = new SqlCommand(cmd, myconn);
                    myconn.Open();
                    {
                        try
                        {
                            mycmd.ExecuteNonQuery();
                            MessageBox.Show("打开中...");
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

        private void button2_Click(object sender, EventArgs e)
        {
            //在listview1和imageList1中清空原有的内容
            listView1.Items.Clear();
            imageList1.Images.Clear();
            string mystr = "select rename as 名称,pic as 图标 ,times from saveexe where rename like'%" + textBox1.Text.Trim() + "%' and username='" + userid + "'" + " order by times DESC";
            SqlDataAdapter myadapter = new SqlDataAdapter(mystr, myconn);
            DataSet mydataset = new DataSet();
            myadapter.Fill(mydataset, "search");
            int n = mydataset.Tables[0].Rows.Count;
            //提取其中图片
            MemoryStream myPic = null;
            byte[] mydata;
            for (int i = 0; i < n; i++)
            {
                mydata = (byte[])(mydataset.Tables[0].Rows[i].ItemArray[1]);
                string rename = mydataset.Tables[0].Rows[i].ItemArray[0].ToString();
                myPic = new MemoryStream(mydata);

                imageList1.Images.Add(Image.FromStream(myPic));
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = rename;
                listView1.Items.Add(listViewItem);
                listView1.Items[i].ImageIndex = i;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {   //在listview1和imageList1中清空原有的内容
            listView1.Items.Clear();
            imageList1.Images.Clear();

            string mystr = "select rename as 名称,pic as 图标, times from saveexe where category=" + "'" + comboBox1.Text.Trim() + "' and username='" + userid + "'" + " order by times DESC";
            SqlDataAdapter myadapter = new SqlDataAdapter(mystr, myconn);
            DataSet mydataset = new DataSet();
            myadapter.Fill(mydataset, "icon");
            int n = mydataset.Tables[0].Rows.Count;
            //提取其中图片
            MemoryStream myPic = null;
            byte[] mydata;

            for (int i = 0; i < n; i++)
            {
                mydata = (byte[])(mydataset.Tables[0].Rows[i].ItemArray[1]);
                string rename = mydataset.Tables[0].Rows[i].ItemArray[0].ToString();
                myPic = new MemoryStream(mydata);

                imageList1.Images.Add(Image.FromStream(myPic));
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = rename;
                listView1.Items.Add(listViewItem);
                listView1.Items[i].ImageIndex = i;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1_Load(this, null);
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void 删除软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = listView1.Items.Count;
            for (int i = 0; i < n; i = i + 1)
            {
                if (listView1.Items[i].Selected == true)
                {
                    string rename = listView1.Items[i].Text;
                    string cmd = "delete from saveexe where rename='"
                    + rename + "' and username='" + userid + "'";
                    SqlCommand mycmd = new SqlCommand(cmd, myconn);
                    myconn.Open();
                    {
                        try
                        {
                            mycmd.ExecuteNonQuery();
                            MessageBox.Show("删除成功！");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                    }
                    myconn.Close();
                }
            }
            listView1.Items.Clear();
            imageList1.Images.Clear();
            string mystr = "select rename as 名称,pic as 图标,times from saveexe where username='" + userid + "'" + " order by times DESC";
            SqlDataAdapter myadapter = new SqlDataAdapter();
            myadapter = new SqlDataAdapter(mystr, myconn);
            DataSet mydataset = new DataSet();
            myadapter.Fill(mydataset, "icon");
            int a = mydataset.Tables[0].Rows.Count;
            //提取其中图片
            MemoryStream myPic = null;
            byte[] mydata;
            //暂时未考虑排名，将所有数据库中记录全部显示出来
            for (int i = 0; i < a; i++)
            {
                mydata = (byte[])(mydataset.Tables[0].Rows[i].ItemArray[1]);
                string rename = mydataset.Tables[0].Rows[i].ItemArray[0].ToString();
                myPic = new MemoryStream(mydata);
                imageList1.Images.Add(Image.FromStream(myPic));
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = rename;
                listView1.Items.Add(listViewItem);
                listView1.Items[i].ImageIndex = i;
            }
        }

        private void 重命名软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label4.Visible = true;
            textBox2.Visible = true;
            textBox2.Text = "";
        }

        private void 查看软件路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = listView1.Items.Count;
            for (int i = 0; i < n; i = i + 1)
            {
                if (listView1.Items[i].Selected == true)
                {
                    //获取选中的网页名称
                    string rename = listView1.Items[i].Text;
                    string mystr = "select spath,times from saveexe where rename='"
                    + rename + "' and username='" + userid + "'";
                    SqlDataAdapter myadapter = new SqlDataAdapter(mystr, myconn);
                    DataSet mydataset = new DataSet();
                    myadapter.Fill(mydataset, "open");
                    string spath = mydataset.Tables["open"].Rows[0].ItemArray[0].ToString();
                    MessageBox.Show(spath);
                }

            }
        }

        private void 打开软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int n = listView1.Items.Count;
            for (int i = 0; i < n; i = i + 1)
            {
                if (listView1.Items[i].Selected == true)
                {

                    string rename = listView1.Items[i].Text;

                    string mystr = "select spath,times from saveexe where rename='"
                    + rename + "' and username='" + userid + "'";
                    SqlDataAdapter myadapter = new SqlDataAdapter(mystr, myconn);
                    DataSet mydataset = new DataSet();
                    myadapter.Fill(mydataset, "open");
                    string spath = mydataset.Tables["open"].Rows[0].ItemArray[0].ToString();
                    int times = Convert.ToInt32(mydataset.Tables["open"].Rows[0].ItemArray[1]) + 1;
                    Process.Start(@spath);
                    string cmd = "update saveexe set times= "
                    + times
                    + "where rename='" + rename
                    + "'and username='" + userid + "'";
                    //MessageBox.Show(cmd);
                    SqlCommand mycmd = new SqlCommand(cmd, myconn);
                    myconn.Open();
                    {
                        try
                        {
                            mycmd.ExecuteNonQuery();
                            MessageBox.Show("打开中...");
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

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                int n = listView1.Items.Count;
                for (int i = 0; i < n; i = i + 1)
                {
                    if (listView1.Items[i].Selected == true)
                    {
                        string rename = listView1.Items[i].Text;
                        string cmd = "update saveexe set rename='" + textBox2.Text.Trim() + "' where rename='" + rename
                    + "'and username='" + userid + "'"; ;
                        SqlCommand mycmd = new SqlCommand(cmd, myconn);
                        myconn.Open();
                        {
                            try
                            {
                                mycmd.ExecuteNonQuery();
                                MessageBox.Show("重命名成功！");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        myconn.Close();
                    }
                }
                label4.Visible = false;
                textBox2.Visible = false;
                listView1.Items.Clear();
                imageList1.Images.Clear();
                string mystr = "select rename as 名称,pic as 图标,times from saveexe where username='" + userid + "'" + " order by times DESC";
                SqlDataAdapter myadapter = new SqlDataAdapter();
                myadapter = new SqlDataAdapter(mystr, myconn);
                DataSet mydataset = new DataSet();
                myadapter.Fill(mydataset, "icon");
                int a = mydataset.Tables[0].Rows.Count;
                //提取其中图片
                MemoryStream myPic = null;
                byte[] mydata;
                for (int i = 0; i < a; i++)
                {
                    mydata = (byte[])(mydataset.Tables[0].Rows[i].ItemArray[1]);
                    string rename = mydataset.Tables[0].Rows[i].ItemArray[0].ToString();
                    myPic = new MemoryStream(mydata);
                    imageList1.Images.Add(Image.FromStream(myPic));
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Text = rename;
                    listView1.Items.Add(listViewItem);
                    listView1.Items[i].ImageIndex = i;
                }
            }
        }
    }
}
