using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class DoubleToCurrency:IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            double valor = ((value as double?) is null)? 0:(double)value;
            string converted= valor.ToString("C", NumberFormatInfo.CurrentInfo);
            return converted;
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            throw new NotImplementedException();
        }
    }
}
