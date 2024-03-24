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
            Console.WriteLine("Введите начальный IP");
            string beginIP = Console.ReadLine();

            Console.WriteLine("Введите конечный IP");
            string endIP = Console.ReadLine();

            IPAddress beginIPAddress = IPAddress.Parse(beginIP);
            IPAddress endIPAddress = IPAddress.Parse(endIP);

            IPAddress subnetMask = CalculateSubnetMask(beginIPAddress, endIPAddress);
            IPAddress networkAddress = CalculateNetworkAddress(beginIPAddress, subnetMask);
            IPAddress broadcastAddress = CalculateBroadcastAddress(networkAddress, subnetMask);
            string macAddress = GetMacAddress(':');

            Console.WriteLine($"Маска сети: {subnetMask.ToString()}");
            Console.WriteLine($"Сетевой адрес: {networkAddress.ToString()}");
            Console.WriteLine($"Broadcast адрес: {broadcastAddress.ToString()}");
            Console.WriteLine("Mac-адрес: " + macAddress + "\n");

        }
    }


    static IPAddress CalculateSubnetMask(IPAddress beginIP, IPAddress endIP)
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


    static IPAddress CalculateNetworkAddress(IPAddress ip, IPAddress subnetMask)
    {
        byte[] ipBytes = ip.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();

        if (ipBytes.Length != maskBytes.Length)
        {
            throw new ArgumentException("Длины IP-адреса и маски подсети должны быть одинаковыми.");
        }

        byte[] networkBytes = new byte[ipBytes.Length];
        for (int i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        return new IPAddress(networkBytes);
    }


    static IPAddress CalculateBroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
    {
        byte[] networkBytes = networkAddress.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();

        if (networkBytes.Length != maskBytes.Length)
        {
            throw new ArgumentException("Длины сетевого адреса и маски подсети должны быть одинаковыми.");
        }

        byte[] broadcastBytes = new byte[networkBytes.Length];
        for (int i = 0; i < networkBytes.Length; i++)
        {
            broadcastBytes[i] = (byte)(networkBytes[i] | ~maskBytes[i]);
        }
        return new IPAddress(broadcastBytes);
    }


    static string GetMacAddress(char separator)
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