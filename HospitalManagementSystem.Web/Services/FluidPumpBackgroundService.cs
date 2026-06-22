namespace HospitalManagementSystem.Web.Services
{
    /// <summary>
    /// Background service that runs a PID-controlled simulated IV fluid pump every 5 seconds.
    /// Demonstrates ASP.NET Core IHostedService + PID algorithm in a real-time scenario.
    ///
    /// Scenario: A patient's blood pressure (BP) is monitored.
    ///   - Setpoint : 120 mmHg (target systolic pressure)
    ///   - Measured : simulated BP with random physiological noise
    ///   - Output   : PID adjustment to IV fluid pump flow rate (mL/hr)
    /// </summary>
    public class FluidPumpBackgroundService : BackgroundService
    {
        private readonly ILogger<FluidPumpBackgroundService> _logger;
        private readonly PidController _pid = new(kp: 0.8, ki: 0.05, kd: 0.1);

        // Shared in-memory log readable by DiagnosticsController
        public static readonly Queue<PidLogEntry> RecentLogs = new();
        private const int MaxLogEntries = 50;

        // Simulated patient state
        private double _simulatedBp = 140.0; // starts hypertensive
        private readonly Random _rng = new();

        public FluidPumpBackgroundService(ILogger<FluidPumpBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PID Fluid Pump background service started.");
            const double setpoint = 120.0; // target BP (mmHg)
            const double dt = 5.0;         // seconds between ticks

            while (!stoppingToken.IsCancellationRequested)
            {
                // Compute PID output
                var pidOutput = _pid.Compute(setpoint, _simulatedBp, dt);

                // Clamp pump rate to realistic range (0–500 mL/hr)
                var pumpRate = Math.Clamp(pidOutput * 10.0, 0.0, 500.0);

                // Simulate BP change: IV fluids lower BP; noise adds physiology
                var noise = (_rng.NextDouble() - 0.5) * 4.0;          // ±2 mmHg noise
                var bpChange = -pumpRate * 0.02 + noise;               // fluid lowers BP
                _simulatedBp = Math.Clamp(_simulatedBp + bpChange, 80, 200);

                var entry = new PidLogEntry
                {
                    Timestamp = DateTime.Now,
                    Setpoint = setpoint,
                    MeasuredBp = Math.Round(_simulatedBp, 1),
                    PumpRateMlHr = Math.Round(pumpRate, 1),
                    Error = Math.Round(setpoint - _simulatedBp, 1)
                };

                lock (RecentLogs)
                {
                    RecentLogs.Enqueue(entry);
                    if (RecentLogs.Count > MaxLogEntries)
                        RecentLogs.Dequeue();
                }

                _logger.LogInformation(
                    "[PID] BP={Bp} mmHg | Error={Err} | PumpRate={Rate} mL/hr",
                    entry.MeasuredBp, entry.Error, entry.PumpRateMlHr);

                await Task.Delay(TimeSpan.FromSeconds(dt), stoppingToken);
            }

            _logger.LogInformation("PID Fluid Pump background service stopped.");
        }
    }

    public record PidLogEntry
    {
        public DateTime Timestamp { get; init; }
        public double Setpoint { get; init; }
        public double MeasuredBp { get; init; }
        public double PumpRateMlHr { get; init; }
        public double Error { get; init; }
    }
}
