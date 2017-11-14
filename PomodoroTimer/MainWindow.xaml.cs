using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBox> InputTextBoxes;

        TimerTracker TimerTracker;

        public MainWindow()
        {
            InitializeComponent();

            InputTextBoxes = new List<TextBox>()
            {
                RoundCountTextBox,
                RoundLengthTextBox,
                ShortBreakTextBox,
                LongBreakTextBox
            };
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // check that all input TextBoxes have valid inputs
            foreach (TextBox textBox in InputTextBoxes)
            {
                int tryValue;
                if (!int.TryParse(textBox.Text, out tryValue) ||
                    tryValue <= 0)
                {
                    return;
                }
            }

            // construct the TimerPattern
            int roundCount, roundLength, shortBreak, longBreak;

            int.TryParse(RoundCountTextBox.Text, out roundCount);
            int.TryParse(RoundLengthTextBox.Text, out roundLength);
            int.TryParse(ShortBreakTextBox.Text, out shortBreak);
            int.TryParse(LongBreakTextBox.Text, out longBreak);

            TimerPattern timerPattern = new TimerPattern(
                roundCount, roundLength, shortBreak, longBreak);
            TimerTracker = new TimerTracker(timerPattern, TimerStep);

            UpdateRemainingRoundsTextBlock(String.Format("({0} rounds left)", timerPattern.RoundCount));

            ConfigurationGrid.Visibility = Visibility.Collapsed;
            TimerGrid.Visibility = Visibility.Visible;

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BeginTimer;
            bgWorker.RunWorkerCompleted += EndTimer;
            bgWorker.RunWorkerAsync();
        }

        private void BeginTimer(object sender, DoWorkEventArgs e)
        {
            do
            {
                // start the timer for the round                
                TimerTracker.StartTime = DateTime.UtcNow;
                TimerTracker.EndTime = TimerTracker.StartTime + new TimeSpan(0, TimerTracker.TimerPattern.RoundLength, 0);
                TimerTracker.Timer.Change(0, 1000);
                if (TimerTracker.RemainingRounds == 1)
                {
                    UpdateRemainingRoundsTextBlock("(1 round left)");
                }
                else
                {
                    UpdateRemainingRoundsTextBlock(String.Format("({0} rounds left)", TimerTracker.RemainingRounds));
                }

                // wait for the round to end
                TimerTracker.AutoResetEvent.WaitOne();
                TimerTracker.RemainingRounds--;
                UpdateRemainingRoundsTextBlock(String.Format("(short break)", TimerTracker.RemainingRounds));

                // do something to signal to the user the round is over


                if (TimerTracker.RemainingRounds > 0)
                {
                    // start the timer for the short break
                    TimerTracker.StartTime = DateTime.UtcNow;
                    TimerTracker.EndTime = TimerTracker.StartTime + new TimeSpan(0, TimerTracker.TimerPattern.ShortBreak, 0);

                    // wait for the short break to end
                    TimerTracker.AutoResetEvent.WaitOne();
                }

            }
            while (TimerTracker.RemainingRounds > 0);

            // start the timer for the long break
            TimerTracker.StartTime = DateTime.UtcNow;
            TimerTracker.EndTime = TimerTracker.StartTime + new TimeSpan(0, TimerTracker.TimerPattern.LongBreak, 0);
            UpdateRemainingRoundsTextBlock(String.Format("(long break)", TimerTracker.RemainingRounds));

            // wait for the long break to end
            TimerTracker.AutoResetEvent.WaitOne();
        }
        
        private void TimerStep(object state)
        {
            TimeSpan elapsedTime = DateTime.UtcNow - TimerTracker.StartTime;
            
            if (DateTime.UtcNow > TimerTracker.EndTime)
            {
                Application.Current.Dispatcher.Invoke(() => DisplayTimeTextBlock.Text = "0:00");
                AutoResetEvent autoResetEvent = (AutoResetEvent)state;
                autoResetEvent.Set();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => DisplayTimeTextBlock.Text = 
                    String.Format("{0}:{1}", elapsedTime.Minutes, elapsedTime.Seconds.ToString().PadLeft(2, '0')));
            }
        }

        private void EndTimer(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
                {
                    TimerGrid.Visibility = Visibility.Collapsed;
                    ConfigurationGrid.Visibility = Visibility.Visible;
                });
        }

        private void UpdateRemainingRoundsTextBlock(string newText)
        {
            Application.Current.Dispatcher.Invoke(() => RemainingRoundsTextBlock.Text = newText);
        }
    }
}
