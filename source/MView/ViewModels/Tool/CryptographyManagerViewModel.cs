using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Tool
{
    public class CryptographyManagerViewModel: ToolViewModelBase
    {
        #region ::Fields::

        public const string ToolContentId = "CryptographyManager";

        #endregion

        #region ::Constructors::

        public CryptographyManagerViewModel() : base("Cryptography Manager")
        {
            ContentId = ToolContentId;
        }

        #endregion
    }
}
