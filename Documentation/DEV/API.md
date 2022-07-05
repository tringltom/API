API sluzi za primanje zahteva i vracanje odgovora.

## Controllers

Svi kontroleri nasledjuju CotrollerBase klasu koja ima anotaciju [ApiController] cime se smanjuje broj linija koda.

https://code-maze.com/apicontroller-attribute-in-asp-net-core-web-api/

Na svakom kontroleru definise se ruta i injektira po potrebi servis za svaki kontroler. Ime rute je u jednini , enpoinit su sortirani redosledom: Head, Get, Put, Patch, Delete, Post. Nazivi enpointa se formiraju o odnosu na resurs sa kojim se barata. https://restfulapi.net/resource-naming/

Svi kontroleri su asinhroni, povratni tip je IActionResult, sto omogucava 200 povratni kod sa resorsom ili povratne kodove koji definisu greske sa ili bez poruka.

## Logs
Nije git tracked i tu se upisuju logovi na lokalu, koristeci Nlog https://nlog-project.org/

## Messages
Hub za chet. Ovde su definisane akcije koje sluze za live chat koristeci SignalR odnosno WebSocket-e.

https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-6.0

## Middleware
Glavni Exception hendler, ovde se susticu svi exceptionu u apiju.

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0

## Validations

Prva vrsta validacije jeste Validation filter koji sluzi za validaciju da id na rutama nije negativan.
Druga vrsta je vezana za validaciju objekata kao ulaznih podataka. Koriscena je Fluent Validacija.
https://docs.fluentvalidation.net/en/latest/
Takodje, postoji i custom pravilo za sifru kao ValidationExtensions.

## Program, Startup
Logovanje se za lokal radi preko Nloga- a na produkciji preko App insights https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview.
Pri pokretanju Aplikacije izvrsava se migracija databaze sa seedovanjem pocetnih podataka.

U startup-u definisu se servisi: UseLazyLoadingProxies omogucava po defoltu lazy loading
Dodaje se Cors policy za razlicite url-ove
Potrebni servisi. menadzeri, automaper, korisit se Identity za korisnicku logiku https://docs.identityserver.io/en/latest/quickstarts/6_aspnet_identity.html.
Autentifikacija se vrsi pomocu JWTBearer tokena i Refresh Tokena, gde se i zahteva postojanje auth tokena nad endpointima (za dozvolu ne autorizovanih korisnika koristi se atribut [AllowUnaUthorized]). Takodje swagger je implementiran radi pregleda kontrolera i testiranja.https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio