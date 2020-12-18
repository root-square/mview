using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Data
{
    public class SaveDataViewModel : FileViewModelBase
    {
        public SaveDataViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
