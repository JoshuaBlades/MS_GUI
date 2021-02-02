using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace MS_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // java -Xmx3072M -Xms3072M -jar serverjar nogui

        // Create an instance of the command class
        Command Command = new Command();

        public MainWindow()
        {
            InitializeComponent();
            HideMidCom();
            HideStartUp();
        }

        /// <summary>
        /// Lauches the GUI and CMD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            // Make sure RAM text is not empty
            if (RAMCB.Text != "")
            {
                string args = "java -Xmx" + RAMCB.Text + " -Xms" + RAMCB.Text + " -jar server.jar nogui";

                // Hide controls
                RAMCB.IsEnabled = false;
                RAMCB.Visibility = Visibility.Hidden;
                StartButton.IsEnabled = false;
                StartButton.Visibility = Visibility.Hidden;
                StartUpLabel.Visibility = Visibility.Hidden;
                ShowGUI();
                ShowMidCom();

                // Start the server
                await Command.InitialiseCMD(args);
            }
            else
            {
                MessageBox.Show("Please select RAM amount!", "Warning!", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Fix this. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close?", "Close", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var process in Process.GetProcessesByName("java"))
                    {
                        // Kills the server and allows everything else to close properly
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
                // Don't close if the user says No or Cancel
                e.Cancel = true;
        }

        /// <summary>
        /// Sends the Say command to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SayTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Create the command
                string outCommand = "/say " + SayTextBox.Text;

                Command.SendCommand(outCommand);

                // Reset the text
                SayTextBox.Text = "";
            }
        }

        /// <summary>
        /// New keydown for the command text box for regular commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (ComTextBox.Text != "")
                    Command.SendCommand(ComTextBox.Text);

                ComTextBox.Text = "";
            }
        }

        #region Events
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void InfoTextBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void InfoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InfoTextBox.ScrollToEnd();
        }

        private void DiffPeaceButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/difficulty peaceful");
        }

        private void DiffEasy_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/difficulty easy");
        }

        private void DiffNormButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/difficulty normal");
        }

        private void DiffHard_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/difficulty hard");
        }

        private void WeaClearButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/weather clear");
        }

        private void WeaRainButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/weather rain");
        }

        private void WeaThunButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/weather thunder");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/save-all");
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/reload");
        }

        /// <summary>
        /// Kicks the selected player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KickButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/kick " + SelectedUserTextBlock.Text.Trim());

            ShowMidCom();

            PlayerDoneButton.Visibility = Visibility.Hidden;
            PlayerBorder.Visibility = Visibility.Hidden;
            KickButton.Visibility = Visibility.Hidden;
            BanButton.Visibility = Visibility.Hidden;
            Gamemode.Visibility = Visibility.Hidden;
            GMCreative.Visibility = Visibility.Hidden;
            GMLabel.Visibility = Visibility.Hidden;
            GMSurvival.Visibility = Visibility.Hidden;
            SelectedUserTextBlock.Text = "";
            SelectedUserLabel.Visibility = Visibility.Hidden;
            FocusLabel.Focus();
        }

        private void PlayersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// Focuses on the selected player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int charIndex = PlayersTextBox.GetCharacterIndexFromPoint(e.GetPosition(PlayersTextBox), true);

            int getLine = PlayersTextBox.GetLineIndexFromCharacterIndex(charIndex);

            string user = PlayersTextBox.GetLineText(getLine);

            // TODO: Make sure this picks up all characters
            SelectedUserTextBlock.Text = user;

            if (SelectedUserTextBlock.Text != "")
            {
                HideMidCom();

                SelectedUserLabel.Visibility = Visibility.Visible;

                PlayerDoneButton.Visibility = Visibility.Visible;
                PlayerBorder.Visibility = Visibility.Visible;
                KickButton.Visibility = Visibility.Visible;
                BanButton.Visibility = Visibility.Visible;
                Gamemode.Visibility = Visibility.Visible;
                GMCreative.Visibility = Visibility.Visible;
                GMLabel.Visibility = Visibility.Visible;
                GMSurvival.Visibility = Visibility.Visible;
                FocusLabel.Focus();
            }
            else
            {
                ShowMidCom();

                PlayerDoneButton.Visibility = Visibility.Hidden;
                PlayerBorder.Visibility = Visibility.Hidden;
                KickButton.Visibility = Visibility.Hidden;
                BanButton.Visibility = Visibility.Hidden;
                Gamemode.Visibility = Visibility.Hidden;
                GMCreative.Visibility = Visibility.Hidden;
                GMLabel.Visibility = Visibility.Hidden;
                GMSurvival.Visibility = Visibility.Hidden;
                SelectedUserTextBlock.Text = "";
                SelectedUserLabel.Visibility = Visibility.Hidden;
                FocusLabel.Focus();
            }
        }

        /// <summary>
        /// Hides the middle commands to show player options
        /// </summary>
        private void HideMidCom()
        {
            DiffBorder.Visibility = Visibility.Hidden;
            DiffEasy.Visibility = Visibility.Hidden;
            DiffHard.Visibility = Visibility.Hidden;
            DifficultyLabel.Visibility = Visibility.Hidden;
            DiffNormButton.Visibility = Visibility.Hidden;
            DiffPeaceButton.Visibility = Visibility.Hidden;
            WeaBorder.Visibility = Visibility.Hidden;
            WeaClearButton.Visibility = Visibility.Hidden;
            Weather.Visibility = Visibility.Hidden;
            WeaThunButton.Visibility = Visibility.Hidden;
            WeaRainButton.Visibility = Visibility.Hidden;
            TimeBorder.Visibility = Visibility.Hidden;
            TimeLabel.Visibility = Visibility.Hidden;
            TimeDay.Visibility = Visibility.Hidden;
            TimeNight.Visibility = Visibility.Hidden;
            GenLabel.Visibility = Visibility.Hidden;
            ProcStatsLabel.Visibility = Visibility.Hidden;
            RamLabelBlock.Visibility = Visibility.Hidden;
            RamStatsLabel.Visibility = Visibility.Hidden;
            CpuStatsLabel.Visibility = Visibility.Hidden;
            CpuLabelBlock.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Hide all GUI elements for start up
        /// </summary>
        private void HideStartUp()
        {
            PlayersLabel.Visibility = Visibility.Hidden;
            StopButton.Visibility = Visibility.Hidden;
            SaveButton.Visibility = Visibility.Hidden;
            ReloadButton.Visibility = Visibility.Hidden;
            SayTextBox.Visibility = Visibility.Hidden;
            SayLabel.Visibility = Visibility.Hidden;
            PlayersLabel.Visibility = Visibility.Hidden;
            PlayersTextBox.Visibility = Visibility.Hidden;
            InfoTextBox.Visibility = Visibility.Hidden;
            LogLabel.Visibility = Visibility.Hidden;
            ComTextBox.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Show all GUI elements after start up
        /// </summary>
        private void ShowGUI()
        {
            PlayersLabel.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
            ReloadButton.Visibility = Visibility.Visible;
            SayTextBox.Visibility = Visibility.Visible;
            SayLabel.Visibility = Visibility.Visible;
            PlayersLabel.Visibility = Visibility.Visible;
            PlayersTextBox.Visibility = Visibility.Visible;
            InfoTextBox.Visibility = Visibility.Visible;
            LogLabel.Visibility = Visibility.Visible;
            ComTextBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Shows the middle commands
        /// </summary>
        private void ShowMidCom()
        {
            DiffBorder.Visibility = Visibility.Visible;
            DiffEasy.Visibility = Visibility.Visible;
            DiffHard.Visibility = Visibility.Visible;
            DifficultyLabel.Visibility = Visibility.Visible;
            DiffNormButton.Visibility = Visibility.Visible;
            DiffPeaceButton.Visibility = Visibility.Visible;
            WeaBorder.Visibility = Visibility.Visible;
            WeaClearButton.Visibility = Visibility.Visible;
            Weather.Visibility = Visibility.Visible;
            WeaThunButton.Visibility = Visibility.Visible;
            WeaRainButton.Visibility = Visibility.Visible;
            TimeBorder.Visibility = Visibility.Visible;
            TimeLabel.Visibility = Visibility.Visible;
            TimeDay.Visibility = Visibility.Visible;
            TimeNight.Visibility = Visibility.Visible;
            GenLabel.Visibility = Visibility.Visible;
            ProcStatsLabel.Visibility = Visibility.Visible;
            RamLabelBlock.Visibility = Visibility.Visible;
            RamStatsLabel.Visibility = Visibility.Visible;
            CpuStatsLabel.Visibility = Visibility.Visible;
            CpuLabelBlock.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Bans the selected player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BanButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/ban " + SelectedUserTextBlock.Text.Trim());

            ShowMidCom();

            PlayerDoneButton.Visibility = Visibility.Hidden;
            PlayerBorder.Visibility = Visibility.Hidden;
            KickButton.Visibility = Visibility.Hidden;
            BanButton.Visibility = Visibility.Hidden;
            Gamemode.Visibility = Visibility.Hidden;
            GMCreative.Visibility = Visibility.Hidden;
            GMLabel.Visibility = Visibility.Hidden;
            GMSurvival.Visibility = Visibility.Hidden;
            SelectedUserTextBlock.Text = "";
            SelectedUserLabel.Visibility = Visibility.Hidden;
            FocusLabel.Focus();
        }

        /// <summary>
        /// Changes selected players gamemode to creative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GMCreative_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/gamemode creative " + SelectedUserTextBlock.Text.Trim());
        }

        /// <summary>
        /// Changes selected players gamemode to survival
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GMSurvival_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/gamemode survival " + SelectedUserTextBlock.Text.Trim());
        }

        /// <summary>
        /// Shows the middle commands after player options have finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerDoneButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerDoneButton.Visibility = Visibility.Hidden;
            PlayerBorder.Visibility = Visibility.Hidden;
            KickButton.Visibility = Visibility.Hidden;
            BanButton.Visibility = Visibility.Hidden;
            Gamemode.Visibility = Visibility.Hidden;
            GMCreative.Visibility = Visibility.Hidden;
            GMLabel.Visibility = Visibility.Hidden;
            GMSurvival.Visibility = Visibility.Hidden;
            SelectedUserTextBlock.Text = "";
            SelectedUserLabel.Visibility = Visibility.Hidden;
            ShowMidCom();

            FocusLabel.Focus();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/stop");
        }

        private void TimeDay_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/time set day");
        }

        private void TimeNight_Click(object sender, RoutedEventArgs e)
        {
            Command.SendCommand("/time set night");
        }
        #endregion
    }
}
