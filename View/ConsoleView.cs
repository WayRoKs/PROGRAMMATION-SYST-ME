using PROGRAMMATION_SYST_ME.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace PROGRAMMATION_SYST_ME.View
{
    class ConsoleView
    {
        private readonly UserInteractionViewModel userInteract;
        private int errorCode;
        public ConsoleView()
        {
            userInteract = new UserInteractionViewModel();
        }
        public void InitialChoice() // offer a choice between update backup jobs or execute backup jobs
        {
            Console.Clear();
            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine(i + 1 + " -> " + userInteract.backupJobs.Name[i]);
            }
            Console.WriteLine("Choose between U (Update backup jobs) or E (Execute backup jobs) or Q (Quit) : ");
            var choice = Console.ReadLine();
            if (choice.Length == 1)
            {
                if (choice == "U")
                {
                    UpdateChoice();
                }
                else if (choice == "E")
                {
                    ExecuteChoice();
                }
                else if (choice == "Q")
                    errorCode = 1; // Normal exit
                else
                    errorCode = 2;
            }
            else 
                errorCode = 2;
            if (errorCode == 0)
                InitialChoice();
            else 
                PrintError(errorCode);
                InitialChoice();
        }
        public void ExecuteChoice() // ask to select the backup jobs to execute
        {
            Console.WriteLine("Select the backup jobs to execute (example : 1-3 or 1;3) or Q to Quit : ");
            var selection = Console.ReadLine();
            if (!(Regex.IsMatch(selection,@"[1-4]-[2-5]") || 
                Regex.IsMatch(selection,@"([1-5];[1-5]+){1,5}" ) || 
                Regex.IsMatch(selection, @"[1-5]") ||
                selection == "Q"))
            {
                errorCode = 2;
                return;
            }
            if (selection == "Q")
                return;
            errorCode = userInteract.ExecuteJob(selection);
        }
        public void UpdateChoice() // ask to select which backup jobs to modify
        {
            Console.WriteLine("Select the backup job to modify : ");
            int jobChoice = 0;
            try // Convertion from string to int is dangerous
            {
                jobChoice = int.Parse(Console.ReadLine()) - 1;
            } catch(System.FormatException)
            {
                errorCode = 2;
                return;
            }
            if (!(jobChoice >= 0 && jobChoice < 5))
            {
                errorCode = 2;
                return;
            }
            Console.WriteLine("Select what you want to change : ");
            ShowParam(jobChoice);
            Console.WriteLine("Q -> Quit");
            var change = Console.ReadLine();
            if (!(change == "N" || change == "S" || change == "D" || change == "T" || change == "Q"))
            {
                errorCode = 2;
                return;
            }
            if (change == "Q")
                return;
            Console.Write("New value : ");
            if (change == "T")
                Console.WriteLine(" (0 for full backup or 1 for differencial backup)");
            var newValue = Console.ReadLine();
            if (newValue == "" || (change == "T" && !(newValue == "0" || newValue == "1")))
            {
                errorCode = 2;
                return;
            }
            errorCode = userInteract.UpdateJob(jobChoice, change, newValue);
            ShowParam(jobChoice);
            Console.WriteLine("Confirm : (M to Modify or anything else to confirm)");
            if (Console.ReadLine() == "M")
            {
                Console.Clear();
                ShowParam(jobChoice);
                UpdateChoice();
            }
        }
        private void ShowParam(int param)
        {
            Console.WriteLine($"N -> Name : {userInteract.backupJobs.Name[param]}");
            Console.WriteLine($"S -> Source path : {userInteract.backupJobs.Source[param]}");
            Console.WriteLine($"D -> Destination path : {userInteract.backupJobs.Destination[param]}");
            Console.WriteLine("T -> Type : {0}", userInteract.backupJobs.Type[param] == 0 ? "Full backup" : "Differential backup");
        }
        private void PrintError(int errorCode) // error handling
        {
            switch (errorCode)
            {
                case 1: Console.WriteLine("Successful exit"); break;
                case 2: FormatError("Invalid input"); break;
                case 3: FormatError(""); break;
            }
        }
        private void FormatError(string msg) // Format error print
        {
            Console.WriteLine($"Error {errorCode} : {msg}");
        }
    }
}
