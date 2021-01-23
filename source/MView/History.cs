using MView.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MView
{
    public class History
    {
        #region ::Singleton Members::

        [NonSerialized]
        private static History _instance;

        public static History Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new History();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Fields::

        private List<TaskRecord> _taskRecordList = new List<TaskRecord>();

        #endregion

        #region ::Properties::

        public List<TaskRecord> TaskRecordList
        {
            get
            {
                return _taskRecordList;
            }
            set
            {
                _taskRecordList = value;
            }
        }

        #endregion
    }
}
