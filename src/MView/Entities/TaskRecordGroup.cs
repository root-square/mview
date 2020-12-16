using System.Collections.ObjectModel;

namespace MView.Entities
{
    public class TaskRecordGroup
    {
        public TaskRecordGroup(string header)
        {
            Header = header;
            Group = new ObservableCollection<TaskRecord>();
        }

        public string Header { get; set; }

        public ObservableCollection<TaskRecord> Group { get; set; }
    }
}
