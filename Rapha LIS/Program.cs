using Microsoft.Extensions.Configuration;
using Rapha_LIS.Models;
using Rapha_LIS.Presenters;
using Rapha_LIS.Repositories;
using Rapha_LIS.Views;
using static MaterialSkin.Controls.MaterialCheckedListBox;

namespace Rapha_LIS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            string? sqlConnectionString = config.GetConnectionString("DefaultConnection");

            using (SigninView signinView = new SigninView())
            {
                ISigninRepository signinRepository = new UserRepository(sqlConnectionString ?? "");
                SigninPresenter signinPresenter = new SigninPresenter(signinView, signinRepository);

                // Show sign-in form and check if login is successful
                if (signinView.ShowDialog() == DialogResult.OK)
                {
                    // If login is successful, proceed to main form
                    using (var mainForm = new Rapha_LIS.Views.Rapha_LIS())
                    {
                        // Initialize Patient Presenter
                        IPatientControlView patientView = mainForm;
                        IPatientControlRepository patientRepository = new PatientRepository(sqlConnectionString ?? "");

                        // Analytics 
                        IPatientAnalyticsView patientAnalyticsView = mainForm;
                        IAnalyticsRepository analyticsRepository = new PatientRepository(sqlConnectionString ?? "");

                        // Result
                        IPatientResult patientResult = mainForm;
                        IPatientResultRepository patientResultRepository = new PatientRepository(sqlConnectionString ?? "");

                        var testList = new TestListView();
                        ITestListView testListView = testList;
                        ITestListRepository testListRepository = new PatientRepository(sqlConnectionString ?? "");

                        new PatientPresenter(patientView, patientRepository, patientAnalyticsView, analyticsRepository
                                              , patientResult, patientResultRepository, testListView, testListRepository);

                        // Initialize User Presenter
                        IUserControlView userControlView = mainForm;
                        IUserControlRepository userControlRepository = new UserRepository(sqlConnectionString ?? "");
                        new UserPresenter(userControlView, userControlRepository);

                        // Run the application with the main form
                        Application.Run(mainForm);
                    }
                }
                else
                {
                    // If login fails or user closes sign-in form, exit the application
                    Application.Exit();
                }
            }
        }
    }
}