using JevoGastosCore;
using JevoGastosCore.Enums;
using JevoGastosCore.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using System.Collections.Specialized;
using JevoGastosCore.ModelView;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Input;
using Windows.System;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP.Forms
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class TransForm : Page
    {
        #region Clases
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
        public class Parameters
        {
            public GastosContainer Container { get; set; }
            public bool IsEditMode { get; set; }
            public Transaccion Transaccion { get; set; }

            public Parameters(GastosContainer container,bool isEditMode=false,Transaccion transaccion=null)
            {
                Container = container;
                IsEditMode = isEditMode;
                Transaccion = transaccion;
            }
        }
        #endregion
        #region Events
        public delegate void CloseRequestedHandler(Parameters parameters);
        public event CloseRequestedHandler CloseRequested;
        #endregion
        #region Atributos
        private VisibilityHandler TipoSelected = new VisibilityHandler();
        private VisibilityHandler TheresOrigenes = new VisibilityHandler();
        private VisibilityHandler TheresDestinos = new VisibilityHandler();
        private List<KeyboardAccelerator> CachedAccelerators=new List<KeyboardAccelerator>();
        private VirtualKey? lastpressed=null;
        public bool EditandoTrans { get; set; } = false;
        private Parameters parameters;
        private Dictionary<TipoTransaccion, int> TTransaccionesOrden = new Dictionary<TipoTransaccion, int>()
        {
            { TipoTransaccion.Entrada,0 },
            { TipoTransaccion.Movimiento,1 },
            { TipoTransaccion.Pago,3 },
            { TipoTransaccion.Prestamo,4 },
            { TipoTransaccion.Salida,2 }
        };
        #endregion
        #region Propiedades
        private ObservableCollection<Ingreso> Ingresos => parameters.Container.IngresoDAO.Items;
        private ObservableCollection<Cuenta> Cuentas => parameters.Container.CuentaDAO.Items;
        private ObservableCollection<Gasto> Gastos => parameters.Container.GastoDAO.Items;
        private ObservableCollection<Credito> Creditos => parameters.Container.CreditoDAO.Items;
        private ObservableCollection<Etiqueta> CuentasCreditos=new ObservableCollection<Etiqueta>();
        #endregion
        public TransForm()
        {
            this.InitializeComponent();
        }
        #region Inicialización
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            parameters = (Parameters)e.Parameter;
            parameters.Container.EtiquetaDAO.Items.CollectionChanged += Etiquetas_CollectionChanged;
            TipoSelected.PropertyChanged -= TipoSelected_PropertyChanged;
            TipoSelected.PropertyChanged += TipoSelected_PropertyChanged;
            CB_Tipo.ItemsSource = Enum.GetValues(typeof(TipoTransaccion)).Cast<TipoTransaccion>().OrderBy(p=>TTransaccionesOrden[p]).ToArray<TipoTransaccion>();
            ResetCuentasCreditos();
            if (parameters.IsEditMode)
            {
                Inicializar_EditMode();
            }
            else
            {
                Inicializar_AddMode();
            }
        }
        private void ResetCuentasCreditos()
        {
            CuentasCreditos.Clear();
            foreach (Etiqueta etiqueta in Enumerable.Concat<Etiqueta>(Cuentas,Creditos))
            {
                CuentasCreditos.Add(etiqueta);
            }
        }
        private void Inicializar_AddMode()
        {

        }
        private void Inicializar_EditMode()
        {
            TB_Title.Text = "Editar transacción";
            TipoTransaccion tipoTransaccion = TransaccionDAO.Tipo(parameters.Transaccion);
            CB_Tipo.SelectedItem = tipoTransaccion;
            CDP_Fecha.Date = parameters.Transaccion.Fecha;
            CB_Origen.SelectedItem = CB_Origen.Items.Where(p => ((Etiqueta)p).Id == (parameters.Transaccion.Origen).Id).First();
            CB_Destino.SelectedItem = CB_Destino.Items.Where(p => ((Etiqueta)p).Id == parameters.Transaccion.Destino.Id).First();
            TB_Valor.Value = parameters.Transaccion.Valor;
            TB_Descripcion.Text = parameters.Transaccion.Descripcion;
        }
        #endregion
        #region Click
        private void AppBarCancelTransButton_Click(object sender, RoutedEventArgs e)
        {
            EditandoTrans = false;
            CloseForm();
        }
        private void AddTransaccion_Click(object sender, RoutedEventArgs e)
        {
            ProcessTransaction();
        }
        private void ProcessTransaction()
        {
            double valor;
            EditandoTrans = false;
            valor = TB_Valor.Value;
            Etiqueta origen = CB_Origen.SelectedItem as Etiqueta;
            Etiqueta destino = CB_Destino.SelectedItem as Etiqueta;
            string descripcion = TB_Descripcion.Text;
            DateTime? date = CDP_Fecha.Date is null ? null : (DateTime?)CDP_Fecha.Date.Value.Date;
            if (parameters.IsEditMode)
            {
                EditTransaccion(valor, origen, destino, descripcion, date);
            }
            else
            {
                AddTransaccion(valor, origen, destino, descripcion, date);
            }
        }
        private void AddTransaccion(double valor,Etiqueta origen,Etiqueta destino,string descripcion,DateTime? date)
        {
            AddTransaccion(origen, destino, valor, descripcion, date);
            CB_Origen.SelectedItem = origen;
            CB_Destino.SelectedItem = destino;
            TB_Descripcion.Text = "";
            TB_Valor.Value = double.NaN;
            TB_Valor.Focus(FocusState.Programmatic);
        }
        private void EditTransaccion(double valor, Etiqueta origen, Etiqueta destino, string descripcion, DateTime? date)
        {
            EditTransaccion(origen, destino, valor, descripcion, date);
            CloseForm();
        }
        private void B_Destino_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_Tipo.SelectedIndex)
            {
                //Entrada
                case 0:
                //Movimiento
                case 1:
                //Prestamo
                case 3:
                    ShowEtiquetaAddForm((Button)sender, TipoEtiqueta.Cuenta);
                    break;
                //Salida
                case 2:
                    ShowEtiquetaAddForm((Button)sender, TipoEtiqueta.Gasto);
                    break;
                //Pago
                case 4:
                    ShowEtiquetaAddForm((Button)sender, TipoEtiqueta.Credito);
                    break;
                default:
                    break;
            }
        }
        private void B_Origen_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_Tipo.SelectedIndex)
            {
                //Entrada
                case 0:
                    ShowEtiquetaAddForm((Button)sender, TipoEtiqueta.Ingreso);
                    break;
                //Movimiento
                case 1:
                //Salida
                case 2:
                //Pago
                case 4:
                    ShowEtiquetaAddForm((Button)sender, TipoEtiqueta.Cuenta);
                    break;
                //Prestamo
                case 3:
                    ShowEtiquetaAddForm((Button)sender, TipoEtiqueta.Credito);
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region Changed
        private void TipoSelected_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((sender as VisibilityHandler).Visibility == Visibility.Collapsed)
            {
                SP_TransForm.Spacing = 0;
            }
            else
            {
                SP_TransForm.Spacing = 8;
            }
        }
        private void TB_Valor_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            TransFormDataValidation();
            if (lastpressed==VirtualKey.Enter)
            {
                ProcessTransaction();
            }
        }
        private void Etiquetas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CuentasCreditos.Count() != Cuentas.Count + Creditos.Count)
            {
                ResetCuentasCreditos();
            }
            CheckOrigenesDestinos();
        }
        private void CB_Origen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TransFormDataValidation();
        }
        private void CDP_Fecha_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            TransFormDataValidation();
        }
        private void CB_TipoSelected(object sender, SelectionChangedEventArgs e)
        {
            CheckTipo();
        }
        #endregion
        #region Metodos
        private void CloseForm()
        {
            if (!EditandoTrans)
            {
                ResetTransaccionForm();
                CB_Tipo.SelectedIndex = -1;
            }
            CheckTipo();
            CloseRequested?.Invoke(parameters);
        }
        private bool TransFormDataValidation()
        {
            bool
                fechaC,
                origenC,
                destinoC,
                valorC,
                correcto;
            fechaC = !(CDP_Fecha.Date is null);
            origenC = CB_Origen.SelectedIndex != -1;
            destinoC = CB_Destino.SelectedIndex != -1;
            valorC = !(TB_Valor.Value is double.NaN);
            correcto =
                 fechaC &
                 origenC &
                 destinoC &
                 valorC
                ;
            APB_GuardarTrans.IsEnabled = correcto;
            return correcto;
        }
        private void CheckTipo()
        {
            ComboBox cb = CB_Tipo;
            TipoTransaccion? item = cb.SelectedItem as TipoTransaccion?;
            if (item is null)
            {
                TipoSelected.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (TipoSelected.Visibility == Visibility.Collapsed)
                {
                    ResetTransaccionForm();
                    TransFormDataValidation();
                }
                EditandoTrans = true;
                TipoSelected.Visibility = Visibility.Visible;
                switch (item)
                {
                    case TipoTransaccion.Entrada:
                        CB_Origen.ItemsSource = Ingresos;
                        CB_Destino.ItemsSource = CuentasCreditos;
                        CB_Origen.PlaceholderText = "Ingresos";
                        CB_Destino.PlaceholderText = "Cuenta o Credito";
                        B_Origen.Content = "Nuevo Ingreso";
                        B_Destino.Content = "Nueva Cuenta";
                        break;
                    case TipoTransaccion.Movimiento:
                        CB_Origen.ItemsSource = Cuentas;
                        CB_Destino.ItemsSource = Cuentas;
                        CB_Origen.PlaceholderText = "Cuentas";
                        CB_Destino.PlaceholderText = "Cuentas";
                        B_Origen.Content = "Nueva Cuenta";
                        B_Destino.Content = "Nueva Cuenta";
                        break;
                    case TipoTransaccion.Salida:
                        CB_Origen.ItemsSource = CuentasCreditos;
                        CB_Destino.ItemsSource = Gastos;
                        CB_Origen.PlaceholderText = "Cuenta o Credito";
                        CB_Destino.PlaceholderText = "Gastos";
                        B_Origen.Content = "Nueva Cuenta";
                        B_Destino.Content = "Nuevo Gasto";
                        break;
                    case TipoTransaccion.Prestamo:
                        CB_Origen.ItemsSource = Creditos;
                        CB_Destino.ItemsSource = Cuentas;
                        CB_Origen.PlaceholderText = "Creditos";
                        CB_Destino.PlaceholderText = "Cuentas";
                        B_Origen.Content = "Nuevo credito";
                        B_Destino.Content = "Nueva cuenta";
                        break;
                    case TipoTransaccion.Pago:
                        CB_Origen.ItemsSource = Cuentas;
                        CB_Destino.ItemsSource = Creditos;
                        CB_Origen.PlaceholderText = "Cuentas";
                        CB_Destino.PlaceholderText = "Creditos";
                        B_Origen.Content = "Nueva cuenta";
                        B_Destino.Content = "Nuevo credito";
                        break;
                    default:
                        break;
                }
                CheckOrigenesDestinos();
            }
        }
        private void ResetTransaccionForm()
        {
            CDP_Fecha.Date = DateTime.Now;
            CB_Origen.SelectedIndex = -1;
            CB_Destino.SelectedIndex = -1;
            TB_Valor.Text = "";
            TB_Descripcion.Text = "";
        }
        private void CheckOrigenesDestinos()
        {
            TheresOrigenes.Visibility =
                (CB_Origen.Items.Count > 0) ?
                Visibility.Visible :
                Visibility.Collapsed;
            TheresDestinos.Visibility =
                (CB_Destino.Items.Count > 0) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }
        private void TransForm_CloseRequested()
        {
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(SP_TransForm);
            flyout.Hide();
        }
        private void ShowEtiquetaAddForm(Button sender, TipoEtiqueta tipoEtiqueta)
        {
            EtiquetaForm.Parameters parameters = new EtiquetaForm.Parameters(this.parameters.Container, tipoEtiqueta: tipoEtiqueta);
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(SP_TransForm);
            FlyoutShowOptions options = new FlyoutShowOptions()
            {
                ShowMode = FlyoutShowMode.Standard,
                Placement = FlyoutPlacementMode.TopEdgeAlignedLeft
            };
            flyout.ShowAt(sender, options);
            Frame_EtiquetaForm.Navigate(
                typeof(EtiquetaForm),
                parameters);
            ((EtiquetaForm)Frame_EtiquetaForm.Content).CloseRequested += TransForm_CloseRequested;
        }
        private void AddTransaccion(Etiqueta Origen, Etiqueta Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            if (!(Origen is null) & !(Destino is null))
            {
                parameters.Container.TransaccionDAO.Transaccion(Origen, Destino, valor, descripcion, dateTime);
            }
        }
        private void EditTransaccion(Etiqueta Origen, Etiqueta Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            if (!(Origen is null) & !(Destino is null))
            {
                parameters.Transaccion.Origen = Origen;
                parameters.Transaccion.Destino = Destino;
                parameters.Transaccion.Valor = valor;
                parameters.Transaccion.Descripcion = descripcion;
                parameters.Transaccion.Fecha = dateTime ?? DateTime.Now;
            }
        }
        private void TB_Valor_GotFocus(object sender, RoutedEventArgs e)
        {
            foreach (var accelerator in APB_GuardarTrans.KeyboardAccelerators)
            {
                CachedAccelerators.Add(accelerator);
            }
            APB_GuardarTrans.KeyboardAccelerators.Clear();
        }
        private void TB_Valor_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (var accelerator in CachedAccelerators)
            {
                APB_GuardarTrans.KeyboardAccelerators.Add(accelerator);
            }
            CachedAccelerators.Clear();
            lastpressed = null;
        }
        private void TB_Valor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            lastpressed = e.Key;
        }
        public void NotifyPageClosed()
        {
            if (!EditandoTrans)
            {
                ResetTransaccionForm();
                CB_Tipo.SelectedIndex = -1;
            }
        }
        #endregion


    }
}
