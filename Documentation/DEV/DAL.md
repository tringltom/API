Sloj za pristup podacima
[[_TOC_]]

## Query
Svi queriji nasledjuju QueryObject koji sluzi kao model za filtriranje i search trenutno. Ideja je da se koroz ove objekte definisu sta se trazi,filtrira,sortira itd..

## Repositories
Postoji Base Repository koji ima metode za bazicne CRUD operacije, svaki repository nasledjuje Base i dodaje metode za specificne querije.

## RepositoryInterfaces
Interfejsi za injektiranje svakog repositorija

## UnitOfWork
Klasa koja sadrzi sve repositorije i obezbedjuje transakciju nad njima. Preko nje se pristupa repositorijumima
