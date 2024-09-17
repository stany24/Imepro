using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageMagick;

namespace TeacherSoftware.Logic;

public class PreviewDisplay:RelativePanel
{
    public static readonly StyledProperty<ObservableCollection<Preview>> PreviewsProperty = 
        AvaloniaProperty.Register<PreviewDisplay, ObservableCollection<Preview>>(nameof(Previews), defaultValue: new ObservableCollection<Preview>());
    public ObservableCollection<Preview> Previews { get; set; } = new();
    
    public static readonly StyledProperty<int> ZoomProperty = 
        AvaloniaProperty.Register<PreviewDisplay, int>(nameof(Zoom), defaultValue: 100);
    private int _zoom;
    public int Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value switch
            {
                < 0 => 0,
                > 100 => 100,
                _ => value
            };
            UpdatePreviewLocationAndSize();
        }
    }

    public PreviewDisplay()
    {
        SizeChanged += (_,_) => UpdatePreviewLocationAndSize();
        Previews.CollectionChanged += (_,_) => UpdatePreviewLocationAndSize();
    }

    public void AddOrUpdatePreview(int id,string name,MagickImage image)
    {
        Preview? preview = Previews.ToList().Find(prev => prev.Id == id);
        using MemoryStream memStream = new();
        image.Write(memStream);
        Bitmap bitmap = new(memStream);
        if (preview != null)
        {
            preview.UpdateInfos(bitmap);
        }
        else
        {
            Previews.Add(new Preview(id,name,bitmap));
        }
    }
    
    public void RemovePreview(int id)
    {
        Preview? preview = Previews.ToList().Find(prev => prev.Id == id);
        if(preview == null){return;}
        Previews.Remove(preview);
    }

    private void UpdatePreviewLocationAndSize()
    {
        
    }
}