using MahApps.Metro.IconPacks;
using System;
using System.Windows.Media;

namespace MView.Entities
{
    public class TaskRecord
    {
        public TaskRecord(TaskType type, string details)
        {
            Type = type;
            DateTime = DateTime.Now;
            Details = details;

            if (type == TaskType.Data)
            {
                IconKind = PackIconMaterialKind.FileDocument;
            }
            else if (type == TaskType.Decrypt || type == TaskType.Encrypt)
            {
                IconKind = PackIconMaterialKind.FileKey;
            }
            else if (type == TaskType.SaveData)
            {
                IconKind = PackIconMaterialKind.ContentSaveEdit;
            }
            else if (type == TaskType.Script)
            {
                IconKind = PackIconMaterialKind.ScriptText;
            }
        }

        public TaskType Type { get; set; }

        public PackIconMaterialKind IconKind { get; set; }

        public DateTime DateTime { get; set; }

        public string Details { get; set; }
    }
}
