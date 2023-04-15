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

namespace program
{
    public partial class Form2 : Form
    {
        string userid;
        public Form2(string s)
        {
            InitializeComponent();
            userid = s;
        }

        SqlConnection myconn = new SqlConnection(@"DataBase=translation;Data Source=LAPTOP-3A7A52AS\WZB;Integrated Security=true;");
        public string name;
        public string path;
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            //浮雕文字
            Brush backBrush = Brushes.White;
            Brush foreBrush = Brushes.Black;
            Font font = new Font("宋体", Convert.ToInt16(40), FontStyle.Regular);
            Graphics g = this.CreateGraphics();
            string text = "添加新软件";
            SizeF size = g.MeasureString(text, font);
            Single posX = (this.Width - Convert.ToInt16(size.Width)) / 2;
            Single posY = (this.Height - Convert.ToInt16(size.Height)) / 20;
            g.DrawString(text, font, backBrush, posX + 1, posY + 1);
            g.DrawString(text, font, foreBrush, posX, posY);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            textBox1.Text = "";
            textBox2.Text = "";
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                name = dialog.SafeFileName;
                path = dialog.FileName;
                this.textBox1.SelectedText = name;
                this.textBox2.Text = path;
                
                //imageList1.Images.Add(ico.ToBitmap());
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(@path);
            Image img = ico.ToBitmap();
            string savepath = Application.StartupPath;
            img.Save(@savepath + @"\" + textBox4.Text.Trim() + ".PNG");
            byte[] mydata;
            FileStream myPic = new FileStream(@savepath + @"\" + textBox4.Text.Trim() + ".PNG", FileMode.Open);
            mydata = new byte[myPic.Length];
            myPic.Read(mydata, 0, (int)(mydata.Length));
            myPic.Close();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = myconn;
            myconn.Open();
            cmd.CommandText = "insert into saveexe values('"+userid+"','"+textBox1.Text.Trim () +"','"+textBox2.Text .Trim ()+"','"
            + textBox3.Text.Trim() + "','" + textBox4.Text.Trim() + "'," + "@imgfile" + "," + 0 + ")";
            SqlParameter par = new SqlParameter("@imgfile", SqlDbType.Image);
            par.Value = mydata;
            cmd.Parameters.Add(par);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("成功添加！");
            }
            catch (Exception ex)
            {
                if (textBox3.Text == "" || textBox4.Text == "")
                    MessageBox.Show("类别与简称不得为空！");
                else
                    MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }
    }
}
