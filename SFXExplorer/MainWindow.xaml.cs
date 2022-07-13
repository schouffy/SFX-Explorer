using SFXExplorer.Model;
using SFXExplorer.Mvvm;
using System;
using System.Collections.Generic;
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
using WinForms = System.Windows.Forms;

namespace SFXExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppViewModel _appViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _appViewModel = new AppViewModel();
            DataContext = _appViewModel;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _appViewModel.SelectedItem = (Item)e.NewValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new WinForms.FolderBrowserDialog();

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                _appViewModel.FolderPath = dialog.SelectedPath;
                _appViewModel.Initialize();
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = ((TextBox)e.Source).Text;
            _appViewModel.Filter(query);
        }

        private void TreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                _appViewModel.PlaySelectedSound();
            }
        }


        private Point start;
        private void TreeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
                this.start = new Point(0,0);
            else
                this.start = e.GetPosition(null);
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mpos = e.GetPosition(null);

            if (this.start.X == 0 && this.start.Y == 0)
                return;

            Vector diff = this.start - mpos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (_appViewModel.SelectedItem == null)
                    return;

                DataObject dataObject = new DataObject(DataFormats.FileDrop, new string[] { _appViewModel.SelectedItem.Path });
                DragDrop.DoDragDrop((TreeView)sender, dataObject, DragDropEffects.Copy);
            }
        }
    }
}
