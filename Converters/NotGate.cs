using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class NotGate:IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            Visibility valor = (Visibility)value;
            Visibility converted = (valor == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            return converted;
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }
}
