using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Data
{
    public class JsonDataViewModel : FileViewModelBase
    {
        public JsonDataViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
