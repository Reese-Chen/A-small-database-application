using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Data.SqlClient;

namespace translation_tools
{
    public partial class textedit : Form
    {
        string userid;
        public textedit(string s)
        {
            InitializeComponent();
            userid = s;
            
        }

        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");
        private void myopenfile(ToolStripMenuItem tol)
        {
            // myopenfile函数基于Microsoft.Office.Interop.Word，用于打开菜单栏下历史记录所对应的文件
            string path = tol.Text;
            //获取所展示出的文件路径
            try
            {
                if (path.Split('.')[1].ToString() == "txt")
                //判断是否为txt格式文件
                {
                    StreamReader sr = new
                             StreamReader(path, Encoding.UTF8);
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();

                }
                //将文件内容写入richtextbox控件
                else if (path.Split('.')[1].ToString() == "doc" || path.Split('.')[1].ToString() == "docx")
                {
                    //判断是否为word文件
                    Word.Application app = new Microsoft.Office.Interop.Word.Application();
                    Word.Document doc = null;
                    object unknow = Type.Missing;
                    object missing = System.Reflection.Missing.Value;
                    app.Visible = false;
                    object file = path;
                    doc = app.Documents.Open(ref file,
                        ref unknow, ref unknow, ref unknow, ref unknow,
                        ref unknow, ref unknow, ref unknow, ref unknow,
                        ref unknow, ref unknow, ref unknow, ref unknow,
                        ref unknow, ref unknow, ref unknow);
                    doc.ActiveWindow.Selection.WholeStory();
                    //全选word文档中的数据
                    doc.ActiveWindow.Selection.Copy();
                    //复制数据到剪切板
                    richTextBox1.Paste();
                    //richTextBox粘贴数据
                    doc.Close(ref unknow, ref unknow, ref unknow);
                    //关闭文件
                    app.Quit(ref unknow, ref unknow, ref unknow);
                    //关闭COM
                }
                else
                {
                    MessageBox.Show("该格式文件无法打开");
                }
            }
            catch { MessageBox.Show("无法找到该文件"); }
            //由于本地文件可能发生路径迁移，导致指定路径未能检索到相应文件，提示错误原因
        }

        //translate函数向指定网站发送post请求，完成指定内容的翻译并返回结果
        private static string Translate(string q, string f, string t)
        {
            if (q != "")
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string url = "https://openapi.youdao.com/api";
                string appKey = "75e932a3ad889c4c";
                string appSecret = "AtlyOHAvpx7xXHGnSVXh5wWvTLwpXaEu";
                string salt = DateTime.Now.Millisecond.ToString();
                string curtime = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                string signStr = appKey + Cal(q) + salt + curtime + appSecret; ;
                string sign = ComputeHash(signStr, new SHA256CryptoServiceProvider());
                dic.Add("q", System.Web.HttpUtility.UrlEncode(q));
                dic.Add("from", f);
                dic.Add("to", t);
                dic.Add("signType", "v3");
                dic.Add("appKey", appKey);
                dic.Add("salt", salt);
                dic.Add("sign", sign);
                dic.Add("curtime", curtime);
                //生成参数表单
                string result = "";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                string res = result.Substring(result.IndexOf("translation"), result.IndexOf("dict") - result.IndexOf("translation"));
                res = res.Substring(res.IndexOf("[") + 2, res.IndexOf("]") - res.IndexOf("[") - 3);
                //获得结果并进行格式处理
                return (res);
            }
            else
            {
                return ("");
            }

        }
        private static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }
        private static string Cal(string q)
        {
            if (q == null)
            {
                return q;
            }
            else if (q.Length <= 20)
            {
                return q;
            }
            else
            {
                return ((q.Substring(0, 10) + q.Length + q.Substring(q.Length - 10, 10)));
            }


        }
        private void button1_Click(object sender, EventArgs e)
        {
            string q = textBox1.Text;
            string t = Translate(q, "", "");
            textBox2.Text = t; 
            //获得控件内容，完成内容翻译并展示结果
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            //关闭翻译功能区
        }

        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //该函数用于打开用户指定路径内容
            //由于涉及数据库插入部分，未采用myopenfile函数
            string path;
            string filename;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                filename = openFileDialog1.SafeFileName;

                DateTime time = DateTime.Now;
                if (filename.Split('.')[1].ToString() == "txt")
                {
                    StreamReader sr = new
                             StreamReader(openFileDialog1.FileName, Encoding.UTF8);
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                    string mysql = "insert into history values('" + filename + "','" + path + "','" + filename.Split('.')[1].ToString() + "','" + userid + "','" + time + "')";
                    //生成相应sql语句，将文件名、路径、格式和使用时间插入到表中
                    SqlCommand mycmd = new SqlCommand(mysql, myconn);
                    myconn.Open();
                    mycmd.ExecuteNonQuery();
                    myconn.Close();

                }
                else if (filename.Split('.')[1].ToString() == "doc" || filename.Split('.')[1].ToString() == "docx")
                {
                    string mysql = "insert into history values('" + filename + "','" + path + "','" + filename.Split('.')[1].ToString() + "','" + userid+"','"+time + "')";
                    //生成相应sql语句，将文件名、路径、格式和使用时间插入到表中
                    SqlCommand mycmd = new SqlCommand(mysql, myconn);
                    myconn.Open();
                    mycmd.ExecuteNonQuery();
                    myconn.Close();
                    Word.Application app = new Microsoft.Office.Interop.Word.Application();
                    Word.Document doc = null;
                    object unknow = Type.Missing;
                    object missing = System.Reflection.Missing.Value;
                    app.Visible = false;
                    object file = path;
                    doc = app.Documents.Open(ref file,
                        ref unknow, ref unknow, ref unknow, ref unknow,
                        ref unknow, ref unknow, ref unknow, ref unknow,
                        ref unknow, ref unknow, ref unknow, ref unknow,
                        ref unknow, ref unknow, ref unknow);
                    doc.ActiveWindow.Selection.WholeStory();
                    doc.ActiveWindow.Selection.Copy();
                    richTextBox1.Paste();
                    doc.Close(ref unknow, ref unknow, ref unknow);
                    app.Quit(ref unknow, ref unknow, ref unknow);
                }
                else
                {
                    MessageBox.Show("该格式文件无法打开");
                    //由于该部分路径由openFileDialog获取，不存在指定路径检索不到文件的情况
                    //故错误原因提示部分与myopenfile函数不同
                }
            }
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox1.Text = richTextBox1.SelectedText;
            //将文本编辑框中选定的内容同步的输入到翻译框中，方便操作
        }


        private void 翻译ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            //点击翻译菜单项，显示翻译功能区
        }

        private void 词汇分析ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            wordlr f2 = new wordlr(userid);
            f2.Show();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }
        private void 最近使用的历史ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            string mysql = "select * from history where username = '"+userid+"' order by historytime desc";
            //将文件打开历史记录表中最近十项记录添加至扩展菜单中
            DataSet mydataset = new DataSet();
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            myadapter.Fill(mydataset, "haha");
            int count = mydataset.Tables["haha"].Rows.Count;
            if (count > 10)
            {
                toolStripMenuItem2.Text = mydataset.Tables[0].Rows[0].ItemArray[1].ToString();
                toolStripMenuItem3.Text = mydataset.Tables[0].Rows[1].ItemArray[1].ToString();
                toolStripMenuItem4.Text = mydataset.Tables[0].Rows[2].ItemArray[1].ToString();
                toolStripMenuItem5.Text = mydataset.Tables[0].Rows[3].ItemArray[1].ToString();
                toolStripMenuItem6.Text = mydataset.Tables[0].Rows[4].ItemArray[1].ToString();
                toolStripMenuItem7.Text = mydataset.Tables[0].Rows[5].ItemArray[1].ToString();
                toolStripMenuItem8.Text = mydataset.Tables[0].Rows[6].ItemArray[1].ToString();
                toolStripMenuItem9.Text = mydataset.Tables[0].Rows[7].ItemArray[1].ToString();
                toolStripMenuItem10.Text = mydataset.Tables[0].Rows[8].ItemArray[1].ToString();
                toolStripMenuItem11.Text = mydataset.Tables[0].Rows[9].ItemArray[1].ToString();
            }
            else
            {
                toolStripMenuItem2.Text = mydataset.Tables[0].Rows[0].ItemArray[1].ToString();
            }

        }
        private void 保存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //保存文本编辑结果
            SaveFileDialog save1 = new SaveFileDialog();
            save1.Title = "选择目录和输入文件名";
            save1.Filter = "文本文件(*.txt)|*.txt";

            if (save1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(save1.FileName);
                sw.Write(richTextBox1.Text);
                sw.Close();
            }
        }
        private void 结果保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //保存翻译结果
            SaveFileDialog save1 = new SaveFileDialog();
            save1.Title = "选择目录和输入文件名";
            save1.Filter = "文本文件(*.txt)|*.txt";
            if (save1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(save1.FileName);
                sw.Write(textBox2.Text);
                sw.Close();
            }
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem2);
        }
        
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem3);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem4);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem5);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem6);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem7);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem8);
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem9);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem10);
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            myopenfile(toolStripMenuItem11);
        }
    }
}
