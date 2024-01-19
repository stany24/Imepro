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
        btnConfirm.Click += Confirm;
        btnVerify.Click += Verify;
        tbxIp.TextChanged += TextChanged;
        Closing += FormClosing;
    }

    private void Verify(object ?sender,RoutedEventArgs e)
    {
        if (tbxIp.Text == null) { return; }
        try
        {
            IPAddress.Parse(tbxIp.Text);
            btnConfirm.IsEnabled = true;
            lblErreur.Text = "";
        }
        catch { lblErreur.Text = "Veuillez entrer un adresse correcte"; }
    }

    private void Confirm(object? sender, RoutedEventArgs e)
    {
        if (tbxIp.Text == null) { return; }
        IpForTheWeek.SetIp(tbxIp.Text);
        _canClose = true;
        Close();
    }

    private void TextChanged(object ?sender, TextChangedEventArgs e)
    {
        btnConfirm.IsEnabled = false;
        lblErreur.Text = string.Empty;
    }

    private void FormClosing(object ?sender,WindowClosingEventArgs e)
    {
        if (!_canClose && !_canCloseWithoutIp) { e.Cancel = true; }
    }
}