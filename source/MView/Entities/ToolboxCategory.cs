using System;
using System.Collections.Generic;
using System.Text;

namespace MView.Entities
{
    public class ToolboxCategory
    {
        #region ::Constructors::

        public ToolboxCategory(string name, bool isExpanded = false)
        {
            Name = name;
            IsExpanded = isExpanded;
            SubItems = new List<ToolboxItem>();
        }

        #endregion

        #region ::Properties::

        public string Name { get; set; }

        public bool IsExpanded { get; set; }

        public List<ToolboxItem> SubItems { get; set; }

        #endregion
    }
}
