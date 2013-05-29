//     Program.cs
//     (c) 2013 Brett Ernst, Gabriel Isenberg https://github.com/gisenberg/piston
//     Licensed under the terms of the MIT license.

using Piston.Push.Driver;

namespace Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var gw = new PushGateway();

            // Windows 8
            gw.Register(Platform.Windows, "https://bn1.notify.windows.com/?token=AgYAAAAmTOnAWNOSj5kPueLw54fLzVxtM27mSHwB79Obk%2fjv03e9MvnW0ZbR4eGdY4LoevOGJn8mmwNX2oLB2QHCxuUdmQ2SOQOCJWVaHS9Y6aHyPJt%2b2uvF%2f88WmzauL3hSkEM%3d", "all");
            var wNote = new PushNotification<WindowsPayload>();
            wNote.Payload.SetToast("Hello World");
            wNote.Tokens.Add("https://bn1.notify.windows.com/?token=AgYAAAAmTOnAWNOSj5kPueLw54fLzVxtM27mSHwB79Obk%2fjv03e9MvnW0ZbR4eGdY4LoevOGJn8mmwNX2oLB2QHCxuUdmQ2SOQOCJWVaHS9Y6aHyPJt%2b2uvF%2f88WmzauL3hSkEM%3d");
            gw.Send(wNote);

            // Android
            var aNote = new PushNotification<AndroidPayload>();
            aNote.Payload.Data.Add("message", "TEST4");
            aNote.Segments.Add("all");
            var res = gw.Send(aNote);
            Console.WriteLine(res.Count);

            // Windows Phone - Toast Message
            var wpNote = new PushNotification<WindowsPhonePayload>();
            wpNote.Segments.Add("all");
            wpNote.Payload.SetToast("Hello World!", "");
            gw.Send(wpNote);


            // Windows Phone - Tile Message
            var wpNote = new PushNotification<WindowsPhonePayload>();
            wpNote.Segments.Add("all");
            //wpNote.Payload.SetTile(9, "Kittens!", "", "", "http://placekitten.com/480/480");
            wpNote.Payload.SetTile(9, "Kittens!", "", "", "http://lorempixel.com/173/173/city");
            gw.Send(wpNote);


            // iOS - Alert
            gw.Register(Platform.iOS, "8B4AEB7E108BA1C4681520BF074F9ABB0AF9A26A2D9F5712D31B5B0B4B5BE1A3", "all", "ipods");
            var iosNote = new PushNotification<IosPayload>();
            iosNote.Segments.Add("all");
            iosNote.Payload.SetAlert("Hello World!");
            var res = gw.Send(iosNote);
            Console.WriteLine(res.Success);
            Console.WriteLine(res.Count);

            // iOS - badge # update
            var iosNote = new PushNotification<IosPayload>();
            iosNote.Segments.Add("all");
            //iosNote.Payload.SetBadge(0);
            iosNote.Payload.SetBadge(3);
            gw.Send(iosNote);
        }
    }
}
