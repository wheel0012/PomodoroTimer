using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Media;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool pomodoroStarted;
        private int runTime = 25;
        private int breakTime = 5;
        private Task pomodoroTask;
        private CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();

            this.pomodoroStarted = false;
            runTime = 25;
            breakTime = 5;

            this.StartBtn.Background = ColorOnFalse();
            cts = new CancellationTokenSource();
        }

        private void InitializeValue()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.PTimeTextBox.Text = runTime.ToString();
                this.BreakTimeTextBox.Text = breakTime.ToString();

                this.MainTextBox.Content = "";
            });
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if(this.pomodoroStarted == true)
            {
                pomodoroStarted = false;
                this.StartBtn.Background = this.ColorOnFalse();
                DisposePomodoroTask();
            }
            else
            {
                pomodoroStarted = true;
                this.StartBtn.Background = this.ColorOnTrue();
                PomodoroProcess();

            }
        }

        private void PTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.pomodoroStarted)
            {
            }
            else
            {
                int time;
                if (int.TryParse(((TextBox)sender).Text, out time))
                {
                    this.runTime = time;
                }
            }
        }

        private void BreakTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.pomodoroStarted)
            {

            }
            else
            {
                int time;
                if (int.TryParse(((TextBox)sender).Text, out time))
                {
                    this.breakTime = time;
                }
            }
        }

        private void PomodoroProcess()
        {
            cts = new CancellationTokenSource();
            var ct = cts.Token;

            this.pomodoroTask = Task.Run(() => 
            {
                while(true)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.MainTextBox.Content = "POMODORO";
                    });

                    SystemSounds.Hand.Play();

                    var ptime = DateTime.Now.AddMilliseconds(this.runTime * 60000);

                    while(DateTime.Now <  ptime)
                    {
                        ct.ThrowIfCancellationRequested();

                        this.Dispatcher.Invoke(() =>
                        {
                            var time = ptime - DateTime.Now;
                            this.PTimeTextBox.Text = time.Minutes +":"+time.Seconds;
                        });
                        Thread.Sleep(1);
                    }


                    this.Dispatcher.Invoke(() =>
                    {
                        this.PTimeTextBox.Text = this.runTime.ToString();
                    });



                    this.Dispatcher.Invoke(() =>
                    {
                        this.MainTextBox.Content = "BREAK";
                        this.PTimeTextBox.Text = this.runTime.ToString();
                    });

                    SystemSounds.Beep.Play();

                    var btime = DateTime.Now.AddMilliseconds(this.breakTime * 60000 );
                    while (DateTime.Now <  btime)
                    {
                        ct.ThrowIfCancellationRequested();

                        this.Dispatcher.Invoke(() =>
                        {
                            var time = btime - DateTime.Now;
                            this.BreakTimeTextBox.Text = time.Minutes + ":" + time.Seconds;
                        });
                        Thread.Sleep(1);
                    }


                    this.Dispatcher.Invoke(() =>
                    {
                        this.BreakTimeTextBox.Text = this.breakTime.ToString();
                    });
                }
            }, ct);
        }

        private void DisposePomodoroTask()
        {
            try
            {
                cts.Cancel();
            }
            catch
            {

            }
            finally
            {
                cts.Dispose();


                this.pomodoroTask = null;
            }
            Task.Run(() =>
            {
                Thread.Sleep(100);
                this.InitializeValue();
            });
            
        }

        private SolidColorBrush ColorOnFalse()
        {
            return new SolidColorBrush(Color.FromArgb(0xff, 127, 127, 127));
        }

        private SolidColorBrush ColorOnTrue()
        {
            return new SolidColorBrush(Color.FromArgb(0xff, 88, 209, 5));
        }
    }
}
