using System;
using System.Collections.Generic;
using System.Text;
using Vishleshan_Astra.Astra.Infrastructure;
using Vishleshan_Astra.Astra.UI;
using static Vishleshan_Astra.Astra_Core.Action;

namespace Vishleshan_Astra
{
    class AdhyayanManager
    {
        static Actions ActionItem { get; set; }

        private ActivityService ActivityHandler { get; set; }

        public static void GetActivityToPerform(string[] arguments)
        {
            ActionItem = ConsoleUI.GetActivityToPerform(arguments: arguments);
        }

        public static void PerformTask()
        {
            var objActivityHandler = new AdhyayanManager().ActivityHandler = new ActivityService();
            switch (ActionItem)
            {
                case Actions.ValidateFileSanity:
                    ConsoleUI.PrintMessage($"Initiating file sanity test at {DateTime.Now}", ConsoleUI.LogType.Information);
                    var lstUnsafeFiles = objActivityHandler.CheckFileSanity();

                    var strArchivePath = objActivityHandler.PublishListOfUnsafeFiles(lstUnsafeFiles);
                    if(!string.IsNullOrWhiteSpace(strArchivePath))
                        ConsoleUI.PrintMessage($"\nList of unsafe files has been archived at {strArchivePath}", ConsoleUI.LogType.Special);

                    ConsoleUI.PrintMessage("\n", ConsoleUI.LogType.Information);
                    ConsoleUI.PrintListOfUnsafeFilesAsync(lstUnsafeFiles);
                    break;

                case Actions.CheckConfiguration:
                    ConsoleUI.PrintMessage($"Initiating configuration validation at {DateTime.Now}", ConsoleUI.LogType.Information);
                    var isConfigurationValid = objActivityHandler.CheckConfiguration();
                    if(isConfigurationValid)
                        ConsoleUI.PrintMessage($"\nNo Configuration issues detected", ConsoleUI.LogType.Special);
                    else
                        ConsoleUI.PrintMessage($"\nInvalid configuration file detected", ConsoleUI.LogType.Error);
                    break;

                case Actions.NotifyStakeHolders:
                        ConsoleUI.PrintMessage($"\nThis feature has not been implemented yet !!!", ConsoleUI.LogType.Warning);
                    break;

                case Actions.Exit:
                    Environment.Exit(exitCode: 0);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(paramName: $"Invalid Option Selected >> Option: {ActionItem}");

            }
        }
    }
}
