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
            foreach (var item in tagCounts)
            {
                list.Add(new TagSummary() { TagName = item.Key, Count = item.Value });
            }
            list.Add(new TagSummary() { TagName = "no tag", Count = noTagCount });
            // create a list of tuples with the tag name and time value for each occurrence
            List<(string TagName, DateTime Time)> tagTimeData = listImage
                .SelectMany(i => i.Tags.Select(t => (t, i.CreationTime.Date.AddHours(i.CreationTime.Hour ))))
                .ToList();
            // group the list by tag name and hour and count the occurrences
            var tagTime = tagTimeData
                .GroupBy(t => new { t.TagName, Hour = t.Time})
                .Select(g => new
                {
                    TagName = g.Key.TagName,
                    Hour = g.Key.Hour,
                    Count = g.Count()
                });
            //get no tag time line
            // create a list of tuples with the tag name and time value for each occurrence, including only ImageFilmStrip objects with empty TagNames list
            var notagTime = listImage
                .Where(i => !i.Tags.Any()) // include only objects with empty TagNames list
                .GroupBy(t => new { Hour = t.CreationTime.Date.AddHours(t.CreationTime.Hour) }).Select(g => new { TagName = "no tag", Hour = g.Key.Hour, Count = g.Count() });
            var timelineList = new List<TagSummaryTime>();


            foreach (var tag in list)
            {
                if(tag.TagName!="no tag")
                timelineList.Add(new TagSummaryTime() { Name = tag.TagName, Values = tagTime.Where(x => x.TagName == tag.TagName).Select(x => new TimeData() { Count = x.Count, Time = x.Hour }).ToList() });
            }
            timelineList.Add(new TagSummaryTime() { Name = "no tag", Values = notagTime.Select(x => new TimeData() { Count = x.Count, Time = x.Hour }).ToList() });
            chart.DataSource = list;
            chart2.DataSource = list;
            diagram_time.SeriesItemsSource = timelineList;
        }
    }
    public class TagSummaryTime
    {
        public List<TimeData> Values { get; set; }
        public string Name { get; set; }
    }
    public class TimeData
    {
        public DateTime Time { get; set; }
        public int Count { get; set; }

    }
    public class TagSummary
    {
        public int Count { get; set; }
        public string TagName { get; set; }
    }
}
