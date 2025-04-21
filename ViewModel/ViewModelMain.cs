using AdministradorDeTareas.View;
using System.Windows;
using System.Windows.Input;

namespace AdministradorDeTareas.ViewModel
{
    public class ViewModelMain : ViewModelBase
    {
        private ViewModelBase _currentChildView;
        public ICommand ShowEditActionsCommand { get; }
        public ICommand ShowDashBoardCommand { get; }
        public ICommand ShowViewOptionsCommand { get; }
        public ICommand OpenGithubProfile {  get; }
        public ICommand LogOutCommand { get; }
       
        public string? userName
        {
            get { return ViewModelBase.GetCurrentUser().FullName; }

        }

        public string StatisticsTitle
        {
            get
            {
                if (ViewModelBase.GetCurrentUser().IsAdmin == true)
                {
                    return "Вся статистика";
                }
                else
                {
                    return "Моя Статистика";

                }
            }
        }

        public string TasksTitle
        {
            get
            {
                if (ViewModelBase.GetCurrentUser().IsAdmin == true)
                {
                    return "Управление задачами";
                }
                else
                {
                    return "Мои задачи";

                }
            }
        }

        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }
        
        public ViewModelMain()
        {
            ShowViewOptionsCommand = new ViewModelCommand(ExecuteShowViewOptionsCommand);
            ShowEditActionsCommand = new ViewModelCommand(ExecuteShowEditActionsCommand);
            ShowDashBoardCommand = new ViewModelCommand(ExecuteShowDashBoardCommand);
            LogOutCommand = new ViewModelCommand(ExecuteLogOutCommand);
           
            ExecuteShowDashBoardCommand(null);
        }
       
        private void ExecuteShowEditActionsCommand(object obj)
        {
            CurrentChildView = new ViewModelEditActions();
        }
        private void ExecuteShowViewOptionsCommand(object obj)
        {
            CurrentChildView = new ViewModelUserAccount();
        }
        private void ExecuteShowDashBoardCommand(object? obj)
        {
            CurrentChildView = new ViewModelDashBoard();
        }
        private void ExecuteLogOutCommand(object obj)
        {
            ViewModelBase.SetCurrentUser(null);
            ViewLogin viewLogin = new ViewLogin();
            viewLogin.Show();
            Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
            if (window != null)
            {
                window.Close();
            }
        }

    }
}
