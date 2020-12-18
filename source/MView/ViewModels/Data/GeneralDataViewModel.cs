using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Data
{
    public class GeneralDataViewModel : FileViewModelBase
    {
        public GeneralDataViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
