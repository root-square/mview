using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.File
{
    public class ScriptFileViewModel : FileViewModelBase
    {
        public ScriptFileViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
