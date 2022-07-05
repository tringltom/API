API sluzi za primanje zahteva i vracanje odgovora.

## Controllers

Svi kontroleri nasledjuju CotrollerBase klasu koja ima anotaciju [ApiController] cime se smanjuje broj linija koda.

https://code-maze.com/apicontroller-attribute-in-asp-net-core-web-api/

Na svakom kontroleru definise se ruta i injektira po potrebi servis za svaki kontroler. Ime rute je u jednini , enpoinit su sortirani redosledom: Head, Get, Put, Patch, Delete, Post. Nazivi enpointa se formiraju o odnosu na resurs sa kojim se barata. https://restfulapi.net/resource-naming/