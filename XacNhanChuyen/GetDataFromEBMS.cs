using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;

namespace XacNhanChuyen
{
    public partial class GetDataFromEBMS : Form
    {
        private int rowIndex = 2;
        private int i = 0;

        Form1 form1 = new Form1();
        public GetDataFromEBMS()
        {
            InitializeComponent();
        }

        private void btnLayDanhSach_Click(object sender, EventArgs e)
        {
            rowIndex = 2;
            i = 0;
            if (txtMaTuyen.Text == ""||textBox3.Text=="")
                return;
            else
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                //chromeOptions.AddArguments("--start-minimized");
                chromeOptions.AddArguments("--start-maximized");
                IWebDriver driver = new ChromeDriver(chromeOptions);
                driver.Url = "http://ebms.vn/";
                dangNhap(textBox1.Text, textBox2.Text, driver);

                DateTime startDate = dateTimePicker1.Value;
                DateTime endDate = dateTimePicker2.Value;
                string url = "";

                
                XSSFWorkbook wb = new XSSFWorkbook();
                ISheet sheet = wb.CreateSheet();
                // Tạo row
                var row0 = sheet.CreateRow(0);
                // Merge lại row đầu 3 cột
                row0.CreateCell(0); // tạo ra cell trc khi merge
                CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 9);
                sheet.AddMergedRegion(cellMerge);
                row0.GetCell(0).SetCellValue("Danh Sách các chuyến chưa xác nhận tuyến " + txtMaTuyen.Text);

                // Ghi tên cột ở row 1
                var row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("STT");
                row1.CreateCell(1).SetCellValue("Ngày");
                row1.CreateCell(2).SetCellValue("Đầu Bến");
                row1.CreateCell(3).SetCellValue("Đơn Vị");
                row1.CreateCell(4).SetCellValue("Giờ xuất bến KH");
                row1.CreateCell(5).SetCellValue("Giờ về bến KH");
                row1.CreateCell(6).SetCellValue("Số xe");
                row1.CreateCell(7).SetCellValue("Chênh lệch xuất bến");
                row1.CreateCell(8).SetCellValue("Chênh lệch về bến");
                row1.CreateCell(9).SetCellValue("Ghi chú");

                

                for (DateTime date = startDate; endDate.AddDays(1.0).CompareTo(date) > 0; date = date.AddDays(1.0))
                {
                    url = "http://dnvt.ebms.vn/EarningYield/Trip/" + txtMaTuyen.Text + "?SrvDate=" + date.Date.Ticks.ToString();
                    nhanDuLieuCacChuyenTuChoi(url, driver, date, sheet);

                }
                string dName = DateTime.Now.Ticks.ToString();
                // xong hết thì save file lại
                FileStream fs = new FileStream(@""+textBox3.Text+""+ dName + ".xlsx", FileMode.CreateNew);
                wb.Write(fs);
                driver.Quit();
            }
           
        }

        private void dangNhap(string userName, string passWord, IWebDriver driver)
        {
            IWebElement user = driver.FindElement(By.Id("username"));
            IWebElement pass = driver.FindElement(By.Id("password"));
            user.SendKeys(userName);
            pass.SendKeys(passWord);
            pass.Submit();
        }
        private void CheckPageIsLoaded(IWebDriver driver)
        {
            while (true)
            {
                bool ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    return;
                Thread.Sleep(100);
            }
        }

        

        private void nhanDuLieuCacChuyenTuChoi(string url, IWebDriver driver, DateTime date, ISheet sheet)
        {
            try { driver.Url = url; }
            catch { }

            driver.FindElement(By.CssSelector("#optTrip_RB2_I_D")).Click();
            driver.FindElement(By.CssSelector("#btnFilter")).Click();
            CheckPageIsLoaded(driver);
            if (i == 0)
            {
                driver.FindElement(By.CssSelector("#ContentSplitter_1_S_CB_Img")).Click();
                CheckPageIsLoaded(driver);
            }
            
            i++;
            IList<IWebElement> all = driver.FindElements(By.CssSelector("#tableContainer > tbody tr"));

            foreach (IWebElement element in all)
            {
                try
                {
                    driver.FindElement(By.CssSelector("#cboRouteVar_I")).Clear();
                    driver.FindElement(By.CssSelector("#cboRouteVar_I")).SendKeys(element.FindElement(By.CssSelector("td:nth-child(4)")).Text);

                    //if (element.FindElement(By.CssSelector("td:nth-child(2) div")).GetAttribute("onclick").Contains("undo"))
                        //continue;
                    string sTT = element.FindElement(By.CssSelector("td:nth-child(4)")).Text;
                    string dauBen = element.FindElement(By.CssSelector("td:nth-child(6)")).Text;
                    string dnvt = element.FindElement(By.CssSelector("td:nth-child(7)")).Text;
                    string gioXuatBenKH = element.FindElement(By.CssSelector("td:nth-child(8)")).Text;
                    string gioVeBenKH = element.FindElement(By.CssSelector("td:nth-child(9)")).Text;
                    string soXe1 = element.FindElement(By.CssSelector("td:nth-child(10)")).Text;
                    string soXe2 = element.FindElement(By.CssSelector("td:nth-child(11)")).Text;
                    string phutXuatBen = element.FindElement(By.CssSelector("td:nth-child(14)")).Text;
                    string phutVeBen = element.FindElement(By.CssSelector("td:nth-child(15)")).Text;
                    string ghiChu = element.FindElement(By.CssSelector("td:nth-child(19)")).GetAttribute("title");

                    var newRow = sheet.CreateRow(rowIndex);

                    newRow.CreateCell(0).SetCellValue(sTT);
                    newRow.CreateCell(1).SetCellValue(date.ToShortDateString());
                    newRow.CreateCell(2).SetCellValue(dauBen);
                    newRow.CreateCell(3).SetCellValue(dnvt);
                    newRow.CreateCell(4).SetCellValue(gioXuatBenKH);
                    newRow.CreateCell(5).SetCellValue(gioVeBenKH);
                    newRow.CreateCell(6).SetCellValue(soXe1+ soXe2);
                    newRow.CreateCell(7).SetCellValue(phutXuatBen);
                    newRow.CreateCell(8).SetCellValue(phutVeBen);
                    newRow.CreateCell(9).SetCellValue(ghiChu);

                    rowIndex++;

                    //richTextBox1.Text += sTT + " | " + date.ToShortDateString()+ " | " + dauBen + " | " + dnvt + " | " + gioXuatBenKH + " | " + gioVeBenKH + " | " + soXe1 + soXe2 + " | " + phutXuatBen + " | " + phutVeBen + " | " + ghiChu + "\n";
                    driver.FindElement(By.CssSelector("#cboRouteVar_I")).SendKeys("");
                    driver.FindElement(By.CssSelector("#cboRouteVar_I")).SendKeys(sTT);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserName = textBox1.Text;
            Properties.Settings.Default.PassWord = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void GetDataFromEBMS_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.UserName;
            textBox2.Text = Properties.Settings.Default.PassWord;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (txtMaTuyen.Text == "")
                return;
            else
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                //chromeOptions.AddArguments("--start-minimized");
                chromeOptions.AddArguments("--start-maximized");
                IWebDriver driver = new ChromeDriver(chromeOptions);
                driver.Url = "http://ebms.vn/";
                dangNhap(textBox1.Text, textBox2.Text, driver);

                DateTime startDate = dateTimePicker1.Value;
                DateTime endDate = dateTimePicker2.Value;
                string url = "";

                for (DateTime date = startDate; endDate.AddDays(1.0).CompareTo(date) > 0; date = date.AddDays(1.0))
                {
                    url = "http://dnvt.ebms.vn/EarningYield/Trip/" + txtMaTuyen.Text + "?SrvDate=" + date.Date.Ticks.ToString();
                    form1.thucHienXacNhanChuyenTheoDS(url, driver);

                }
                driver.Quit();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (txtMaTuyen.Text == "")
                return;
            else
            {
                DateTime startDate = dateTimePicker1.Value;
                DateTime endDate = dateTimePicker2.Value;
                string url = "";
                for (DateTime date = startDate; endDate.AddDays(1.0).CompareTo(date) > 0; date = date.AddDays(1.0))
                {
                    url = "http://dnvt.ebms.vn/EarningYield/Trip/" + txtMaTuyen.Text + "?SrvDate=" + date.Date.Ticks.ToString();
                    System.Diagnostics.Process.Start(url);

                }
            }
        }
    }
}
