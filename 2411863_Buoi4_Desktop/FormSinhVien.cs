using _2411863_Buoi4_Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2411863_Buoi4_Desktop
{
    public partial class FormSinhVien : Form   
    {
        private QuanLySinhVien qlsv = new QuanLySinhVien();
        public FormSinhVien()
        {
            InitializeComponent();
        }
        private void FormSinhVien_Load(object sender, EventArgs e)
        {
            ResetControls();
        }

        // Reset các control
        private void ResetControls()
        {
            txtMSSV.Clear();
            txtHoTenLot.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            txtCMND.Clear();
            txtDiaChi.Clear();
            rdNam.Checked = true;
            txtTen.Clear();
            cboLop.SelectedIndex = -1;
            txtSoDT.Clear();
            for (int i = 0; i < clbMonHoc.Items.Count; i++)
                clbMonHoc.SetItemChecked(i, false);
        }

        // Lấy sinh viên từ controls
        private ClassSinhVien GetSinhVienFromControls()
        {
            ClassSinhVien sv = new ClassSinhVien();
            sv.MSSV = txtMSSV.Text.Trim();
            sv.HoTenLot = txtHoTenLot.Text.Trim();
            sv.Ten = txtTen.Text.Trim();
            sv.NgaySinh = dtpNgaySinh.Value;
            sv.Lop = cboLop.Text;
            sv.CMND = txtCMND.Text.Trim();
            sv.SoDT = txtSoDT.Text.Trim();
            sv.DiaChiLienLac = txtDiaChi.Text.Trim();
            sv.GioiTinh = rdNam.Checked;

            for (int i = 0; i < clbMonHoc.Items.Count; i++)
                if (clbMonHoc.GetItemChecked(i))
                    sv.MonHocDangKy.Add(clbMonHoc.Items[i].ToString());

            return sv;
        }

        // Hiển thị sinh viên lên controls
        private void DisplaySinhVien(ClassSinhVien sv)
        {
            if (sv == null) return;
            txtMSSV.Text = sv.MSSV;
            txtHoTenLot.Text = sv.HoTenLot;
            txtTen.Text = sv.Ten;
            dtpNgaySinh.Value = sv.NgaySinh;
            cboLop.Text = sv.Lop;
            txtCMND.Text = sv.CMND;
            txtSoDT.Text = sv.SoDT;
            txtDiaChi.Text = sv.DiaChiLienLac;
            rdNam.Checked = sv.GioiTinh;
            rdNu.Checked = !sv.GioiTinh;

            for (int i = 0; i < clbMonHoc.Items.Count; i++)
                clbMonHoc.SetItemChecked(i,
                    sv.MonHocDangKy.Contains(clbMonHoc.Items[i].ToString()));
        }

        // Tải danh sách lên ListView
        private void LoadListView()
        {
            lvSinhVien.Items.Clear();
            foreach (ClassSinhVien sv in qlsv.dsSinhVien)
            {
                ListViewItem item = new ListViewItem(sv.MSSV);
                item.SubItems.Add(sv.HoTenLot);
                item.SubItems.Add(sv.Ten);
                item.SubItems.Add(sv.NgaySinh.ToString("dd/MM/yyyy"));
                item.SubItems.Add(sv.Lop);
                item.SubItems.Add(sv.CMND);
                item.SubItems.Add(sv.SoDT);
                item.SubItems.Add(sv.DiaChiLienLac);
                lvSinhVien.Items.Add(item);
            }
        }

        // ===== NÚT THÊM MỚI =====
        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            ClassSinhVien sv = GetSinhVienFromControls();

            if (string.IsNullOrEmpty(sv.MSSV))
            {
                MessageBox.Show("Vui lòng nhập MSSV!");
                return;
            }

            if (qlsv.TimTheoMSSV(sv.MSSV) != null)
            {
                MessageBox.Show("MSSV đã tồn tại!");
                return;
            }

            qlsv.Them(sv);
            LoadListView();
            ResetControls();
            MessageBox.Show("Thêm sinh viên thành công!");
        }

        // ===== NÚT CẬP NHẬT =====
        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (lvSinhVien.SelectedItems.Count == 0)
            {
                MessageBox.Show("Chọn sinh viên cần cập nhật!");
                return;
            }

            ClassSinhVien svMoi = GetSinhVienFromControls();
            if (qlsv.Sua(svMoi))
            {
                LoadListView();
                ResetControls();
                MessageBox.Show("Cập nhật thành công!");
            }
            else
                MessageBox.Show("Không tìm thấy sinh viên để cập nhật!");
        }

        // ===== NÚT TÌM KIẾM =====
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string mssv = txtMSSV.Text.Trim();
            if (string.IsNullOrEmpty(mssv))
            {
                MessageBox.Show("Nhập MSSV để tìm kiếm!");
                return;
            }

            ClassSinhVien sv = qlsv.TimTheoMSSV(mssv);
            if (sv == null)
                MessageBox.Show("Không tìm thấy sinh viên!");
            else
                DisplaySinhVien(sv);
        }

        // ===== NÚT THOÁT =====
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ===== CHỌN DÒNG TRÊN LISTVIEW =====
        private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhVien.SelectedItems.Count == 0) return;
            string mssv = lvSinhVien.SelectedItems[0].Text;
            ClassSinhVien sv = qlsv.TimTheoMSSV(mssv);
            if (sv != null) DisplaySinhVien(sv);
        }
        // ==================== TXT ====================
        private void mnuDocTXT_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Title = "Chọn file TXT cần đọc";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                qlsv.DocTXT(ofd.FileName);
                LoadListView();
                MessageBox.Show($"Đã đọc {qlsv.dsSinhVien.Count} sinh viên từ TXT.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void mnuGhiTXT_Click(object sender, EventArgs e)
        {
            if (qlsv.dsSinhVien.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để ghi!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt";
            sfd.FileName = "DanhSachSV.txt";
            sfd.InitialDirectory = Application.StartupPath;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                qlsv.GhiTXT(sfd.FileName);
                MessageBox.Show($"Đã ghi {qlsv.dsSinhVien.Count} sinh viên ra TXT.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ==================== JSON ====================
        private void mnuDocJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON Files (*.json)|*.json";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Title = "Chọn file JSON cần đọc";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                qlsv.DocJSON(ofd.FileName);
                LoadListView();
                MessageBox.Show($"Đã đọc {qlsv.dsSinhVien.Count} sinh viên từ JSON.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void mnuGhiJSON_Click(object sender, EventArgs e)
        {
            if (qlsv.dsSinhVien.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để ghi!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON Files (*.json)|*.json";
            sfd.FileName = "DanhSachSV.json";
            sfd.InitialDirectory = Application.StartupPath;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                qlsv.GhiJSON(sfd.FileName);
                MessageBox.Show($"Đã ghi {qlsv.dsSinhVien.Count} sinh viên ra JSON.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ==================== XML ====================
        private void mnuDocXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*.xml";
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Title = "Chọn file XML cần đọc";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                qlsv.DocXML(ofd.FileName);
                LoadListView();
                MessageBox.Show($"Đã đọc {qlsv.dsSinhVien.Count} sinh viên từ XML.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void mnuGhiXML_Click(object sender, EventArgs e)
        {
            if (qlsv.dsSinhVien.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để ghi!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML Files (*.xml)|*.xml";
            sfd.FileName = "DanhSachSV.xml";
            sfd.InitialDirectory = Application.StartupPath;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                qlsv.GhiXML(sfd.FileName);
                MessageBox.Show($"Đã ghi {qlsv.dsSinhVien.Count} sinh viên ra XML.",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblMSSV_Click(object sender, EventArgs e)
        {

        }

        private void txtCMND_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblDiaChi_Click(object sender, EventArgs e)
        {

        }

        private void lblMonHoc_Click(object sender, EventArgs e)
        {

        }

        private void lblDanhSach_Click(object sender, EventArgs e)
        {

        }
    }
}
