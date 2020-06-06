using JevoGastosCore;
using System;
using System.Collections.Generic;
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

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace JevoGastosUWP.ControlesPersonalizados
{
    public sealed partial class AddEtiquetaForm : UserControl
    {
        public static readonly DependencyProperty TipoEtiquetaProperty =
            DependencyProperty.Register(
                "TipoEtiqueta",
                typeof(TipoEtiqueta),
                typeof(UserControl),
                new PropertyMetadata(default(TipoEtiqueta)));
        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register(
                "Container",
                typeof(GastosContainer),
                typeof(UserControl),
                new PropertyMetadata(default(GastosContainer)));
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(
                "HeaderBackground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(1,1,1,1)))
                );
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register(
                "HeaderForeground",
                typeof(Brush),
                typeof(UserControl),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(1, 0, 0, 0)))
                );
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                "HeaderText",
                typeof(string),
                typeof(UserControl),
                new PropertyMetadata("Nueva etiqueta")
                );

        public TipoEtiqueta TipoEtiqueta
        {
            get => (TipoEtiqueta)GetValue(TipoEtiquetaProperty);
            set => SetValue(TipoEtiquetaProperty, value);
        }
        public GastosContainer Container
        {
            get => (GastosContainer)GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }
        public Brush HeaderBackground
        {
            get => (Brush)GetValue(HeaderBackgroundProperty);
            set => SetValue(HeaderBackgroundProperty, value);
        }
        public Brush HeaderForeground
        {
            get => (Brush)GetValue(HeaderForegroundProperty);
            set => SetValue(HeaderForegroundProperty, value);
        }
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }
        public TextBox TextBox
        {
            get => TB_Name;
        }

        public AddEtiquetaForm()
        {
            this.InitializeComponent();
        }

        private void B_Save_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public event RoutedEventHandler Click;
        
    }
}
