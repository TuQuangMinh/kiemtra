using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using kiemTra_TuQuangMinh_2280601971.Model;

namespace kiemTra_TuQuangMinh_2280601971
{
    public partial class frmSinhVien : Form
    {
        private StudentContextDB context;
        private SinhVien selectedSinhVien;
        public frmSinhVien()
        {
            InitializeComponent();
        }
        private void FillFalcutyComboBox(List<Lop> listFalcutys)
        {
            this.cboLopHoc.DataSource = listFalcutys;
            this.cboLopHoc.DisplayMember = "TenLop";
            this.cboLopHoc.ValueMember = "MaLop";
        }
        private void BindGrid(List<SinhVien> listStudents)
        {
            dtgSinhVien.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dtgSinhVien.Rows.Add();
                dtgSinhVien.Rows[index].Cells[0].Value = item.MaSV;
                dtgSinhVien.Rows[index].Cells[1].Value = item.HoTenSV;                
                dtgSinhVien.Rows[index].Cells[3].Value = item.NgaySinh;
                dtgSinhVien.Rows[index].Cells[2].Value = item.Lop.MaLop;
            }
        }
        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtNgaySinh.Value = DateTime.Now;
            cboLopHoc.SelectedIndex = -1;
           
        }

        private void LoadData()
        {
            try
            {
                context = new StudentContextDB();
                List<Lop> listFalcutys = context.Lops.ToList();
                List<SinhVien> listStudents = context.SinhViens.ToList();
                FillFalcutyComboBox(listFalcutys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtMaSV_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgSinhVien.Rows[e.RowIndex];
                string studentID = row.Cells[0].Value.ToString();

                selectedSinhVien = context.SinhViens.FirstOrDefault(s => s.MaSV == studentID);

                if (selectedSinhVien != null)
                {

                    txtMaSV.Text = selectedSinhVien.MaSV.ToString();
                    txtHoTen.Text = selectedSinhVien.HoTenSV;
                    dtNgaySinh.Text = selectedSinhVien.NgaySinh.ToString();
                    cboLopHoc.SelectedValue = selectedSinhVien.MaLop;
                   
                }
            }
        }

        private void dtgSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgSinhVien.Rows[e.RowIndex];
                string studentID = row.Cells[0].Value.ToString();

                selectedSinhVien = context.SinhViens.FirstOrDefault(s => s.MaSV == studentID);

                if (selectedSinhVien != null)
                {

                    txtMaSV.Text = selectedSinhVien.MaSV.ToString();
                    txtHoTen.Text = selectedSinhVien.HoTenSV;
                    dtNgaySinh.Text = selectedSinhVien.NgaySinh.ToString();
                    cboLopHoc.SelectedValue = selectedSinhVien.MaLop;
                    // Kích hoạt btnLuu sau khi chọn sinh viên
                    btnLuu.Enabled = true;
                }
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchValue = txtTim.Text.Trim(); // Lấy giá trị từ ô nhập liệu

            if (string.IsNullOrEmpty(searchValue)) // Kiểm tra nếu không có dữ liệu nhập
            {
                MessageBox.Show("Vui lòng nhập thông tin để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tìm kiếm dựa trên MaSV, HoTenSV hoặc TenLop
                var searchResults = context.SinhViens
                    .Where(s => s.MaSV.Contains(searchValue)
                             || s.HoTenSV.Contains(searchValue)
                             || s.Lop.TenLop.Contains(searchValue))
                    .ToList();

                if (searchResults.Count > 0)
                {
                    // Hiển thị dữ liệu tìm được trên DataGridView
                    BindGrid(searchResults);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy kết quả nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) || cboLopHoc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy dữ liệu từ form
                string maSV = txtMaSV.Text.Trim();
                string hoTen = txtHoTen.Text.Trim();
                DateTime ngaySinh = dtNgaySinh.Value;
                string maLop = cboLopHoc.SelectedValue.ToString();

                using (var context = new StudentContextDB())
                {
                    // Kiểm tra trùng mã sinh viên
                    var existingStudent = context.SinhViens.FirstOrDefault(s => s.MaSV == maSV);
                    if (existingStudent != null)
                    {
                        MessageBox.Show("Mã sinh viên đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Tạo đối tượng SinhVien mới
                    var newSinhVien = new SinhVien
                    {
                        MaSV = maSV,
                        HoTenSV = hoTen,
                        NgaySinh = ngaySinh,
                        MaLop = maLop
                    };

                    // Lưu đối tượng vào biến selectedSinhVien (tạm thời giữ trong bộ nhớ)
                    selectedSinhVien = newSinhVien;

                    // Kích hoạt nút Lưu
                    btnLuu.Enabled = true;

                    MessageBox.Show("Đã thêm sinh viên mới. Nhấn Lưu để lưu vào cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (selectedSinhVien == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị hộp thoại xác nhận trước khi xóa
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa sinh viên ! " + selectedSinhVien.HoTenSV + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            // Nếu người dùng chọn Yes, thực hiện xóa
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Xóa sinh viên khỏi cơ sở dữ liệu
                    using (var context = new StudentContextDB())
                    {
                        // Tìm sinh viên cần xóa trong cơ sở dữ liệu
                        var sinhVienToDelete = context.SinhViens.FirstOrDefault(s => s.MaSV == selectedSinhVien.MaSV);

                        if (sinhVienToDelete != null)
                        {
                            // Xóa sinh viên từ DbSet
                            context.SinhViens.Remove(sinhVienToDelete);
                            context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                            MessageBox.Show("Sinh viên đã được xóa khỏi cơ sở dữ liệu.");

                            // Cập nhật lại giao diện (nếu cần)
                            LoadData();
                            ClearForm();  // Làm sạch form sau khi xóa
                            btnLuu.Enabled = false;  // Tắt nút Lưu
                        }
                        else
                        {
                            MessageBox.Show("Sinh viên không tồn tại trong cơ sở dữ liệu.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi xóa sinh viên: " + ex.Message);
                }
            }
            else
            {
                // Nếu người dùng chọn No, không làm gì
                MessageBox.Show("Thao tác xóa đã bị hủy.");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (selectedSinhVien == null)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) || cboLopHoc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật dữ liệu trong đối tượng selectedSinhVien
                selectedSinhVien.HoTenSV = txtHoTen.Text.Trim();
                selectedSinhVien.NgaySinh = dtNgaySinh.Value;
                selectedSinhVien.MaLop = cboLopHoc.SelectedValue.ToString();

                // Kích hoạt nút Lưu sau khi sửa
                btnLuu.Enabled = true;

                // Lưu thay đổi vào cơ sở dữ liệu khi người dùng nhấn nút Lưu
                MessageBox.Show("Bạn đã sửa thông tin sinh viên thành công. Nhấn Lưu để cập nhật vào cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new StudentContextDB())
                {
                    // Kiểm tra nếu là thêm sinh viên mới
                    if (selectedSinhVien != null)
                    {
                        // Kiểm tra xem sinh viên đã tồn tại trong cơ sở dữ liệu chưa
                        var existingSinhVien = context.SinhViens.FirstOrDefault(s => s.MaSV == selectedSinhVien.MaSV);

                        if (existingSinhVien == null)
                        {
                            // Nếu sinh viên chưa có, thêm vào cơ sở dữ liệu
                            context.SinhViens.Add(selectedSinhVien);
                            context.SaveChanges(); // Lưu vào cơ sở dữ liệu
                            MessageBox.Show("Sinh viên đã được thêm vào cơ sở dữ liệu.");
                        }
                        else
                        {
                            // Nếu sinh viên đã có, cập nhật thông tin sinh viên
                            existingSinhVien.HoTenSV = selectedSinhVien.HoTenSV;
                            existingSinhVien.NgaySinh = selectedSinhVien.NgaySinh;
                            existingSinhVien.MaLop = selectedSinhVien.MaLop;
                            context.SaveChanges(); // Lưu thay đổi
                            MessageBox.Show("Thông tin sinh viên đã được cập nhật.");
                        }

                        // Cập nhật lại giao diện
                        LoadData();
                        ClearForm();
                        btnLuu.Enabled = false; // Disable nút Lưu sau khi lưu
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKhongLuu_Click(object sender, EventArgs e)
        {
           
            // Hủy bỏ thay đổi và tắt nút Lưu và KhongLuu
            ClearForm();
            btnLuu.Enabled = false;
            btnKhongLuu.Enabled = false;
        

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Ban co chac chan thoat !", "Xac Nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (rs == DialogResult.Yes)
            {
                this.Close();
            }
        }

       
    }
}
