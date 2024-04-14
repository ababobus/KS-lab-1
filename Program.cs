using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        while (true)
        {
            string beginIP = Console.ReadLine();
            string endIP = Console.ReadLine();

            IPAddress beginIPAddress = IPAddress.Parse(beginIP);
            IPAddress endIPAddress = IPAddress.Parse(endIP);

            IPAddress subnetMask = Mask(beginIPAddress, endIPAddress);
            IPAddress networkAddress = NetworkAddress(beginIPAddress, subnetMask);
            IPAddress broadcastAddress = BroadcastAddress(networkAddress, subnetMask);
            string macAddress = MacAddress(':');

            Console.WriteLine($"Маска сети: {subnetMask.ToString()}");
            Console.WriteLine($"Сетевой адрес: {networkAddress.ToString()}");
            Console.WriteLine($"Broadcast адрес: {broadcastAddress.ToString()}");
            Console.WriteLine("Mac-адрес: " + macAddress + "\n");

        }
    }


    static IPAddress Mask(IPAddress beginIP, IPAddress endIP)
    {
        var begin = beginIP.GetAddressBytes();
        var end = endIP.GetAddressBytes();
        byte[] mask = new byte[4];
        bool edge = false;
        for (int i = 0; i < 4; i++)
            for (byte b = 128; b >= 1; b /= 2)
            {
                if (!edge && ((begin[i] & b) == (end[i] & b)))
                {
                    mask[i] |= b;
                }
                else
                {
                    edge = true;
                    mask[i] = (byte)(mask[i] & ~b);
                }
            }


        return new IPAddress(mask);
    }


    static IPAddress NetworkAddress(IPAddress ip, IPAddress subnetMask)
    {
        byte[] ipBytes = ip.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();
        byte[] networkBytes = new byte[ipBytes.Length];

        for (int i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        return new IPAddress(networkBytes);
    }


    static IPAddress BroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
    {

        byte[] maskBytes = subnetMask.GetAddressBytes();
        byte[] networkBytes = networkAddress.GetAddressBytes();
        byte[] broadcastBytes = new byte[networkBytes.Length];

        for (int i = 0; i < networkBytes.Length; i++)
        {
            broadcastBytes[i] = (byte)(networkBytes[i] | ~maskBytes[i]);
        }
        return new IPAddress(broadcastBytes);
    }


    static string MacAddress(char separator)
    {
        string macAddress = "";
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.OperationalStatus == OperationalStatus.Up)
            {
                byte[] macBytes = nic.GetPhysicalAddress().GetAddressBytes();

                macAddress = BitConverter.ToString(macBytes).Replace("-", separator.ToString());
                break;
            }
        }
        return macAddress;
    }
}