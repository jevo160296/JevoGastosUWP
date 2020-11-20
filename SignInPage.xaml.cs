using JevoCrypt;
using JevoCrypt.Classes;
using JevoGastosUWP.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        #region Clases
        public class Parameters
        {
            public UsersContainer UsersContainer { get; set; }
            public LaunchActivatedEventArgs e { get; set; }
            public Parameters(UsersContainer usersContainer, LaunchActivatedEventArgs e)
            {
                this.UsersContainer = usersContainer;
                this.e = e;
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
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            public VisibleBool(bool value = false)
            {
                this.value = value;
            }
        }
        #endregion
        #region Parametros
        private UsersContainer UsersContainer;
        private ObservableCollection<User> Users;
        private VisibleBool IncorrectSignIn = new VisibleBool();
        private int actualState = -1;
        #endregion

        public SignInPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UsersContainer = ((Parameters)(e.Parameter)).UsersContainer;
            Users = UsersContainer.UserDAO.Items;
            IncorrectSignIn.PropertyChanged += IncorrectSignIn_PropertyChanged;
            this.SizeChanged += SignInPage_SizeChanged;
            CheckSelectedUser();
            CheckIncorrectPassword();
        }

        private void SignInPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = ((FrameworkElement)sender).ActualWidth;
            if (width>1198)
            {
                if (actualState != 2)
                {
                    actualState = 2;
                    VisualStateManager.GoToState(UC_TextControl, "Text_LargeState", false);
                    VisualStateManager.GoToState(this, "Grid_LargeState", false);
                    //LargeBreakPoint
                }
            }
            else if(width>859)
            {
                if (actualState != 1)
                {
                    actualState = 1;
                    VisualStateManager.GoToState(UC_TextControl, "Text_MediumState", false);
                    VisualStateManager.GoToState(this, "Grid_MediumState", false);
                    //MediumBreakPoint
                }
            }
            else
            {
                if (actualState != 0)
                {
                    actualState = 0;
                    VisualStateManager.GoToState(UC_TextControl, "Text_SmallState", false);
                    VisualStateManager.GoToState(this, "Grid_SmallState", false);
                    //SmallBreakPoint
                }
            }
        }

        private void IncorrectSignIn_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckIncorrectPassword();
        }

        private void AddUser_Clic(object sender, RoutedEventArgs e)
        {
            UserForm.Parameters parameters = new UserForm.Parameters(UsersContainer);
            if (Frame_AddUser.Content is null)
            {
                Frame_AddUser.Navigate(typeof(UserForm), parameters, new SuppressNavigationTransitionInfo());
                ((UserForm)Frame_AddUser.Content).CloseRequested += SignInPage_CloseRequested;
            }
        }
        private void SignInPage_CloseRequested(bool f=false)
        {
            B_AddUser.Flyout.Hide();
        }
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string userName = ((User)(CB_Username.SelectedItem))?.UserName;
            string password = PB_Password.Password;
            bool keepSignIn = CB_KeepSignIn.IsChecked ?? false;

            User user= UsersContainer.UserDAO.SignIn(userName, password, keepSignIn);
            if (user is null)
            {
                IncorrectSignIn.Value = true;
            }
            else
            {
                IncorrectSignIn.Value = false;
                this.Frame.Navigate(typeof(LoadingPage), new LoadingPage.Parameters(user,UsersContainer, null), new DrillInNavigationTransitionInfo());
            }
        }
        private void CB_Username_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSelectedUser();
        }
        private void CheckSelectedUser()
        {
            if (CB_Username.SelectedIndex == -1)
            {
                B_SignIn.IsEnabled = false;
            }
            else
            {
                B_SignIn.IsEnabled = true;
            }
        }
        private void CheckIncorrectPassword()
        {
            if (IncorrectSignIn.Value)
            {
                Grid.CornerRadius = new CornerRadius(8, 8, 0, 0);
            }
            else
            {
                Grid.CornerRadius = new CornerRadius(8, 8, 8, 8);
            }
        }
    }
}
