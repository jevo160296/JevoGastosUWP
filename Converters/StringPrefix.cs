using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class StringPrefix:IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            string valor = (value as string);
            string parametro = parameter is null ? "" : (parameter as string)+" ";
            string converted= parametro+valor;
            return converted;
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            throw new NotImplementedException();
        }
    }
}
