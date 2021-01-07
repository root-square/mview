using MView.Utilities.Audio.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities.Audio
{
    public class SpectrumAnalyzerVisualization : IVisualizationPlugin
    {
        private readonly SpectrumAnalyzerControl spectrumAnalyser = new SpectrumAnalyzerControl();

        public string Name => "Spectrum Analyser";

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
