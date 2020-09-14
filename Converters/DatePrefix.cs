using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class DatePrefix:IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            DateToString dateToString = new DateToString();
            StringPrefix stringPrefix = new StringPrefix();
            string converted=(string)stringPrefix
                .Convert((string)dateToString
                .Convert(value,typeof(string),null,language),
                typeof(string),parameter,language);
            return converted;
        }

        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            throw new NotImplementedException();
        }
    }
}
