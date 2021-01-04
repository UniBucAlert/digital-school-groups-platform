# digital-school-groups-platform

## Functionalitati
- sa existe **4 tipuri de utilizatori**: vizitator neinregistrat, utilizator inregistrat, moderator si administrator. (0.5p :heavy_check_mark:) 
- orice utilizator poate cauta **grupuri** in platforma (0.5p :heavy_check_mark:) si poate vizualiza informatii despre grupul respectiv. (0.5p :heavy_check_mark:) Grupurile vor fi impartite pe **categorii**: muzica, hobby-uri, cultura, etc (create dinamic), existand posibilitatea de adaugare a noi categorii (adminul poate face CRUD pe categorii). (0.5p :heavy_check_mark:)
- utilizatorii inregistrati pot crea grupuri noi sau se pot alatura altor grupuri. (1.0p :heavy_check_mark:). Utilizatorul care a creat un grup devine implicit **moderatorul** grupului respectiv (ceea ce inseamna ca poate sterge grupul daca doreste). (1.0p :heavy_check_mark:)
- moderatorul grupului poate accepta membrii noi sau poate revoca membrii care au un comportament neadecvat. Moderatorul poate sa permita **dreptul de moderare** si altor membrii ai grupului. (1.0p :heavy_check_mark:)
- membrii unui grup pot adauga **mesaje** si pot adauga noi **activitati** in calendarul grupului. De asemenea, pot sterge si edita propriile mesaje. (1.5p :heavy_check_mark: - Obs: date picker WIP!)
- membrii unui grup pot fi vizualizati intr-o lista separata. (0.5p :heavy_check_mark:)
- un utilizator inregistrat poate sa faca parte din mai multe grupuri. Astfel utilizatorul are la dispozitie o lista cu grupurile din care face parte. (1.0p :heavy_check_mark:)
- administratorul se ocupa de buna functionare a platformei. Acesta poate sterge grupurile care au continut neadecvat, mesaje, etc. sau poate revoca sau activa drepturile utilizatorilor si editorilor. (1.0p :heavy_check_mark:). 

## Observatii
- Proiectele trebuie realizate in **ASP.NET MVC 5**.
- Interfata cu utilizatorul trebuie sa existe si se poate folosi Bootstrap sau orice alt framework pentru frontend, chiar si un template
(pentru lipsa designului se scad pana la 2 pct).
- Atentie la campurile obligatorii in momentul completarii datelor in formulare, cat si la respectarea tipului de date pentru fiecare camp
existent (Validari).
- Accesarea paginilor sa se realizeze prin intermediul butoanelor sau link-urilor.
- Administratorul are control deplin asupra aplicatiei (poate face CRUD – CREATE, READ, UPDATE, DELETE – adauga, vizualiza, edita, sterge
orice tip de informatie corespunzatoare aplicatiei).
- Nota maxima se obtine in momentul in care se respecta cerintele, dar si aceste reguli esentiale dezvoltarii unei aplicatii web.
