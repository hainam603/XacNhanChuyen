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

namespace XacNhanChuyen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void dangNhap(string userName, string passWord, IWebDriver driver)
        {
            IWebElement user = driver.FindElement(By.Id("username"));
            IWebElement pass = driver.FindElement(By.Id("password"));
            user.SendKeys(userName);
            pass.SendKeys(passWord);
            pass.Submit();
        }
        string[] suCo = { "không về bến", "khong ve ben", "không vê bến", "không về bến", "không hoạt động", "khong hoat dong",
            " không hoat động", "không hoạt đông", "mât chuyến", "mat chuyen", "mất chuyến", "mc",
            "Xe hu", "xe hu", "xe hư", "thay vo", "thay vỏ", "be vo", "bể vỏ", "va quẹt", "va quet", "tai nan", "tai nạn", "dut day",
            "đứt dây" };
        private string[] dsTuyenTheoNgay(DateTimePicker dtpk)
        {
            DateTime dt = dtpk.Value;
            string day = dt.Date.Ticks.ToString();
            string[] dstuyen = {
                                "http://dnvt.ebms.vn/EarningYield/Trip/2?SrvDate=" +day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/24?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/26?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/34?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/43?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/48?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/52?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/55?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/59?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/64?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/70?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/73?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/76?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/77?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/78?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/79?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/106?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/112?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/113?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/116?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/119?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/127?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/129?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/130?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/11?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/15?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/18?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/178?SrvDate="+day,
                                "http://dnvt.ebms.vn/EarningYield/Trip/179?SrvDate="+day
                                };
            return dstuyen;
        }
        private bool kiemTraChuyenRong(IWebElement element)
        {
            bool rs;
            if (element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title") == "Đã xác nhận chuyến"
                || element.FindElement(By.CssSelector("td:nth-child(5) div")).GetAttribute("title") == "Đã từ chối chuyến"
                || element.FindElement(By.CssSelector("td:nth-child(2) div")).GetAttribute("onclick").Contains("undo"))
                rs = false;
            else
            {
                if (element.FindElement(By.CssSelector("td:nth-child(11)")).Text == ""
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
        private void thucHienXacNhanChuyenTheoDS(string url, IWebDriver driver)
        {
            
            driver.Url = url;


            driver.FindElement(By.CssSelector("#optTrip_RB2_I_D")).Click();
            driver.FindElement(By.CssSelector("#btnFilter")).Click();
            CheckPageIsLoaded(driver);

            int j = 0;
            int i = 0;
            IList<IWebElement> all = driver.FindElements(By.CssSelector("#tableContainer > tbody tr"));
      
            foreach (IWebElement element in all)
            {
                if (i == 5)
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
            
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "xnchuyen3";
            textBox2.Text = "H@inam123456";
        }
        private string getphut(string sophut)
        {
            if (sophut.Contains("("))
                sophut = "-" + sophut.Substring(1, 2);
            else
                sophut = sophut.Substring(0, 2);

            return sophut;
      }
        private string xacNhanChuyen(int phutXuatBen, int PhutVeBen, string ghiChu, IWebElement checkBox, IWebElement tuChoi, IWebDriver driver)
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
                        else //phutVeBen >15
                        {
                            if (PhutVeBen >= 60)
                                ms = "Không đủ điều kiện xác nhận";
                            else // 15< phutVeBen <60
                            {
                                if (ghiChu != "")
                                {
                                    ms = "Đủ điều kiện xác nhận";
                                    checkBox.Click();
                                }
                                else //Không ghi chú
                                    ms = "Không đủ điều kiện xác nhận";
                            }

                        }
                    }
                }
            }
            return ms;
            
        }
        private void dsCacChuyenChuaXacNhan(string url, IWebDriver driver)
        {
            driver.Url = url;
            driver.FindElement(By.CssSelector("#optTrip_RB2_I_D")).Click();
            driver.FindElement(By.CssSelector("#btnFilter_CD")).Click();
        }
        private void moTabMoi(IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("body")).SendKeys(OpenQA.Selenium.Keys.Control + 'w');
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
        private void button1_Click(object sender, EventArgs e)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("--start-minimized");
            chromeOptions.AddArguments("--start-maximized");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Url = "http://ebms.vn/";
            dangNhap(textBox1.Text, textBox2.Text, driver);
            foreach (string url in dsTuyenTheoNgay(dateTimePicker1))
            {
                thucHienXacNhanChuyenTheoDS(url, driver);

            }
            //foreach (string url in dsTuyenTheoNgay(dateTimePicker1))
            //{
            //    dsCacChuyenChuaXacNhan(url, driver);
            //    moTabMoi(driver);
            //}

            driver.Quit();
        }
        private void button2_Click(object sender, EventArgs e)
        {
   
            foreach (string url in dsTuyenTheoNgay(dateTimePicker1))
            {
                System.Diagnostics.Process.Start(url);
            }
        }
    }
}
