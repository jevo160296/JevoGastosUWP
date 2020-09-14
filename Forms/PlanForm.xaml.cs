using JevoGastosCore;
using JevoGastosCore.Enums;
using JevoGastosCore.Model;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP.Forms
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class PlanForm : Page
    {
        #region Clases
        public class Parameters
        {
            public GastosContainer Container;
            public Plan Plan;
            public bool IsEditMode = false;

            public Parameters(GastosContainer container,Plan plan=null,bool isEditMode = false)
            {
                Container = container;
                Plan = plan;
                IsEditMode = isEditMode;
            }
        }
        #endregion
        #region Variables internas
        private Parameters parameters;
        private GastosContainer container;
        private Plan plan;
        private bool planAdded = false;
        private VirtualKey? lastPressed=null;

        private Dictionary<string, TipoEtiqueta> BusquedaEtiquetas = new Dictionary<string, TipoEtiqueta>()
        {
            {"Ingreso",TipoEtiqueta.Ingreso},
            {"Gasto",TipoEtiqueta.Gasto },
            {"Credito",TipoEtiqueta.Credito }
        };
        #endregion
        #region Inicialización
        public PlanForm()
        {
            this.InitializeComponent();
            SetupView();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            parameters = e.Parameter as Parameters;
            container = parameters.Container;

            if (parameters.IsEditMode)
            {
                plan = parameters.Plan;
                if (plan.Etiqueta is Ingreso)
                {
                    CB_TipoEtiqueta.SelectedItem = "Ingreso";
                }
                else if (plan.Etiqueta is Credito)
                {
                    CB_TipoEtiqueta.SelectedItem = "Credito";
                }
                else if (plan.Etiqueta is Gasto)
                {
                    CB_TipoEtiqueta.SelectedItem = "Gasto";
                }
                CB_Etiqueta.SelectedItem = plan.Etiqueta;
                CB_TipoPlan.SelectedItem = plan.Tipo.ToString();
                TS_EsMesFijo.IsOn = plan.EsMesFijo;
                NB_Meta.Value = plan.Meta;
                planAdded = true;
                TB_Titulo.Text = "Editar plan";
            }
        }

        private void SetupView()
        {
            CB_TipoPlan.ItemsSource = Enum.GetNames(typeof(TipoPlan));
            CB_TipoEtiqueta.ItemsSource = BusquedaEtiquetas.Keys;
            CheckTipoEtiquetaSelection();
            CheckSePuedeAnadir();
            CB_TipoEtiqueta.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }
        #endregion
        #region Changed
        private void CB_TipoEtiqueta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckTipoEtiquetaSelection();
        }
        private void CB_Etiqueta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSePuedeAnadir();
        }
        private void CB_TipoPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSePuedeAnadir();
            TipoPlan selected = (TipoPlan)CB_TipoPlan.SelectedIndex;
            switch (selected)
            {
                case TipoPlan.Diario:
                    TS_EsMesFijo.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;
                case TipoPlan.Mensual:
                    TS_EsMesFijo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                default:
                    TS_EsMesFijo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }
        }
        private void NB_Meta_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            CheckSePuedeAnadir();
            if (lastPressed==VirtualKey.Enter)
            {
                TryProcessTransaction();
            }
        }
        private void TS_EsMesFijo_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
        #endregion
        #region Clickers
        private void B_Agregar_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TryProcessTransaction();
        }
        #endregion
        #region Checkers
        private bool CheckTipoEtiquetaSelection()
        {
            bool isTipoEtiquetaSelected= CB_TipoEtiqueta.SelectedIndex > -1;
            CB_Etiqueta.IsEnabled = isTipoEtiquetaSelected;
            CheckSePuedeAnadir();

            if (isTipoEtiquetaSelected)
            {
                TipoEtiqueta v = (TipoEtiqueta)BusquedaEtiquetas[CB_TipoEtiqueta.SelectedItem as string];
                switch (v)
                {
                    case TipoEtiqueta.Ingreso:
                        SetEtiquetas(container.IngresoDAO.Items);
                        break;
                    case TipoEtiqueta.Cuenta:
                        SetEtiquetas(container.CuentaDAO.Items);
                        break;
                    case TipoEtiqueta.Gasto:
                        SetEtiquetas(container.GastoDAO.Items);
                        break;
                    case TipoEtiqueta.Credito:
                        SetEtiquetas(container.CreditoDAO.Items);
                        break;
                    default:
                        break;
                }
            }
            return isTipoEtiquetaSelected;
        }
        private bool CheckSePuedeAnadir()
        {
            bool respuesta = CB_Etiqueta.SelectedIndex > -1 &&
                CB_TipoPlan.SelectedIndex > -1;
            B_Agregar.IsEnabled = respuesta;
            return respuesta;
        }
        #endregion
        #region Auxiliares
        private void SetEtiquetas(IEnumerable<Etiqueta> etiquetas)
        {
            CB_Etiqueta.ItemsSource = etiquetas;
            int count = 0;
            foreach (Etiqueta item in etiquetas)
            {
                count++;
            }
            CB_Etiqueta.PlaceholderText = count > 0 ? "Seleccione una opción" : "Vacío";
        }
        private void TryProcessTransaction()
        {
            if (CheckSePuedeAnadir())
            {
                Etiqueta et = CB_Etiqueta.SelectedItem as Etiqueta;
                TipoPlan tp = (TipoPlan)CB_TipoPlan.SelectedIndex;
                bool emf = TS_EsMesFijo.IsOn;
                double m = NB_Meta.Value is double.NaN ? 0 : NB_Meta.Value;
                if (parameters.IsEditMode)
                {
                    TryEditPlan(et, tp, emf, m);
                }
                else
                {
                    TryAddPlan(et, tp, emf, m);
                }
                if (((this.Parent as Frame)?.Parent as FlyoutPresenter)?.Parent is Popup)
                {
                    (((this.Parent as Frame)?.Parent as FlyoutPresenter)?.Parent as Popup).IsOpen = false;
                }
            }
        }
        private void TryAddPlan(Etiqueta et,TipoPlan tp,bool emf,double m)
        {
            if (!planAdded)
            {
                container.PlanDAO.Add(et,tp,emf,m);
                planAdded = true;
            }
        }
        private void TryEditPlan(Etiqueta et, TipoPlan tp, bool emf, double m)
        {
            plan.Etiqueta = et;
            plan.Tipo = tp;
            plan.EsMesFijo = emf;
            plan.Meta = m;
        }
        private void NB_Meta_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            lastPressed = e.Key;
        }
        private void NB_Meta_GettingFocus(Windows.UI.Xaml.UIElement sender, Windows.UI.Xaml.Input.GettingFocusEventArgs args)
        {
            lastPressed = null;
        }
        #endregion
    }
}