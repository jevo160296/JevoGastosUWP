using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class NullTo0:IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            double valor = (double)value;
            double converted= valor;
            return converted;
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            double valor = (double)value;
            double converted = double.IsNaN(valor) ? 0 : valor;
            return converted;
        }
    }
}
