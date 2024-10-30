using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace De2Form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        public void LoadDataGridView()
        {
            string link = "http://localhost:90/hoand2/api/thoitiettrongngay";
            HttpWebRequest request = WebRequest.CreateHttp(link);
            WebResponse response = request.GetResponse();
            string jsonResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = reader.ReadToEnd();
            }

            jsonResponse = ConvertToRequiredDateFormat(jsonResponse);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ThoiTietTrongNgay[]));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonResponse)))
            {
                ThoiTietTrongNgay[] arr = (ThoiTietTrongNgay[])js.ReadObject(ms);
                dataGridView1.DataSource = arr;
            }
        }
        private string ConvertToRequiredDateFormat(string jsonResponse)
        {
            return Regex.Replace(jsonResponse, @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}", match =>
            {
                DateTime dt = DateTime.Parse(match.Value);
                long milliseconds = (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
                return $"\\/Date({milliseconds})\\/";
            });
        }
        private void btnHienThi_Click(object sender, EventArgs e)
        {
            LoadDataGridView();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string post = string.Format("?gio={0}&maKV={1}&nhietdo={2}&doam={3}", txtGio.Text,
                txtMaKV.Text, txtNhietDo.Text, txtDoAm.Text);
            string link = " http://localhost:90/hoand2/api/thoitiettrongngay" + post;
            HttpWebRequest request = HttpWebRequest.CreateHttp(link);
            
            request.Method = "POST";
            Stream dataStream = request.GetRequestStream();
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(bool));
            object data = js.ReadObject(request.GetResponse().GetResponseStream());
            bool kq = (bool)data;
            if (kq)
            {
                LoadDataGridView();
                MessageBox.Show("Thêm thành công");
            }
            else
                MessageBox.Show("Thêm  thất bại");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string gio = txtGio.Text;
            DateTime gioDateTime = DateTime.Parse(gio);
            string maKV = txtMaKV.Text;
            string post = string.Format("?gio={0}&maKV={1}", gioDateTime, maKV);
            string link = " http://localhost:90/hoand2/api/thoitiettrongngay" + post;
            HttpWebRequest request = HttpWebRequest.CreateHttp(link);

            request.Method = "Delete";

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(bool));
            object data = js.ReadObject(request.GetResponse().GetResponseStream());
            bool kq = (bool)data;
            if (kq)
            {
                LoadDataGridView();
                MessageBox.Show("Xóa thành công");
            }
            else
                MessageBox.Show("Xóa  thất bại");


        }

        private void Cell_Click(object sender, DataGridViewCellEventArgs e)
        {
            int d = e.RowIndex;
            txtGio.Text = dataGridView1.Rows[d].Cells[0].Value.ToString();
            txtMaKV.Text = dataGridView1.Rows[d].Cells[1].Value.ToString();
            txtNhietDo.Text = dataGridView1.Rows[d].Cells[2].Value.ToString();
            txtDoAm.Text = dataGridView1.Rows[d].Cells[3].Value.ToString();
        }
    }
}
