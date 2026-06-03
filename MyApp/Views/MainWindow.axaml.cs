using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Media;

namespace MyApp.Views;

public partial class MainWindow : Window
{
    DispatcherTimer timer; //timer for project

    public MainWindow()
    {
        InitializeComponent();
        Position = new Avalonia.PixelPoint(0, 0); //postion of the timer on the screen

        //initially hide the eye break image and timer text for the 20/20/20 timer
        EyeBreak.IsVisible = false;
        
        //timer 
        timer = new DispatcherTimer(); //initialize timer
        timer.Interval = TimeSpan.FromSeconds(1); //set timer interval to 1 second
        timer.Tick += (s, e) => _20Timer(); //call _20Timer to start the 20/20/20 timer
        timer.Start(); //start the timer

    }

    public void PlaySound()
    {
        System.Console.Beep();
    }


    // 20/20/20 timer variables and methods starts here
    #region 20/20/20 timer
    public enum TimerState //Enum for Time states to 20/20/20 timer
    {
        work,
        workToBreak,
        breakTime,
        breakToWork
    }

    public TimerState timerState = TimerState.work; //Timer state variable to keep track of the current state of the 20/20/20 timer
    int _20remainingSeconds = 20 * 60; // 20/20/20 timer in seconds

    bool Aktiv = false; //Variable to track if the eye break is active, used to control the visibility of the eye break image and timer text


    private void _20Timer()
    {

        if (timerState == TimerState.work) //logic for work state of the 20/20/20 timer
        {
            _20timeUpdate();
            
            if (_20remainingSeconds < 0) //if the timer reaches 0, change the state to workToBreak
            {
                timerState = TimerState.workToBreak;
            }
        }
        else if (timerState == TimerState.workToBreak) //logic for workToBreak state of the 20/20/20 timer
        {
            if (!Aktiv) // this is to make sure that the thing doenst run multiple times
            {
                PlaySound();
                //switch the visibility of the eye break image and timer text
                EyeBreak.IsVisible = true;
                TimerTextBlock.IsVisible = false;
                Aktiv = true;
            }
        
        }
        else if (timerState == TimerState.breakTime) //logic for breakTime state of the 20/20/20 timer
        {
            _20timeUpdate();

            if (Aktiv)            
            {
                EyeBreak.IsVisible = false;
                TimerTextBlock.IsVisible = true;
                Aktiv = false;
            }


            if (_20remainingSeconds < 0)
            {
                timerState = TimerState.breakToWork;
            }
        }
        else if (timerState == TimerState.breakToWork) //logic for breakToWork state of the 20/20/20 timer
        {
            _20remainingSeconds = 20 * 60; // Reset timer to 20 minutes
            PlaySound();
            _20UpdateTimerText(_20remainingSeconds);
            timerState = TimerState.work;
        }

        
    }

    

    /// <summary>
    /// function that updates the 20/20/20 timer by decreasing the remaining seconds and updating the timer text on the UI
    /// </summary>
    public void _20timeUpdate() 
    {
            _20remainingSeconds -= 1;
            _20UpdateTimerText(_20remainingSeconds);
    }

    /// <summary>
    /// updates the timer 20/20/20 timer text on the UI with the remaining time in minutes and seconds format
    /// </summary>
    public void _20UpdateTimerText(int text)
    {
        int minutes = text / 60;
        int seconds = text % 60;
        TimerTextBlock.Text = $"20/20/20 Timer : {minutes:D2}:{seconds:D2}";

    }


    /// <summary>
    /// function used to manually start the eye break by clicking the eye break button
    /// </summary>
    public void EyeBreakButton_Click(object sender, RoutedEventArgs e)
    {
            timerState = TimerState.breakTime;
            
            _20remainingSeconds = 25; 
        
    }
    #endregion






}