using MView.Audio.Visualization.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Audio.Visualization
{
    public class SpectrumAnalyzerVisualization : IVisualizationPlugin
    {
        private readonly SpectrumAnalyzerControl spectrumAnalyser = new SpectrumAnalyzerControl();

        public string Name => "Spectrum Analyzer";

        public object Content => spectrumAnalyser;

        public void OnMaxCalculated(float min, float max)
        {
            // nothing to do
        }

        public void OnFftCalculated(NAudio.Dsp.Complex[] result)
        {
            spectrumAnalyser.Update(result);
        }
    }
}
