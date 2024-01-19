using ImageMagick;
using ScreenCapture.NET;

namespace ClassLibrary6;

public class ScreenShotTaker
{
    private readonly List<IScreenCapture> _screenCaptures = new();
    private readonly List<ICaptureZone> _iCaptureZones = new();
    public ScreenShotTaker()
    {
        IScreenCaptureService screenCaptureService = new X11ScreenCaptureService();
        
        IEnumerable<GraphicsCard> graphicsCards = screenCaptureService.GetGraphicsCards();
        
        IEnumerable<Display> displays = screenCaptureService.GetDisplays(graphicsCards.First());

        foreach (Display display in displays)
        {
            _screenCaptures.Add(screenCaptureService.GetScreenCapture(display));
        }

        foreach (IScreenCapture screenCapture in _screenCaptures)
        {
            _iCaptureZones.Add(screenCapture.RegisterCaptureZone(0, 0, screenCapture.Display.Width, screenCapture.Display.Height));
            Task.Run(() => screenCapture.CaptureScreen());
        }
    }
    public MagickImage TakeAllScreenShot()
    {
        MagickImageCollection images = new();
        
        foreach (ICaptureZone captureZone in _iCaptureZones)
        {
            using (captureZone.Lock())
            {
                images.Add(new MagickImage(captureZone.RawBuffer));
            }
        }
        
        return (MagickImage)images.Mosaic();
    }

    public MagickImage TakeScreenShot(int screenId)
    {
        ICaptureZone captureZone = _iCaptureZones[screenId];
        using (captureZone.Lock())
        {
            return new MagickImage(captureZone.RawBuffer);
        }
    }
}