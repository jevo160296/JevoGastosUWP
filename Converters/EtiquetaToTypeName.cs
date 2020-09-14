using JevoGastosCore.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JevoGastosUWP.Converters
{
    public class EtiquetaToTypeName : IValueConverter
    {
        public object Convert(object value,Type targetType,object parameter,string language)
        {
            Etiqueta valor = (Etiqueta)value;
            string converted = "??";
            if (valor is Cuenta)
            {
                converted = "Cuenta";
            }
            else if (valor is Ingreso)
            {
                converted = "Ingreso";
            }
            else if (valor is Gasto)
            {
                converted = "Gasto";
            }
            else if (valor is Credito)
            {
                converted = "Credito";
            }
            return converted;
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language)
        {
            throw new NotImplementedException();
        }
    }
}
