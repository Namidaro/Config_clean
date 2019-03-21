using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using BEMN.Errors;

namespace UniconGS
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly string _errorFilePath = System.Windows.Forms.Application.StartupPath + @"\ELog.lg";
        private ErrorLookUp _errorer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var log = ErrorLookUp.Open(_errorFilePath);
            _errorer = log ?? new ErrorLookUp();


            UniconGsBootstrapper bootstrapper = new UniconGsBootstrapper();
            bootstrapper.Run();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (MessageBox.Show(
                    "Во время работы программы произошла непредвиденная ошибка.\r\nСохранить отчет об ошибке?",
                    "Непредвиденная ошибка", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _errorer.Errors.Add(new ErrorInfo(DateTime.Now, e.Exception, Assembly.GetAssembly(typeof(App)),
                    "БЭМН", AssemblyVersion));
                if (_errorer.Save(_errorFilePath))
                    MessageBox.Show("Отчет об ошибке был успешно создан", "Отчет об ошибке", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                else
                    MessageBox.Show("Во время сохранения отчета об ошибках произошла ошибка", "Ошибка создания отчета",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
            e.Handled = true;
            Shutdown();
        }

        #region AssemblyInfo

        private string AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion
    }
}