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
using System.Windows.Threading;
using WinForms = System.Windows.Forms;

namespace SFXExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppViewModel _appViewModel;

        private DispatcherTimer _searchUpdate;

        public MainWindow()
        {
            InitializeComponent();
            _appViewModel = new AppViewModel();
            DataContext = _appViewModel;

            Closed += MainWindow_Closed;

            _searchUpdate = new DispatcherTimer();
            _searchUpdate.Interval = TimeSpan.FromMilliseconds(250);
            _searchUpdate.Tick += SearchUpdate_Tick;
            _searchUpdate.Start();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _appViewModel.Dispose();

            _searchUpdate.Tick -= SearchUpdate_Tick;
            _searchUpdate.Stop();
            _searchUpdate = null;
        }

        private void SearchUpdate_Tick(object? sender, EventArgs e)
        {
            if (_lastInput != DateTime.MinValue && _lastInput.Add(TimeSpan.FromMilliseconds(350)) < DateTime.Now)
            {
                _appViewModel.Filter(_query);
                _lastInput = DateTime.MinValue;
            }
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

        string _query;
        DateTime _lastInput = DateTime.MinValue;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _lastInput = DateTime.Now;
            _query = ((TextBox)e.Source).Text;
        }

        private void TreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                _appViewModel.TogglePlaySelected();
            }
            if (e.Key == Key.O)
            {
				_appViewModel.OpenSelectedFolder();
			}
            if (e.Key == Key.S)
            {
                _appViewModel.StopPlayback();
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
