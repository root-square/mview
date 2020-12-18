using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class SaveDataManagerViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "SaveDataManager";

        #endregion

        #region ::Constructors::

        public SaveDataManagerViewModel() : base("Save Data Manager")
        {
            ContentId = ToolContentId;
        }

        #endregion
    }
}
