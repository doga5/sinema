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
        //0-SQL Mimarisi
        //1-treeView'i çalışır hale getirmek(29 Mayıs Cumartesiye kadar)

        //2-treeView_AfterSelect'i kullanarak
        //  SQL'e gidip satislar tablosundan tıkladığım seansın satışlarını
        //  bir ArrayList'e(satilmislar) tek tek eklememiz gerekiyor.
        //  İpucu: İlk iş tıkladığım seansın ID'sine ulaşmak
        //         treeView1.SelectedNode.Name


        //3-Butonları oluşturacağız panelin içinde(treeView_AfterSelect)
        //  Eğer butonun text'i satilmislar dizisinde varsa kırmızı
        //  yoksa yeşil çizecek
        //  İpucu: Her şey afterselect
        //  İpucu2: Her seferinde önce panel'i clear etmeli

        //4-btn_Click'i
        //  secilmisler ArrayList'imiz var 
        //  Eğer koltuk yeşil ise sarıya çevir ve diziye ekle
        //  Eğer koltuk sarı ise yeşile çevirecek ve diziden çıkaracak
        //  İpucu: b.BackColor == Color.Green

        //5-button1_Click
        //  Form2'ye secilmisler dizisini ve seansID'yi götüreceğiz

        //6-Form2_Load
        //  Gelen veriyi Form2'deki textboxlarda göstereceğiz
        //  secilmisler dizisindeki elemanları üstteki textBox'a
        //  Kaç koltuk seçildiyse * 30'u alttaki textbox'a fiyat olarak

        //7-satisiTamamla butonu
        //  Veritabanında INSERT işlemi yapılacarak satislar tablosuna 
        //  seansID ve secilmisler dizisindeki her eleman tek tek eklenecek
        //  İpucu: INSERT işlemi döngü içerisinde olacak ki her satırda
        //  1 koltuk satışı girebilin
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
