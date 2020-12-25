using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.File
{
    public class ImageFileViewModel : FileViewModelBase
    {
        public ImageFileViewModel(string filePath) : base(filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
