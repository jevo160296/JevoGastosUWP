using JevoGastosCore;
using JevoGastosCore.Model;
using JevoGastosCore.ModelView;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace JevoGastosUWP
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private class VisibilityHandler : INotifyPropertyChanged
        {
            private Visibility visibility = Visibility.Collapsed;
            public Visibility Visibility
            {
                get
                {
                    return visibility;
                }
                set
                {
                    visibility = value;
                    OnPropertyChanged();
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        private ObservableCollection<Transaccion> relatedTrans = new ObservableCollection<Transaccion>();

        private GastosContainer Container;
        private ObservableCollection<Etiqueta> Etiquetas => Container.EtiquetaDAO.Items;
        private ObservableCollection<Ingreso> Ingresos => Container.IngresoDAO.Items;
        private ObservableCollection<Cuenta> Cuentas => Container.CuentaDAO.Items;
        private ObservableCollection<Gasto> Gastos => Container.GastoDAO.Items;
        private ObservableCollection<Transaccion> Transacciones => Container.TransaccionDAO.Items;
        private ObservableCollection<Transaccion> RelatedTrans => relatedTrans;
        private VisibilityHandler ErrorVisibility = new VisibilityHandler();
        private VisibilityHandler TipoSelected = new VisibilityHandler();

        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Container = (GastosContainer)e.Parameter;
            ConfigureView();
        }
        private void ConfigureView()
        {
        }
        private void AddIngreso(string name)
        {
            Container.IngresoDAO.Add(name);
        }
        private void AddCuenta(string name)
        {
            Container.CuentaDAO.Add(name);
        }
        private void AddGasto(string name)
        {
            Container.GastoDAO.Add(name);
        }
        private void AddEntrada(Ingreso Origen, Cuenta Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            AddTransaccion(Origen, Destino, valor, descripcion, dateTime);
        }
        private void AddMovimiento(Cuenta Origen, Cuenta Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            AddTransaccion(Origen, Destino, valor, descripcion, dateTime);
        }
        private void AddSalida(Cuenta Origen, Gasto Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            AddTransaccion(Origen, Destino, valor, descripcion, dateTime);
        }
        private void AddTransaccion(Etiqueta Origen, Etiqueta Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            if (!(Origen is null) & !(Destino is null))
            {
                Container.TransaccionDAO.Transaccion(Origen, Destino, valor, descripcion, dateTime);
            }
        }

        private void DelIngreso()
        {
            if (Ingresos.Count > 0)
            {
                try
                {
                    EtiquetaDAO.Delete(Ingresos[0], Container);
                    CouldDeleteEtiqueta();
                }
                catch (System.Exception)
                {
                    CantDeleteEtiqueta<Ingreso>(Ingresos[0]);
                }
            }
        }
        private void DelCuenta()
        {
            if (Cuentas.Count > 0)
            {
                try
                {
                    EtiquetaDAO.Delete(Cuentas[0], Container);
                    CouldDeleteEtiqueta();
                }
                catch (System.Exception)
                {
                    CantDeleteEtiqueta<Cuenta>(Cuentas[0]);
                }
            }
        }
        private void DelGasto()
        {
            if (Gastos.Count > 0)
            {
                try
                {
                    EtiquetaDAO.Delete(Gastos[0], Container);
                    CouldDeleteEtiqueta();
                }
                catch (System.Exception)
                {
                    CantDeleteEtiqueta<Gasto>(Gastos[0]);
                }
            }
        }
        private void CantDeleteEtiqueta<T>(T etiqueta)
            where T : Etiqueta
        {
            RelatedTrans.Clear();
            if (!(etiqueta.TransaccionesOrigen is null))
            {
                foreach (var item in etiqueta.TransaccionesOrigen)
                {
                    RelatedTrans.Add(item);
                }
            }
            if (!(etiqueta.TransaccionesDestino is null))
            {
                foreach (var item in etiqueta.TransaccionesDestino)
                {
                    RelatedTrans.Add(item);
                }
            }
            Transacciones.CollectionChanged -= Transacciones_CollectionChanged;
            Transacciones.CollectionChanged += Transacciones_CollectionChanged;
            ErrorVisibility.Visibility = Visibility.Visible;
        }

        private void Transacciones_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        RelatedTrans.Add((Transaccion)item);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        RelatedTrans.Remove((Transaccion)item);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    RelatedTrans.Clear();
                    break;
                default:
                    break;
            }
        }

        private void CouldDeleteEtiqueta()
        {
            ErrorVisibility.Visibility = Visibility.Collapsed;
        }

        private void DelTransaccion()
        {
            if (Transacciones.Count > 0)
            {
                Container.TransaccionDAO.Remove(Transacciones[0]);
            }
        }

        private void AddIngreso_Click(object sender, RoutedEventArgs e)
        {
            AddIngreso(TB_NuevoIngreso.Text);
        }
        private void AddCuenta_Click(object sender, RoutedEventArgs e)
        {
            AddCuenta(TB_NuevaCuenta.Text);
        }
        private void AddGasto_Click(object sender, RoutedEventArgs e)
        {
            AddGasto(TB_NuevoGasto.Text);
        }
        private void AddTransaccion_Click(object sender, RoutedEventArgs e)
        {
            AddTransaccion(CB_Origen.SelectedItem as Etiqueta, CB_Destino.SelectedItem as Etiqueta, double.Parse(TB_Valor.Text), TB_Descripcion.Text, CDP_Fecha.Date.Value.Date);
        }

        private void DIngreso_Click(object sender, RoutedEventArgs e)
        {
            DelIngreso();
        }
        private void DCuenta_Click(object sender, RoutedEventArgs e)
        {
            DelCuenta();
        }
        private void DGasto_Click(object sender, RoutedEventArgs e)
        {
            DelGasto();
        }
        private void DTransaccion_Click(object sender, RoutedEventArgs e)
        {
            DelTransaccion();
        }

        private void CB_TipoSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int index = cb.SelectedIndex;
            if (index == -1)
            {
                TipoSelected.Visibility = Visibility.Collapsed;
            }
            else
            {
                TipoSelected.Visibility = Visibility.Visible;
                switch (index)
                {
                    //Entrada
                    case 0:
                        CB_Origen.ItemsSource = Ingresos;
                        CB_Destino.ItemsSource = Cuentas;
                        break;
                    //Movimiento
                    case 1:
                        CB_Origen.ItemsSource = Cuentas;
                        CB_Destino.ItemsSource = Cuentas;
                        break;
                    //Salida
                    case 2:
                        CB_Origen.ItemsSource = Cuentas;
                        CB_Destino.ItemsSource = Gastos;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
