using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.File
{
    public class GeneralFileViewModel : FileViewModelBase
    {
        public GeneralFileViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
