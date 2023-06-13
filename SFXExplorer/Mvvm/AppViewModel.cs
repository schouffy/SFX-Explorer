using SFXExplorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFXExplorer.Mvvm
{
    public class AppViewModel : ViewModelBase
    {
        public String FolderPath { get; set; }

        private List<Item> _allItems;
        private List<Item> _items;
        public List<Item> Items
        {
            get { return _items; }
            private set { _items = value; OnPropertyChanged(nameof(Items)); }
        }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set {
                _selectedItem = value;
                OnPropertyChanged(nameof(Items));

                if (_selectedItem is FileItem)
                {
                    if (Autoplay)
                        PlaySound((FileItem)_selectedItem);
                    else
                        StatusMessage = "Press 'Space' to play the selected audio file";
                }
            }
        }

        private bool _autoplay;
        public bool Autoplay
        {
            get { return _autoplay; }
            set { _autoplay = value; OnPropertyChanged(nameof(Autoplay)); }
        }

        private String _statusMessage;
        public String StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        public AppViewModel()
        {
            StatusMessage = "Select a root folder containing audio files";
        }

        public void Initialize()
        {
            var itemProvider = new ItemProvider();
            _allItems = itemProvider.GetItems(FolderPath);
            Items = _allItems;

            StatusMessage = "Press 'Space' to play the selected audio file";
        }

        private List<Item> GetFilteredItems(List<Item> items, string query)
        {
            var filteredItems = new List<Item>();
            foreach (var item in items)
            {
                if (item is FileItem && item.SimplifiedPath.Contains(query))
                {
                    filteredItems.Add(item);
                }
                if (item is DirectoryItem)
                {
                    filteredItems.AddRange(GetFilteredItems(((DirectoryItem)item).Items, query));
                }
            }
            return filteredItems;
        }

        public void PlaySelectedSound()
        {
            if (_selectedItem is FileItem)
            {
                PlaySound((FileItem)_selectedItem);
            }
        }

        public  void Filter(string query)
        {
            if (String.IsNullOrEmpty(query) || query.Length < 3)
                Items = _allItems;
            else
            {
                var simplifiedQuery = ItemProvider.GetSimplified(query);
                Items = GetFilteredItems(_allItems, simplifiedQuery);
            }
        }

        public void OpenSelectedFolder()
        {
			System.Diagnostics.Process.Start("explorer.exe", "/select, " + _selectedItem.Path);
		}

        System.Diagnostics.Process _process;
        System.Diagnostics.ProcessStartInfo _startInfo;
		void PlaySound(FileItem fileItem)
        {
            if (_process != null && !_process.HasExited)
                _process.Kill();

			_process = new System.Diagnostics.Process();
			_startInfo = new System.Diagnostics.ProcessStartInfo();
			_startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			_startInfo.FileName = "\"C:\\Program Files\\VideoLAN\\VLC\\vlc.exe\"";
			_startInfo.Arguments = $" --no-loop --no-repeat -I dummy --dummy-quiet \"{fileItem.Path}\" vlc://quit"; 
			_process.StartInfo = _startInfo;
			_process.Start();

			//try
			//{
			//    if (fileItem.Path.EndsWith(".wav"))
			//    {
			//        using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(fileItem.Path))
			//        {
			//            player.Play();
			//        }
			//    }
			//    else
			//    {
			//        StatusMessage = "Error: This file format is not supported";
			//    }
			//}
			//catch (Exception e)
			//{
			//    StatusMessage = "Error: " + e.Message;
			//}
		}
    }
}
