﻿using PROGRAMMATION_SYST_ME.ViewModel;
using System;

namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Console view for the user (interface)
    /// </summary>
    class ConsoleView
    {
        private readonly UserInteractionViewModel userInteract = new UserInteractionViewModel();
        private int errorCode;
        /// <summary>
        /// Method that allows for initial selection between executing backup jobs or editing backup jobs
        /// </summary>
        public void InitialChoice() 
        {
            Console.Clear();
            // foreach utilisable avec identifiant dans le xml
            for (var i = 0; i < userInteract.BackupJobsData.Count; i++)
            {
                Console.WriteLine(i + 1 + " -> " + userInteract.BackupJobsData[i].Name);
            }
            Console.WriteLine("Choose between U (Update backup jobs) or E (Execute backup jobs) or Q (Quit) : ");
            var choice = Console.ReadLine();
            if (choice == "U") // switch case
            {
                UpdateChoice();
            }
            else if (choice == "E")
            {
                ExecuteChoice();
            }
            else if (choice == "Q")
            {
                errorCode = 1; // Normal exit
            }
            else
                errorCode = 2;
            if (errorCode == 0)
                InitialChoice();
            else 
                PrintError(errorCode);
                InitialChoice();
        }
        /// <summary>
        /// Method that asks the user for the backup job(s) to execute, then executes the selected backup job(s)
        /// </summary>
        public void ExecuteChoice() 
        {
            Console.WriteLine("Select the backup jobs to execute (example : 1-3 or 1;3) or Q to Quit : ");
            var selection = Console.ReadLine();
            errorCode = userInteract.ExecuteJob(selection);
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

            // Utilisation d'une méthode pour la lisibilité de la condition
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
        private void PrintError(int errorCode)
        {
            switch (errorCode)
            {
                case 1: 
                    Console.WriteLine("Successful exit");
                    Environment.Exit(0);
                    break;
                case 2: FormatError("Invalid input"); break;
                case 3: FormatError("Source directory not found"); break;
            }
        }
        /// <summary>
        /// Formats the error and displays it
        /// </summary>
        /// <param name="msg"></param>
        private void FormatError(string msg)
        {
            Console.WriteLine($"Error {errorCode} : {msg}");
            Console.ReadKey();
        }
    }
}
