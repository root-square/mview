using MView.Bases;
using MView.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MView.ViewModels.File
{
    public class GeneralFileViewModel : FileViewModelBase
    {
        #region ::Fields::

        private string _textContent = string.Empty;

        #endregion

        #region ::Constructors::

        public GeneralFileViewModel(string filePath) : base(filePath)
        {
            Initialize(filePath);
        }

        #endregion

        #region ::Methods::

        private async void Initialize(string filePath)
        {
            var task = Task.Run(() =>
            {
                _textContent = FileManager.ReadTextFile(filePath, Encoding.UTF8);
            });

            await task;
        }

        #endregion

        #region ::Properties::

        public string TextContent
        {
            get
            {
                return _textContent;
            }
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                    RaisePropertyChanged(nameof(TextContent));

                    IsDirty = true;
                }
            }
        }

        #endregion
    }
}
