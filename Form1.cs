using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sinemaProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = "Server=.\\sqlexpress;Database=sinema;Trusted_Connection=true";
            cn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select * from salonlar";
            cmd.Connection = cn;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable salonlar = new DataTable();
            da.Fill(salonlar);

            for(int i = 0; i<salonlar.Rows.Count;i++)
            {
                int salonID = (int)salonlar.Rows[i]["salonID"];
                string salonAdi = (string)salonlar.Rows[i]["salonAdi"];

                treeView1.Nodes.Add(salonID.ToString(), salonAdi);

                SqlConnection cn2 = new SqlConnection();
                cn2.ConnectionString = "Server=.\\sqlexpress;Database=sinema;Trusted_Connection=true";
                cn2.Open();

                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandText = "select * from seanslar s INNER JOIN filmler f ON s.filmID = f.filmID where s.SalonID = @sid";
                cmd2.Parameters.AddWithValue("sid", salonID);
                cmd2.Connection = cn2;

                SqlDataReader dr = cmd2.ExecuteReader();
                while (dr.Read())
                {
                    treeView1.Nodes[i].Nodes.Add(dr["seansID"].ToString(), dr["saat"] + "(" + dr["filmAdi"] + ")");
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ArrayList satilmislar = new ArrayList();
            int seansID = Convert.ToInt32(treeView1.SelectedNode.Name);


            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = "Server=.\\sqlexpress;Database=sinema;Trusted_Connection=true";
            cn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select * from satislar where seansID = @sid";
            cmd.Parameters.AddWithValue("sid", seansID);
            cmd.Connection = cn;

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                satilmislar.Add(dr["koltukNo"].ToString());
            }
            //3.madde:
            panel1.Controls.Clear();
            int sayac = 1;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Button btn = new Button();
                    btn.Width = 40;
                    btn.Height = 40;
                    btn.Left = j * 42;
                    btn.Top = i * 42;
                    btn.Text = ((i * 10) + (j + 1)).ToString();
                    sayac++;
                    if(satilmislar.Contains(btn.Text))
                    {
                        btn.BackColor = Color.Tomato;
                    }
                    else
                    {
                        btn.BackColor = Color.Green;
                    }
                    panel1.Controls.Add(btn);
                }
            }
        }
    }
}
