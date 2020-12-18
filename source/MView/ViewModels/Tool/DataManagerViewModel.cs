using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class DataManagerViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "DataManager";

        #endregion

        #region ::Constructors::

        public DataManagerViewModel() : base("Data Manager")
        {
            ContentId = ToolContentId;
        }

        #endregion
    }
}
