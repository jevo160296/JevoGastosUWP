﻿using JevoGastosCore;
using JevoGastosCore.Model;
using JevoGastosCore.ModelView;
using JevoGastosUWP.ControlesPersonalizados;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

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
        private VisibilityHandler TheresOrigenes = new VisibilityHandler();
        private VisibilityHandler TheresDestinos = new VisibilityHandler();
        private bool EditandoTrans = false;

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
            TipoSelected.PropertyChanged -= TipoSelected_PropertyChanged;
            TipoSelected.PropertyChanged += TipoSelected_PropertyChanged;
            Ingresos.CollectionChanged -= Etiquetas_CollectionChanged;
            Cuentas.CollectionChanged -= Etiquetas_CollectionChanged;
            Gastos.CollectionChanged -= Etiquetas_CollectionChanged;
            Ingresos.CollectionChanged += Etiquetas_CollectionChanged;
            Cuentas.CollectionChanged += Etiquetas_CollectionChanged;
            Gastos.CollectionChanged += Etiquetas_CollectionChanged;
        }
        private void Etiquetas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckOrigenesDestinos();
        }
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
        #region Click
        private void AddIngreso_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddIngreso(((AddEtiquetaForm)sender).TextBox.Text);
                AEF_Ingreso.IsErrorRaised = false;
            }
            catch (Exception)
            {
                AEF_Ingreso.IsErrorRaised = true;
            }
        }
        private void AddCuenta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddCuenta(((AddEtiquetaForm)sender).TextBox.Text);
                AEF_Cuenta.IsErrorRaised = false;
            }
            catch (Exception)
            {
                AEF_Cuenta.IsErrorRaised = true;
            }
        }
        private void AddGasto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddGasto(((AddEtiquetaForm)sender).TextBox.Text);
                AEF_Gasto.IsErrorRaised = false;
            }
            catch (Exception)
            {
                AEF_Gasto.IsErrorRaised = true;
            }
        }
        private void AddTransaccion_Click(object sender, RoutedEventArgs e)
        {
            double valor;
            EditandoTrans = false;
            if (!double.TryParse(TB_Valor.Text, out valor))
            {
                valor = 0;
            }
            Etiqueta origen = CB_Origen.SelectedItem as Etiqueta;
            Etiqueta destino = CB_Destino.SelectedItem as Etiqueta;
            string descripcion = TB_Descripcion.Text;
            DateTime? date = CDP_Fecha.Date is null ? null : (DateTime?)CDP_Fecha.Date.Value.Date;
            AddTransaccion(origen, destino, valor, descripcion, date);
            CB_Origen.SelectedItem = origen;
            CB_Destino.SelectedItem = destino;
            TB_Descripcion.Text = "";
            TB_Valor.Value = double.NaN;
            TB_Valor.Focus(FocusState.Programmatic);
        }
        private void Editar_Click(object sender, RoutedEventArgs e)
        {

        }
        private void EditarTrans_Click(object sender, RoutedEventArgs e) { }
        private async void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            Etiqueta etiqueta = (Etiqueta)(((MenuFlyoutItem)sender).CommandParameter);
            CD_Eliminar.Content = $"¿Está seguro que desea eliminar {etiqueta.Name}?";
            CD_Eliminar.Visibility = Visibility.Visible;
            ContentDialogResult contentDialogResult = await CD_Eliminar.ShowAsync();
            switch (contentDialogResult)
            {
                case ContentDialogResult.Primary:
                    try
                    {
                        EtiquetaDAO.Delete(etiqueta, Container);
                    }
                    catch (Exception)
                    {

                    }
                    break;
            }
        }
        private void EliminarTrans_Click(object sender, RoutedEventArgs e)
        {
            Transaccion transaccion = (Transaccion)(((MenuFlyoutItem)sender).CommandParameter);
            Container.TransaccionDAO.Remove(transaccion);
        }
        private void AppBarCancelTransButton_Click(object sender, RoutedEventArgs e)
        {
            EditandoTrans = false;
            var flyout = ABB_AddTrans.Flyout;
            flyout.Hide();
        }
        private void B_Origen_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_Tipo.SelectedIndex)
            {
                //Entrada
                case 0:
                    ShowIngresoForm();
                    break;
                //Movimiento
                case 1:
                    ShowCuentaForm();
                    break;
                //Salida
                case 2:
                    ShowCuentaForm();
                    break;
                default:
                    break;
            }
        }
        private void B_Destino_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_Tipo.SelectedIndex)
            {
                //Entrada
                case 0:
                    ShowCuentaForm();
                    break;
                //Movimiento
                case 1:
                    ShowCuentaForm();
                    break;
                //Salida
                case 2:
                    ShowGastoForm();
                    break;
                default:
                    break;
            }
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
        #endregion
        private void ResetTransaccionForm()
        {
            CDP_Fecha.Date = DateTime.Now;
            CB_Origen.SelectedIndex = -1;
            CB_Destino.SelectedIndex = -1;
            TB_Valor.Text = "";
            TB_Descripcion.Text = "";
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
                if (TipoSelected.Visibility == Visibility.Collapsed)
                {
                    ResetTransaccionForm();
                    TransFormDataValidation();
                }
                EditandoTrans = true;
                TipoSelected.Visibility = Visibility.Visible;
                switch (index)
                {
                    //Entrada
                    case 0:
                        CB_Origen.ItemsSource = Ingresos;
                        CB_Destino.ItemsSource = Cuentas;
                        CB_Origen.PlaceholderText = "Ingresos";
                        CB_Destino.PlaceholderText = "Cuentas";
                        B_Origen.Content = "Nuevo Ingreso";
                        B_Destino.Content = "Nueva Cuenta";
                        break;
                    //Movimiento
                    case 1:
                        CB_Origen.ItemsSource = Cuentas;
                        CB_Destino.ItemsSource = Cuentas;
                        CB_Origen.PlaceholderText = "Cuentas";
                        CB_Destino.PlaceholderText = "Cuentas";
                        B_Origen.Content = "Nueva Cuenta";
                        B_Destino.Content = "Nueva Cuenta";
                        break;
                    //Salida
                    case 2:
                        CB_Origen.ItemsSource = Cuentas;
                        CB_Destino.ItemsSource = Gastos;
                        CB_Origen.PlaceholderText = "Cuentas";
                        CB_Destino.PlaceholderText = "Gastos";
                        B_Origen.Content = "Nueva Cuenta";
                        B_Destino.Content = "Nuevo Gasto";
                        break;
                    default:
                        break;
                }
                CheckOrigenesDestinos();
            }
        }
        private void CheckOrigenesDestinos()
        {
            TheresOrigenes.Visibility = (CB_Origen.Items.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            TheresDestinos.Visibility = (CB_Destino.Items.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }
        private void LV_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var deleteCommand = new StandardUICommand(StandardUICommandKind.None);
            var editCommand = new StandardUICommand(StandardUICommandKind.None);
            Etiqueta data = args.Item as Etiqueta;
            MenuFlyout menuFlyout = new MenuFlyout();
            MenuFlyoutItem eliminar = new MenuFlyoutItem()
            {
                Text = "Eliminar",
                Icon = new SymbolIcon(Symbol.Delete),
                Command = deleteCommand,
                CommandParameter = data
            };
            MenuFlyoutItem editar = new MenuFlyoutItem()
            {
                Text = "Editar",
                Icon = new SymbolIcon(Symbol.Edit),
                Command = editCommand,
                CommandParameter = data
            };
            eliminar.Click += Eliminar_Click;
            editar.Click += Editar_Click;
            menuFlyout.Items.Add(eliminar);
            menuFlyout.Items.Add(editar);
            args.ItemContainer.ContextFlyout = menuFlyout;
        }
        private void LV_Transacciones_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var deleteCommand = new StandardUICommand(StandardUICommandKind.None);
            var editCommand = new StandardUICommand(StandardUICommandKind.None);
            Transaccion data = args.Item as Transaccion;
            MenuFlyout menuFlyout = new MenuFlyout();
            MenuFlyoutItem eliminar = new MenuFlyoutItem()
            {
                Text = "Eliminar",
                Icon = new SymbolIcon(Symbol.Delete),
                Command = deleteCommand,
                CommandParameter = data
            };
            MenuFlyoutItem editar = new MenuFlyoutItem()
            {
                Text = "Editar",
                Icon = new SymbolIcon(Symbol.Edit),
                Command = editCommand,
                CommandParameter = data
            };
            eliminar.Click += EliminarTrans_Click;
            editar.Click += EditarTrans_Click;
            menuFlyout.Items.Add(eliminar);
            menuFlyout.Items.Add(editar);
            args.ItemContainer.ContextFlyout = menuFlyout;
        }
        private void AddTransFlyout_Closing(Windows.UI.Xaml.Controls.Primitives.FlyoutBase sender, Windows.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
        {
            if (!EditandoTrans)
            {
                CB_Tipo.SelectedIndex = -1;
            }
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
        private void CDP_Fecha_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            TransFormDataValidation();
        }
        private void CB_Origen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TransFormDataValidation();
        }
        private void TB_Valor_ValueChanged(muxc.NumberBox sender, muxc.NumberBoxValueChangedEventArgs args)
        {
            TransFormDataValidation();
        }
        private void ShowIngresoForm_Click(object sender, RoutedEventArgs e)
        {
            ShowIngresoForm();
        }
        private void ShowCuentaForm_Click(object sender, RoutedEventArgs e)
        {
            ShowCuentaForm();
        }
        private void ShowGastoForm_Click(object sender, RoutedEventArgs e)
        {
            ShowGastoForm();
        }
        private void ShowIngresoForm()
        {
            ResetPopups();
            Popup_IngresoForm.IsOpen = true;
            AEF_Ingreso.Focus(FocusState.Programmatic);
        }
        private void ShowCuentaForm()
        {
            ResetPopups();
            Popup_CuentaForm.IsOpen = true;
            AEF_Cuenta.Focus(FocusState.Programmatic);
        }
        private void ShowGastoForm()
        {
            ResetPopups();
            Popup_GastoForm.IsOpen = true;
            AEF_Gasto.Focus(FocusState.Programmatic);
        }
        private void ResetPopups()
        {
            Popup_IngresoForm.IsOpen = false;
            Popup_CuentaForm.IsOpen = false;
            Popup_GastoForm.IsOpen = false;
        }
    }
}
