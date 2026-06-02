using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MyApp.Views;

public partial class MainWindow : Window
{
    public enum TimerState
    {
        work,
        breakTime
    }

    public TimerState timerState = TimerState.work;
    int remainingSeconds = 20 * 60; // 20 minutes in seconds
    DispatcherTimer timer;
    public MainWindow()
    {
        InitializeComponent();
        
        
        //timer 
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (s, e) => Timer();
        timer.Start();

    }

    private void Timer()
    {

        if (remainingSeconds > 0)
        {
            remainingSeconds -= 1;
            UpdateTimerText(remainingSeconds);
        }
        else if (timerState == TimerState.work)
        {
            timerState = TimerState.breakTime; // Switch to break time
            remainingSeconds = 25; 
            PlaySound(); 
            UpdateTimerText(remainingSeconds);
        }
        else if (timerState == TimerState.breakTime)
        {
            remainingSeconds = 20*60; // Reset timer to 20 minutes
            timerState = TimerState.work; 
            PlaySound();
            UpdateTimerText(remainingSeconds);
        }
    }

    public void PlaySound()
    {
        System.Console.Beep();
    }

    public void UpdateTimerText(int text)
    {
        int minutes = text / 60;
        int seconds = text % 60;
        TimerTextBlock.Text = $"Timer : {minutes:D2}:{seconds:D2}";

    }


}