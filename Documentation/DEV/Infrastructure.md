Komunikacija sa 3rd party komponentama i za generisanje tokena
[[_TOC_]]

## Email

Za slanje svih mejlova preko http://www.mimekit.net/# koriste se podaci iz Settings-a.

SendGrid je (Twillio-v) servis putem koga saljemo elektronsku postu, koristeci STMProtokol.

Detaljnje informacije mogu se naci putem ovog linka https://www.twilio.com/blog/send-email-in-csharp-dotnet-using-smtp-and-sendgrid
dok ovaj dokument sadrzi nephodne informacije za razumevanje SendGrida za nase potrebe.

Nas SendGrid resors na Azure-u

https://portal.azure.com/#@TringltoM.onmicrosoft.com/resource/subscriptions/c193b773-7cfa-492b-8abb-34abd2cb1180/resourceGroups/Ekviti/providers/Microsoft.SaaS/resources/SendGrid/overview

Otvoriti SendGrid stranicu (koristiti EkvitiDev@outlook.com nalog)

Activity sekcija - Pregled aktivnosti za nalog
https://app.sendgrid.com/email_activity?page=1

ApiKeys sekcija - Sekcija za manipulisanje i kreiranje api kljuceva (trenutni Api kljuc ima odobrenje da samo salje mailove)
https://app.sendgrid.com/settings/api_keys

Bitne varijable

 - Sender - EkvitiDev@Outlook.com (Taj sender smo verifikovali na nasem SendGrid resorsu za Azure)
 - UserName - apikey (defaltni userName za SMTP konekciju putem sendgrid servera sa api kljucem)
 - Password - **** (api kljuc koji smo dobili od sendGrida, injektovan u release putem azure secrets-a)
 - Server - smtp.sendgrid.net (defaltni server)
 - Port - 587 (defaltni port) 

#322

## Media

Za cuvanje slika koristi se https://cloudinary.com/. U bazi se cuva id slike dok je slika na Cloudinariju. Koriste se podaci iz Settings-a

## Security

Hendlovanje access i refresh tokena, pristupa podacima u tokenima. User accessor jedna od koriscenijih klasa, sluzi za izvlacenje korisnickog imena ili korisnickog id-ja iz http konteksta koji ima podatke iz tokena.

##Settings

Modeli settingsa iz konfiga, koriscen je Options pattern https://www.c-sharpcorner.com/article/asp-net-core-how-to-acces-configurations-using-options-pattern/.