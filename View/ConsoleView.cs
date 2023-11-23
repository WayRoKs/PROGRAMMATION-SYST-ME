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
        }
        public void ExecuteChoice() // ask to select the backup jobs to execute
        {
            Console.WriteLine("Select the backup jobs to execute (example : 1-3 or 1;3) : ");
            var selection = Console.ReadLine();
            if (!(Regex.IsMatch(selection,@"[1-4]-[2-5]") || 
                Regex.IsMatch(selection,@"([1-5];[1-5]+){1,5}" ) || 
                Regex.IsMatch(selection, @"[1-5]")))
            {
                errorCode = 2;
                return;
            }
            errorCode = userInteract.ExecuteJob(selection);
        }
        public void UpdateChoice() // ask to select which backup jobs to modify
        {
            Console.WriteLine("Select the backup job to modify : ");
            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine(i + 1 + " -> " + userInteract.backupJobs[i].Name);
            }
            int jobChoice = 0;
            try // Convertion from string to int is dangerous
            {
                jobChoice = int.Parse(Console.ReadLine());
            } catch(System.FormatException e)
            {
                errorCode = 2;
                return;
            }
            if (!(jobChoice >= 1 && jobChoice <= 5))
            {
                errorCode = 2;
                return;
            }
            Console.WriteLine("Select what you want to change : ");
            // Propose choices + Q for quit
            var change = Console.ReadLine();
            if (!(change == "N" || change == "S" || change == "D" || change == "T" || change == "Q"))
            {
                errorCode = 2;
                return;
            }
            Console.WriteLine("New value : ");
            var newValue = Console.ReadLine();
            if (newValue == "")
            {
                errorCode = 2;
                return;
            }
            errorCode = userInteract.UpdateJob(jobChoice, change, newValue);
        }
        private void PrintError(int errorCode) // error handling
        {
            switch (errorCode)
            {
                case 1: Console.WriteLine("Succeful exit"); break;
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
