using System;
using System.Collections.Generic;
using System.Linq;


namespace _2411863_Buoi4_Desktop
{
    public class ClassSinhVien
    {


        public string MSSV { get; set; }
        public string HoTenLot { get; set; }
        public string Ten { get; set; }
        public DateTime NgaySinh { get; set; }
        public string Lop { get; set; }
        public string CMND { get; set; }
        public string SoDT { get; set; }
        public string DiaChiLienLac { get; set; }
        public bool GioiTinh { get; set; }   
        public List<string> MonHocDangKy { get; set; }


        public ClassSinhVien()
        {
            MonHocDangKy = new List<string>();
        }

        public ClassSinhVien(string ms, string ht, DateTime ngay, string dc, string lop,
                        string hinh, bool gt, List<string> cn)
        {
            MSSV = ms;
            HoTenLot = ht;
            NgaySinh = ngay;
            DiaChiLienLac   = dc;
            Lop = lop;
            CMND = hinh;
            GioiTinh = gt;
            MonHocDangKy = cn ?? new List<string>();
        }
        public override string ToString()
        {
            string gioiTinh = GioiTinh ? "Nam" : "Nữ";
            string monHoc = string.Join(", ", MonHocDangKy.ToArray());
            return $"{MSSV} - {HoTenLot} {Ten} - {NgaySinh:dd/MM/yyyy} - {DiaChiLienLac} - {Lop} - {gioiTinh} - {monHoc}";
        }
    }
     
}

