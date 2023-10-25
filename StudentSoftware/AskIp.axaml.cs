using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Net;

namespace StudentSoftware;

public partial class AskIp : Window
{
    private bool canClose = false;
    public AskIp()
    {
        InitializeComponent();
        btnConfirm.Click += Confirm;
        btnVerify.Click += Verify;
        tbxIp.TextChanged += TextChanged;
        Closing += FormClosing;
    }

    private void Verify(object ?sender,RoutedEventArgs e)
    {
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
        IpForTheWeek.SetIp(tbxIp.Text);
        canClose = true;
    }

    private void TextChanged(object ?sender, TextChangedEventArgs e)
    {
        btnConfirm.IsEnabled = false;
        lblErreur.Text = string.Empty;
    }

    private void FormClosing(object ?sender,WindowClosingEventArgs e)
    {
        if (!canClose) { e.Cancel = true; }
    }
}