using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class BingModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string STitle { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 保存日期
        /// </summary>
        public DateTime Date { get; set; }
    }

    public class Walle
    {
        public List<Images> images { get; set; }
        public Tooltips tooltips { get; set; }
    }

    public class Images
    {
        public string startdate { get; set; }
        public string fullstartdate { get; set; }
        public string enddate { get; set; }
        public string url { get; set; }
        public string urlbase { get; set; }
        public string copyright { get; set; }
        public string copyrightlink { get; set; }
        public string title { get; set; }
        public string quiz { get; set; }
        public bool wp { get; set; }
        public string hsh { get; set; }
        public int drk { get; set; }
        public int top { get; set; }
        public int bot { get; set; }
        public List<int> hs { get; set; }
    }

    public class Tooltips
    {
        public string loading { get; set; }
        public string previous { get; set; }
        public string next { get; set; }
        public string walle { get; set; }
        public string walls { get; set; }
    }
}
