using System.Collections.Generic;

namespace SFXExplorer.Model
{
    public class Item
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string SimplifiedPath { get; set; }
    }

    public class FileItem : Item
    {
        public bool IsMatch(string[] sanitizedQuery)
        {
            foreach (var word in sanitizedQuery)
            {
                if (!SimplifiedPath.Contains(word))
                    return false;
            }
            return true;
        }
    }

    public class DirectoryItem : Item
    {
        public List<Item> Items { get; set; }

        public DirectoryItem()
        {
            Items = new List<Item>();
        }
    }
}

