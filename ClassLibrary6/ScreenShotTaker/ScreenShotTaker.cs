using ImageMagick;
using ScreenCapture.NET;

namespace ClassLibrary6.ScreenShotTaker;

public class ScreenShotTaker
{
    private readonly List<IScreenCapture> _screenCaptures = new();
    private readonly List<ICaptureZone> _iCaptureZones = new();
    public ScreenShotTaker()
    {
        IScreenCaptureService screenCaptureService;
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            screenCaptureService = new DX11ScreenCaptureService();
        }
        else
        {
            screenCaptureService = new X11ScreenCaptureService();
        }
        
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
                MagickImage imageMagick = new(MagickColors.White, captureZone.Width,captureZone.Height);
                for (int i = 0; i < captureZone.Width; i++)
                {
                    for (int j = 0; j < captureZone.Height; j++)
                    {
                        imageMagick.GetPixels().SetPixel(i,j,new byte[]{captureZone.Image[i, j].R,captureZone.Image[i, j].G,captureZone.Image[i, j].B});
                    }
                }
                images.Add(imageMagick);
            }
        }

        MagickImage final = (MagickImage)images.Mosaic();
        
        final.Format = MagickFormat.Rgb;
        return final;
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