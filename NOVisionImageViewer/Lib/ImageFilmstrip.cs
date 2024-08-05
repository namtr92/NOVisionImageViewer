using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NOVisionImageViewer.Lib
{
    public class ImageFilmstrip : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public static object image_loader = new object();
        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<string> _tags { get; set; } = new ObservableCollection<string>() { };
        public ObservableCollection<string> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                if (_tags != value)
                {
                    _tags = value;

                }
            }
        }
        public void AddTag(string tag)
        {
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                //RaisePropertyChanged("Tags");
            }
            
        }
        DateTime _created_time;
        public DateTime CreationTime
        {
            get
            {
                return _created_time;
            }
            set
            {
                if (_created_time != value)
                {
                    _created_time = value;
                    RaisePropertyChanged("CreationTime");
                }
            }
        }

        string _full_path;


        public string FullPath
        {
            get
            {
                return _full_path;
            }
            set
            {
                if (_full_path != value)
                {
                    _full_path = value;
                    RaisePropertyChanged("FullPath");
                }
            }
        }
        string file_name;
        public string FileName
        {
            get
            {
                return file_name;
            }
            set
            {
                if (file_name != value)
                {
                    file_name = value;
                    RaisePropertyChanged("FileName");
                }
            }
        }

        bool _is_loaded = true;
        public bool IsLoaded
        {
            get
            {
                return _is_loaded;
            }
            set
            {
                if (_is_loaded != value)
                {
                    _is_loaded = value;
                    RaisePropertyChanged("IsLoaded");
                }
            }
        }
        public string Tag { get; set; }


        public double Score { get; set; }
        private System.Windows.Media.ImageSource _image;
        public System.Windows.Media.ImageSource Image
        {
            get
            {
                if (IsLoaded)
                {
                    Task.Run(new Action(() =>
                    {
                        lock (image_loader)
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.DecodePixelHeight = 140;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(_full_path);
                            bitmap.EndInit();
                            bitmap.Freeze();
                            //Thread.Sleep(500);
                            IsLoaded = false;
                            Image = bitmap;

                        }
                    }));
                    return null;
                }

                return _image;

            }
            internal set
            {
                _image = value;
                RaisePropertyChanged("Image");
            }

        }
        public ImageFilmstrip(string FullPath)
        {
            this.FullPath = FullPath;
            try
            {
                FileName = System.IO.Path.GetFileName(FullPath);
                CreationTime = File.GetLastWriteTime(FullPath);
            }
            catch (Exception ex)
            {
                FileName = "Error File Name";
            }

        }
        public static BitmapImage Convert(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
