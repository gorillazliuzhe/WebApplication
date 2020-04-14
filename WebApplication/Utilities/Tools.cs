using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using Microsoft.AspNetCore.Hosting;
using SautinSoft;
using WebApplication.Models;

namespace WebApplication.Utilities
{
    public static class Tools
    {
        private static Settings _settings;

        public static void SetUtilsProviderConfiguration(Settings settings)
        {
            _settings = settings;
        }
        public static Settings GetInitConst()
        {
            return _settings;
        }


        public static async Task<bool> ToExcelAsync(IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var pdfFile = Path.Combine(webHostEnvironment.WebRootPath, "UploadFiles");
                DirectoryInfo folder = new DirectoryInfo(pdfFile);
                if (folder.GetFiles().Length > 0)
                {
                    var citylist = GetCityTownList(webHostEnvironment);
                    List<Bsjh> bsjhs = new List<Bsjh>();
                    #region PDF->XML
                    Console.WriteLine("开始生成XML");
                    foreach (FileInfo file in folder.GetFiles())
                    {
                        if (file.Extension.ToUpper() != ".PDF") continue;
                        var b = PdfToXMLAsFiles(file.FullName);
                        Console.WriteLine(b ? file.Name + "生成数据成功" : file.Name + "生成数据失败");
                    }
                    Console.WriteLine("生成XML结束");
                    #endregion

                    #region XML->LIST
                    int index = 1;
                    string yueri = DateTime.Now.AddDays(1).Date.ToString("MM-dd");
                    string xq = GetWeek();
                    //string jihuafile = yueri + " 星期" + xq + ".xlsx";
                    string jihuafile = DateTime.Now.Date.ToString("yyyy-M-d") + ".xlsx";
                    foreach (FileInfo file in folder.GetFiles())
                    {
                        if (file.Extension.ToUpper() == ".XML")
                        {

                            Bsjh bsjh = new Bsjh
                            {
                                Id = index,
                                Riqi = yueri,
                                XingQi = xq,
                                IceCar = "Y"
                            };
                            index++;
                            List<string> ishave = new List<string>();
                            XmlDocument doc = new XmlDocument();
                            doc.Load(file.FullName);
                            XmlNode rootNode = doc.DocumentElement;
                            StringBuilder sb = new StringBuilder();
                            StringBuilder lxsb = new StringBuilder();
                            foreach (XmlNode node in rootNode.ChildNodes)
                            {
                                if (node.Name == "page")
                                {
                                    foreach (XmlNode node2 in node.ChildNodes)
                                    {
                                        if (node2.Name == "table")
                                        {
                                            foreach (XmlNode node3 in node2.ChildNodes)
                                            {
                                                if (node3.Name == "row")
                                                {
                                                    foreach (XmlNode cellnode in node3.ChildNodes)
                                                    {
                                                        if (cellnode.Name == "cell")
                                                        {
                                                            var va = cellnode.InnerText;
                                                            #region 线路号
                                                            if (!ishave.Contains("lxh") && va.Contains("百胜中国运输派车单"))
                                                            {
                                                                va = va.Replace("百胜中国运输派车单 派车单号: ", "");
                                                                var varr = va.Split('-');
                                                                if (varr.Length > 0)
                                                                {
                                                                    ishave.Add("lxh");
                                                                    int.TryParse(varr[0], out int xianluhao);
                                                                    bsjh.LuXianHao = xianluhao;
                                                                }
                                                            }
                                                            #endregion

                                                            #region 箱数
                                                            if (!ishave.Contains("ss") && va.Contains("总件数"))
                                                            {
                                                                ishave.Add("ss");
                                                                double.TryParse(cellnode.NextSibling.InnerText, out double ss);
                                                                bsjh.XiangShu = ss;
                                                            }
                                                            #endregion

                                                            #region 立方数
                                                            if (!ishave.Contains("lfs") && va.Contains("总体积立方"))
                                                            {
                                                                ishave.Add("lfs");
                                                                double.TryParse(cellnode.NextSibling.InnerText, out double lfs);
                                                                bsjh.LiFangShu = lfs;
                                                            }
                                                            #endregion

                                                            #region 重量
                                                            if (!ishave.Contains("zl") && va.Contains("总重量KG"))
                                                            {
                                                                ishave.Add("zl");
                                                                double.TryParse(cellnode.NextSibling.InnerText, out double zl);
                                                                bsjh.ZhongLiang = zl;
                                                            }
                                                            #endregion

                                                            #region 公里数
                                                            if (!ishave.Contains("gls") && va.Contains("总里程"))
                                                            {
                                                                ishave.Add("gls");
                                                                double.TryParse(cellnode.NextSibling.InnerText, out double gls);
                                                                bsjh.GongLiShu = gls;
                                                            }
                                                            #endregion

                                                            #region 吨位
                                                            if (!ishave.Contains("dw") && va.Contains("车型:"))
                                                            {
                                                                var arry = cellnode.NextSibling.InnerText.Split("吨", StringSplitOptions.RemoveEmptyEntries);
                                                                if (arry.Length > 0)
                                                                {
                                                                    ishave.Add("dw");
                                                                    bsjh.DunWei = arry[0] + "T";
                                                                }
                                                            }
                                                            #endregion

                                                            #region 客户,路线,路线名称 [正则匹配] 
                                                            string rex = @"[\d]{2}\..*\.[\d]*"; // 匹配站点
                                                            var m = Regex.Match(va, rex);
                                                            // 正则匹配:03.鞍山云景.80131116 这种形式的  
                                                            if (m.Success)
                                                            {
                                                                var zdstr = m.Value;
                                                                #region 路线
                                                                if (zdstr.Contains("01.")) // 客户
                                                                {
                                                                    ishave.Add("01");
                                                                    string kehu = "肯德基";
                                                                    if (zdstr.Contains("天津"))
                                                                    {
                                                                        kehu = "百胜(天津)";
                                                                    }
                                                                    if (zdstr.Contains("北京"))
                                                                    {
                                                                        kehu = "百胜(北京)";
                                                                    }
                                                                    bsjh.KeHu = kehu;
                                                                }
                                                                else
                                                                {
                                                                    var arry = zdstr.Split(".", StringSplitOptions.RemoveEmptyEntries);
                                                                    if (arry.Length >= 2 && !ishave.Contains(arry[0]))
                                                                    {
                                                                        ishave.Add(arry[0]);
                                                                        string zm = arry[1].Trim();
                                                                        string fiststr = zm.Substring(0, 1);
                                                                        if (int.TryParse(fiststr, out int _))
                                                                        {
                                                                            zm = zm.Replace(fiststr, "");
                                                                        }
                                                                        string laststr = zm.Substring(zm.Length - 1, 1);
                                                                        if (int.TryParse(laststr, out int _))
                                                                        {
                                                                            zm = zm.Replace(laststr, "");
                                                                        }
                                                                        sb.Append("-" + zm);
                                                                    }

                                                                    #region 路线名称
                                                                    foreach (var cityTown in citylist)
                                                                    {
                                                                        var city = cityTown.CityName.Replace("市", "");
                                                                        var town = cityTown.TownName.Replace("市", "").Replace("区", "").Replace("自治县", "").Replace("县", "");
                                                                        if (city != "" && zdstr.Contains(city) && !ishave.Contains(city))
                                                                        {
                                                                            ishave.Add(city);
                                                                            lxsb.Append("/" + city);
                                                                        }
                                                                        if (town != "" && zdstr.Contains(town) && !ishave.Contains(town))
                                                                        {
                                                                            ishave.Add(town);
                                                                            lxsb.Append("/" + town);
                                                                        }
                                                                    }
                                                                    #endregion
                                                                }
                                                                #endregion
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (sb.Length > 0)
                            {
                                sb.Remove(0, 1);
                                bsjh.LuXian = sb.ToString();
                            }
                            if (lxsb.Length > 0)
                            {
                                lxsb.Remove(0, 1);
                                bsjh.LuXianName = lxsb.ToString();
                            }
                            bsjhs.Add(bsjh);
                        }
                    }
                    #endregion

                    XyunJh jh = new XyunJh { Bsjhs = bsjhs };
                    await ExportByTemplate(webHostEnvironment.WebRootPath + @"\JiHua\" + jihuafile, jh, webHostEnvironment);
                    try
                    {
                        DelectDir(pdfFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("删除文件异常:" + ex);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
        /// <summary>
        /// pdf生成xml
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        public static bool PdfToXMLAsFiles(string pdfFile)
        {
            try
            {
                string pathToXml = Path.ChangeExtension(pdfFile, ".xml");
                // Convert PDF file to XML file.
                PdfFocus f = new PdfFocus();

                // This property is necessary only for registered version.
                // f.Serial = "XXXXXXXXXXX";

                // Let's convert only tables to XML and skip all textual data.
                f.XmlOptions.ConvertNonTabularDataToSpreadsheet = false;

                f.OpenPdf(pdfFile);

                if (f.PageCount > 0)
                {
                    int result = f.ToXml(pathToXml);
                    if (result == 0)
                    {
                        //Show XML document in browser 选择直接打开
                        // Process.Start(new ProcessStartInfo(pathToXml) { UseShellExecute = true });
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        /// <summary>
        /// 使用模板导出excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="jh"></param>
        /// <param name="webHostEnvironment"></param>
        /// <returns></returns>
        public static async Task ExportByTemplate(string filePath, XyunJh jh, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                //模板路径
                string tplPath = webHostEnvironment.WebRootPath + @"\Template\Template.xlsx";
                //创建Excel导出对象 
                IExportFileByTemplate exporter = new ExcelExporter();
                //导出路径 
                if (File.Exists(filePath)) File.Delete(filePath);
                //根据模板导出 
                await exporter.ExportByTemplate(filePath, jh, tplPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 删除目录文件
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        /// <summary>
        /// 获取辽宁所有城市
        /// </summary>
        /// <returns></returns>
        public static List<CityTown> GetCityTownList(IWebHostEnvironment webHostEnvironment)
        {
            List<CityTown> data = new List<CityTown> { new CityTown { CityName = "长春", TownName = "" } };
            var path = webHostEnvironment.WebRootPath+@"\Template\CityTown.txt";
            foreach (string str in File.ReadAllLines(path))
            {
                var arry = str.Split("\t", StringSplitOptions.RemoveEmptyEntries);
                if (arry.Length == 2)
                {
                    data.Add(new CityTown
                    {
                        CityName = arry[1],
                        TownName = arry[0]
                    });
                }
            }
            return data;
        }

        /// <summary>
        /// 获取星期几
        /// </summary>
        /// <returns></returns>
        public static string GetWeek()
        {
            var week = (int) DateTime.Now.AddDays(1).DayOfWeek switch
            {
                0 => "日",
                1 => "一",
                2 => "二",
                3 => "三",
                4 => "四",
                5 => "五",
                _ => "六"
            };
            return week;
        }
    }
}
