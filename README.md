Piston Push Services
============
Client and server binaries for push services hosted on Windows with support for:

* Android 
* iOS (Alerts and Badges)
* Windows Phone 8 (Tiles, Toast and Images) 
* Windows 8

##Installation and Setup
See docs/pushgw_installation.docx for detailed configuration information.

##Examples

```
            // Windows 8
            var gw = new PushGateway();
            gw.Register(Platform.Windows, "https://bn1.notify.windows.com/?token=AgYAAAAmTOnAWNOSj5kPueLw54fLzVxtM27mSHwB79Obk%2fjv03e9MvnW0ZbR4eGdY4LoevOGJn8mmwNX2oLB2QHCxuUdmQ2SOQOCJWVaHS9Y6aHyPJt%2b2uvF%2f88WmzauL3hSkEM%3d", "all");
            var wNote = new PushNotification<WindowsPayload>();
            wNote.Payload.SetToast("Hello World");
            wNote.Tokens.Add("https://bn1.notify.windows.com/?token=AgYAAAAmTOnAWNOSj5kPueLw54fLzVxtM27mSHwB79Obk%2fjv03e9MvnW0ZbR4eGdY4LoevOGJn8mmwNX2oLB2QHCxuUdmQ2SOQOCJWVaHS9Y6aHyPJt%2b2uvF%2f88WmzauL3hSkEM%3d");
            gw.Send(wNote);
```

```
            // Android
            var gw = new PushGateway();
            var aNote = new PushNotification<AndroidPayload>();
            aNote.Payload.Data.Add("message", "TEST4");
            aNote.Segments.Add("all");
            var res = gw.Send(aNote);
```

```
            // Windows Phone - Toast Message
            var gw = new PushGateway();
            var wpNote = new PushNotification<WindowsPhonePayload>();
            wpNote.Segments.Add("all");
            wpNote.Payload.SetToast("Hello World!", "");
            gw.Send(wpNote);
```

```
            // Windows Phone - Tile Message
            var gw = new PushGateway();
            var wpNote = new PushNotification<WindowsPhonePayload>();
            wpNote.Segments.Add("all");
            wpNote.Payload.SetTile(9, "Kittens!", "", "", "http://placekitten.com/480/480");
            gw.Send(wpNote);
```

```
            // iOS - Alert
            var gw = new PushGateway();
            gw.Register(Platform.iOS, "8B4AEB7E108BA1C4681520BF074F9ABB0AF9A26A2D9F5712D31B5B0B4B5BE1A3", "all", "ipods");
            var iosNote = new PushNotification<IosPayload>();
            iosNote.Segments.Add("all");
            iosNote.Payload.SetAlert("Hello World!");
            var res = gw.Send(iosNote);
```

```
            // iOS - badge # update
            var gw = new PushGateway();
            var iosNote = new PushNotification<IosPayload>();
            iosNote.Segments.Add("all");
            iosNote.Payload.SetBadge(3);
```

##Licensing
Piston is licensed under the terms of the MIT License, see the included MIT-LICENSE file.
