using MView.Bases;
using MView.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MView.ViewModels
{
    public class ToolHostViewModel : ViewModelBase
    {
        #region ::Fields::

        private string _title = string.Empty;
        private double _width = 0;
        private double _height = 0;

        private Page _toolPage = null;

        #endregion

        #region ::Constructors::

        public ToolHostViewModel(Page toolPage, double width = 300, double height = 300)
        {
            if (!string.IsNullOrEmpty(toolPage.Title))
            {
                _title = toolPage.Title;
            }

            _width = width;
            _height = height;

            _toolPage = toolPage;
        }

        #endregion

        #region ::Properties::

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public double Width
        {
            get
            {
                return _width;
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
        }

        public Page ToolPage
        {
            get
            {
                return _toolPage;
            }
        }

        #endregion
    }
}
