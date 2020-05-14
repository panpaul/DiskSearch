using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace MachineCode
{
    public static class MachineCode
    {
        public static string GetMachineCode()
        {
            var macAddresses = new Dictionary<string, long>();
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                if (nic.OperationalStatus == OperationalStatus.Up)
                    macAddresses[nic.GetPhysicalAddress().ToString()] =
                        nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived;
            long maxValue = 0;
            var mac = "";
            foreach (var (key, value) in macAddresses.Where(pair => pair.Value > maxValue))
            {
                mac = key;
                maxValue = value;
            }

            return mac;
        }
    }
}