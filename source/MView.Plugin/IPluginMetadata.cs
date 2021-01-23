using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Plugin
{
    public interface IPluginMetadata
    {
        public string Name { get; }

        public string Author { get; }

        public string Description { get; }

        public string Copyright { get; }

        public string License { get; }

        public string WebSite { get; }
    }
}
