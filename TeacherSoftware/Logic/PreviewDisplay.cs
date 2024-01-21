using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageMagick;

namespace TeacherSoftware.Logic;

public class PreviewDisplay:RelativePanel
{
    private readonly List<Preview> _previews = new();
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
            UpdateZoom();
        }
        
    }

    public void AddOrUpdatePreview(int id,string name,MagickImage image)
    {
        Preview? preview = _previews.Find(prev => prev.Id == id);
        using MemoryStream memStream = new();
        image.Write(memStream);
        Bitmap bitmap = new(memStream);
        if (preview != null)
        {
            preview.UpdateInfos(bitmap);
        }
        else
        {
            _previews.Add(new Preview(id,name,bitmap));
        }
    }
    
    public void RemovePreview(int id)
    {
        Preview? preview = _previews.Find(prev => prev.Id == id);
        if(preview == null){return;}
        _previews.Remove(preview);
    }

    private void UpdateZoom()
    {
        
    }
}