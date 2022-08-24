using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

#nullable disable

namespace Jaya.Shell.Converters
{
    public class WindowIconToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value != null) && (value is WindowIcon windowIcon))
            {
                Bitmap retBmp = null;
                MemoryStream winIconStream = new MemoryStream();
                
                try
                {
                    windowIcon.Save(winIconStream);
                    winIconStream.Position = 0;
                    retBmp = new Bitmap(winIconStream);
                }
                catch (ArgumentNullException)
                {
                    try
                    {
                        winIconStream.Position = 0;
                        Icon icon = new Icon(winIconStream);
                        
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        bmp.Save(winIconStream, ImageFormat.Png);
                        winIconStream.Position = 0;
                        retBmp = new Bitmap(winIconStream);
                    }
                    catch (ArgumentException)
                    {
                        string executablePath = Assembly.GetEntryAssembly().Location;
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Path.GetExtension(executablePath).TrimStart('.').Equals("dll", StringComparison.OrdinalIgnoreCase))
                            executablePath = Path.ChangeExtension(executablePath, "exe");
                        else
                            executablePath = Path.ChangeExtension(executablePath, null);
                        if (!File.Exists(executablePath))
                            return null;
                        
                        Icon icon = Icon.ExtractAssociatedIcon(executablePath);
                        System.Drawing.Bitmap bmp = icon.ToBitmap();
                        Stream exeIconStream = new MemoryStream();
                        bmp.Save(exeIconStream, ImageFormat.Png);
                        exeIconStream.Position = 0;
                        retBmp = new Bitmap(exeIconStream);
                        exeIconStream.Close();
                    }
                }

                winIconStream.Close();
                return retBmp;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}