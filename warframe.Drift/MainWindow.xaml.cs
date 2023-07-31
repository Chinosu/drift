using System.Threading;

namespace warframe.Suspender;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public MainWindow()
    {
        InitializeComponent();
    }

    internal const string TargetProcessName = "Warframe.x64";

    internal readonly TimeSpan Duration = new(0, 40, 0);
    internal readonly TimeSpan DownTime = new(0, 0, 30);

    internal System.Timers.Timer timer = default!;

    public bool CanStart
    {
        get => _canStart;
        set
        {
            _canStart = value;
            OnPropertyChanged(nameof(CanStart));
        }
    }
    private bool _canStart = true;

    public bool CanStop
    {
        get => _canStop;
        set
        {
            _canStop = value;
            OnPropertyChanged(nameof(CanStop));
        }
    }
    private bool _canStop = false;

    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }
    private string _status = "Inactive!";

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null!) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void OnStart(object sender, RoutedEventArgs e)
    {
        Suspend();
        CanStart = !CanStart;
        CanStop = !CanStop;
        Status = "Active!";

        timer = new(Duration.TotalMilliseconds);
        timer.Elapsed += (s, e) =>
        {
            UnSuspend();
            Status = "Refreshing";
            Thread.Sleep(Convert.ToInt32(DownTime.TotalMilliseconds));
            Suspend();
            Status = "Active!";
        };
        timer.AutoReset = true;
        timer.Start();
    }
    private void OnStop(object sender, RoutedEventArgs e)
    {
        Status = "Inactive!";
        UnSuspend();
        CanStart = !CanStart;
        CanStop = !CanStop;
        timer.Stop();
    }

    private void Suspend() => Process.GetProcessesByName(TargetProcessName).ToList().ForEach(p => ProcessManager.Suspend(p.Id));
    private void UnSuspend() => Process.GetProcessesByName(TargetProcessName).ToList().ForEach(p => ProcessManager.Resume(p.Id));
}
