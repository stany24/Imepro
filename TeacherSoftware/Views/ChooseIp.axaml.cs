using System;
using System.Collections.Generic;
using System.Net;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TeacherSoftware.Views;

public partial class ChooseIp : Window
{
    private IPAddress _ipAddress;
    public ChooseIp(List<IPAddress> addresses)
    {
        InitializeComponent();
        foreach (IPAddress ipAddress in addresses)
        {
            LbxIp.Items.Add(ipAddress);
        }

        Closing += VerifyAndClose;
        BtnConfirm.Click += Verify;
    }

    private void Verify(object? sender, RoutedEventArgs e)
    {
        if(LbxIp.SelectedItems == null){return;}
        if (LbxIp.SelectedItems.Count != 1) return;
        if(LbxIp.SelectedItems[0] is not IPAddress address){return;}
        _ipAddress = address;
        Close();
    }

    private void VerifyAndClose(object? sender, WindowClosingEventArgs windowClosingEventArgs)
    {
        if (!windowClosingEventArgs.IsProgrammatic)
        {
            windowClosingEventArgs.Cancel = true;
        }
    }

    public IPAddress GetChoosenIp()
    {
        return _ipAddress;
    }
}