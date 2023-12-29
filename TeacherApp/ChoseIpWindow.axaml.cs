using System.Collections.Generic;
using System.Net;
using Avalonia.Controls;

namespace TeacherApp;

public partial class ChoseIpWindow : Window
{
    public ChoseIpWindow(List<IPAddress> addresses)
    {
        InitializeComponent();
        foreach (IPAddress address in addresses) { LbxIps.Items.Add(address); }
    }

    public IPAddress GetChoosenIp()
    {
        if (LbxIps.SelectedItems is { Count: > 0 })
        {
            return LbxIps.SelectedItems[0] as IPAddress;
        }
        else
        {
            return LbxIps.Items[0] as IPAddress;
        }
    }
}