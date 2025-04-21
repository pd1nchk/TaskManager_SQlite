using AdministradorDeTareas.Model;
using LiveCharts;
using LiveCharts.Wpf;
using AdministradorDeTareas.Model.DAO;
using AdministradorDeTareas.View;
using System.Windows.Controls;

namespace AdministradorDeTareas.ViewModel
{
    public class ViewModelDashBoard : ViewModelBase
    {
        public ViewModelDashBoard() {
            PriorityTasksCollection = new SeriesCollection();
            StatusTasksCollection = new SeriesCollection();
            GetTasksFromApi();
        }
        #region Atributos
        private readonly TaskModelDAO _taskModelDao = new TaskModelDAO();
        private List<TaskModel>? _tasks;
        private List<TaskModel>? _pendingTasks;
        private List<TaskModel>? _highPriorityTasks;
        private List<TaskModel>? _lastTaskAdded;

        public List<TaskModel> TasksList
        {
            get
            {
                return _tasks;
            }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(TasksList));
            }
        }

        public List<TaskModel> PendingTasks
        {
            get { return _pendingTasks; }
            set
            {
                _pendingTasks = value;
                OnPropertyChanged(nameof(PendingTasks));
            }
        }
        public List<TaskModel> HighPriorityTasks
        {
            get { return _highPriorityTasks; }
            set
            {
                _highPriorityTasks = value;
                OnPropertyChanged(nameof(HighPriorityTasks));
            }
        }
        public List<TaskModel> LastTaskAdded
        {
            get { return _lastTaskAdded; }
            set
            {
                _lastTaskAdded = value;
                OnPropertyChanged(nameof(LastTaskAdded));
            }
        }        
        public SeriesCollection PriorityTasksCollection { get; set; }
        public SeriesCollection StatusTasksCollection { get; set; }

        private async void GetTasksFromApi() {
            try {
                // все задачи беру из бд
                if (ViewModelBase.GetCurrentUser().IsAdmin == true)
                {
                    TasksList = await _taskModelDao.GetAllAdmin();
                }
                else
                {
                    TasksList = await _taskModelDao.GetAll();
                }
            
               // если есть данные создаем диаграммы и информацию
               if (TasksList != null)
               {
                   CreatePieCharts();
                   ShowTasksInfo();
               }
            }
            catch (Exception ex) {
                CustomMessageBox.ShowCustomMessageBox($"Error in dashboard. Operation could not be completed. message: {ex.Message}");
            }
        }

        private void CreatePieCharts()
        {
            // задачи у которых есть статус
            var tasksByStatus = TasksList.Where(x => x.TaskStatus != null).GroupBy(t => t.TaskStatus.StatusName)
                .Select(c => new { StatusName = c.Key, Count = c.Count() });
            
            // задачи у которых есть приоритет
            var tasksByPriority = TasksList.Where(x => x.Priority != null).GroupBy(t => t.Priority.PriorityStatus)
                              .Select(c => new { PriorityStatus = c.Key, Count = c.Count() });

            // Добавляем сектора в диаграмму Статус
            foreach (var group in tasksByStatus)
            {
                //переводим на русский
                String rusStatus = "";
                if (group.StatusName == "Pending") {
                    rusStatus = "Ожидает";
                } else if (group.StatusName == "In Progress") {
                    rusStatus = "В прогрессе";
                } else {
                    rusStatus = "Завершена";
                }
                  
                StatusTasksCollection.Add(new PieSeries
                {
                    Title = rusStatus,
                    Values = new ChartValues<int> { group.Count },
                });
            }
            // Добавляем сектора в диаграмму Приоритет
            foreach (var group in tasksByPriority)
            {
                //переводим на русский
                String rusPriority = "";
                if (group.PriorityStatus == "Low")
                {
                    rusPriority = "Низкий";
                }
                else if (group.PriorityStatus == "High")
                {
                    rusPriority = "Высокий";
                }
                else
                {
                    rusPriority = "Средний";
                }
                PriorityTasksCollection.Add(new PieSeries
                {
                    Title = rusPriority,
                    Values = new ChartValues<int> { group.Count }
                });
            }
        }
        private void ShowTasksInfo() {
            try {
                // задачи в статусе ожидания
                var pendingTasks = TasksList.Where(x => x.StatusID == 1).Reverse().ToList();
                
                // с высоким приоритетом
                var highPriorityTasks = TasksList.Where(x => x.PriorityID == 3).Reverse().ToList();
                
                //переворачиваем лист чтобы получить последние 3 задачи 
                var lasTaskAdded = TasksList.ToList();
                lasTaskAdded.Reverse();
                
                HighPriorityTasks = highPriorityTasks.Take(3).ToList();
                LastTaskAdded = lasTaskAdded.Take(3).ToList();
                PendingTasks = pendingTasks.Take(3).ToList();
            }
            catch (Exception ex) {
                CustomMessageBox.ShowCustomMessageBox($"Error in dashboard. Operation could not be completed 'Load info tasks'. message: {ex.Message}");
            }
        }
        #endregion Metodos
    }
}
