using NOVisionImageViewer.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsAPICodePack.Dialogs;

namespace NOVisionImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        public ViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel = ViewModel.Instance;
        }

        private void lst_view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        

        private void btn_select_directory_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeDirectory();
        }

        private void btn_edit_tag_list_Click(object sender, RoutedEventArgs e)
        {
            Windows.TagListWindow wd = new Windows.TagListWindow();
            wd.Owner = this;
            wd.Show();
        }

        private void lst_view_KeyDown(object sender, KeyEventArgs e)
        {
            var countSelected = e.Key - Key.D1;
            if (countSelected >= 0)
            {
                if (ViewModel.Instance.TagList.Count > countSelected)
                {
                    //if (!ViewModel.Instance.SelectedImage.Tags.Contains(ViewModel.Instance.TagList[countSelected].Name)){
                    //    ViewModel.Instance.SelectedImage.Tags.Add(ViewModel.Instance.TagList[countSelected].Name);
                    //}
                   
                    if (!ViewModel.Instance.SelectedImage.Tags.Contains(ViewModel.Instance.TagList[countSelected].Name)){
                        ViewModel.Instance.AddTag(ViewModel.Instance.TagList[countSelected].Name);
                        ViewModel.Instance.Message = "Add tag " + ViewModel.Instance.TagList[countSelected].Name + " to file: " + ViewModel.Instance.SelectedImage.FileName;
                        ViewModel.Instance.RenderSelectedImage();
                    }
                    else
                    {
                        ViewModel.Instance.SelectedImage.Tags.Remove(ViewModel.Instance.TagList[countSelected].Name);
                        ViewModel.Instance.Message = "Remove tag " + ViewModel.Instance.TagList[countSelected].Name + " from file: " + ViewModel.Instance.SelectedImage.FileName;
                        ViewModel.Instance.RenderSelectedImage();
                    }
                    
                }

            }
        }

        private void btn_remove_tag_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button).DataContext;
            if (selected != null)
            {
                ViewModel.Instance.SelectedImage.Tags.Remove(selected as string);
                ViewModel.Instance.RenderSelectedImage();
                ViewModel.Instance.Message = "Remove tag " + selected + " from file: " + ViewModel.Instance.SelectedImage.FileName;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.SaveMetaData();
        }

        private void btn_filter_tag_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.FilterTag();
        }

        private void btn_filter_empty_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.ShowEmptyTag();
        }

        private void btn_reset_filter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.ResetFilter();
        }

        private void btn_show_chart_Click(object sender, RoutedEventArgs e)
        {
            Windows.SummaryWindow wd = new Windows.SummaryWindow();
            wd.Show();
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            lst_view.SelectedIndex++;
            lst_view.ScrollIntoView(lst_view.SelectedItem);
            lst_view.Focus();
        }

        private void btn_pre_Click(object sender, RoutedEventArgs e)
        {
            lst_view.SelectedIndex = Math.Max(-1, lst_view.SelectedIndex-1);
            lst_view.ScrollIntoView(lst_view.SelectedItem);
            lst_view.Focus();
        }

        private void btn_filter_time_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.FilterTime();
        }
    }
    public class ViewModel:INotifyPropertyChanged
    {
        static ViewModel _instance = new ViewModel();
        public static ViewModel Instance
        {
            get
            {
                return _instance;
            }
        }
        private ViewModel()
        {

        }
        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                if (_is_loading != value)
                {
                    _is_loading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged("Message");
                }
            }
        }
        string _selected_directory;
        public string SelectedDirectory
        {
            get
            {
                return _selected_directory;
            }
            set
            {
                if (_selected_directory != value)
                {
                    _selected_directory = value;
                    RaisePropertyChanged("SelectedDirectory");
                }
            }
        }
        List<Tag> _current_tag;
        public List<Tag> CurrentTag
        {
            get
            {
                return _current_tag;
            }
            set
            {
                if (_current_tag != value)
                {
                    _current_tag = value;
                    RaisePropertyChanged("CurrentTag");
                }
            }
        }

        public ObservableCollection<Tag> TagList { get; set; } = new ObservableCollection<Tag>();
        List<ImageFilmstrip> _list_image;
        public List<ImageFilmstrip> ListImage
        {
            get
            {
                return _list_image;
            }
            set
            {
                if (_list_image != value)
                {
                    _list_image = value;
                    RaisePropertyChanged("ListImage");
                }
            }
        }
        ICollectionView _list_image_view;
        public ICollectionView ListImageView
        {
            get
            {
                return _list_image_view;
            }
            set
            {
                if (_list_image_view != value)
                {
                    _list_image_view = value;
                    RaisePropertyChanged("ListImageView");
                }
            }
        }
        DateTime _time_start=DateTime.Now;
        public DateTime TimeStart
        {
            get
            {
                return _time_start;
            }
            set
            {
                if (_time_start != value)
                {
                    _time_start = value;
                    RaisePropertyChanged("TimeStart");
                }
            }
        }
        DateTime _time_stop= DateTime.Now;
        public DateTime TimeStop
        {
            get
            {
                return _time_stop;
            }
            set
            {
                if (_time_stop != value)
                {
                    _time_stop = value;
                    RaisePropertyChanged("TimeStop");
                }
            }
        }


        string _text_filter="";
        public string TextFilter
        {
            get
            {
                return _text_filter;
            }
            set
            {
                if (_text_filter != value)
                {
                    _text_filter = value;
                    RaisePropertyChanged("TextFilter");
                }
            }
        }

        BitmapImage _image_original;
        public BitmapImage ImageOriginal
        {
            get
            {
                return _image_original;
            }
            set
            {
                if (_image_original != value)
                {
                    _image_original = value;
                    RaisePropertyChanged("ImageOriginal");
                }
            }
        }
        BitmapImage _image_screenshot;
        public BitmapImage ImageScreenshot
        {
            get
            {
                return _image_screenshot;
            }
            set
            {
                if (_image_screenshot != value)
                {
                    _image_screenshot = value;
                    RaisePropertyChanged("ImageScreenshot");
                }
            }
        }
        ImageFilmstrip _selected_image;
        public ImageFilmstrip SelectedImage
        {
            get
            {
                return _selected_image;
            }
            set
            {
                if (_selected_image != value)
                {
                    _selected_image = value;
                    ChangeImage(value);
                    RaisePropertyChanged("SelectedImage");
                }
            }
        }
        public void AddTag(string tag)
        {
            SelectedImage.AddTag(tag);
            //RaisePropertyChanged("SelectedImage");
        }

        BitmapImage ReadImage(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        public void ChangeImage(ImageFilmstrip selected)
        {
            if (selected != null)
            {
                Task.Run(() =>
                {
                    try
                    {

                        ImageOriginal = ReadImage(selected.FullPath);
                        var screenshotpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(selected.FullPath), "screenshot", System.IO.Path.GetFileName(selected.FullPath));
                        if (System.IO.File.Exists(screenshotpath))
                        {
                            ImageScreenshot = ReadImage(screenshotpath);
                        }
                        else
                        {
                            ImageScreenshot = null;
                        }
                        RenderTag(selected);

                    }
                    catch (Exception ex)
                    {

                    }

                });

            }
        }
        public void RenderSelectedImage()
        {
            RenderTag(SelectedImage);
        }
        public void RenderTag(ImageFilmstrip selected)
        {
            var tags = new List<Tag>();
            foreach (var item in TagList)
            {
                if (selected.Tags.Contains(item.Name))
                {
                    tags.Add(new Tag() { Name = item.Name, Color = item.Color });
                }
            }
            CurrentTag = tags;
        }

        public void ChangeDirectory()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            //diag.folder
            // diag.SelectedPath = acq.Record_path;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                int count = 0;
                var lst_images = new List<ImageFilmstrip>();
                string[] files = Directory.GetFiles(dialog.FileName, "*.bmp").Union(Directory.GetFiles(dialog.FileName, "*.jpg")).Union(Directory.GetFiles(dialog.FileName, "*.png")).Union(Directory.GetFiles(dialog.FileName, "*.tif")).ToArray();
                foreach (string file in files)
                {

                    lst_images.Add(new ImageFilmstrip(file) { ID= count });
                    count++;
                }
                ListImage = lst_images;
                ListImageView = CollectionViewSource.GetDefaultView(ListImage);
                ListImageView.Filter = TagFilter;
                SelectedDirectory = dialog.FileName;
                MetadataFile = System.IO.Path.Combine(SelectedDirectory, "data.novision.csv");
                LoadMetaData();
            }
        }
        private bool EmptyTagFilter(object item)
        {
            ImageFilmstrip selected = (ImageFilmstrip)item;
            return selected.Tags.Count==0;
        }
        private bool TagFilter(object item)
        {
            if (TextFilter == "")
            {
                return true;
            }
            ImageFilmstrip selected = (ImageFilmstrip)item;
            return selected.Tags.Contains(TextFilter);
        }
        private bool TagTime(object item)
        {
            if (TimeStart == null | TimeStop==null)
            {
                return true;
            }
            ImageFilmstrip selected = (ImageFilmstrip)item;
            return selected.CreationTime>=TimeStart & selected.CreationTime<=TimeStop;
        }
        public void ShowEmptyTag()
        {
            if (ListImageView == null)
            {
                return;
            }
            ListImageView.Filter = EmptyTagFilter;
            ListImageView?.Refresh();
        }
        public void ResetFilter()
        {
            if (ListImageView == null)
            {
                return;
            }
            ListImageView.Filter = null;
            ListImageView.Refresh();
        }
        public void FilterTag()
        {
            if (ListImageView == null)
            {
                return;
            }
            ListImageView.Filter = TagFilter;
            ListImageView.Refresh();
        }
        public void FilterTime()
        {
            if (ListImageView == null)
            {
                return;
            }
            ListImageView.Filter = TagTime;
            ListImageView.Refresh();
        }
        public void SaveMetaData()
        {
            Task.Run(() =>
            {
                IsLoading = true;
                try
                {
                    var csv = new StringBuilder();
                    foreach (var item in ListImage)
                    {
                        csv.AppendLine(string.Format("{0},{1}", item.FileName, string.Join("\t", item.Tags)));
                    }
                    File.WriteAllText(MetadataFile, csv.ToString());
                }
                catch (Exception ex)
                {

                }
                IsLoading = false;
            });
            
        }
        public void LoadMetaData()
        {
            Task.Run(() =>
            {
                IsLoading = true;
                try
                {
                    var lines = File.ReadAllLines(MetadataFile);
                    char c = '\t';
                    foreach (var line in lines)
                    {
                        string[] data = line.Split(',');
                        var selected = ListImage.FirstOrDefault(x => x.FileName == data[0]);
                        if (selected != null)
                        {
                            foreach (string tag in data[1].Split(c))
                            {
                                if (tag != "")
                                {
                                    selected.Tags.Add(tag);
                                }

                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                IsLoading = false;
            });
            
        }
        
        public string MetadataFile { get; set; }
    }
    public class Tag
    {
        public Color Color { get; set; }
        public string Name { get; set; }
    }  
}
