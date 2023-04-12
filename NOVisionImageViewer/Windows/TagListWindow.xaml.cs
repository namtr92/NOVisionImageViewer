using DevExpress.Xpf.Core;
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
    /// Interaction logic for TagListWindow.xaml
    /// </summary>
    public partial class TagListWindow : ThemedWindow
    {
        public TagListWindow()
        {
            InitializeComponent();
            lst_tag.ItemsSource = ViewModel.Instance.TagList;
        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Instance.TagList.Add(new NOVisionImageViewer.Tag() { Name="new",Color=Colors.Green});
        }

        private void btn_remove_tag_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button).DataContext as NOVisionImageViewer.Tag;
            if (selected != null)
            {
                ViewModel.Instance.TagList.Remove(selected);
            }
        }
    }
}
