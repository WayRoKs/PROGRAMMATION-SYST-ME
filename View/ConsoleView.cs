using PROGRAMMATION_SYST_ME.Model;
using PROGRAMMATION_SYST_ME.ViewModel;
using System;
using System.Reflection.Metadata.Ecma335;
public enum errorCode
    {
        SUCCESS = 0,
        NORMAL_EXIT = 1,
        INPUT_ERROR = 2,
        SOURCE_ERROR = 3
    };
namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Console view for the user (interface)
    /// </summary>
    class ConsoleView
    {
        private readonly UserInteractionViewModel userInteract = new UserInteractionViewModel();
        public errorCode error { get; set; }
        /// <summary>
        /// Method that allows for initial selection between executing backup jobs or editing backup jobs
        /// </summary>
        public void InitialChoice() 
        {
            Console.Clear();
            foreach (BackupJobDataModel job in userInteract.BackupJobsData)
            {
                Console.WriteLine(job.Id + 1 + " -> " + job.Name);
            }
            Console.WriteLine("Choose between U (Update backup jobs) or E (Execute backup jobs) or Q (Quit) : ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "U":
                    UpdateChoice();
                    break;
                case "E":
                    ExecuteChoice(); 
                    break;
                case "Q":
                    error = errorCode.NORMAL_EXIT;
                    break;
                default:
                    error = errorCode.INPUT_ERROR;
                    break;
            }
            if (error == errorCode.SUCCESS)
                InitialChoice();
            else 
                PrintError(error);
                InitialChoice();
        }
        /// <summary>
        /// Method that asks the user for the backup job(s) to execute, then executes the selected backup job(s)
        /// </summary>
        public void ExecuteChoice() 
        {
            Console.WriteLine("Select the backup jobs to execute (example : 1-3 or 1;3) or Q to Quit : ");
            var selection = Console.ReadLine();
            error = userInteract.ExecuteJob(selection);
        }
        /// <summary>
        /// Method that allows the user to select which backup job he'd like to modify
        /// </summary>
        public void UpdateChoice() 
        {
            Console.WriteLine("Select the backup job to modify (Between 1 to 5) : ");
            int jobChoice;
            try // Convertion from string to int is risky
            {
                jobChoice = int.Parse(Console.ReadLine()) - 1;
            } catch(System.FormatException)
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            if (!(jobChoice >= 0 && jobChoice < 5))
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            Console.WriteLine("Select what you want to change : ");
            ShowParam(jobChoice);
            Console.WriteLine("Q -> Quit");
            var change = Console.ReadLine();

            // Utilisation d'une méthode pour la lisibilité de la condition
            if (!IsValidInputChange(change))
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            if (change == "Q")
                return;
            Console.Write("New value : ");
            if (change == "T")
                Console.WriteLine(" (0 for full backup or 1 for differencial backup)");
            
            var newValue = Console.ReadLine();
            if (!IsValidNewValue(newValue, change))
            {
                error = errorCode.INPUT_ERROR;
                return;
            }
            
            error = userInteract.UpdateJob(jobChoice, change, newValue);
            ShowParam(jobChoice);
            Console.WriteLine("Confirm : (M to Modify or anything else to confirm)");
            if (Console.ReadLine() == "M")
            {
                Console.Clear();
                ShowParam(jobChoice);
                UpdateChoice();
            }
        }
        /// <summary>
        /// Private method that shows the current backup job's properties
        /// </summary>
        /// <param name="jobChoice"></param>
        private void ShowParam(int jobChoice)
        {
            Console.WriteLine($"N -> Name : {userInteract.BackupJobsData[jobChoice].Name}");
            Console.WriteLine($"S -> Source path : {userInteract.BackupJobsData[jobChoice].Source}");
            Console.WriteLine($"D -> Destination path : {userInteract.BackupJobsData[jobChoice].Destination}");
            Console.WriteLine("T -> Type : {0}", userInteract.BackupJobsData[jobChoice].Type == 0 ? "Full backup" : "Differential backup");
        }
        /// <summary>
        /// Private method to handle errors
        /// </summary>
        /// <param name="errorCode"></param>
        private void PrintError(errorCode error)
        {
            switch (error)
            {
                case errorCode.NORMAL_EXIT: 
                    Console.WriteLine("Successful exit");
                    Environment.Exit(0);
                    break;
                case errorCode.INPUT_ERROR: FormatError("Invalid input"); break;
                case errorCode.SOURCE_ERROR: FormatError("Source directory not found"); break;
            }
        }
        /// <summary>
        /// Formats the error and displays it
        /// </summary>
        /// <param name="msg"></param>
        private void FormatError(string msg)
        {
            Console.WriteLine($"Error {error} : {msg}");
            Console.ReadKey();
        }
        private bool IsValidInputChange(string change) => (change == "N" || change == "S" || change == "D" || change == "T" || change == "Q");
        private bool IsValidNewValue(string newValue, string change) => newValue == "" || (change == "T" && !(newValue == "0" || newValue == "1"));
    }
}
