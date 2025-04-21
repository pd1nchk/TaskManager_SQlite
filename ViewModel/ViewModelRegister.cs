using AdministradorDeTareas.Model;
using AdministradorDeTareas.Model.DAO;
using AdministradorDeTareas.View;
using System.Windows;
using System.Windows.Input;

namespace AdministradorDeTareas.ViewModel
{
    public class ViewModelRegister : ViewModelBase
    {
        readonly UsersModelDAO usersModelDAO = new UsersModelDAO();
        readonly TaskModelDAO taskModelDAO = new TaskModelDAO();
        private string? _userName;
        private string? _password;
        private string? _email;
        private string? _name;
        public string? userName {
            get { return _userName; }
            set 
            { 
                  this._userName = value;
                  OnPropertyChanged(nameof(userName));
            } 
        }
        public string? password { 
            get { return _password; } 
            set {
                this._password = value;
                OnPropertyChanged(nameof(password));
            } 
        }
        public string? email { 
            get { return _email; } 
            set 
            { 
                this._email = value;
                OnPropertyChanged(nameof(email));
            } 
        }
        public string? name { 
            get { return _name; } 
            set 
            { 
                this._name = value;
                OnPropertyChanged(nameof(name));
            } 
        }
        public ViewModelRegister() 
        {
            ReturnLoginPageCommand = new ViewModelCommand(ExecuteReturnLoginPage);       
            RegisterCommand = new ViewModelCommand(ExecuteRegister);
        }
        public ICommand ReturnLoginPageCommand { get; }
        public ICommand RegisterCommand { get; }

        public void ExecuteReturnLoginPage(object obj)
        {
            ReturnLoginPage();
        }
      
        public void ExecuteRegister(object obj)
        {
            Register();
        }
        public void ReturnLoginPage()
        {
            ViewLogin loginView = new ViewLogin();
            loginView.Show();
            Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.Close();
            }
        }
        private async void Register() {
            try {
                if (await isValidCredentialsAsync()) {
                    UsersModel newUser = new UsersModel();
                    newUser.UserName = this.userName;
                    newUser.FullName = this.name;
                    newUser.Email = this.email;
                    newUser.PasswordHash = this.password;
                    newUser.IsAdmin = false;
                    if (await usersModelDAO.Add(newUser)) {
                        // Переходим на логин
                          ViewLogin loginView = new ViewLogin();
                          loginView.Show();
                          closeActualWindow();
                    }
                }
            }
            catch (Exception ex) {
                CustomMessageBox.ShowCustomMessageBox("Ошибка: "+ex.Message);
            }
        }
        private async Task<bool> isValidCredentialsAsync()
        {
            if (!string.IsNullOrWhiteSpace(password)
                && !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(email)
                && !string.IsNullOrWhiteSpace(name))
            {
                if (!await usersModelDAO.Exist(this.userName))
                {
                    if (this.password.Length > 4)
                    {
                        return true;
                    }

                    CustomMessageBox.ShowCustomMessageBox("Пароль должен быть минимум 4 символа");
                    return false;
                }

                CustomMessageBox.ShowCustomMessageBox("Такой пользователь уже есть");
                return false;
            }
            CustomMessageBox.ShowCustomMessageBox("Введите все поля");
            return false;
        }
        private void closeActualWindow()
        {
            Window? window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
