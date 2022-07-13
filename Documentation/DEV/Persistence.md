Izmene seme i podataka nad bazom
[[_TOC_]]


##Migrations

Koristi se code first pristup, dakle definisanjem dDbSetova domenskih modela kada se pokrene generisu se migracije. Svaka migracija u sebi sadrzi izmene nad bazom kada se primeni ili kada se skine (up and Down)

##DataContext
Kada se domenski objekat napise, potrebno je dodati DB set u DataContext, takodje moze se dodati i seed podataka. u PK manager consoli se pokrene prvo:
Add-Migration InitialCreate gde je default Project Persistence, a zatim ako bild prodje pokrene se Update-Database.
https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs

