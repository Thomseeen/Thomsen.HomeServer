using System;

using Thomsen.HomeServer.Core.InfluxDb.Models;

namespace Thomsen.HomeServer.Widget.ViewModels;

public class MeasurementsViewModel : BaseViewModel {
    private readonly string _name;
    private SensorMeasurements[] _sensorMeasurements = Array.Empty<SensorMeasurements>();

    public string Name => _name;

    public SensorMeasurements[] SensorMeasurements { get => _sensorMeasurements; set => SetProperty(ref _sensorMeasurements, value); }

    public MeasurementsViewModel(string name) {
        _name = name;
    }
}