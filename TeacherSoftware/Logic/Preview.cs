using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace TeacherSoftware.Logic;

public class Preview:Grid
{
    public readonly int Id;
    private readonly string _name;
    private readonly Image _screenShot;
    private readonly TextBlock _textBlock;
    private readonly Button _btnSaveImage;

    public Preview(int id,string name,IImage image)
    {
        Id = id;
        _name = name;
        RowDefinitions = new RowDefinitions("Auto,5,Auto");
        ColumnDefinitions = new ColumnDefinitions("Auto,5,Auto");
        
        _screenShot = new Image
        {
            Source = image
        };
        SetColumn(_screenShot,0);
        SetColumnSpan(_screenShot,3);
        SetRow(_screenShot,0);
        
        _textBlock = new TextBlock
        {
            Text = _name+": "+DateTime.Now.ToLongTimeString()
        };
        SetColumn(_textBlock,2);
        SetRow(_textBlock,2);

        _btnSaveImage = new Button
        {
            Content = "Sauvegarder"
        };
        _btnSaveImage.Click += (_,_) => SaveScreenShot();
        SetColumn(_btnSaveImage,0);
        SetRow(_btnSaveImage,2);
    }

    public void UpdateInfos(IImage image)
    {
        _textBlock.Text = _name+": "+DateTime.Now.ToLongTimeString();
        _screenShot.Source = image;
    }

    private void SaveScreenShot()
    {
        string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string downloadFolderPath = Path.Combine(userProfilePath, "Downloads");
        if (_screenShot.Source is not Bitmap imageSource) return;
        using FileStream stream = new(downloadFolderPath, FileMode.Create);
        imageSource.Save(stream);
    }
}