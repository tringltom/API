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