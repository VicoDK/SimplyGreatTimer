using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Interactivity;
using Avalonia.Layout;
using System.IO;
using NAudio.Wave;
using Avalonia.Input;




namespace MyApp.Views;

public partial class TimePanel : Window
{

    

    DispatcherTimer timer;

    public List<ClassicTimer> timers = new();   
    StackPanel timerPanel = new StackPanel();

    private bool _menuOpen = false;


    /// <summary>
    /// creation of the timer window
    /// </summary>
    /// <param name="presets"></param>
    /// <param name="LayoutPoint"></param>
    /// <param name="showerTimer"></param>
    /// <param name="audioSetting"></param>
    public TimePanel(List<ClassicTimerPreset> presets, string LayoutPoint, bool showerTimer, ClassicTimer.AudioSetting audioSetting)
    {


        InitializeComponent();

        this.Opened += (_, __) => SetPosition(LayoutPoint); 

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);

        foreach (var preset in presets)
        {
            timers.Add(new ClassicTimer(
                preset.WorkTime,
                preset.BreakTime,
                preset.ToBreakButton,
                preset.BackToWorkButton,
                preset.Name,
                timerPanel, 
                showerTimer,
                audioSetting
            ));
        }

        foreach (var t in timers)
        {
            timer.Tick += (s, e) => t.ClassicWorkUpdateSection();
        }

        Content = timerPanel; // FIXED (removed self-add bug)

        timer.Start();
        

  
        var menuButton = new Button
        {
            Content = "Menu",
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsVisible = false
        };

        this.PointerEntered += (_, _) =>
        {
            menuButton.IsVisible = true;
        };

        this.PointerExited += (_, _) =>
        {
            if (!_menuOpen)
                menuButton.IsVisible = false;
        };

        menuButton.Classes.Add("timer-button");

        // Create flyout
        var flyout = new MenuFlyout
        {
            Placement = PlacementMode.BottomEdgeAlignedLeft
        };

        // Pause item
        pauseItem = new MenuItem
        {
            Header = "Pause",
            MinWidth = 120
        };

        pauseItem.Classes.Add("timer-menu");
        pauseItem.Click += Pause_Click;

        // Back item
        var backItem = new MenuItem
        {
            Header = "Back",
            MinWidth = 120
        };

        flyout.Opened += (_, _) =>
        {
            _menuOpen = true;
            menuButton.IsVisible = true;
        };

        flyout.Closed += (_, _) =>
        {
            _menuOpen = false;

            if (!this.IsPointerOver)
                menuButton.IsVisible = false;
        };


        backItem.Classes.Add("timer-menu");
        backItem.Click += Back_Click;

        // Add items
        flyout.Items.Add(pauseItem);
        flyout.Items.Add(backItem);

        // Attach flyout
        menuButton.Flyout = flyout;

        // Add to panel
        timerPanel.Children.Add(menuButton);


    }
    /// <summary>
    /// function for placing the timer 
    /// </summary>
    /// <param name="layoutPoint"></param>
    private void SetPosition(string layoutPoint)
    {
        var screen = Screens.ScreenFromVisual(this) ?? Screens.Primary;
        var area = screen.WorkingArea;
        var w = (int)ClientSize.Width;
        var h = (int)ClientSize.Height;

        

        int x = 0;
        int y = 0;

        switch (layoutPoint)
        {
            case "TopLeft":
                x = area.X;
                y = area.Y;
                break;

            case "TopCenter":
                x = area.X + (area.Width - w) / 2;
                y = area.Y;
                break;

            case "TopRight":
                x = area.X + area.Width - w;
                y = area.Y;
                break;

            case "LeftCenter":
                x = area.X;
                y = area.Y + (area.Height - h) / 2;
                break;

            case "RightCenter":
                x = area.X + area.Width - w;
                y = area.Y + (area.Height - h) / 2;
                break;

            case "BottomLeft":
                x = area.X;
                y = area.Y + area.Height - h;
                break;

            case "BottomCenter":
                x = area.X + (area.Width - w) / 2;
                y = area.Y + area.Height - h;
                break;

            case "BottomRight":
                x = area.X + area.Width - w;
                y = area.Y + area.Height - h;
                break;
            case "Freemove":
                x = area.X + (area.Width - w) / 2;
                y = area.Y + (area.Height - h) / 2;
                Moveable = true;

            break;
        }

        Position = new PixelPoint(x, y);
    }
  



    /// <summary>
    /// button for going back to start menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Back_Click(object? sender, RoutedEventArgs e)
    {

        MainWindow mainWindow = new MainWindow();
        mainWindow.TimerContinue = timers ;

        foreach (ClassicTimer times in timers)
        {
            mainWindow.TimesRemaining.Add(times.Time);
        }

        mainWindow.ContinueButton.IsVisible = true;
        timer.Stop();
        mainWindow.Show();
        this.Close();
    }

    private MenuItem pauseItem;
    bool isPaused = true;

    /// <summary>
    /// the button click for pausing the timer 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Pause_Click(object? sender, RoutedEventArgs e)
    {
        isPaused = !isPaused;
        pauseItem.Header = isPaused ? "Paused" : "Resume";

        

        foreach (ClassicTimer timer in timers)
        {
            timer.Pause();
        }
    }


    bool Moveable = false;
    /// <summary>
    /// function that always you to drag windown when option is on 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && Moveable)
        {
            BeginMoveDrag(e);
        }
    }
}
public class ClassicTimer
{

    public string Name;
    bool showTimer;

    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;
    private bool _isPlaying;
    /// <summary>
    /// Creates all the right timers and the button in timer panel
    /// </summary>
    /// <param name="workTimeSet"></param>
    /// <param name="breakTimeSet"></param>
    /// <param name="needToBreakButton"></param>
    /// <param name="needBackToWorkButton"></param>
    /// <param name="name"></param>
    /// <param name="timerPanel"></param>
    /// <param name="showTimer"></param>
    /// <param name="audioSetting"></param>
    public ClassicTimer(int workTimeSet, int breakTimeSet, bool needToBreakButton, bool needBackToWorkButton, string name, StackPanel timerPanel, bool showTimer, AudioSetting audioSetting)
{
    //audio setup


    string filePath = Path.Combine(
    AppContext.BaseDirectory,
    "Audio",
    "CurrentTrack.mp3");

    if (File.Exists(filePath))
    {
        audioFile = new AudioFileReader(filePath);

        outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFile);
        outputDevice.PlaybackStopped += OnPlaybackStopped;
    }
    this.audioSetting = audioSetting;



    this.showTimer = showTimer; 
    WorkTimeSet = workTimeSet;
    Time = WorkTimeSet;
    BreakTimeSet = breakTimeSet;
    this.Name = name;
    StackPanel panel = timerPanel;

    TimerTextBlock = new TextBlock
    {
        Text = $" {name} Timer : {workTimeSet}",
        
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        Margin = new Thickness(4, 4, 10, 0)
       
    };
    WorkUpdateTimerText(workTimeSet);
    panel.Children.Add(TimerTextBlock);

    if (needToBreakButton)
    {
        ToBreak = new Button
        {
            Content = $" {name} Break",
            Margin = new Thickness(10),
            IsVisible = false
    
        };

        ToBreak.Click += BreakButton_Click;
        panel.Children.Add(ToBreak);
    }

    if (needBackToWorkButton)
    {
        BackToWork = new Button
        {
            Content = "Back to Work",
            Margin = new Thickness(10),
            IsVisible = false
        };

        BackToWork.Click += BackToWork_Click;
        panel.Children.Add(BackToWork);




    }
}

    public enum AudioSetting //variable for the timer to know which sounds it should use
    {
        ConsoleBeep,
        CustomeSound

    }
    public AudioSetting audioSetting;


    public int WorkTimeSet; 
    public int BreakTimeSet; 
    TextBlock TimerTextBlock;

    public Button? ToBreak;
    public Button? BackToWork;

    public int Time;


    public enum ClassicTimerState //Enum for Time states to DeepWork timer
    {
        Work,
        DeepWorkToBreak,
        BreakTime,
        BreakToDeepWork
    }

    ClassicTimerState WorkTimerState = ClassicTimerState.Work; // DeepWork timer state variable
    bool ClassicAktiv = false; // Variable to track if the deep work break is active
    
    /// <summary>
    /// this is for stateMachine for the timer
    /// </summary>
    public void ClassicWorkUpdateSection()
    {
        switch (WorkTimerState)
        {
            case ClassicTimerState.Work:
                WorktimeUpdate();
                if (Time <= 0)
                {
                    WorkTimerState = ClassicTimerState.DeepWorkToBreak;
                }
                break;
            case ClassicTimerState.DeepWorkToBreak:
                if (!ClassicAktiv && BreakTimeSet != 0)
                {

                    WorkDoneSound();
                    if (ToBreak != null)
                    {
                        ToBreak.IsVisible = true;
                        
                    }
                    else if (ToBreak == null)
                    {
                        BreakButton_Click();
                    }
                    
                    TimerTextBlock.IsVisible = false;
                    ClassicAktiv = true;
                }
                else if (BreakTimeSet == 0)
                {
                    BreakDoneSound();
                }
                break;
            case ClassicTimerState.BreakTime:
                WorktimeUpdate();
                if (ClassicAktiv)
                {
                    if (ToBreak != null)
                    {
                        ToBreak.IsVisible = false;
                    }
                    TimerTextBlock.IsVisible = true;
                    ClassicAktiv = false;
                }
                if (Time <= 0)
                {
                    WorkTimerState = ClassicTimerState.BreakToDeepWork;
                }
                break;
            case ClassicTimerState.BreakToDeepWork:
                if (BackToWork != null)
                {
                    BackToWork.IsVisible = true;

                    
                }
                TimerTextBlock.IsVisible = false;

                if (BackToWork == null)
                {
                    BackToWork_Click();
                    
                }
                BreakDoneSound();
                break;
        }
        
    }


    /// <summary>
    /// this is the Timer function
    /// </summary>
    public void WorktimeUpdate() 
    {
        if (pauseBool)
        {
            Time -= 1;
            if (Time > -1)
            {
                WorkUpdateTimerText(Time);
            }
        }

    }



    bool pauseBool = true;
    /// <summary>
    /// this is the pause function for the timer
    /// </summary>
    public void Pause()
    {
        pauseBool = !pauseBool;
    }

    /// <summary>
    /// function for updation the timer and all its logic
    /// </summary>
    public void WorkUpdateTimerText(int text)
    {
        int hours = text / 3600;
        int minutes = (text % 3600) / 60;
        int seconds = text % 60;
        if (WorkTimerState == ClassicTimerState.Work)
        {
            if (showTimer && hours > 0)
            {
                TimerTextBlock.Text = $" {Name} Timer: {hours:D2}:{minutes:D2}";
            }
            else if (showTimer)
            {
                TimerTextBlock.Text = $" {Name} Timer: {minutes:D2}:{seconds:D2}";
            }
            else if (!showTimer)
            {
                TimerTextBlock.Text = $" {Name} Timer: Working";
            }

        }
        else if (WorkTimerState == ClassicTimerState.BreakTime)
        {
            if (showTimer && hours > 0)
            {
                TimerTextBlock.Text = $" {Name} Break Timer : {hours:D2}:{minutes:D2}";
            }
            else if (showTimer)
            {
                TimerTextBlock.Text = $" {Name} Break Timer : {minutes:D2}:{seconds:D2}";
            }
            else if (!showTimer)
            {
                TimerTextBlock.Text = $" {Name} Break Timer : Break";
            }
        }
        
    }

    /// <summary>
    /// Button for going on brea
    /// </summary>
    public void BreakButton_Click(object? sender, RoutedEventArgs e)
    {

        BreakButton_Click();
    }

    /// <summary>
    /// function to go on break
    /// </summary>
    public void BreakButton_Click()
    {
        WorkTimerState = ClassicTimerState.BreakTime;
        Time = BreakTimeSet;
    }


    /// <summary>
    /// Button to go back to work
    /// </summary>
    public void BackToWork_Click(object? sender, RoutedEventArgs e)
    {
        BackToWork_Click();
    }

    /// <summary>
    /// function to go back to work
    /// </summary>
    public void BackToWork_Click()
    {   
        WorkTimerState = ClassicTimerState.Work;
        TimerTextBlock.IsVisible = true;
        Time = WorkTimeSet;
        WorkUpdateTimerText(Time);
        if (BackToWork != null)
        {
            BackToWork.IsVisible = false;
            
        }

    }

    /// <summary>
    /// this continuously plays sound when break is done
    /// </summary>
    public void BreakDoneSound()
    {
        switch (audioSetting)
        {
            case AudioSetting.ConsoleBeep:
            
            System.Console.Beep();

            break;
            case AudioSetting.CustomeSound:

            if (_isPlaying)
                return;

            _isPlaying = true;
            
            //goes back to the start of audio file
            audioFile.Position = 0;   
            outputDevice.Play();

            break;
            
        }

    }

    //this runs when the sound is done
    /// <summary>
    /// this function is used to make sure sounds dont restart if it is called more then ones 
    /// </summary>
    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        _isPlaying = false;
    }

    
/// <summary>
/// Playes the sounds when Work is done
/// </summary>
    public void WorkDoneSound()
    {   
        switch (audioSetting)
        {
            case AudioSetting.ConsoleBeep:
            
            System.Console.Beep();

            break;
            case AudioSetting.CustomeSound:
            
            //goes back to the start of audio file
            audioFile.Position = 0;   
            outputDevice.Play();

            break;
            
        }

    }


}
