##Domenski objekti

Ovde se definisu domenski objekti.
Veze
- 1 : 1 - u prvoj klasi treba definisati virtuelni objekat druge, takodje u drugoj virtuelni objekat prve
- 1 : many - u prvoj se definise virtual ICollection<Druga> a u drugoj virtuelni objekat prve
- many : many - u spojnoj se definisu po jedan virtualni objekat prve i druge klase a u njima se definisu virtual ICollection<Spojna>