using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAB7_HONGPHUC
{
    public partial class Form1 : Form
    {
        private DataTable foodTable;
        public Form1()
        {
            InitializeComponent();
        }
    
        private void LoadCategory()
        {
            string connectionString = "Data Source=HONGPHUC\\SQLEXPRESS;Initial Catalog=RestaurantManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ID, Name FROM Category";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            // Mở kết nối
            conn.Open();
            // Lấy DL từ csdl đưa vào datatable
            adapter.Fill(dt);
            // Đóng kết nối và giải phóng bộ nhớ
            conn.Close();
            conn.Dispose();
            // Đưa dữ liệu vào combo box
            cbbCategory.DataSource = dt;
            // Hiển thị tên nhóm sản phẩm
            cbbCategory.DisplayMember = "Name";
            //Nhưng khi lấy giá trị thì lấy ID của nhóm
            cbbCategory.ValueMember = "ID";

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.LoadCategory();
        }

        private void cbbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbCategory.SelectedIndex == -1) return;
            string connectionString = "Data Source=HONGPHUC\\SQLEXPRESS;Initial Catalog=RestaurantManagement;Integrated Security=True;";
            SqlConnection conn = new SqlConnection(connectionString);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Food WHERE FoodCategoryID = @categoryId";
            // Truyền tham số
            cmd.Parameters.Add("@categoryId", SqlDbType.Int);
            if(cbbCategory.SelectedValue is DataRowView)
            {
                DataRowView rowView = cbbCategory.SelectedValue as DataRowView;
                cmd.Parameters["@categoryId"].Value = rowView["ID"];
            }
            else
            {
                cmd.Parameters["@categoyrId"].Value = cbbCategory.SelectedValue;
            }
            // Tạo bộ điều phiếu dl
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            foodTable = new DataTable();
            // Mở kết nối
            conn.Open();
            //Lấy dl từ csdl đưa vào DataTable
            adapter.Fill(foodTable);
            //Đóng kết nối và giải phóng bộ nhớ
            conn.Close();
            conn.Dispose();
            // Đưa dl vào data gridview
            dgvFoodList.DataSource = foodTable;
            // Tính số lượng mẫu tin
            lblQuantity.Text = foodTable.Rows.Count.ToString();
            lblCatName.Text = cbbCategory.Text;
        }

        private void tsmCalculateQuantity_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=HONGPHUC\\SQLEXPRESS;Initial Catalog=RestaurantManagement;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT @numSaleFood = sum(Quantity)FROM BillDetails WHERE FoodID = @foodId";
            // Lấy thông tin sp được chọn
            if(dgvFoodList.SelectedRows.Count>0)
            {
                DataGridViewRow selectedRow = dgvFoodList.SelectedRows[0];
                DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
                // Truyền tham số
                cmd.Parameters.Add("@foodId", SqlDbType.Int);
                cmd.Parameters["@foodId"].Value = rowView["ID"];
                cmd.Parameters.Add("@numSaleFood", SqlDbType.Int);
                // Mở kết nối csdl
                conn.Open();
                // Thực thi truy vấn và lấy dữ liệu từ tham số
                cmd.ExecuteNonQuery();
                string result = cmd.Parameters["@numSaleFood"].Value.ToString();
                MessageBox.Show("Tổng số lượng món" + rowView["Name"] + "đã bán là:" + result + "" + rowView["Unit"]);
                // Đóng kết nối
                conn.Close();

            }
            cmd.Dispose();
            conn.Dispose();
        }
    }
    
}
