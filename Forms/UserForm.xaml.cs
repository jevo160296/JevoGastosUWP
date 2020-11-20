using JevoCrypt;
using JevoCrypt.Classes;
using JevoCrypt.ModelView;
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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace JevoGastosUWP.Forms
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class UserForm : Page
    {
        #region Clases
        public enum TipoInicialización
        {
            add,
            changepassword,
            changeusername,
            deleteaccount
        }
        public class Parameters
        {
            public UsersContainer UsersContainer { get; set; }
            public GastosContainer GastosContainer { get; set; }
            public User User { get; set; }
            public TipoInicialización TipoInicialización { get; set; }
            public bool CerrarSesion { get; set; } = false;
            
            public Parameters(
                UsersContainer usersContainer,
                User user=null,
                TipoInicialización tipoInicialización=TipoInicialización.add,
                GastosContainer gastosContainer=null,
                bool cerrarSesion=false
                )
            {
                this.UsersContainer = usersContainer;
                this.User = user;
                this.TipoInicialización = tipoInicialización;
                this.GastosContainer = gastosContainer;
                this.CerrarSesion = cerrarSesion;
            }
        }
        #endregion
        #region Atributos
        private Parameters parameters;
        #endregion
        #region Events
        public delegate void CloseFormRequestedHandler(bool closeSesion);
        public event CloseFormRequestedHandler CloseRequested;
        #endregion
        public UserForm()
        {
            this.InitializeComponent();
        }
        #region Metodos
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.parameters = (Parameters)(e.Parameter);
            ConfigureView();
        }

        private void ConfigureView()
        {
            TB_UserName.IsEnabled = this.parameters.User is null;
            TB_UserName.Text = this.parameters.User?.UserName ?? "";
            
            switch (this.parameters.TipoInicialización)
            {
                case TipoInicialización.add:
                    TB_UserName.Visibility = Visibility.Visible;
                    PB_Contraseña.Visibility = Visibility.Visible;
                    TB_NUserName.Visibility = Visibility.Collapsed;
                    PB_NContraseña.Visibility = Visibility.Collapsed;
                    break;
                case TipoInicialización.changepassword:
                    TB_UserName.Visibility = Visibility.Visible;
                    PB_Contraseña.Visibility = Visibility.Visible;
                    TB_NUserName.Visibility = Visibility.Collapsed;
                    PB_NContraseña.Visibility = Visibility.Visible;
                    break;
                case TipoInicialización.changeusername:
                    TB_UserName.Visibility = Visibility.Visible;
                    PB_Contraseña.Visibility = Visibility.Visible;
                    TB_NUserName.Visibility = Visibility.Visible;
                    PB_NContraseña.Visibility = Visibility.Collapsed;
                    break;
                case TipoInicialización.deleteaccount:
                    TB_UserName.Visibility = Visibility.Visible;
                    PB_Contraseña.Visibility = Visibility.Visible;
                    TB_NUserName.Visibility = Visibility.Collapsed;
                    PB_NContraseña.Visibility = Visibility.Collapsed;
                    break;
                default:
                    TB_UserName.Visibility = Visibility.Collapsed;
                    PB_Contraseña.Visibility = Visibility.Collapsed;
                    TB_NUserName.Visibility = Visibility.Collapsed;
                    PB_NContraseña.Visibility = Visibility.Collapsed;
                    break;
            }
            if (!TB_UserName.IsEnabled)
            {
                PB_Contraseña.Focus(FocusState.Programmatic);
            }
        }

        private async void Listo_Clic(object sender, RoutedEventArgs e)
        {
            switch (this.parameters.TipoInicialización)
            {
                case TipoInicialización.add:
                    parameters.UsersContainer.UserDAO.AddUser(TB_UserName.Text, PB_Contraseña.Password);
                    RequestCloseForm();
                    break;
                case TipoInicialización.changepassword:
                    bool passwordChanged = 
                        parameters.
                        UsersContainer.
                        UserDAO.
                        ChangePass(TB_UserName.Text, PB_Contraseña.Password, PB_NContraseña.Password);
                    if (passwordChanged)
                    {
                        PB_Contraseña.Password = "";
                        PB_NContraseña.Password = "";
                        TB_Message.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkGreen);
                        TB_Message.Text = "La contraseña se ha cambiado correctamente";
                        TB_Message.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PB_Contraseña.Password = "";
                        PB_NContraseña.Password = "";
                        ShowMessage("Contraseña incorrecta", Windows.UI.Colors.DarkRed);
                    }
                    break;
                case TipoInicialización.changeusername:
                    bool userChanged=false;
                    try
                    {
                        userChanged =
                        parameters.
                        GastosContainer.
                        ChangeUserName(TB_UserName.Text, PB_Contraseña.Password, TB_NUserName.Text);
                    }
                    catch (GastosContainer.FileNotUserException exception)
                    {
                        ShowMessage(exception.Message,Windows.UI.Colors.YellowGreen);
                    }
                    catch(GastosContainer.PasswordException exception)
                    {
                        ShowMessage(exception.Message, Windows.UI.Colors.DarkRed);
                    }
                    catch(UserDAO.UserExistsException exception)
                    {
                        ShowMessage(exception.Message, Windows.UI.Colors.YellowGreen);
                    }
                    if (userChanged)
                    {
                        TB_UserName.Text = this.parameters.User.UserName;
                        PB_Contraseña.Password = "";
                        TB_NUserName.Text = "";
                        ShowMessage("Nombre de usuario cambiado correctamente",Windows.UI.Colors.DarkGreen);
                    }
                    else
                    {
                        PB_Contraseña.Password = "";
                        TB_NUserName.Text = "";
                    }
                    break;
                case TipoInicialización.deleteaccount:
                    bool validPassword = this.parameters.User.IsCorrectPassword(PB_Contraseña.Password);
                    if (validPassword)
                    {
                        CD_MultiUser.Title = "Guardar datos de usuario";
                        CD_MultiUser.Content = "¿Desea conservar los datos del usuario?. " +
                            "De esta forma se podrá recuperar la cuenta posteriormente creando un usuario con el mismo nombre.";
                        CD_MultiUser.PrimaryButtonText = "Si";
                        CD_MultiUser.SecondaryButtonText = "No";
                        CD_MultiUser.CloseButtonText = "Cancelar";
                        CD_MultiUser.DefaultButton = ContentDialogButton.Close;
                        CD_MultiUser.Visibility = Visibility.Visible;
                        ContentDialogResult result1 = await CD_MultiUser.ShowAsync(), result2;
                        switch (result1)
                        {
                            case ContentDialogResult.None:
                                RequestCloseForm();
                                break;
                            case ContentDialogResult.Primary:
                                CD_MultiUser.Content = "Puede recuperar los datos posteriormente creando una nueva cuenta con el mismo nombre de usuario (" +
                                    parameters.User.UserName + ").";
                                break;
                            case ContentDialogResult.Secondary:
                                CD_MultiUser.Content = "Esta acción eliminará también los datos asociados. No se podrán recuperar.";
                                break;
                            default:
                                break;
                        }
                        if (result1==ContentDialogResult.Primary||result1==ContentDialogResult.Secondary)
                        {
                            CD_MultiUser.Title = "¿Seguro que desea eliminar esta cuenta?";
                            CD_MultiUser.PrimaryButtonText = "Si";
                            CD_MultiUser.SecondaryButtonText = "No";
                            CD_MultiUser.CloseButtonText = "";
                            CD_MultiUser.DefaultButton = ContentDialogButton.Secondary;
                            CD_MultiUser.Visibility = Visibility.Visible;
                            result2 = await CD_MultiUser.ShowAsync();
                            switch (result2)
                            {
                                case ContentDialogResult.None:
                                    RequestCloseForm();
                                    break;
                                case ContentDialogResult.Primary:
                                    DeleteUser();
                                    if (result1==ContentDialogResult.Secondary)
                                    {
                                        DeletDatabase();
                                    }
                                    RequestCloseForm(true);
                                    break;
                                case ContentDialogResult.Secondary:
                                    RequestCloseForm();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        ShowMessage("Contraseña inválida", Windows.UI.Colors.DarkRed);
                    }
                    break;
                default:
                    break;
            }
        }
        public void RequestCloseForm(bool closeSesion=false)
        {
            TB_UserName.Text = "";
            PB_Contraseña.Password = "";
            TB_NUserName.Text = "";
            PB_NContraseña.Password = "";
            CloseRequested?.Invoke(closeSesion);
        }
        private void ShowMessage(string message,Windows.UI.Color color)
        {
            TB_Message.Foreground = new SolidColorBrush(color);
            TB_Message.Text = message;
            TB_Message.Visibility = Visibility.Visible;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!TB_UserName.IsEnabled)
            {
                PB_Contraseña.Focus(FocusState.Programmatic);
            }
        }
        private void DeletDatabase()
        {
            this.parameters.CerrarSesion = true;
            this.parameters.GastosContainer.DeleteDB();
        }
        private void DeleteUser()
        {
            this.parameters.CerrarSesion = true;
            this.parameters.UsersContainer.UserDAO.RemoveUser(this.parameters.User.UserName);
        }
        #endregion
    }
}