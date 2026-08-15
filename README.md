# အဓိဋ္ဌာန် — .NET 10 Blazor WebAssembly PWA

မူရင်း `base.apk` (package `com.a_dhi_htan`, version `1.3.1`) ထဲမှ screen flow၊ အဓိဋ္ဌာန်အချက်အလက်၊ အစီအစဉ်နှင့် မူရင်း assets များကို ပြန်လည်ဖော်ထုတ်ပြီး **standalone .NET 10 Blazor WebAssembly** PWA အဖြစ် ပြန်လည်တည်ဆောက်ထားသည်။ ASP.NET Core server project မလိုဘဲ Vercel static hosting ပေါ်တွင် တိုက်ရိုက်တင်နိုင်သည်။

## ပါဝင်သော flow များ

- Login သို့မဟုတ် server account မလိုသော local-only အသုံးပြုမှု
- ကိုးနဝင်း၊ ခန္တီစေတီ၊ ဂုဏ်တော်တစ်ထောင်၊ ဝါတွင်းသုံးလနှင့် စိတ်ကြိုက်တစ်ရက်ပုတီး
- APK မှ ပြန်လည်ရရှိသော schedule ၁၉၇ ခုနှင့် အသေးစိတ်လမ်းညွှန်များ
- ပုတီးကောင်တာပုံစံ ၁၃ မျိုး၊ အသံ၊ တုန်ခါမှု၊ reset confirmation နှင့် wake lock
- Light/dark/high-contrast theme၊ ရာသီနောက်ခံများနှင့် APK မူရင်း Rabbit rules အတိုင်း Unicode/Zawgyi ပြောင်းလဲမှု
- IndexedDB ထဲတွင် အစီအစဉ်၊ ရေတွက်မှုနှင့် မှတ်တမ်းများ အလိုအလျောက်သိမ်းဆည်းခြင်း
- JSON backup ထုတ်ခြင်းနှင့် ပြန်သွင်းခြင်း
- ပထမတစ်ကြိမ် online ဖွင့်ပြီးနောက် app၊ WebAssembly runtime၊ ပုံ၊ အသံနှင့် data အားလုံးကို offline အသုံးပြုနိုင်ခြင်း

## Local run

```powershell
dotnet run --project .\Adhihtan\Adhihtan.csproj
```

Console မှပြသော development URL ကို browser ဖြင့်ဖွင့်ပါ။ Development mode တွင် service worker က cache မလုပ်ပါ။ Offline/PWA အပြည့်အစုံကို Release publish ဖြင့်စမ်းပါ။

## Static publish

```powershell
dotnet publish .\Adhihtan\Adhihtan.csproj -c Release -o .\artifacts\publish
```

Static host အဖြစ် `artifacts/publish/wwwroot` folder ထဲကဖိုင်များကို deploy လုပ်ပါ။ Client-side routes အားလုံးကို `/index.html` သို့ fallback/rewrite လုပ်ထားရမည်။ PWA install နှင့် service worker အတွက် production တွင် HTTPS ဖြင့် host လုပ်ပါ။

## Vercel deploy

Repository root ရှိ `vercel.json` က အောက်ပါတို့ကို အလိုအလျောက်လုပ်ပေးသည်။

- Vercel Linux build image တွင် .NET 10 SDK ကို local `.dotnet` folder သို့ install လုပ်ခြင်း
- `Adhihtan` ကို Release publish လုပ်ခြင်း
- `artifacts/vercel/wwwroot` ကို static output အဖြစ် deploy လုပ်ခြင်း
- Blazor client-side routes အားလုံးကို `/index.html` သို့ rewrite လုပ်ခြင်း
- service worker ကို revalidate လုပ်ပြီး fingerprinted framework files ကို long-term cache လုပ်ခြင်း

Vercel dashboard မှ repository ကို import လုပ်ပြီး deploy လုပ်နိုင်သည်။ Project settings ထဲတွင် framework preset သို့မဟုတ် build/output commands ကို ထပ်ဖြည့်ရန်မလိုပါ။ Vercel CLI ရှိပါက repository root မှ—

```powershell
vercel --prod
```

## APK data ပြန်လည်ထုတ်ယူခြင်း

`tools/recover-hermes-data.mjs` သည် decompiled Hermes modules မှ category/schedule data ကို reproducibly ပြန်ထုတ်ပေးသည်။ လက်ရှိအသုံးပြုသော output သည် `Adhihtan/wwwroot/data/recovered-content.json` ဖြစ်သည်။
