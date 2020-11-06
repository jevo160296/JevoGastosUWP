using JevoGastosCore;
using JevoGastosCore.Enums;
using JevoGastosCore.Model;
using JevoGastosCore.ModelView;
using JevoGastosUWP.ControlesPersonalizados;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP.Forms
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class EtiquetaForm : Page
    {
        #region Clases
        public class Parameters
        {
            public GastosContainer Container { get; set; }
            public TipoEtiqueta? TipoEtiqueta { get; set; }
            public Etiqueta Etiqueta { get; set; }
            public bool IsEditMode { get; set; }
            public Parameters(
                GastosContainer container,
                Etiqueta etiqueta=null,
                TipoEtiqueta? tipoEtiqueta= null,
                bool isEditMode=false
                )
            {
                Container = container;
                TipoEtiqueta = tipoEtiqueta;
                Etiqueta = etiqueta;
                IsEditMode = isEditMode;
            }

        }
        #endregion
        #region Events
        public delegate void CloseFormRequestedHandler();
        public event CloseFormRequestedHandler CloseRequested;
        #endregion
        #region Atributos
        private Parameters parameters;
        #endregion
        public EtiquetaForm()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            parameters = (Parameters)e.Parameter;
            parameters.TipoEtiqueta = parameters.TipoEtiqueta ?? (parameters.Etiqueta is null ? TipoEtiqueta.Ingreso : EtiquetaDAO.Tipo(parameters.Etiqueta));
            AddEtiquetaForm coso = new AddEtiquetaForm();
            if (parameters.TipoEtiqueta==TipoEtiqueta.Cuenta)
            {
                AEF_Etiqueta.CB_Visibility = Visibility.Visible;
            }
            else
            {
                AEF_Etiqueta.CB_Visibility = Visibility.Collapsed;
            }
            if (parameters.IsEditMode)
            {
                InitializeEditMode();
            }
            else
            {
                InitializeAddMode();
            }
        }
        private void InitializeAddMode()
        {
            switch (parameters.TipoEtiqueta)
            {
                case TipoEtiqueta.Ingreso:
                    AEF_Etiqueta.HeaderText = "Nueva fuente de ingresos";
                    break;
                case TipoEtiqueta.Cuenta:
                    AEF_Etiqueta.HeaderText = "Nueva cuenta";
                    break;
                case TipoEtiqueta.Gasto:
                    AEF_Etiqueta.HeaderText = "Nuevo gasto";
                    break;
                case TipoEtiqueta.Credito:
                    AEF_Etiqueta.HeaderText = "Nuevo crédito";
                    break;
                default:
                    break;
            }
            AEF_Etiqueta.Click += Add_Click;
        }
        private void InitializeEditMode()
        {
            AEF_Etiqueta.HeaderText = "Editar etiqueta";
            AEF_Etiqueta.Etiqueta = parameters.Etiqueta;
            AEF_Etiqueta.TextBox.Text = parameters.Etiqueta.Name;
            if (parameters.TipoEtiqueta==TipoEtiqueta.Cuenta)
            {
                AEF_Etiqueta.CheckBox.IsChecked = ((Cuenta)parameters.Etiqueta).EsAhorro;
            }
            AEF_Etiqueta.Click += EditClick;
            AEF_Etiqueta.TextBox.SelectAll();
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            AEF_Etiqueta.Etiqueta.Name = AEF_Etiqueta.TextBox.Text;
            if (parameters.TipoEtiqueta == TipoEtiqueta.Cuenta)
            {
                ((Cuenta)AEF_Etiqueta.Etiqueta).EsAhorro = AEF_Etiqueta.CheckBox.IsChecked ?? false;
            }
            RequestCloseForm();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEtiquetaForm form = (AddEtiquetaForm)sender;
            try
            {
                switch (parameters.TipoEtiqueta)
                {
                    case TipoEtiqueta.Ingreso:
                        AddIngreso(form.TextBox.Text);
                        break;
                    case TipoEtiqueta.Cuenta:
                        AddCuenta(form.TextBox.Text, form.CheckBox.IsChecked ?? false);
                        break;
                    case TipoEtiqueta.Gasto:
                        AddGasto(form.TextBox.Text);
                        break;
                    case TipoEtiqueta.Credito:
                        AddCredito(form.TextBox.Text);
                        break;
                    default:
                        break;
                }
                AEF_Etiqueta.IsErrorRaised = false;
            }
            catch (Exception)
            {
                AEF_Etiqueta.IsErrorRaised = true;
            }
        }
        
        private void AddIngreso(string name)
        {
            parameters.Container.IngresoDAO.Add(name);
        }
        private void AddCuenta(string name,bool esAhorro)
        {
            parameters.Container.CuentaDAO.Add(name,esAhorro);
        }
        private void AddGasto(string name)
        {
            parameters.Container.GastoDAO.Add(name);
        }
        private void AddCredito(string name)
        {
            parameters.Container.CreditoDAO.Add(name);
        }

        public void RequestCloseForm() 
        {
            CloseRequested?.Invoke();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AEF_Etiqueta.Focus(FocusState.Programmatic);
        }
    }
}
