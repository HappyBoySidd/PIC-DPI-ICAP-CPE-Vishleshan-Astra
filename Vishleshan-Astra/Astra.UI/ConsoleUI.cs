using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Vishleshan_Astra.Astra_Core.Action;

namespace Vishleshan_Astra.Astra.UI
{
    static class ConsoleUI
    {
        public static Actions GetActivityToPerform(string[] arguments)
        {
            if (arguments.Length > 0 && !string.IsNullOrWhiteSpace(arguments[0]) && int.TryParse(arguments[0], out var result))
                return (Actions)result;

            return (Actions)GetUserInput();
        }

        /// <summary>
        /// Parses the user input / console arguments
        /// </summary>
        /// <returns>Returns the console argument passed (if any) or the user input</returns>
        private static int GetUserInput()
        {
            var strMessage = new StringBuilder();
            strMessage.AppendLine("[blue]Select the task to be performed from the list of actions given below");
            strMessage.AppendLine("(1) => Check configuration");
            strMessage.AppendLine("(2) => Verify File Sanity");
            strMessage.AppendLine("(3) => Notify relevant stakeholders");
            strMessage.AppendLine("(4) => Exit");
            strMessage.AppendLine("Provide your input: [/]");

            return AnsiConsole.Prompt(
            new TextPrompt<int>(strMessage.ToString())
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Please enter valid option[/]")
                .Validate(option =>
                {
                    if (option < 1 || option > 4)
                        return ValidationResult.Error("[red]Invalid option entered[/]");
                    return ValidationResult.Success();
                }));      
        }

        public static async Task PrintListOfUnsafeFilesAsync(List<Tuple<string, string, DateTime, DateTime>> lstUnsafeFiles)
        {
            var tblUnsafeFiles = new Table().Expand().BorderColor(Color.Grey); 

            var numRowCounter = 0;

            await AnsiConsole.Live(tblUnsafeFiles)
            .AutoClear(false)   // Do not remove when done
            .Overflow(VerticalOverflow.Ellipsis) // Show ellipsis when overflowing
            .Cropping(VerticalOverflowCropping.Top) // Crop overflow at top
            .StartAsync(async ctx =>
            {
                tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]#[/]").Centered());
                ctx.Refresh();
                Thread.Sleep(200);
                tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]File Name[/]").Centered());
                ctx.Refresh();
                Thread.Sleep(200);
                tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]File Path[/]").Centered());
                ctx.Refresh();
                Thread.Sleep(200);
                tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]Check In Date[/]").Centered());
                ctx.Refresh();
                Thread.Sleep(200);
                tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]Last Modified Date[/]").Centered());
                ctx.Refresh();
                Thread.Sleep(200);
                do
                {
                    AddUnsafeFile(tblUnsafeFiles, numRowCounter, lstUnsafeFiles[numRowCounter].Item1, lstUnsafeFiles[numRowCounter].Item2, lstUnsafeFiles[numRowCounter].Item3, lstUnsafeFiles[numRowCounter].Item4);
                    numRowCounter++;
                    ctx.Refresh();
                    Thread.Sleep(200);
                }
                while (numRowCounter <= lstUnsafeFiles.Count);
                await Task.Delay(200);
            });

            #region old code
            //tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]#[/]").Centered());
            //tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]File Name[/]").Centered());
            //tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]File Path[/]").Centered());
            //tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]Check In Date[/]").Centered());
            //tblUnsafeFiles.AddColumn(new TableColumn("[gold3_1]Last Modified Date[/]").Centered());

            //foreach (var (strFileName, strFilePath, dtCheckIn, dtLastModified) in lstUnsafeFiles)
            //{
            //    tblUnsafeFiles.AddRow($"[orangered1]{numRowCounter++}[/]", $"[orangered1]{strFileName}[/]", $"[orangered1]{strFilePath}[/]", $"[orangered1]{dtCheckIn}[/]", $"[orangered1]{dtLastModified}[/]");
            //}
            //tblUnsafeFiles.Expand();
            #endregion
            AnsiConsole.Write(tblUnsafeFiles);
        }
        private static void AddUnsafeFile(Table tblUnsafeFiles, int numRowCounter, string strFileName, string strFilePath, DateTime dtCheckIn, DateTime dtLastModified)
        {
            tblUnsafeFiles.AddRow($"[orangered1]{numRowCounter}[/]", $"[orangered1]{strFileName}[/]", $"[orangered1]{strFilePath}[/]", $"[orangered1]{dtCheckIn}[/]", $"[orangered1]{dtLastModified}[/]");
        }

        #region Logging methods
        public static void PrintMessage(string strMessage, LogType logType = LogType.Information)
        {
            string strColour;
            switch (logType)
            {
                case LogType.Warning:
                    strColour = "orange3";
                    break;
                case LogType.Error:
                    strColour = "red";
                    break;
                case LogType.Special:
                    strColour = "gold3_1";
                    break;
                default:
                    strColour = "blue";
                    break;
            }

            AnsiConsole.MarkupInterpolated($"[{strColour}]{strMessage}[/]");
        }

        public enum LogType
        {
            Information,
            Warning,
            Error,
            Special
        }
        #endregion
    }
}
