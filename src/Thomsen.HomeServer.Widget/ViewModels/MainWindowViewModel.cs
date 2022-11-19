using Microsoft.Extensions.Options;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Thomsen.HomeServer.Core.InfluxDb;
using Thomsen.HomeServer.Core.InfluxDb.Models;

namespace Thomsen.HomeServer.Widget.ViewModels;

public class MainWindowViewModel : BaseViewModel {
    private readonly IOptions<WidgetOptions> _options;
    private readonly IInfluxDbAdapter _influxAdapter;

    private string _status = "";
    private bool _connected;

    private readonly InfluxFieldInfo[] _influxFieldInfos;
    private readonly MeasurementsViewModel[] _measurementsViewModels;

    private readonly System.Timers.Timer _timer = new(5000);

    public static string WindowTitle => "HomeServer Widget";

    public bool Connected { get => _connected; set => SetProperty(ref _connected, value); }

    public string Status { get => _status; set => SetProperty(ref _status, value); }

    public MeasurementsViewModel[] MeasurementsViewModels => _measurementsViewModels;

    public MainWindowViewModel(IOptions<WidgetOptions> options, IInfluxDbAdapter influxAdapter) {
        _options = options;
        _influxAdapter = influxAdapter;

        _influxFieldInfos = options.Value.InfluxFields;

        _measurementsViewModels = _influxFieldInfos.Select(info => new MeasurementsViewModel(info.Name)).ToArray();

        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();
    }

    private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
        _timer.Stop();

        try {
            await RefreshSensorsAsync();
            Status = "";
            Connected = true;
        } catch (Exception ex) {
            Status = ex.Message;
            Connected = false;
            Debug.WriteLine(ex.GetAllMessages());
        }

        _timer.Interval = 5000;
        _timer.Start();
    }

    public async Task RefreshSensorsAsync() {
        foreach (InfluxFieldInfo info in _influxFieldInfos) {
            SensorMeasurements[] measurements = await _influxAdapter.GetAbsSensorMeasurementsAsync(
                bucket: _options.Value.InfluxBucket,
                hourLookBack: _options.Value.LookBackHours,
                fieldName: info.Name,
                unit: info.Unit,
                groupingColumns: _options.Value.InfluxGroupingColumns);

            MeasurementsViewModel vm = MeasurementsViewModels.Single(vm => vm.Name == info.Name);
            Application.Current?.Dispatcher.Invoke(() => vm.SensorMeasurements = measurements);
        }
    }
}