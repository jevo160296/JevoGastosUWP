using JevoGastosCore;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace JevoGastosUWP.ControlesPersonalizados
{
    public sealed partial class AddEtiquetaForm : UserControl
    {
        #region DependencyProperties
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
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(1, 1, 1, 1)))
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
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                "Label",
                typeof(string),
                typeof(UserControl),
                new PropertyMetadata("Guardar")
                );
        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register(
                "ErrorText",
                typeof(string),
                typeof(UserControl),
                new PropertyMetadata("Error")
                );
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(string),
                typeof(UserControl),
                new PropertyMetadata("Nombre")
                );
        public static readonly DependencyProperty IsErrorRaisedProperty =
            DependencyProperty.Register(
                "IsErrorRaised",
                typeof(string),
                typeof(UserControl),
                new PropertyMetadata(false)
                );
        #endregion
        #region Properties
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
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public TextBox TextBox
        {
            get => TB_Name;
        }
        public bool IsErrorRaised
        {
            get => (bool)GetValue(IsErrorRaisedProperty);
            set => SetValue(IsErrorRaisedProperty, value);
        }
        #endregion

        public AddEtiquetaForm()
        {
            this.InitializeComponent();
        }

        private void B_Save_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
            TB_Name.Text = "";
        }

        public event RoutedEventHandler Click;
        
    }
}
