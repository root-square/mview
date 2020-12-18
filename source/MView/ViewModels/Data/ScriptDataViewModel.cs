using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Data
{
    public class ScriptDataViewModel : FileViewModelBase
    {
        public ScriptDataViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
