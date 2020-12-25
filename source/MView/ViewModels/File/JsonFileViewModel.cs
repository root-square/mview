using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.File
{
    public class JsonFileViewModel : FileViewModelBase
    {
        public JsonFileViewModel(string filePath) : base(filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
