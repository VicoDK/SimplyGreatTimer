using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Input;




namespace MyApp.Views;

public partial class MainWindow : Window
{
    string LayoutPoint = "TopLeft";
    List<ClassicTimerPreset> ClassicTimerPreset = new();

    //make sure program is loaded
    private bool _isLoaded = false;

    //Continue
    public List<ClassicTimer> TimerContinue = new List<ClassicTimer>();
    public List<int> TimesRemaining = new List<int>();

    public MainWindow()
    {
        InitializeComponent();
            _isLoaded = true;


            Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("White"));
            Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("Red"));

    SoundSelector.AddHandler(
        InputElement.PointerWheelChangedEvent,
        ComboBox_PointerWheelChanged,
        RoutingStrategies.Tunnel);

    ColorSelect.AddHandler(
        InputElement.PointerWheelChangedEvent,
        ComboBox_PointerWheelChanged,
        RoutingStrategies.Tunnel);


        



        PositionButton_Click(TopLeft, new RoutedEventArgs());
    }

    public void StartButton_Click(object sender, RoutedEventArgs e)
    {
        bool showerTimer = false;

        if (WorkTimeShow.IsChecked == true)
        {
            showerTimer = true;
        }
  
        bool TimerSelected = false;
        //workTimers
        if (DeepWorkTimer.IsChecked == true)
        {
            TimerSelected = true;
            var preset = new ClassicTimerPreset(
                90 * 60,
                20 * 60,
                true,
                true,
                "DeepWork"
            );

            ClassicTimerPreset.Add(preset);
        }


        if (PomodoroTimer.IsChecked == true)
        {
            TimerSelected = true;
            var preset = new ClassicTimerPreset(
                25 * 60,
                5 * 60,
                true,
                true,
                "Pomodoro"
            );

            ClassicTimerPreset.Add(preset);
        }

        if (_52_17Timer.IsChecked == true)
        {
            TimerSelected = true;
            var preset = new ClassicTimerPreset(
                57 * 60,
                17 * 60,
                true,
                true,
                "57-17"
            );

            ClassicTimerPreset.Add(preset);
        }

        if (CustomeTimer.IsChecked == true)
        {
            TimerSelected = true;
            var preset = new ClassicTimerPreset(
                (int.TryParse(CustomeTimerWorkTime.Text, out var w) ? w : 1) * 60,
                (int.TryParse(CustomeTimerBreakTime.Text, out var b) ? b : 1) * 60,
                true,
                true,
                "Custome"
            );

            ClassicTimerPreset.Add(preset);


        }

        if (CountDown.IsChecked == true)
        {
                        TimerSelected = true;
            var preset = new ClassicTimerPreset(
                (int.TryParse(CountDownTimerWorkTime.Text, out var w) ? w : 1) * 60,
                0,
                false,
                false,
                "CountDown"
            );

            ClassicTimerPreset.Add(preset);
        }
        


        //Extra Timers
        if (TwentyTwentyTwentyTimer.IsChecked == true)
        {
            TimerSelected = true;
            var preset = new ClassicTimerPreset(
                20 * 60,
                25,
                true,
                false,
                "20-20-20"
            );

            ClassicTimerPreset.Add(preset);
        }

        if (_30_30rule.IsChecked == true)
        {
            TimerSelected = true;
            var preset = new ClassicTimerPreset(
                30 * 60 ,
                30,
                true,
                false,
                "30-30"
            );

            ClassicTimerPreset.Add(preset);
        }

        TimePanel timePanel = new TimePanel(ClassicTimerPreset, LayoutPoint, showerTimer, audioSetting);

        if (TimerSelected)
        {
            timePanel.Show();
            this.Close();
            
        }

    }

    public void Continue(object? sender, RoutedEventArgs e)
    {

        bool showerTimer = false;

        if (WorkTimeShow.IsChecked == true)
        {
            showerTimer = true;
        }

        for (int i = 0; i < TimerContinue.Count; i++)
        {
            bool ToWorkButton = false;
            bool ToBreakButton= false;

            if (TimerContinue[i].ToBreak != null)
            {
                ToBreakButton = true;
            }

            if (TimerContinue[i].BackToWork != null)
            {
                ToWorkButton = true;
            }

            var preset = new ClassicTimerPreset(
                TimerContinue[i].WorkTimeSet,
                TimerContinue[i].BreakTimeSet,
                ToBreakButton,
                ToWorkButton,
                TimerContinue[i].Name
            );

            
            ClassicTimerPreset.Add(preset);
        }
        TimePanel timePanel = new TimePanel(ClassicTimerPreset, LayoutPoint, showerTimer, audioSetting);

         for (int i = 0; i < timePanel.timers.Count; i++)
        {
            timePanel.timers[i].Time = TimesRemaining[i];

        }
        timePanel.Show();
        this.Close();
        
    }

    public void CustomeTimer_CheckedChanged(object? sender, RoutedEventArgs e)
    {

        CustomeTimerGrid.IsVisible = CustomeTimer.IsChecked ?? false; 
        
    }

    public void CountDownTimer_CheckedChanged(object? sender, RoutedEventArgs e)
    {

        CountDownTimeStackPanel.IsVisible = CountDown.IsChecked ?? false; 
        
    }


    private void Modes_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (Modes.IsChecked == false)
        {
            // SimpleMode
            tab1.Opacity=0;
            tab1.IsHitTestVisible = false;
            tab2.Opacity=0;
            tab2.IsHitTestVisible = false;

            //resets settings
            if (_selectedButton != null)
            {
                _selectedButton.ClearValue(Button.BackgroundProperty);
            }

            LayoutPoint = "TopLeft";
            _selectedButton = TopLeft;
            TopLeft.Background = Brushes.Gray;


            TwentyTwentyTwentyTimer.IsChecked = false;
            _30_30rule.IsChecked = false;
            ColorSelect.SelectedIndex = 0;
            WorkTimeShow.IsChecked = true;
            SoundSelector.SelectedIndex = 0;
            
        }
        else
        {
            // AdvanceMode
            tab1.Opacity=1;
            tab1.IsHitTestVisible = true;
            tab2.Opacity=1;
            tab2.IsHitTestVisible = true;




        }
    }

    private Button? _selectedButton;

    private void PositionButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button clickedButton)
            return;

        if (_selectedButton != null)
        {
            _selectedButton.ClearValue(Button.BackgroundProperty);
        }

        clickedButton.Background = Brushes.Gray;

        _selectedButton = clickedButton;
        LayoutPoint = _selectedButton.Name;
    }

    void ChangeColor_Click(object sender, SelectionChangedEventArgs e)
    {
        if (!_isLoaded)
        return; 
        string selected = ColorSelect.SelectedItem?.ToString();
        switch (selected)
            {
                case "White":
                    CustomeColorGrid.IsVisible=false;
                    Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("White"));
                    Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("Red"));
                    break;
                case "Blue":
                    CustomeColorGrid.IsVisible=false;
                    Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("DodgerBlue"));
                    Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("DodgerBlue"));
                    break;
                case "Red":
                    CustomeColorGrid.IsVisible=false;
                    Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("red"));
                    Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("red"));
                    break;
                case "Green":
                    CustomeColorGrid.IsVisible=false;
                    Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("LimeGreen"));
                    Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("LimeGreen"));
                    break;
                case "Purple":
                    CustomeColorGrid.IsVisible=false;
                    Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("Purple"));
                    Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("Purple"));
                    break;
                case "Costom":
                    CustomeColorGrid.IsVisible=true;

                break;
            }

        
    }


    void PickColor_click(object sender, RoutedEventArgs args)
    {
        Application.Current.Resources["Primary"] = new SolidColorBrush(Color.Parse("#" + CostumePrimary.Text));

        Application.Current.Resources["Secondary"] = new SolidColorBrush(Color.Parse("#" + CostumeSecondary.Text));
    }
  

    public async void SaveFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        //open files menu and lets user pick a audio file
        var files = await this.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select an audio file",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("Audio Files")
                    {
                        Patterns = ["*.mp3", "*.wav", "*.flac", "*.ogg", "*.m4a"]
                    }
                ]
            });

            if (files == null || files.Count == 0)
            return;

            //make folder for audio to be stored ind
            string audioFolder = Path.Combine(AppContext.BaseDirectory, "Audio");
            Directory.CreateDirectory(audioFolder);

            //takes file and gets it ready to store

            var selectedFile = files[0];
            string destinationPath = Path.Combine(audioFolder, "CurrentTrack.mp3");

            //stores file
            await using var source = await selectedFile.OpenReadAsync();
            await using var destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);





        }

        ClassicTimer.AudioSetting audioSetting;

        
        public void SoundSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
            return; 

             var selected = SoundSelector.SelectedItem?.ToString();

          switch (selected)
            {
                case "ConsoleBeep":
                    NewAudioFile.IsVisible = false;
                    audioSetting = ClassicTimer.AudioSetting.ConsoleBeep;
                    break;

                case "UploadAudio":
                    NewAudioFile.IsVisible = true;
                    audioSetting = ClassicTimer.AudioSetting.CustomeSound;
                    break;
            }


            
        }


    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed )
        {
            BeginMoveDrag(e);
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs args)
    {
        
        Close();
    }


private void conbo_PointerPressed(object? sender, PointerPressedEventArgs e)
{
    e.Handled = true;
}





private void ComboBox_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
{
    if (sender is ComboBox cb)
    {
        // Only block scroll when dropdown is closed
        if (!cb.IsDropDownOpen)
        {
            e.Handled = true;
        }
    }
}

}

public class ClassicTimerPreset
{
    public int WorkTime;
    public int BreakTime;
    public bool ToBreakButton;
    public bool BackToWorkButton;
    public string Name;

    public ClassicTimerPreset(
        int workTime,
        int breakTime,
        bool toBreakButton,
        bool backToWorkButton,
        string name)
    {
        WorkTime = workTime;
        BreakTime = breakTime;
        ToBreakButton = toBreakButton;
        BackToWorkButton = backToWorkButton;
        Name = name;


    }


}