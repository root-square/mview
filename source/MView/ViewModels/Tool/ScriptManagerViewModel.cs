using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class ScriptManagerViewModel : ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "ScriptManager";

        #endregion

        #region ::Constructors::

        public ScriptManagerViewModel() : base("Script Manager")
        {
            ContentId = ToolContentId;
        }

        #endregion
    }
}
