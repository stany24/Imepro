using System.Net;
using Avalonia.Controls;
using Avalonia.Interactivity;
using StudentSoftware.Logic;

namespace StudentSoftware.Views;

public partial class AskIp : Window
{
    private bool _canClose;
    private readonly bool _canCloseWithoutIp;
    public AskIp(bool canCloseWithoutIp)
    {
        _canCloseWithoutIp = canCloseWithoutIp;
        InitializeComponent();
        BtnConfirm.Click += Confirm;
        BtnVerify.Click += Verify;
        TbxIp.TextChanged += TextChanged;
        Closing += FormClosing;
    }

    private void Verify(object ?sender,RoutedEventArgs e)
    {
        if (TbxIp.Text == null) { return; }
        try
        {
            IPAddress.Parse(TbxIp.Text);
            BtnConfirm.IsEnabled = true;
            LblErreur.Text = "";
        }
        catch { LblErreur.Text = "Veuillez entrer un adresse correcte"; }
    }

    private void Confirm(object? sender, RoutedEventArgs e)
    {
        if (TbxIp.Text == null) { return; }
        IpForTheWeek.SetIp(TbxIp.Text);
        _canClose = true;
        Close();
    }

    private void TextChanged(object ?sender, TextChangedEventArgs e)
    {
        BtnConfirm.IsEnabled = false;
        LblErreur.Text = string.Empty;
    }

    private void FormClosing(object ?sender,WindowClosingEventArgs e)
    {
        if (!_canClose && !_canCloseWithoutIp) { e.Cancel = true; }
    }
}