using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities.Indexing
{
    public class IndexedItem
    {
        public IndexedItemType Type { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public long Size { get; set; }

        public bool IsSelected { get; set; }

        public IndexedItem? Parent { get; set; }

        public List<IndexedItem> SubItems { get; set; }

        public IndexedItem()
        {
            Name = string.Empty;
            FullName = string.Empty;
            Parent = null;
            SubItems = new List<IndexedItem>();
        }
    }
}
