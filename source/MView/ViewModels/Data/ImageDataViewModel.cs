using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Data
{
    public class ImageDataViewModel : FileViewModelBase
    {
        public ImageDataViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
