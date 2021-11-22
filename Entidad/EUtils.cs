using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Device.Location;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
namespace Entidad
{
    public class EUtils
    {
        static bool invalid = false;
        //new string[] {“BOT”, “-0400”, “”},
        static public string ServerDir = ConfigurationManager.AppSettings.Get("ImagenesDir");
        static public string ServerDirVirtual = ConfigurationManager.AppSettings.Get("ImagenesDirVirtual");

        public static string MinutosToHMString(long minutos)
        {
            if (minutos == 0)
            {
                return "0 horas";
            }
            string res = "";
            long horas = minutos / 60;
            if (horas > 0)
            {
                res += horas + " horas ";
            }
            long m = minutos % 60;
            if (m > 0)
            {
                res += m + " minutos ";
            }
            return res;
        }

        public static string ScaleImageNH(string imagebase64, int nh)
        {
            Image image = ToImage(imagebase64);
            int nw = image.Width * nh / image.Height;

            if (image.Width > nw)
            {
                int maxWidth = nw, maxHeight = nh;

                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                newImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = stream.ToArray();
                return Convert.ToBase64String(byteImage);
            }
            return imagebase64;
        }

        public static string ScaleImage(string imagebase64, int ancho)
        {
            Image image = ToImage(imagebase64);

            if (image.Width > ancho || image.Height > ancho)
            {
                int maxWidth = ancho, maxHeight;
                maxHeight = image.Height * ancho / image.Width;

                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                newImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = stream.ToArray();
                return Convert.ToBase64String(byteImage);
            }
            return imagebase64;
        }

        public static string ScaleImage(System.Drawing.Image image, int ancho)
        {
            int maxWidth = ancho, maxHeight;
            maxHeight = image.Height * ancho / image.Width;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            return ToBase64(newImage);
        }

        public static Image ToImage(string base64Image)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            return Image.FromStream(ms, true);
        }

        public static string ToBase64(Image newImage)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            newImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteImage = stream.ToArray();
            return Convert.ToBase64String(byteImage);
        }

        public static Image ScaleImageImage(Image image, int ancho)
        {
            int maxWidth = ancho, maxHeight;
            maxHeight = image.Height * ancho / image.Width;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static DateTime Now
        {
            get { return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time")); }
        }


        public static string DateFormatDateHour
        {
            get { return "dd/MM/yyyy HH:mm:ss"; }
        }
        public static string DateFormatDate
        {
            get { return "dd/MM/yyyy"; }
        }

        public static string DateFormatDiaDate
        {
            get { return "dddd dd/MM/yyyy"; }
        }

        public static string DateFormatDiaDateHour
        {
            get { return "dddd dd/MM/yyyy HH:mm:ss"; }
        }

        public static string DateFormatServer
        {
            get { return "yyyy-MM-dd'T'HH:mm:ss"; }
        }

        public static string DecimalFormat
        {
            get { return "#,##0.00"; }
        }

        public static DateTime NowDate
        {
            get { return TimeZoneInfo.ConvertTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time")); }
        }

        public static TimeSpan TimeNow
        {
            get { return Now.TimeOfDay; }
        }
        public static string CodeHTMLtoTexto(string texto)
        {
            var resultado = Regex.Replace(texto, "<.*?>", string.Empty);
            return resultado;
        }

        public static  bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        private static  string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        static public DateSincronizacion ObtenerFechaSincronizacion(string Fecha)
        {
            DateTime date = DateTime.ParseExact("1990-01-01T00:00:00", "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
            bool PrimeraVez = true;
            if (!string.IsNullOrEmpty(Fecha) && !Fecha.Equals("1990-01-01T00:00:00"))
            {
                date = DateTime.ParseExact(Fecha, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                PrimeraVez = false;
            }
            return new DateSincronizacion() { PrimeraVez = PrimeraVez, Date = date };
        }

        public static double ConvertCoordStrToDouble(string coordenada)
        {
            decimal NewCoord = Convert.ToDecimal(coordenada.Replace(".", ","));
            if (NewCoord < -200 || NewCoord > 200)
            {
                NewCoord = decimal.Parse(coordenada);
            }
            return System.Convert.ToDouble(NewCoord);
        }

        /// <summary>
        /// GetDistance realiza el calculo de coordenas de latitud y longitud de un origen y destino
        /// </summary>
        /// <param name="latO">Latitud Origen</param>
        /// <param name="lngO">Longitud Origen</param>
        /// <param name="latD">Latitud Destino</param>
        /// <param name="lngD">Latitud Destino</param>
        /// <returns>Distancia en metros , double</returns>
        public static double GetDistance(double latO, double lngO, double latD, double lngD)
        {
            var sCoord = new GeoCoordinate(latO, lngO);
            var eCoord = new GeoCoordinate(latD, lngD);
            return sCoord.GetDistanceTo(eCoord);
        }

        public static bool validarContrasena(string password)
        {
            bool formatoCorrecto = Regex.IsMatch(password, @"(?=^\S{6,20}$)(^(?=\S*[a-zA-Z])(?=\S*\d).*$)");

            return formatoCorrecto;
        }


        public static int obtenerDiaDeSemanaFromFecha(DateTime date)
        {
            if (date != null)
            {
                int day = (int)date.DayOfWeek;

                DateTime first = new DateTime(date.Year, date.Month, 1);
                GregorianCalendar _gc = new GregorianCalendar();
                int week = _gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - _gc.GetWeekOfYear(first, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

                return day;
            }

            return 0;

        }

        public static int obtenerSemanaDelMesFromFecha(DateTime date)
        {
            if (date != null)
            {
                DateTime first = new DateTime(date.Year, date.Month, 1);
                GregorianCalendar _gc = new GregorianCalendar();
                int week = _gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - _gc.GetWeekOfYear(first, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

                return week;
            }

            return 0;

        }

        public static string TiempoPublicacion(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format(" {0} {1} atrás",
                years, years == 1 ? "año" : "años");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format(" {0} {1} atrás",
                months, months == 1 ? "mes" : "meses");
            }
            if (span.Days > 0)
                return String.Format(" {0} {1} atrás",
                span.Days, span.Days == 1 ? "día" : "días");
            if (span.Hours > 0)
                return String.Format(" {0} {1} atrás",
                span.Hours, span.Hours == 1 ? "hora" : "horas");
            if (span.Minutes > 0)
                return String.Format(" {0} {1} atrás",
                span.Minutes, span.Minutes == 1 ? "minuto" : "minutos");
            if (span.Seconds > 5)
                return String.Format(" {0} segundos atrás", span.Seconds);
            if (span.Seconds <= 5)
                return "recientemente";
            return string.Empty;
        }

        public static void EscribirEnLog(Exception ex, string pNombreMetodo, params string[] pParametros)
        {
            var logHab = ConfigurationManager.AppSettings["logHabilitado"];
            if (Convert.ToBoolean(logHab))
            {
                StringBuilder sb = new StringBuilder();

                var filePath = ConfigurationManager.AppSettings["PathLog"];
                string titulo = "--------------------------------------------   " + DateTime.Now.ToString() + "   --------------------------------------------";
                sb.AppendLine(titulo);

                string nombreMetodo = "Método:          " + pNombreMetodo;
                sb.AppendLine(nombreMetodo);

                if (pParametros != null)
                {
                    string parametros = "Parámetros:      " + pParametros[0];
                    sb.AppendLine(parametros);
                    for (int i = 1; i < pParametros.Length - 1; i++)
                    {
                        parametros = "                 " + pParametros[i];
                        sb.AppendLine(parametros);
                    }
                }
                else
                {
                    string parametros = "Parámetros:      Sin parámetros";
                    sb.AppendLine(parametros);
                }

                string mensajeExcepcion = "Mensaje:         ";
                if (ex.Message != null)
                    mensajeExcepcion += ex.Message;
                sb.AppendLine(mensajeExcepcion);

                string innerException = "Inner Exception: ";
                if (ex.InnerException != null)
                    innerException += ex.InnerException.ToString();
                sb.AppendLine(innerException);

                string clase = "Origen:          ";
                if (ex.Source != null)
                    clase += ex.Source.ToString();
                sb.AppendLine(clase);

                string targetSite = "                 ";
                if (ex.TargetSite != null)
                    targetSite += ex.TargetSite.ToString();
                sb.AppendLine(targetSite);
                sb.AppendLine("");

                var fecha = DateTime.Now.ToString("yyyy-MM-dd");
                File.AppendAllText(filePath + fecha + ".txt", sb.ToString());

                sb.Clear();
            }
        }


        public static string InsertarImagen(string SubCarpeta, string SrcBase64)
        {
           
            string Dir = SubCarpeta;
            string FDir = ServerDir + "\\" + SubCarpeta;
            bool exists = Directory.Exists(FDir);

            if (!exists)
            {
                Directory.CreateDirectory(FDir);
            }

            Image Image = Base64ToImage(SrcBase64);
            try
            {
                ImageFormat format = Image.RawFormat as ImageFormat;
                string Nombre = "F" + EUtils.GenerateRandomCodeByN(20) + GetExtensionFromImageFormat(format);
                string Path = Dir + "//" + Nombre;
                string FPath = FDir + "\\" + Nombre;
                Image.Save(FPath, ImageFormat.Png);
                return Path;
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo almacenar la imagen: " + ex.Message);
            }
        }



        public static string InsertarImagenMultipart(string SubCarpeta,string localFileName, string fileName,string extension,int n)
        {

            string Dir = SubCarpeta;
            string FDir = ServerDir + "\\" + SubCarpeta;
            bool exists = Directory.Exists(FDir);

            if (!exists)
            {
                Directory.CreateDirectory(FDir);
            }

            try
            {
                fileName = "F" + EUtils.GenerateRandomCodeByN(20) +n+ extension;

                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }
                File.Move(localFileName, Path.Combine(FDir, fileName));
                string path = Dir + "//" + fileName;
                return path;
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo almacenar la imagen: " + ex.Message);
            }
        }

        public static string URLImagen(string Src)
        {
            if (Src == "")
            {
                return "";
            }
            return ServerDirVirtual + Src;
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = System.Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static string ImageToBase64(Image Image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                Image.Save(m, Image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static string ObtenerImagenBase64(string Src)
        {
            if (Src!="")
            {
                try
                {
                    Image img = Image.FromFile(ServerDir + "\\" + Src);
                    return ImageToBase64(img);
                }
                catch(BussinessException e)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
       
        }
        public static string GetExtensionFromImageFormat(ImageFormat img)
        {
            if (img.Equals(ImageFormat.Jpeg)) return ".jpg";
            else if (img.Equals(ImageFormat.Png)) return ".png";
            else if (img.Equals(ImageFormat.Gif)) return ".gif";
            else if (img.Equals(ImageFormat.Bmp)) return ".bmp";
            else if (img.Equals(ImageFormat.Tiff)) return ".tif";
            else if (img.Equals(ImageFormat.Icon)) return ".ico";
            else if (img.Equals(ImageFormat.Emf)) return ".emf";
            else if (img.Equals(ImageFormat.Wmf)) return ".wmf";
            else if (img.Equals(ImageFormat.Exif)) return ".exif";
            else if (img.Equals(ImageFormat.MemoryBmp)) return ".bmp";
            return ".unknown";
        }


        public static string GenerateRandomCode()
        {
            Random random = new Random();
            string s = "";
            for (int i = 0; i < 10; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }


        public static string GenerateRandomCodeByN(int n)
        {
            Random random = new Random();
            string s = "";
            for (int i = 0; i < n; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;

        }
    }



    public class DateSincronizacion
    {
        public bool PrimeraVez { get; set; }
        public DateTime Date { get; set; }
    }
}
