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
using OpenQA.Selenium.Support;
using System.Threading;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;

namespace XacNhanChuyen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void dangNhap(string userName, string passWord, IWebDriver driver)
        {
            IWebElement user = driver.FindElement(By.Id("username"));
            IWebElement pass = driver.FindElement(By.Id("password"));
            user.SendKeys(userName);
            pass.SendKeys(passWord);
            pass.Submit();
        }
        public string[] suCo = { "tainan", "khongveben", "khonghoatdong", "matchuyen", "xehu", "baohu", "huxe", "thayvo", "bevo", "lungvo", "vaquet", "vacham", "dutday", "duthetday", "khonglydo", "khongcolydo" };

        public static string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        public string xuLyChuoi(string chuoi)
        {
            string[] dau = { ",", ".", " ", "?", ":", ";", "!", "@", "#", "$", "%", "^","&","*","(",")","-","_","+","=","`","~" };
            chuoi = chuoi.Trim();
            foreach(string d in dau)
            {
                chuoi=chuoi.Replace(d,"");
            }
            chuoi = RemoveVietnameseTone(chuoi);
            return chuoi;
        }
        public string[] dsTuyenTheoNgay(DateTimePicker dtpk, string dst)
        {
            DateTime dt = dtpk.Value;
            string[] dstuyen=new string[dst.Split(',').Count()];
            string day = dt.Date.Ticks.ToString();
            int i=0;
            foreach (string idTuyen in dst.Split(','))
            {
                dstuyen[i]="http://dnvt.ebms.vn/EarningYield/Trip/"+idTuyen+"?SrvDate=" +day;
                i++;
            }
            return dstuyen;
        }
        public bool kiemTraChuyenRong(IWebElement element)
        {
            bool rs;
            if (element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title") == "Đã xác nhận chuyến"
                || element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title") == "Đã từ chối chuyến"
                || element.FindElement(By.CssSelector("td:nth-child(2) div")).GetAttribute("onclick").Contains("undo"))
                rs = false;
            else
            {
                if (element.FindElement(By.CssSelector("td:nth-child(10)")).Text == ""
                && element.FindElement(By.CssSelector("td:nth-child(11)")).Text == ""
                && element.FindElement(By.CssSelector("td:nth-child(12)")).Text == ""
                && element.FindElement(By.CssSelector("td:nth-child(13)")).Text == ""
                && element.FindElement(By.CssSelector("td:nth-child(14)")).Text == ""
                && element.FindElement(By.CssSelector("td:nth-child(15)")).Text == ""
                && element.FindElement(By.CssSelector("td:nth-child(19)")).GetAttribute("title") == "")
                    rs = true;
                else
                    rs = false;
            }
            
            
            return rs;
        }
        public void thucHienXacNhanChuyenTheoDS(string url, IWebDriver driver)
        {
            try { driver.Url = url; }
            catch { }
            
            driver.FindElement(By.CssSelector("#optTrip_RB2_I_D")).Click();
            driver.FindElement(By.CssSelector("#btnFilter")).Click();
            CheckPageIsLoaded(driver);

            int j = 0;
            int i = 0;
            IList<IWebElement> all = driver.FindElements(By.CssSelector("#tableContainer > tbody tr"));
      
            foreach (IWebElement element in all)
            {
                if (i == 20)
                    break;
                try
                {
                    driver.FindElement(By.CssSelector("#cboRouteVar_I")).Clear();
                    driver.FindElement(By.CssSelector("#cboRouteVar_I")).SendKeys(element.FindElement(By.CssSelector("td:nth-child(4)")).Text);
                    
                    if (kiemTraChuyenRong(element) == true)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        if (element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title") == "Đã xác nhận chuyến" 
                            || element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title") == "Đã từ chối chuyến"
                            || element.FindElement(By.CssSelector("td:nth-child(2) div")).GetAttribute("onclick").Contains("undo"))
                            continue;
                        string sTT = element.FindElement(By.CssSelector("td:nth-child(4)")).Text;
                        string gioXuatBen = element.FindElement(By.CssSelector("td:nth-child(8)")).Text;
                        string soXe = element.FindElement(By.CssSelector("td:nth-child(11)")).Text;
                        string phutXuatBen = element.FindElement(By.CssSelector("td:nth-child(14)")).Text;
                        string phutVeBen = element.FindElement(By.CssSelector("td:nth-child(15)")).Text;
                        string ghiChu = element.FindElement(By.CssSelector("td:nth-child(19)")).GetAttribute("title").ToLower();
                        ghiChu = xuLyChuoi(ghiChu);
                        string dauBen = element.FindElement(By.CssSelector("td:nth-child(8)")).Text;
                        IWebElement check = element.FindElement(By.CssSelector("td:nth-child(1)"));
                        IWebElement tuChoi = element.FindElement(By.CssSelector("td:nth-child(2)"));
                        driver.FindElement(By.CssSelector("#cboRouteVar_I")).SendKeys("");
                        driver.FindElement(By.CssSelector("#cboRouteVar_I")).SendKeys(sTT);
                        if (phutVeBen != "" && phutXuatBen != "")
                        {
                            string ms = xacNhanChuyen(Int32.Parse(getphut(phutXuatBen)), Int32.Parse(getphut(phutVeBen)), ghiChu, check, tuChoi, driver);
                        }
                        else
                        {

                            //string[] matChuyen = { "không về bến", "khong ve ben", "không vê bến", "không về bến", "không hoạt động", "khong hoat dong", " không hoat động", "không hoạt đông", "mât chuyến", "Mat chuyen", "Mất chuyến", "mat chuyen", "mất chuyến","mc","Mc", "xe hu", "xe hư", "thay vo", "thay vỏ", "be vo", "bể vỏ", "va quẹt", "va quet", "tai nan", "tai nạn", "dut day", "đứt dây" };
                            foreach (string s in suCo)
                            {
                                if (ghiChu.Contains(s))
                                {
                                    tuChoi.Click();
                                    CheckPageIsLoaded(driver);
                                    try
                                    {
                                        driver.FindElement(By.CssSelector("#btnPopupConfirm_CD")).Click();
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                                    //#btnPopupConfirm_CD
                                    CheckPageIsLoaded(driver);
                                    break;
                                }
                            }

                        }
                        j++;
                    }
                    

                }
                catch (Exception ex){
                    //MessageBox.Show(ex.ToString());
                }
            }

            //Click xác nhận chuyến
            try
            {
                driver.FindElement(By.CssSelector("#btnConfirmTripSchedule_CD")).Click();
                driver.FindElement(By.CssSelector("#btnPopupConfirm_CD")).Click();
            }
            catch (Exception ex) { }
       
            //driver.Close();
        }
        public void thucHienXacNhanCoTheoDS(string url, IWebDriver driver)
        {
            try { driver.Url = url; }
            catch { }

            driver.FindElement(By.CssSelector("#optTrip_RB3_I_D")).Click();
            driver.FindElement(By.CssSelector("#btnFilter")).Click();
            CheckPageIsLoaded(driver);

            int j = 0;
            int i = 0;
            IList<IWebElement> all = driver.FindElements(By.CssSelector("#tableContainer > tbody tr"));

            foreach (IWebElement element in all)
            {
                if (i == 20)
                {
                    break;
                }
                    
                try
                {
                    if (kiemTraChuyenRong(element) == true)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        if(element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title").ToString().Contains("Đã xác nhận chuyến"))
                        {
                            if (element.FindElement(By.CssSelector("td:nth-child(17)")).Text == "Có")
                                continue;

                            IWebElement xemChiTiet = element.FindElement(By.CssSelector("td:nth-child(21) > div.cmdTripDetail"));
                            xemChiTiet.Click();
                            //CheckPageIsLoaded(driver);

                            IWebElement elKetXe = kiemtraElement(driver, "#ui-accordion-dailyTripAccordion-header-5");
                            elKetXe.Click();
                            IWebElement elCo = kiemtraElement(driver, "#optConfirmTrafficJam_RB1_I_D");
                            elCo.Click();
                            driver.FindElement(By.CssSelector("#btnConfirmTrafficJam_CD")).Click();
                            driver.FindElement(By.CssSelector("#btnClose_CD")).Click();
                            j++;
                        }
                       
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }
            //driver.Close();
        }

        public void thucHienGoChuyen(string url, IWebDriver driver, int from, int to, string dauBen)
        {
            try { driver.Url = url; }
            catch { }
            driver.FindElement(By.CssSelector("#cboRouteVar_B-1")).Click();
            driver.FindElement(By.CssSelector("#cboRouteVar_DDD_L_LBI"+dauBen+"T0")).Click();
            driver.FindElement(By.CssSelector("#btnFilter")).Click();
            CheckPageIsLoaded(driver);

            IList<IWebElement> all = driver.FindElements(By.CssSelector("#tableContainer > tbody tr"));

            foreach (IWebElement element in all)
            {
              
                try
                {
                    int sTT =Int32.Parse(element.FindElement(By.CssSelector("td:nth-child(4)")).Text);
                    if(sTT>=from&&sTT<=to)
                    if (element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title").ToString().Contains("Đã xác nhận chuyến"))
                    {
                            element.FindElement(By.CssSelector("td:nth-child(2) div")).Click();
                            driver.FindElement(By.CssSelector("#btnPopupConfirm_CD")).Click();
                    }

                    
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }

        }
        public IWebElement kiemtraElement(IWebDriver driver, string cssSelector)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement elCo = wait.Until<IWebElement>((d) =>
            {
                IWebElement eCo = d.FindElement(By.CssSelector(cssSelector));
                if (eCo.Displayed && eCo.Enabled)
                {
                    return eCo;
                }

                return null;
            });

            return elCo;
        }
        public void Form1_Load(object sender, EventArgs e)
        {
            cbbDauTuyen.SelectedIndex = 0;
            textBox1.Text = Properties.Settings.Default.UserName;
            textBox2.Text = Properties.Settings.Default.PassWord;
            textBox4.Text = Properties.Settings.Default.DsTuyen;
            textBox3.Text = Properties.Settings.Default.DSTuyenBoQua;
            label6.Text = textBox4.Text.Split(',').Count().ToString();
            label7.Text = textBox3.Text.Split(',').Count().ToString();
        }
        public string getphut(string sophut)
        {
            if (sophut.Contains("("))
                sophut = "-" + sophut.Substring(1, 2);
            else
                sophut = sophut.Substring(0, 2);

            return sophut;
      }
        public string xacNhanChuyen(int phutXuatBen, int PhutVeBen, string ghiChu, IWebElement checkBox, IWebElement tuChoi, IWebDriver driver)
        {
            //string[] suCo = {"xe hu", "xe hư", "thay vo", "thay vỏ", "be vo", "bể vỏ", "va quẹt", "va quet", "tai nan", "tai nạn", "dut day", "đứt dây" };
            string ms = "";
            int flag=0;
            foreach (string s in suCo)
            {
                if (ghiChu.Contains(s))
                {
                    tuChoi.Click();
                    try
                    {
                        driver.FindElement(By.CssSelector("#btnPopupConfirm_CD")).Click();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                    flag = 1;
                    break;
                }
            }
            if (flag == 1)
            {
                ms = "Không đủ điều kiện xác nhận";
            }
            else
            {
                if (phutXuatBen >= 10)
                    ms = "Không đủ điều kiện xác nhận";
                else // phutXuatBen < 10
                {
                    if (phutXuatBen <= -10)
                        ms = "Không đủ điều kiện xác nhận";
                    else
                    {
                        if (PhutVeBen < 15)
                        {
                            ms = "Đủ điều kiện xác nhận";
                            checkBox.Click();
                        }
                        //else //phutVeBen >15
                        //{
                        //    if (PhutVeBen >= 60)
                        //        ms = "Không đủ điều kiện xác nhận";
                        //    else // 15< phutVeBen <60
                        //    {
                        //        //if (ghiChu != "")
                        //        //{
                        //        //    ms = "Đủ điều kiện xác nhận";
                        //        //    checkBox.Click();
                        //        //}
                        //        //else //Không ghi chú
                        //        //    ms = "Không đủ điều kiện xác nhận";
                        //    }

                        //}
                    }
                }
            }
            return ms;
            
        }
        public void dsCacChuyenChuaXacNhan(string url, IWebDriver driver)
        {
            try {
                driver.Url = url;
                driver.FindElement(By.CssSelector("#optTrip_RB2_I_D")).Click();
                driver.FindElement(By.CssSelector("#btnFilter_CD")).Click();
            }
            catch { }
            
        }
        public void moTabMoi(IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("body")).SendKeys(OpenQA.Selenium.Keys.Control + 'w');
        }
        public void CheckPageIsLoaded(IWebDriver driver)
        {
            while (true)
            {
                bool ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    return;
                Thread.Sleep(100);
            }
        }
        public List<string> locTuyen(string chuoi, string[] dsTuyen)
        {
            List<string> list = new List<string>(dsTuyen);
            foreach (string url in dsTuyen)
            {
                if (chuoi != "")
                {
                    foreach (string c in chuoi.Split(','))
                    {
                        if (url.Substring(url.LastIndexOf('/') + 1, url.IndexOf('?') - url.LastIndexOf('/') - 1) == c.Trim())
                        {
                            list.Remove(url);
                            break;
                        }
                    }
                }
            }
            return list;
        }
        public void button1_Click(object sender, EventArgs e)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("--start-minimized");
            chromeOptions.AddArguments("--start-maximized");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Url = "http://ebms.vn/";
            dangNhap(textBox1.Text, textBox2.Text, driver);
            string chuoi = textBox3.Text;
            List<string> dsTuyen = locTuyen(chuoi, dsTuyenTheoNgay(dateTimePicker1,textBox4.Text));
            foreach(string url in dsTuyen)
            {
                thucHienXacNhanChuyenTheoDS(url, driver);
            }
           
            driver.Quit();
        }
        public void button2_Click(object sender, EventArgs e)
        {
            string chuoi = textBox3.Text;
            List<string> dsTuyen = locTuyen(chuoi, dsTuyenTheoNgay(dateTimePicker1,textBox4.Text));
            foreach (string url in dsTuyen)
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        public void textBox3_TextChanged(object sender, EventArgs e)
        {
            label6.Text = textBox4.Text.Split(',').Count().ToString();
            label7.Text = textBox3.Text.Split(',').Count().ToString();
        }

        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                Properties.Settings.Default.DsTuyen = textBox4.Text;
                Properties.Settings.Default.DSTuyenBoQua = textBox3.Text;
                Properties.Settings.Default.Save();
            }
        }

        public void label4_Click(object sender, EventArgs e)
        {

        }

        public void textBox4_TextChanged(object sender, EventArgs e)
        {
            label6.Text = textBox4.Text.Split(',').Count().ToString();
            label7.Text = textBox3.Text.Split(',').Count().ToString();
        }

        public void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                textBox3.Text = "";
            else
                textBox3.Text = Properties.Settings.Default.DSTuyenBoQua;
        }

        public void button3_Click(object sender, EventArgs e)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("--start-minimized");
            chromeOptions.AddArguments("--start-maximized");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Url = "http://ebms.vn/";
            dangNhap(textBox1.Text, textBox2.Text, driver);
            string chuoi = textBox3.Text;
            List<string> dsTuyen = locTuyen(chuoi, dsTuyenTheoNgay(dateTimePicker1, textBox4.Text));
            foreach (string url in dsTuyen)
            {
                thucHienXacNhanCoTheoDS(url, driver);
            }

            driver.Quit();
        }

        public void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserName = textBox1.Text;
            Properties.Settings.Default.PassWord = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        public void button4_Click(object sender, EventArgs e)
        {

        }

        public void btnGoChuyen_Click(object sender, EventArgs e)
        {
            if (txtMaTuyen.Text == "" || txtFrom.Text == "" || txtTo.Text == "")
                return;
            else
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                //chromeOptions.AddArguments("--start-minimized");
                chromeOptions.AddArguments("--start-maximized");
                IWebDriver driver = new ChromeDriver(chromeOptions);
                driver.Url = "http://ebms.vn/";
                dangNhap(textBox1.Text, textBox2.Text, driver);
                string url = "http://dnvt.ebms.vn/EarningYield/Trip/" + txtMaTuyen.Text + "?SrvDate=" + dateTimePicker1.Value.Date.Ticks.ToString();
                int from = Int32.Parse(txtFrom.Text);
                int to = Int32.Parse(txtTo.Text);
                string dauBen = cbbDauTuyen.SelectedIndex.ToString().Trim();
                thucHienGoChuyen(url, driver, from, to, dauBen);

                driver.Quit();
            }

        }
    }
}
