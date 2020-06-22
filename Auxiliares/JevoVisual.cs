using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace JevoGastosUWP.Auxiliares
{
    public static class JevoVisual
    {
        public static void GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            Popup popup = (Popup)sender;
            popup.Opacity = 0;
            if (popup.IsOpen)
            {
                Opened(popup, new object());
            }
            popup.Opened -= Opened;
            popup.Opened += Opened;
            (popup.Child as FrameworkElement).SizeChanged -= RefreshCenteredPopup;
            (popup.Child as FrameworkElement).SizeChanged += RefreshCenteredPopup;
        }
        private static void Opened(object sender, object e)
        {
            Popup popup = sender as Popup;
            popup.Opacity = 0;
            ShowCenteredPopup(popup);
        }
        private static void ShowCenteredPopup(Popup popup)
        {
            popup.IsOpen = true;
            RefreshCenteredPopup(popup, 0);
            popup.Opacity = 1;
        }
        private static void RefreshCenteredPopup(object sender, object e)
        {
            Popup popup;
            if (sender is Popup)
            {
                popup = sender as Popup;
                //popup.HorizontalOffset = -(popup.Child as FrameworkElement).ActualWidth /2;
                //popup.VerticalOffset = -(popup.Child as FrameworkElement).ActualHeight / 2;
            }
            else
            {
                popup = (sender as FrameworkElement).Parent as Popup;
                //popup.HorizontalOffset = -(sender as FrameworkElement).ActualWidth / 2;
                //popup.VerticalOffset = -(sender as FrameworkElement).ActualHeight / 2;
            }
            popup.HorizontalOffset = (popup.Parent as FrameworkElement).ActualWidth/2-(popup.Child as FrameworkElement).ActualWidth/2;
            popup.VerticalOffset = (popup.Parent as FrameworkElement).ActualHeight /2 -(popup.Child as FrameworkElement).ActualHeight / 2;
        }
    }
}
