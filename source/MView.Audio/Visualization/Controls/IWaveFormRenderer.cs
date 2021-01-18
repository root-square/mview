using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Audio.Visualization.Controls
{
    public interface IWaveFormRenderer
    {
        void AddValue(float maxValue, float minValue);
    }
}
