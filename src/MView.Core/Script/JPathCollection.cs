using System.Collections.Generic;

namespace MView.Core.Script
{
    public class JPathCollection
    {
        public string Header { get; set; }

        public List<string> Paths { get; set; }

        public JPathCollection(string header)
        {
            Header = header;
            Paths = new List<string>();
        }
    }
}
