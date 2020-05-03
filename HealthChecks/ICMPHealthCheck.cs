using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private readonly string _host;
        private readonly int _timeout;

        public ICMPHealthCheck(string host, int timeout)
        {
            _host = host;
            _timeout = timeout;
        }


        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(_host);

                switch (reply.Status)
                {
                    case IPStatus.Success:
                        var msg = MakeMsg(reply.RoundtripTime);
                        return (reply.RoundtripTime > _timeout)
                            ? HealthCheckResult.Degraded(msg)
                            : HealthCheckResult.Healthy(msg);
                    default:
                        string err = MakeErrorMsg(reply.Status);
                        return HealthCheckResult.Unhealthy(err);
                }
            }
            catch (Exception e)
            {
                string err = MakeErrorMsg(e.Message);
                return HealthCheckResult.Unhealthy(err);
            }
        }

        private string MakeErrorMsg(string message)
        {
            return $"IMCP to {_host} failed: {message}";
        }

        private string MakeErrorMsg(IPStatus status)
        {
            return $"IMCP to {_host} failed: {status}";
        }

        private string MakeMsg(long roundTripTime)
        {
            return $"IMCP to {_host} took {roundTripTime} ms.";
        }

    }
}
