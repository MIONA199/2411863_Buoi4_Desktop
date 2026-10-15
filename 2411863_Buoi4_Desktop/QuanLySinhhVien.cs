using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace _2411863_Buoi4_Desktop
{
    public delegate int SoSanh(object sv1, object sv2);
    public class QuanLySinhVien
    {
        public List<ClassSinhVien> dsSinhVien;

        public QuanLySinhVien()
        {
            dsSinhVien = new List<ClassSinhVien>();
        }

        public void Them(ClassSinhVien sv) => dsSinhVien.Add(sv);

        public ClassSinhVien Tim(object obj, SoSanh ss)
        {
            foreach (ClassSinhVien sv in dsSinhVien)
                if (ss(obj, sv) == 0)
                    return sv;
            return null;
        }

        public ClassSinhVien TimTheoMSSV(string mssv)
        {
            foreach (ClassSinhVien sv in dsSinhVien)
                if (sv.MSSV == mssv)
                    return sv;
            return null;
        }

        public bool Sua(ClassSinhVien svMoi)
        {
            for (int i = 0; i < dsSinhVien.Count; i++)
                if (dsSinhVien[i].MSSV == svMoi.MSSV)
                {
                    dsSinhVien[i] = svMoi;
                    return true;
                }
            return false;
        }

        public void Xoa(object obj, SoSanh ss)
        {
            for (int i = dsSinhVien.Count - 1; i >= 0; i--)
                if (ss(obj, dsSinhVien[i]) == 0)
                    dsSinhVien.RemoveAt(i);
        }

        public void XoaTheoMSSV(string mssv)
        {
            for (int i = dsSinhVien.Count - 1; i >= 0; i--)
                if (dsSinhVien[i].MSSV == mssv)
                    dsSinhVien.RemoveAt(i);
        }

        // ================== TXT ==================
        public void DocTXT(string filename)
        {
            if (!File.Exists(filename)) return;
            dsSinhVien.Clear();
            string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
            var formats = new[] { "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd" };

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split('\t');
                if (parts.Length < 10) continue;

                ClassSinhVien sv = new ClassSinhVien();
                sv.MSSV = parts[0].Trim();
                sv.HoTenLot = parts[1].Trim();
                sv.Ten = parts[2].Trim();
                sv.NgaySinh = DateTime.ParseExact(parts[3].Trim(), formats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None);
                sv.Lop = parts[4].Trim();
                sv.CMND = parts[5].Trim();
                sv.SoDT = parts[6].Trim();
                sv.DiaChiLienLac = parts[7].Trim();
                sv.GioiTinh = parts[8].Trim() == "1";

                string[] mh = parts[9].Split(new[] { ", ", "," },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in mh)
                    if (!string.IsNullOrWhiteSpace(s))
                        sv.MonHocDangKy.Add(s.Trim());

                dsSinhVien.Add(sv);
            }
        }

        public void GhiTXT(string filename)
        {
            List<string> lines = new List<string>();
            foreach (ClassSinhVien sv in dsSinhVien)
            {
                string gt = sv.GioiTinh ? "1" : "0";
                string mh = string.Join(", ", sv.MonHocDangKy.ToArray());
                lines.Add($"{sv.MSSV}\t{sv.HoTenLot}\t{sv.Ten}\t{sv.NgaySinh:MM/dd/yyyy}\t" +
                          $"{sv.Lop}\t{sv.CMND}\t{sv.SoDT}\t{sv.DiaChiLienLac}\t{gt}\t{mh}");
            }
            File.WriteAllLines(filename, lines.ToArray(), Encoding.UTF8);
        }

        // ================== JSON ==================
        public void DocJSON(string filename)
        {
            if (!File.Exists(filename)) return;

            try
            {
                string json = File.ReadAllText(filename, Encoding.UTF8);
                dsSinhVien.Clear();

                try
                {
                    var wrapper = JsonConvert.DeserializeObject<QuanLySinhVienWrapper>(json);
                    if (wrapper?.students != null)
                    {
                        foreach (ClassSinhVien sv in wrapper.students)
                            dsSinhVien.Add(sv);
                        return;
                    }
                }
                catch { }

                var ds = JsonConvert.DeserializeObject<List<ClassSinhVien>>(json);
                if (ds != null)
                    foreach (ClassSinhVien sv in ds)
                        dsSinhVien.Add(sv);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc JSON: " + ex.Message, "Lỗi");
            }
        }

        public void GhiJSON(string filename)
        {
            string json = JsonConvert.SerializeObject(dsSinhVien, Formatting.Indented);
            File.WriteAllText(filename, json, Encoding.UTF8);
        }

        // ================== XML ==================
        public void DocXML(string filename)
        {
            if (!File.Exists(filename)) return;
            XDocument doc = XDocument.Load(filename);
            dsSinhVien.Clear();
            var formats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };

            foreach (XElement node in doc.Root.Elements("SinhVien"))
            {
                ClassSinhVien sv = new ClassSinhVien();
                sv.MSSV = (string)node.Element("MSSV") ?? "";
                sv.HoTenLot = (string)node.Element("HoTenLot") ?? "";
                sv.Ten = (string)node.Element("Ten") ?? "";
                string ngayStr = (string)node.Element("NgaySinh") ?? "";
                if (!string.IsNullOrEmpty(ngayStr))
                    sv.NgaySinh = DateTime.ParseExact(ngayStr, formats,
                        CultureInfo.InvariantCulture, DateTimeStyles.None);
                sv.Lop = (string)node.Element("Lop") ?? "";
                sv.CMND = (string)node.Element("CMND") ?? "";
                sv.SoDT = (string)node.Element("SoDT") ?? "";
                sv.DiaChiLienLac = (string)node.Element("DiaChiLienLac") ?? "";
                sv.GioiTinh = ((string)node.Element("GioiTinh") ?? "0") == "1";

                XElement mhNode = node.Element("MonHocDangKy");
                if (mhNode != null)
                    foreach (XElement mon in mhNode.Elements("Mon"))
                        sv.MonHocDangKy.Add((string)mon);

                dsSinhVien.Add(sv);
            }
        }

        public void GhiXML(string filename)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("DanhSachSinhVien",
                    dsSinhVien.Select(sv =>
                        new XElement("SinhVien",
                            new XElement("MSSV", sv.MSSV),
                            new XElement("HoTenLot", sv.HoTenLot),
                            new XElement("Ten", sv.Ten),
                            new XElement("NgaySinh", sv.NgaySinh.ToString("dd/MM/yyyy")),
                            new XElement("Lop", sv.Lop),
                            new XElement("CMND", sv.CMND),
                            new XElement("SoDT", sv.SoDT),
                            new XElement("DiaChiLienLac", sv.DiaChiLienLac),
                            new XElement("GioiTinh", sv.GioiTinh ? "1" : "0"),
                            new XElement("MonHocDangKy",
                                sv.MonHocDangKy.Select(mh => new XElement("Mon", mh)))
                        )
                    )
                )
            );
            doc.Save(filename);
        }
    }
}
