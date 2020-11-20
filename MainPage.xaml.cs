using JevoCrypt;
using JevoCrypt.Classes;
using JevoGastosCore;
using JevoGastosCore.Enums;
using JevoGastosCore.Model;
using JevoGastosCore.ModelView;
using JevoGastosUWP.ControlesPersonalizados;
using JevoGastosUWP.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
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
        private class VisibleBool : INotifyPropertyChanged
        {
            private bool value;
            public bool Value
            {
                get => value;
                set
                {
                    this.value = value;
                    OnPropertyChanged();
                }

            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            public VisibleBool(bool value = false)
            {
                this.value = value;
            }
        }
        private class ListViewWraper
        {
            public Etiqueta Item { get; set; }
            public ListViewItem ListViewItem { get; set; }
        }
        public class Parameters
        {
            public User User { get; set; }
            public UsersContainer UsersContainer { get; set; }
            public GastosContainer Container { get; set; }
            public LaunchActivatedEventArgs e { get; set; }

            public Parameters(GastosContainer container,User user,UsersContainer usersContainer,LaunchActivatedEventArgs e)
            {
                this.Container = container;
                this.User = user;
                this.UsersContainer = usersContainer;
                this.e = e;
            }
        }
        #endregion
        #region InternalVariables
        private ObservableCollection<Transaccion> relatedTrans = new ObservableCollection<Transaccion>();
        private ObservableCollection<Plan> relatedPlanes = new ObservableCollection<Plan>();

        private Parameters parameters;
        private GastosContainer Container;
        private ObservableCollection<Etiqueta> Etiquetas => Container.EtiquetaDAO.Items;
        private ObservableCollection<Ingreso> Ingresos => Container.IngresoDAO.Items;
        private ObservableCollection<Cuenta> Cuentas => Container.CuentaDAO.Items;
        private ObservableCollection<Gasto> Gastos => Container.GastoDAO.Items;
        private ObservableCollection<Credito> Creditos => Container.CreditoDAO.Items;
        private ObservableCollection<Transaccion> Transacciones => Container.TransaccionDAO.Items;
        private ObservableCollection<Transaccion> RelatedTrans => relatedTrans;
        private ObservableCollection<Plan> Planes => Container.PlanDAO.Items;
        private ObservableCollection<Plan> RelatedPlanes => relatedPlanes;
        private VisibilityHandler ErrorVisibility = new VisibilityHandler();
        private VisibilityHandler TipoSelected = new VisibilityHandler();
        private VisibilityHandler TheresOrigenes = new VisibilityHandler();
        private VisibilityHandler TheresDestinos = new VisibilityHandler();
        private VisibilityHandler EmptyRelatedTrans = new VisibilityHandler();
        private VisibilityHandler EmptyRelatedPlans = new VisibilityHandler();
        private VisibilityHandler RelatedTransMode = new VisibilityHandler();
        private VisibleBool PendentChanges = new VisibleBool(false);
        #endregion
        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += MainPage_CloseRequested;
        }
        #region Inicializacion
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.parameters = (Parameters)e.Parameter;
            Container = parameters.Container;
            ConfigureView();
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested -= MainPage_CloseRequested;
        }
        private async void MainPage_CloseRequested(object sender, Windows.UI.Core.Preview.SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            Windows.Foundation.Deferral deferral = e.GetDeferral();
            ContentDialogResult contentDialogResult = await MainPage_CloseRequested();
            switch (contentDialogResult)
            {
                case ContentDialogResult.None:
                    e.Handled = true;
                    break;
                case ContentDialogResult.Primary:
                    await SaveAllAsync();
                    break;
                case ContentDialogResult.Secondary:
                    break;
                default:
                    break;
            }
            deferral.Complete();
        }
        private async Task<ContentDialogResult> MainPage_CloseRequested()
        {
            if (PendentChanges.Value)
            {
                CD_Salir.Visibility = Visibility.Visible;
                ContentDialogResult contentDialogResult = await CD_Salir.ShowAsync();
                return contentDialogResult;
            }
            return ContentDialogResult.Secondary;
        }
        private void ConfigureView()
        {
            Container.Context.ChangeTracker.StateChanged -= ChangeTracker_StateChanged;
            Container.Context.ChangeTracker.StateChanged += ChangeTracker_StateChanged;
            Container.Context.ChangeTracker.Tracked -= ChangeTracker_Tracked;
            Container.Context.ChangeTracker.Tracked += ChangeTracker_Tracked;
            PendentChanges.Value = Container.Context.ChangeTracker.HasChanges();
            Transacciones.CollectionChanged -= Transacciones_CollectionChanged;
            Transacciones.CollectionChanged += Transacciones_CollectionChanged;
            Planes.CollectionChanged -= Planes_CollectionChanged;
            Planes.CollectionChanged += Planes_CollectionChanged;
            RelatedTrans.CollectionChanged -= RelatedTrans_CollectionChanged;
            RelatedTrans.CollectionChanged += RelatedTrans_CollectionChanged;
            RelatedPlanes.CollectionChanged -= RelatedPlanes_CollectionChanged;
            RelatedPlanes.CollectionChanged += RelatedPlanes_CollectionChanged;
        }
        #endregion
        #region ChangesTracker
        private void ChangeTracker_Tracked(object sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityTrackedEventArgs e)
        {
            if (e.Entry.State!=EntityState.Unchanged)
            {
                PendentChanges.Value = true;
            }
        }
        private void ChangeTracker_StateChanged(object sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityStateChangedEventArgs e)
        {
            if ((e.NewState == EntityState.Modified) | (e.NewState == EntityState.Added) | (e.NewState == EntityState.Deleted))
            {
                PendentChanges.Value = true;
            }
        }

        
        private void Transacciones_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
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
        private void Planes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        RelatedPlanes.Remove((Plan)item);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    RelatedPlanes.Clear();
                    break;
                default:
                    break;
            }
        }
        private void RelatedTrans_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            EmptyRelatedTrans.Visibility = RelatedTrans.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private void RelatedPlanes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            EmptyRelatedPlans.Visibility = RelatedPlanes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
        #region Adders
        private void AddTransaccion(Etiqueta Origen, Etiqueta Destino, double valor, string descripcion = null, DateTime? dateTime = null)
        {
            if (!(Origen is null) & !(Destino is null))
            {
                Container.TransaccionDAO.Transaccion(Origen, Destino, valor, descripcion, dateTime);
            }
        }
        #endregion
        #region Deleters
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
            ErrorVisibility.Visibility = Visibility.Visible;
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
        #endregion
        #region Click
        private void AddPlan_Click(object sender, RoutedEventArgs e)
        {
            Frame_FlyoutAddPlan.Navigate(typeof(PlanForm), new PlanForm.Parameters(Container), new SuppressNavigationTransitionInfo());
        }
        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            Etiqueta etiqueta = (Etiqueta)(((MenuFlyoutItem)sender).CommandParameter);
            ShowEtiquetaEditForm(etiqueta);
        }
        private void EditarTrans_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuItem = (MenuFlyoutItem)sender;
            ListViewItem row = (ListViewItem)menuItem.CommandParameter;
            Transaccion transaccion = (Transaccion)row.Content;
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(G_Transacciones);
            FlyoutShowOptions options = new FlyoutShowOptions()
            {
                Placement = FlyoutPlacementMode.Auto
            };
            flyout.ShowAt(row, options);
            TransForm.Parameters parameters = new TransForm.Parameters(Container, true, transaccion);
            Frame_FlyoutEditTrans.Navigate(typeof(TransForm), parameters, new SuppressNavigationTransitionInfo());
            ((TransForm)Frame_FlyoutEditTrans.Content).CloseRequested += EditTransForm_CloseRequested;
        }

        private void EditTransForm_CloseRequested(TransForm.Parameters parameters)
        {
            FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(G_Transacciones);
            flyout.Hide();
        }

        private void EditarPlan_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow dataGridRow = (DataGridRow)((MenuFlyoutItem)sender).CommandParameter;
            Plan plan = (Plan)dataGridRow.DataContext;
            FlyoutBase
                .GetAttachedFlyout(DG_Planes)
                .ShowAt(
                dataGridRow,
                new FlyoutShowOptions() { Placement = FlyoutPlacementMode.Auto });
            Frame_FlyoutEditPlan.Navigate(
                typeof(PlanForm),
                new PlanForm.Parameters(
                    Container,
                    plan,
                    true),
                new SuppressNavigationTransitionInfo());
        }
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
        private void Trans_Click(object sender, RoutedEventArgs e)
        {
            RelatedTransMode.Visibility = Visibility.Visible;
            ListViewItem senderItem = (ListViewItem)((MenuFlyoutItem)sender).CommandParameter;
            Etiqueta senderEtiqueta = (Etiqueta)senderItem.Content;
            relatedTrans.Clear();
            foreach (Transaccion item in Transacciones)
            {
                if (item.Origen == senderEtiqueta | item.Destino == senderEtiqueta)
                {
                    relatedTrans.Add(item);
                }
            }
            FlyoutBase
                .GetAttachedFlyout(G_BaseGrid)
                .ShowAt
                (senderItem
                ,
                new FlyoutShowOptions() { Placement = FlyoutPlacementMode.RightEdgeAlignedTop }
                );

        }
        private void Plans_Click(object sender, RoutedEventArgs e)
        {
            RelatedTransMode.Visibility = Visibility.Collapsed;
            ListViewItem senderItem = (ListViewItem)((MenuFlyoutItem)sender).CommandParameter;
            Etiqueta senderEtiqueta = (Etiqueta)senderItem.Content;
            relatedPlanes.Clear();
            foreach (Plan item in Planes)
            {
                if (item.Etiqueta == senderEtiqueta)
                {
                    relatedPlanes.Add(item);
                }
            }
            FlyoutBase
                .GetAttachedFlyout(G_BaseGrid)
                .ShowAt
                (senderItem,
                new FlyoutShowOptions() { Placement = FlyoutPlacementMode.RightEdgeAlignedTop }
                );
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
        private void ShowCreditoForm_Click(object sender, RoutedEventArgs e)
        {
            ShowEtiquetaAddForm(TipoEtiqueta.Credito);
        }
        #endregion
        #region Otros
        private void LV_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var deleteCommand = new StandardUICommand(StandardUICommandKind.None);
            var editCommand = new StandardUICommand(StandardUICommandKind.None);
            var transCommand = new StandardUICommand(StandardUICommandKind.None);
            var plansCommand = new StandardUICommand(StandardUICommandKind.None);
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
            MenuFlyoutSeparator sep = new MenuFlyoutSeparator();
            MenuFlyoutItem trans = new MenuFlyoutItem()
            {
                Text = "Transacciones",
                Icon = new SymbolIcon(Symbol.Bookmarks),
                Command = transCommand,
                CommandParameter = args.ItemContainer as ListViewItem
            };
            MenuFlyoutItem plans = new MenuFlyoutItem()
            {
                Text = "Planes",
                Icon = new SymbolIcon(Symbol.Flag),
                Command = plansCommand,
                CommandParameter = args.ItemContainer as ListViewItem
            };
            eliminar.Click += Eliminar_Click;
            editar.Click += Editar_Click;
            trans.Click += Trans_Click;
            plans.Click += Plans_Click;
            menuFlyout.Items.Add(editar);
            menuFlyout.Items.Add(eliminar);
            menuFlyout.Items.Add(sep);
            menuFlyout.Items.Add(trans);
            menuFlyout.Items.Add(plans);
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
                CommandParameter = args.ItemContainer
            };
            eliminar.Click += EliminarTrans_Click;
            editar.Click += EditarTrans_Click;
            menuFlyout.Items.Add(eliminar);
            menuFlyout.Items.Add(editar);
            args.ItemContainer.ContextFlyout = menuFlyout;
        }
        private void AddTransFlyout_Closing(Windows.UI.Xaml.Controls.Primitives.FlyoutBase sender, Windows.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
        {
        }
        private void ShowIngresoForm()
        {
            ShowEtiquetaAddForm(TipoEtiqueta.Ingreso);
        }
        private void ShowCuentaForm()
        {
            ShowEtiquetaAddForm(TipoEtiqueta.Cuenta);
        }
        private void ShowGastoForm()
        {
            ShowEtiquetaAddForm(TipoEtiqueta.Gasto);
        }
        private void ShowEtiquetaAddForm(TipoEtiqueta tipoEtiqueta)
        {
            ResetPopups();
            EtiquetaForm.Parameters parameters = new EtiquetaForm.Parameters(Container,tipoEtiqueta:tipoEtiqueta);
            Popup_MultiUse.IsOpen = true;
            Frame_MultiUse.Navigate(
                typeof(EtiquetaForm),
                parameters, new SuppressNavigationTransitionInfo());
        }
        private void ShowEtiquetaEditForm(Etiqueta etiqueta)
        {
            ResetPopups();
            EtiquetaForm.Parameters parameters =
                new EtiquetaForm.Parameters(Container, etiqueta, isEditMode: true);
            Popup_MultiUse.IsOpen = true;
            Frame_MultiUse.Navigate(
                typeof(EtiquetaForm),
                parameters, new SuppressNavigationTransitionInfo());
            ((EtiquetaForm)Frame_MultiUse.Content).CloseRequested += FrameMultiUser_RequestClose;
        }

        private void FrameMultiUser_RequestClose()
        {
            Popup_MultiUse.IsOpen = false;
        }
        private void UserForm_RequestClose(bool cerrarSesion)
        {
            FrameMultiUser_RequestClose();
            if (cerrarSesion)
            {
                this.Frame.Navigate(typeof(SignInPage), new SignInPage.Parameters(this.parameters.UsersContainer, null), new ContinuumNavigationTransitionInfo());
            }
        }

        private void ResetPopups()
        {
            Popup_MultiUse.IsOpen = false;
        }

        private async void ABB_Guardar_Click(object sender, RoutedEventArgs e)
        {
            bool terminado;
            SP_Guardando.Visibility = Visibility.Visible;
            ABB_Guardar.IsEnabled = false;
            terminado = await SaveAllAsync();
            SP_Guardando.Visibility = terminado ? Visibility.Collapsed : Visibility.Visible;
        }
        private async Task<bool> SaveAllAsync()
        {
            bool respuesta;
            respuesta = await Task.Run(
                () =>
                {
                    Container.SaveChanges();
                    return true;
                }
                );
            PendentChanges.Value = false;
            return respuesta;
        }

        private async void ClearIngresos_Click(object sender, RoutedEventArgs e)
        {
            await ClearEtiquetas(TipoEtiqueta.Ingreso, "¿Desea eliminar todas las fuentes de ingreso?");
        }
        private async void ClearCuentas_Click(object sender, RoutedEventArgs e)
        {
            await ClearEtiquetas(TipoEtiqueta.Cuenta, "¿Desea eliminar todas las cuentas?");
        }
        private async void ClearGastos_Click(object sender, RoutedEventArgs e)
        {
            await ClearEtiquetas(TipoEtiqueta.Gasto, "¿Desea eliminar todas las etiquetas de gasto?");
        }
        private async void ClearCreditos_Click(object sender, RoutedEventArgs e)
        {
            await ClearEtiquetas(TipoEtiqueta.Credito, "¿Desea eliminar todas las etiquetas de credito?");
        }

        private async void ClearTransacciones_Click(object sender, RoutedEventArgs e)
        {
            await ClearTrans(Transacciones, "¿Desea eliminar todas las transacciones?");
        }
        private async void ClearPlanes_Click(object sender, RoutedEventArgs e)
        {
            await ClearPlanes(Planes, "¿Desea eliminar todos los planes?");
        }
        private async void ClearRelaTrans_Click(object sender, RoutedEventArgs e)
        {
            await ClearRelaTrans(RelatedTrans, "¿Desea eliminar todas las transacciones relacionadas?");
        }
        private async void ClearRelaPlans_Click(object sender, RoutedEventArgs e)
        {
            await ClearRelaPlans(RelatedPlanes, "¿Desea eliminar todos los planes relacionados?");
        }
        private async Task ClearRelaTrans(IList list, string confirmationtext = null)
        {
            if (!(confirmationtext is null) || confirmationtext.Length > 0)
            {
                CD_Clear.Content = confirmationtext;
                ContentDialogResult response = await CD_Clear.ShowAsync();
                switch (response)
                {
                    case ContentDialogResult.None:
                        break;
                    case ContentDialogResult.Primary:
                        break;
                    case ContentDialogResult.Secondary:
                        return;
                    default:
                        break;
                }
            }
            Container.TransaccionDAO.Remove(RelatedTrans);
            RelatedTrans.Clear();
        }
        private async Task ClearRelaPlans(IList list, string confirmationtext = null)
        {
            if (!(confirmationtext is null) || confirmationtext.Length > 0)
            {
                CD_Clear.Content = confirmationtext;
                ContentDialogResult response = await CD_Clear.ShowAsync();
                switch (response)
                {
                    case ContentDialogResult.None:
                        break;
                    case ContentDialogResult.Primary:
                        break;
                    case ContentDialogResult.Secondary:
                        return;
                    default:
                        break;
                }
            }
            Container.PlanDAO.Remove(RelatedPlanes);
            RelatedPlanes.Clear();
        }
        private async Task ClearTrans(IList list, string confirmationtext = null)
        {
            if (!(confirmationtext is null) || confirmationtext.Length > 0)
            {
                CD_Clear.Content = confirmationtext;
                ContentDialogResult response = await CD_Clear.ShowAsync();
                switch (response)
                {
                    case ContentDialogResult.None:
                        break;
                    case ContentDialogResult.Primary:
                        break;
                    case ContentDialogResult.Secondary:
                        return;
                    default:
                        break;
                }
            }
            Container.TransaccionDAO.Clear();
        }
        private async Task ClearPlanes(IList list, string confirmationtext = null)
        {
            if (!(confirmationtext is null) || confirmationtext.Length > 0)
            {
                CD_Clear.Content = confirmationtext;
                ContentDialogResult response = await CD_Clear.ShowAsync();
                switch (response)
                {
                    case ContentDialogResult.None:
                        break;
                    case ContentDialogResult.Primary:
                        break;
                    case ContentDialogResult.Secondary:
                        return;
                    default:
                        break;
                }
            }
            Container.PlanDAO.Clear();
        }
        private async Task ClearEtiquetas(TipoEtiqueta tipo, string confirmationtext = null)
        {
            if (!(confirmationtext is null) || confirmationtext.Length > 0)
            {
                CD_Clear.Content = confirmationtext;
                ContentDialogResult response = await CD_Clear.ShowAsync();
                switch (response)
                {
                    case ContentDialogResult.None:
                        break;
                    case ContentDialogResult.Primary:
                        break;
                    case ContentDialogResult.Secondary:
                        return;
                    default:
                        break;
                }
                EtiquetaDAO.Clear(tipo, Container);
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var deleteCommand = new StandardUICommand(StandardUICommandKind.None);
            var editCommand = new StandardUICommand(StandardUICommandKind.None);

            Plan data = e.Row.DataContext as Plan;
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
                CommandParameter = e.Row,
            };
            eliminar.Click += EliminarPlan_Click;
            editar.Click += EditarPlan_Click;
            menuFlyout.Items.Add(editar);
            menuFlyout.Items.Add(eliminar);

            e.Row.ContextFlyout = menuFlyout;
        }

        private void EliminarPlan_Click(object sender, RoutedEventArgs e)
        {
            Planes.Remove(((MenuFlyoutItem)sender).CommandParameter as Plan);
        }

        private void ConfiguracionButton_Click(object sender, RoutedEventArgs e)
        {
            ResetPopups();
            Popup_MultiUse.IsOpen = true;
            Frame_MultiUse.Navigate(
                typeof(SettingsPage),
                new SettingsPage.Parameters(Container.PayDaysDAO),
                new SuppressNavigationTransitionInfo());
        }
        #endregion
        private void AddTrans_Click(object sender, RoutedEventArgs e)
        {
            TransForm.Parameters parameters = new TransForm.Parameters(Container);
            if (Frame_AddTransaction.Content is null)
            { 
                Frame_AddTransaction.Navigate(typeof(TransForm), parameters, new SuppressNavigationTransitionInfo());
                ((TransForm)Frame_AddTransaction.Content).CloseRequested += AddTransacctionForm_CloseRequested;
            }
        }

        private void AddTransacctionForm_CloseRequested(TransForm.Parameters e)
        {
            ABB_AddTrans.Flyout.Hide();
        }

        private async void SignOut_Click(object sender, RoutedEventArgs e)
        {
            this.parameters.UsersContainer.UserDAO.SignOut();
            ContentDialogResult contentDialogResult = await MainPage_CloseRequested();
            switch (contentDialogResult)
            {
                case ContentDialogResult.None:
                    break;
                case ContentDialogResult.Primary:
                    await SaveAllAsync();
                    this.Frame.Navigate(typeof(SignInPage), new SignInPage.Parameters(this.parameters.UsersContainer, null),new ContinuumNavigationTransitionInfo());
                    break;
                case ContentDialogResult.Secondary:
                    this.Frame.Navigate(typeof(SignInPage), new SignInPage.Parameters(this.parameters.UsersContainer, null), new ContinuumNavigationTransitionInfo());
                    break;
                default:
                    break;
            }
        }

        private void CambiarUsuario_Click(object sender, RoutedEventArgs e)
        {
            Popup_MultiUse.IsOpen = true;
            Frame_MultiUse.Navigate(typeof(UserForm),
                new UserForm.Parameters(
                    this.parameters.UsersContainer,
                    this.parameters.User,
                    UserForm.TipoInicialización.changeusername,
                    this.Container
                    ),
                new SuppressNavigationTransitionInfo());
            ((UserForm)Frame_MultiUse.Content).CloseRequested += UserForm_RequestClose;
        }

        private void CambiarContraseña_Click(object sender, RoutedEventArgs e)
        {
            Popup_MultiUse.IsOpen = true;
            Frame_MultiUse.Navigate(
                typeof(UserForm),
                new UserForm.Parameters(
                    this.parameters.UsersContainer,
                    this.parameters.User,
                    UserForm.TipoInicialización.changepassword
                    ),
                new SuppressNavigationTransitionInfo());
            ((UserForm)Frame_MultiUse.Content).CloseRequested += UserForm_RequestClose;
        }

        private void EliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            bool respuesta = false;
            UserForm.Parameters parameter = new UserForm.Parameters(
                    this.parameters.UsersContainer,
                    this.parameters.User,
                    UserForm.TipoInicialización.deleteaccount,
                    this.Container,
                    respuesta
                    );
            Popup_MultiUse.IsOpen = true;
            Frame_MultiUse.Navigate(
                typeof(UserForm),
                parameter,
                new SuppressNavigationTransitionInfo());
            ((UserForm)Frame_MultiUse.Content).CloseRequested += UserForm_RequestClose;
        }
    }
}