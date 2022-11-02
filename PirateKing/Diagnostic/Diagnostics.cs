namespace PirateKing.Diagnostic
{
    public sealed class Diagnostics
    {
        public static string MetricPrefix => "METRIC - ";

        public static string GetMetricMessage(string message) => $"{MetricPrefix}{message}";
    }
}
