using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StudentSoftware.Models;

public class MainViewModel: ObservableObject
{
    private ObservableCollection<string> _messages = new();
    public ObservableCollection<string> Messages { 
        get => _messages;
        set => SetProperty(ref _messages, value);
    }
    
    private ObservableCollection<string> _infos = new();
    public ObservableCollection<string> Infos { 
        get => _infos;
        set => SetProperty(ref _infos, value);
    }
}