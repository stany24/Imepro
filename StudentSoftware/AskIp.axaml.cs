using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Net;

namespace StudentSoftware;

public partial class AskIp : Window
{
    private bool canClose = false;
    private readonly bool CanCloseWithoutIp;
    public AskIp(bool canCloseWithoutIp)
    {
        CanCloseWithoutIp = canCloseWithoutIp;
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
        canClose = true;
        Close();
    }

    private void TextChanged(object ?sender, TextChangedEventArgs e)
    {
        btnConfirm.IsEnabled = false;
        lblErreur.Text = string.Empty;
    }

    private void FormClosing(object ?sender,WindowClosingEventArgs e)
    {
        if (!canClose && !CanCloseWithoutIp) { e.Cancel = true; }
    }
}