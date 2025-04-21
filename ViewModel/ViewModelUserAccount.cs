using System.Windows.Input;
using AdministradorDeTareas.Model;
using AdministradorDeTareas.Model.DAO;
using AdministradorDeTareas.View;

namespace AdministradorDeTareas.ViewModel
{
    public class ViewModelUserAccount : ViewModelBase
    {

        public ViewModelUserAccount() {
            ShowEditCredentialsCommand = new ViewModelCommand(ExecuteShowEditCredentials);
            ShowChangePasswordCommand = new ViewModelCommand(ExecuteShowChangePassword);
            GetAccoutCredentials();
        }
       
        private UsersModel _CurrentUser;
        public UsersModel CurrentUser
        {
            get { return _CurrentUser; }
            set
            {
                _CurrentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }
        public ICommand ShowEditCredentialsCommand { get; }
        public ICommand ShowChangePasswordCommand { get; }

        public string? role
        {
            get {
                if (ViewModelBase.GetCurrentUser().IsAdmin == true)
                {
                    return "Администратор";
                }
                else
                {
                    return "Пользователь";
                }
            }

        }


        private void ExecuteShowChangePassword(object obj) {
            ViewChangePassword viewChangePassword = new ViewChangePassword();
            viewChangePassword.ShowDialog();
        }

        private void ExecuteShowEditCredentials(object obj) {
            try {
                ViewEditUserCredentials viewEditUserCredentials = new ViewEditUserCredentials();
                ViewModelEditCredentials viewModelEditCredentials = viewEditUserCredentials.DataContext as ViewModelEditCredentials;
                if (viewModelEditCredentials != null) {
                    viewModelEditCredentials.AccountCredentialsEdited += GetAccoutCredentials;
                    viewEditUserCredentials.ShowDialog();
                }
            }
            catch (Exception ex) {
                CustomMessageBox.ShowCustomMessageBox("Ошибка");
            }
        }

        public void GetAccoutCredentials() {
            CurrentUser = ViewModelBase.GetCurrentUser();
        }
    }

}
