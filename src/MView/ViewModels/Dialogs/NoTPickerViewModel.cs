using Caliburn.Micro;
using MView.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MView.ViewModels.Dialogs
{
    public class NoTPickerViewModel : Screen
    {
        private int _numberOfThreads = 0;

        public int NumberOfThreads
        {
            get => _numberOfThreads;
            set => Set(ref _numberOfThreads, value);
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }

        public async void Confirm(Window window)
        {
            if (window == null)
            {
                Log.Warning("The dialog shouldn't be null.");
                return;
            }

            await TryCloseAsync(true);
        }
    }
}
