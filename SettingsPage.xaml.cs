using JevoGastosCore.Model;
using JevoGastosCore.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
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
        #endregion
        #region Atributos
        VisibilityHandler NonSelectedVisibility = new VisibilityHandler();
        #endregion
        private PayDaysDAO PayDaysDAO;
        public class Parameters
        {
            public PayDaysDAO PayDaysDAO;
            public Parameters(PayDaysDAO payDaysDAO)
            {
                PayDaysDAO = payDaysDAO;
            }
        }
        public SettingsPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Parameters par = (Parameters)e.Parameter;
            this.PayDaysDAO = par.PayDaysDAO;
            PayDaysDAO.Items.CollectionChanged += Items_CollectionChanged;
            this.NonSelectedVisibility.Visibility =
                PayDaysDAO.NonSelectedItems.Count == 0 ?
                Visibility.Collapsed :
                Visibility.Visible;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    this.NonSelectedVisibility.Visibility =
                        PayDaysDAO.NonSelectedItems.Count == 1 ?
                        Visibility.Collapsed :
                        Visibility.Visible;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    this.NonSelectedVisibility.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void AddDay_ItemClick(object sender, ItemClickEventArgs e)
        {
            PayDay dayToAdd = (PayDay)e.ClickedItem;
            PayDaysDAO.Add(dayToAdd);
        }
        private void RemoveDay_ItemClick(object sender, ItemClickEventArgs e)
        {
            PayDay dayToRemove = (PayDay)e.ClickedItem;
            PayDaysDAO.Remove(dayToRemove);
        }
    }
}
