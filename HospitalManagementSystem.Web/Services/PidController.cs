namespace HospitalManagementSystem.Web.Services
{
    /// <summary>
    /// PID Controller implementation for a simulated medical fluid pump.
    ///
    /// ALGORITHM: PID (Proportional–Integral–Derivative) Control
    /// ─────────────────────────────────────────────────────────────────────────
    ///  output = Kp * error            ← Proportional: react to current error
    ///         + Ki * integral         ← Integral:     eliminate steady-state offset
    ///         + Kd * derivative       ← Derivative:   dampen oscillations
    ///
    ///  Clinical scenario: maintain target blood pressure (120 mmHg) by
    ///  controlling the flow rate of an IV fluid pump (mL/hr).
    /// ─────────────────────────────────────────────────────────────────────────
    /// </summary>
    public class PidController
    {
        // Tuning parameters (pre-tuned for the demo scenario)
        private readonly double _kp;  // Proportional gain
        private readonly double _ki;  // Integral gain
        private readonly double _kd;  // Derivative gain

        private double _integral;
        private double _previousError;

        // Anti-windup clamp for the integral term
        private const double IntegralMax = 50.0;

        public PidController(double kp = 0.8, double ki = 0.05, double kd = 0.1)
        {
            _kp = kp;
            _ki = ki;
            _kd = kd;
        }

        /// <summary>
        /// Compute PID output for one time step.
        /// </summary>
        /// <param name="setpoint">Desired value (e.g. 120 mmHg)</param>
        /// <param name="measured">Current sensor reading</param>
        /// <param name="dt">Time elapsed since last call, in seconds</param>
        /// <returns>Control output (pump flow rate adjustment, mL/hr)</returns>
        public double Compute(double setpoint, double measured, double dt)
        {
            if (dt <= 0) return 0;

            var error = setpoint - measured;

            // Integral term with anti-windup clamping
            _integral = Math.Clamp(_integral + error * dt, -IntegralMax, IntegralMax);

            // Derivative term (rate of change of error)
            var derivative = (error - _previousError) / dt;
            _previousError = error;

            return _kp * error + _ki * _integral + _kd * derivative;
        }

        public void Reset()
        {
            _integral = 0;
            _previousError = 0;
        }
    }
}
