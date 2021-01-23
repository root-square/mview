using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Plugin
{
    public interface IUserTranslator
    {
        public string Name { get; }

        public List<KeyValuePair<CultureInfo, CultureInfo>> TranslatableLanguagePairs { get; }

        public bool UseAsync { get; }

        public string Translate(CultureInfo source, CultureInfo target, string text);

        public Task<string> TranslateAsync(CultureInfo source, CultureInfo target, string text);
    }
}
