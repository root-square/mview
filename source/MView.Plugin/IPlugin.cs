using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Plugin
{
    public interface IPlugin
    {
        public void OnLoaded();

        public void OnUnloaded();
    }
}
