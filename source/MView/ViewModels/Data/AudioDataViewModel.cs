using MView.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView.ViewModels.Data
{
    public class AudioDataViewModel : FileViewModelBase
    {
        public AudioDataViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }
    }
}
