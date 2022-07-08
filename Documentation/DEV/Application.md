Core projekta i biznis logika

[[_TOC_]]

## Errors

Klase koje sluze za greske na apiju. Sadrze kod greske, poruke i imaju metodu Response() koja vraca odgovarajuci IActionResult, pa mogu da se koriste u servisima kao deo povratnog objekta za kontroler (videti servise).

## InfractructureInterfaces

Iterfejsi za 3rd party sisteme odnosno njihove metode. Ubaceno je ovde u svrhu lake promene 3rd partija.

## InfrastructureModels

Modeli za komunikaciju Application sloja sa infrastrukturnim.

## ManagerInterfaces

Interfejsi menadzera za injektiranje.

## Managers

Ideja je da ovde bude reusable kod na nivou servisa. Dakle ako se logika u jednom servisu ponavlja kao i u drugom, potebno je nju izdvojiti u menadzera i pozivati ga iz oba servisa.

## Mappings

Ovde se definisu sva mapiranja iz aplikacijskih modela i domenskih objekata i obrnuto.
Korisit se https://automapper.org/

## Models

Svi modeli koji se koriste u aplikacijskom sloju, ulazni i izlazni. (DTO)

## ServiceInterfaces

Interfejsi servisa za injektiranje.

## Services