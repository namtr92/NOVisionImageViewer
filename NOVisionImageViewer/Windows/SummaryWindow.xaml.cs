using DevExpress.Xpf.Core;
using NOVisionImageViewer.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace NOVisionImageViewer.Windows
{
    /// <summary>
    /// Interaction logic for SummaryWindow.xaml
    /// </summary>
    public partial class SummaryWindow : ThemedWindow
    {
        public SummaryWindow()
        {
            InitializeComponent();
            LoadData();
        }
        void LoadData()
        {
            if(ViewModel.Instance.ListImageView == null)
            {
                return;
            }
            IEnumerable<ImageFilmstrip> listImage = ViewModel.Instance.ListImageView.Cast<ImageFilmstrip>();
            List<TagSummary> list = new List<TagSummary>();
            var noTagCount  = listImage.Where(x => x.Tags.Count == 0).Count();
            var haveTagList = listImage.Where(x => x.Tags.Count > 0);
            var tagCounts = listImage.SelectMany(td => td.Tags)
                                   .GroupBy(t => t)
                                   .ToDictionary(g => g.Key, g => g.Count());
            
            foreach(var item in tagCounts)
            {
                list.Add(new TagSummary() { TagName = item.Key, Count = item.Value });
            }
            list.Add(new TagSummary() { TagName = "no tag", Count = noTagCount });
            chart.DataSource = list;
            chart2.DataSource = list;
        }
    }
    public class TagSummary
    {
        public int Count { get; set; }
        public string TagName { get; set; }
    }
}
