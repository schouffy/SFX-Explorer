using SFXExplorer.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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
                        Play(((FileItem)_selectedItem).Path);
                    else
                        StatusMessage = "Press 'Space' to play the selected audio file";
                }
            }
        }

        private bool _autoplay = true;
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

        private string[] GetSanitizedQuery(string query)
        {
            return RemoveDiacritics(query).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private List<Item> GetFilteredItems(List<Item> items, string[] sanitizedQuery)
        {
            var filteredItems = new List<Item>();
            foreach (var item in items)
            {
                if (item is FileItem fileItem && fileItem.IsMatch(sanitizedQuery))
                {
                    filteredItems.Add(item);
                }
                if (item is DirectoryItem)
                {
                    filteredItems.AddRange(GetFilteredItems(((DirectoryItem)item).Items, sanitizedQuery));
                }
            }
            return filteredItems;
        }

        static string RemoveDiacritics(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public void TogglePlaySelected()
        {
            if (_selectedItem is FileItem)
            {
                Play(((FileItem)_selectedItem).Path);
            }
        }

        public void Filter(string query)
        {
            if (String.IsNullOrEmpty(query) || query.Length < 3)
                Items = _allItems;
            else
            {
                var simplifiedQuery = ItemProvider.GetSimplified(query);
                Items = GetFilteredItems(_allItems, GetSanitizedQuery(query));
            }
        }

        public void OpenSelectedFolder()
        {
			System.Diagnostics.Process.Start("explorer.exe", "/select, " + _selectedItem.Path);
		}

        public void StopPlayback()
        {
            KillPlayProcess();
        }

        System.Diagnostics.Process _process;
        System.Diagnostics.ProcessStartInfo _startInfo;
        void Play(String path)
        {
            // VLC:
            KillPlayProcess();

            _process = new System.Diagnostics.Process();
            _startInfo = new System.Diagnostics.ProcessStartInfo();
            _startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            _startInfo.FileName = "\"C:\\Program Files\\VideoLAN\\VLC\\vlc.exe\"";
            _startInfo.Arguments = $" --no-loop --no-repeat -I dummy --dummy-quiet \"{path}\" vlc://quit";
            _process.StartInfo = _startInfo;
            _process.Start();

            // Windows integrated reader:
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

        void KillPlayProcess()
        {
            if (_process != null && !_process.HasExited)
                _process.Kill();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            KillPlayProcess();
        }

        // TODO
        // search/filter (mvvm to avoid freeze + non-hammer + diacritics + several words)

        // Auto open last folder
        // load folder in other thread, with a progress prompt
        // integrated wav player / external program to use
        // right click => open folder, copy, copy name, copy full path
        // short list
        // show file duration (lazy load)
    }
}
