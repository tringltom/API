Komunikacija sa 3rd party komponentama i za generisanje tokena
[[_TOC_]]

## Email

Za slanje svih mejlova preko http://www.mimekit.net/# koriste se podaci iz Settings-a.

## Media

Za cuvanje slika koristi se https://cloudinary.com/. U bazi se cuva id slike dok je slika na Cloudinariju. Koriste se podaci iz Settings-a

## Security

Hendlovanje access i refresh tokena, pristupa podacima u tokenima. User accessor jedna od koriscenijih klasa, sluzi za izvlacenje korisnickog imena ili korisnickog id-ja iz http konteksta koji ima podatke iz tokena.

##Settings

Modeli settingsa iz konfiga, koriscen je Options pattern https://www.c-sharpcorner.com/article/asp-net-core-how-to-acces-configurations-using-options-pattern/.