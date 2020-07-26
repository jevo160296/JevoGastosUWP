using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class DateToString:IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            DateTime valor = (DateTime)value;
            string converted= valor.ToShortDateString();
            return converted;
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            throw new NotImplementedException();
        }
    }
}
