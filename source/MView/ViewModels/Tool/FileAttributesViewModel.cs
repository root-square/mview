using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class FileAttributesViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "FileAttributes";

        #endregion

        #region ::Constructors::

        public FileAttributesViewModel() : base("File Attributes")
        {
            ContentId = ToolContentId;
        }

        #endregion
    }
}
