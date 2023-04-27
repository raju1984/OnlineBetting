using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace QuickBetCore.Models
{
    public class ApplicatiopnCommonFunction
    {
        public static string GetRandomPassword(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }
        public static string FormatStringCurrency(decimal decimalValue)
        {
            return string.Format("{0:#,##0.00}", decimalValue);
        }
        public static string AlphanumbericNumber()
        {
            //Random generator = new Random();
            //return generator.Next(0, 999999).ToString("D6");
            string number = DateTime.UtcNow.Year.ToString() + DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString();
            return number;
        }
        //public static Byte[] GenerateBarcode(string BarCodeNumber)
        //{
        //    Byte[] byteArray;
        //    var width = 250; // width of the Qr Code
        //    var height = 250; // height of the Qr Code
        //    var margin = 0;
        //    var qrCodeWriter = new ZXing.BarcodeWriterPixelData
        //    {
        //        Format = ZXing.BarcodeFormat.QR_CODE,
        //        Options = new QrCodeEncodingOptions
        //        {
        //            Height = height,
        //            Width = width,
        //            Margin = margin
        //        }
        //    };
        //    var pixelData = qrCodeWriter.Write(BarCodeNumber);
        //    // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference
        //    // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB
        //    using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        //    {
        //        using (var ms = new MemoryStream())
        //        {
        //            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        //            try
        //            {
        //                // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
        //                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
        //            }
        //            finally
        //            {
        //                bitmap.UnlockBits(bitmapData);
        //            }
        //            // save to stream as PNG
        //            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //            byteArray = ms.ToArray();
        //        }
        //    }
        //    return byteArray;
        //}

        public static string GetImage(string img, string imgtype)
        {
            if (!string.IsNullOrEmpty(img))
            {
                //string filepath = HostingEnvironment.MapPath(img);
                //if (File.Exists(filepath))
                //{
                //    return img;
                //}
                return img;
            }
            if (imgtype == ImageType.dp.ToString())
            {
                return ApplicationVariable.defaultdp;
            }
            else
            {
                return "";
            }
        }

        public static bool SendEmail(string email, string name, string password, int MailType)
        {
            bool IsSend = false;
            try
            {
                var client = new SmtpClient(MailServerHost, Convert.ToInt32(MailServerPort))
                {
                    Credentials = new NetworkCredential(MailServerUsername, MailServerPassword),
                    EnableSsl = false,
                };
                //MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName),);
                if (MailType == 1)
                {
                    MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                    mail.Subject = "Password Reset";
                    mail.Body = string.Format("Hi {0},<br><br>We received a request for a new password for " +
                        "your account associated with this email address.<br><br>Your temporary password is : <b>{1}</b><br><br>Please open the app and sign in to your account with this temporary password." +
                        "<br><br>Sincerely,<br>{2}", name, password, SincerelyName);
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                    IsSend = true;
                }
                else if (MailType == 2)
                {

                    email = "vinod190@mailinator.com";
                    MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                    mail.Subject = "Someone Contact Us";
                    mail.Body = string.Format("Hi Team,<br><br>Please find below detail: " +
                       "<br><br><b>Name:{0}</b><br><br><b>Email:{1}</b><br><b>Message:{2}</b>", name, email, password);
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                    IsSend = true;
                }
                else if (MailType == 3)
                {

                    MailMessage mail = new MailMessage(new MailAddress(MailFromAddress, MailFromName), new MailAddress(email));
                    mail.Subject = "Verify your email address to complete the sign up process";
                    mail.Body = string.Format("Hi {0},<br><br>Please use the following verification code to verify your email with AdequateTravel: " +
                       "<br><br><b>{1}</b><br><br>If you didn't request this code, you can safely ignore this email. " +
                       "Someone else might have typed your email address by mistake." +
                        "<br><br>Sincerely,<br>{2}", name, password, SincerelyName);
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                    IsSend = true;
                }
                return IsSend;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        //public static bool SendEmail(MailModelViewModel email)
        //{

        //    using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
        //    {
        //        MailMessage mail = new MailMessage();
        //        mail.To.Add(email.To);
        //        mail.From = new MailAddress(email.From);
        //        mail.Subject = email.Subject;
        //        string Body = email.Body;
        //        mail.Body = Body;
        //        mail.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            UseDefaultCredentials = true,
        //            Credentials = new System.Net.NetworkCredential("schnaubiiteam@gmail.com", "Schnaubii@1234"), // Enter seders User name and password   
        //            EnableSsl = true
        //        };
        //        smtp.Send(mail);
        //        return true;
        //    }

        //}
    }
}
