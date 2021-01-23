using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Core.Script.Entities
{
    public class Script
    {
        #region ::Fields::

        private string _originalJson;
        private JPathCollection _jPaths;
        private DataTable _data;

        #endregion

        #region ::Constructors::

        public Script()
        {

        }

        #endregion

        #region ::Properties::

        public string OriginalJson
        {
            get
            {
                return _originalJson;
            }
            set
            {
                _originalJson = value;
            }
        }

        public JPathCollection JPaths
        {
            get
            {
                return _jPaths;
            }
            set
            {
                _jPaths = value;
            }
        }

        public DataTable Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        #endregion
    }
}
