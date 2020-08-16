using JevoGastosCore;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GastosContainer gastosContainer;
            double max = 7;
            //gastosContainer = await LoadDataAsync();
            gastosContainer = await LoadContainerAsync();
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value = 1 / max*100;
            TB_Detalles.Text = "Cargando transacciones...";
            await LoadTransaccionesAsync(gastosContainer);
            ProgressBar.Value = 2 / max*100;
            TB_Detalles.Text = "Cargando etiquetas...";
            await LoadEtiquetasAsync(gastosContainer);
            TB_Detalles.Text = "Cargando ingresos...";
            ProgressBar.Value = 3 / max*100;
            await LoadIngresosAsync(gastosContainer);
            TB_Detalles.Text = "Cargando cuentas...";
            ProgressBar.Value = 4 / max*100;
            await LoadCuentasAsync(gastosContainer);
            TB_Detalles.Text = "Cargando gastos...";
            ProgressBar.Value = 5 / max*100;
            await LoadGastosAsync(gastosContainer);
            TB_Detalles.Text = "Cargando plan...";
            ProgressBar.Value = 6 / max * 100;
            await LoadPlanesAsync(gastosContainer);
            ProgressBar.Value = 7 / max*100;
            this.Frame.Navigate(typeof(MainPage), gastosContainer);
        }

        private async Task<GastosContainer> LoadDataAsync()
        {
            GastosContainer gastosContainer = await Task.Run<GastosContainer>(() => LoadData());
            return gastosContainer;
        }
        private async Task<GastosContainer> LoadContainerAsync()
        {
            GastosContainer gastosContainer = await Task.Run<GastosContainer>(() => LoadContainer());
            return gastosContainer;
        }
        private async Task LoadTransaccionesAsync(GastosContainer gastosContainer) 
        {
            await Task.Run(() => LoadTransacciones(gastosContainer));
        }
        private async Task LoadEtiquetasAsync(GastosContainer gastosContainer)
        {
            await Task.Run(() => LoadEtiquetas(gastosContainer));
        }
        private async Task LoadIngresosAsync(GastosContainer gastosContainer)
        {
            await Task.Run(() => LoadIngresos(gastosContainer));
        }
        private async Task LoadCuentasAsync(GastosContainer gastosContainer)
        {
            await Task.Run(() => LoadCuentas(gastosContainer));
        }
        private async Task LoadGastosAsync(GastosContainer gastosContainer)
        {
            await Task.Run(() => LoadGastos(gastosContainer));
        }
        private async Task LoadPlanesAsync(GastosContainer gastosContainer)
        {
            await Task.Run(() => LoadPlanes(gastosContainer));
        }

        private GastosContainer LoadData()
        {
            GastosContainer gastosContainer = LoadContainer();
            LoadTransacciones(gastosContainer);
            LoadEtiquetas(gastosContainer);
            LoadIngresos(gastosContainer);
            LoadCuentas(gastosContainer);
            LoadGastos(gastosContainer);
            return gastosContainer;
        }
        private GastosContainer LoadContainer()
        {
            GastosContainer gastosContainer = 
                new GastosContainer(Windows.Storage.ApplicationData.Current.LocalFolder.Path,false);
            return gastosContainer;
        }
        private void LoadTransacciones(GastosContainer gastosContainer)
        {
            var transacciones = gastosContainer.TransaccionDAO.Items;
        }
        private void LoadEtiquetas(GastosContainer gastosContainer)
        {
            var etiquetas = gastosContainer.EtiquetaDAO.Items;
        }
        private void LoadIngresos(GastosContainer gastosContainer)
        {
            var ingresos = gastosContainer.IngresoDAO.Items;
        }
        private void LoadCuentas(GastosContainer gastosContainer)
        {
            var cuentas = gastosContainer.CuentaDAO.Items;
        }
        private void LoadGastos(GastosContainer gastosContainer)
        {
            var gastos = gastosContainer.GastoDAO.Items;
        }
        private void LoadPlanes(GastosContainer gastosContainer)
        {
            var planes = gastosContainer.PlanDAO.Items;
        }
    }
}
