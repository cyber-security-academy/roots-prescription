# Incident Response Team (IRT) tar kontakt!

<details>
<summary>Incident Response Team (IRT) er...</summary>
... teamet som håndterer hendelser i organisasjonen. De har gjerne oversikt over alle systemer og deres logger. Blir et system angrepet, er det de som undersøker hva som har skjedd. 
</details><br>

Denne oppgaven kommer i 3 steg. Det er lurt å gjør stegene etter hverandre og deploye til prod mellom hver gang. Dette er en litt mer komplisert oppgave enn tidligere, ikke nøl med å spørre om hjelp!

---

### Steg 1: Hvem gjør kallet?
Incident Response Teamet (IRT) har tatt kontakt! 
De ser at det er unormalt mange kall mot endepunktene for nedlasting av filer, 
men de forstår ikke hva som skjer fordi loggmeldingene er uklare!

Dere får i oppgave å gjøre loggene mer tydelige slik at IRT-teamet får undersøkt hva som foregår.
IRT ber dere rette opp i [denne loggmeldingen](/RootsPrescription/Controllers/InvoiceController.cs#L83). De har ett krav **de trenger å vite hvilken bruker som gjør kallet**.

> Tips!💡 Funksjonen har kode som er kommentert ut. Dere kan fjerne kommentarene (gjøre det om til kode) for å hente brukeren. Sjekk funksjonen over, `GetMyInvoices()` for inspirasjon. [Linje 43](/RootsPrescription/Controllers/InvoiceController.cs#L43) viser hvordan dere kan logge et brukernavn.

Legg til endringene i produksjonsmiljøet. Fortsett på steg 2, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

---

### Steg 2: Er det en uautorisert bruker som gjør kallet?
Dere ser i loggene at det er noen uten bruker som henter filer! Å nei!
Det betyr at vi har samme sårbarhet som i den store hendelsen. 
IRT ber oss legge til informasjon i loggene. Kravet er å logge **forsøk på kall av brukere som ikke er autorisert til å gjøre kallet**.

Et eksempel på en som ikke er autorisert er en bruker som er autentisert (innlogget), men ikke eier PDFen som kallet prøver å hente ut.

> Tips!💡 Sjekk linje 61-67 i funksjonen som allerede er fikset på etter den store hendelsen, [GetPDF i PrescriptionController.cs](/RootsPrescription/Controllers/PrescriptionController.cs#61-67). Der kan dere få inspirasjon. 

Legg til endringene i produksjonsmiljøet. Fortsett på steg 3, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

---

### Steg 3: Prøver noen å hente en fil som ikke finnes?
Esra i utviklingsteamet spør IRT om de ikke burde logge forsøk på filer som ikke finnes. 
IRT er _helt_ enig! De gir dere kravet å logge **forsøk på å laste ned filer som ikke finnes, og hvem som gjør kallet**.


## Neste oppgave
Legg til endringene i produksjonsmiljøet. Sjekk loggene i Splunk. Når det er klart, gå til neste oppgave, [bonusoppgaven](./6_fiks_s%C3%A5rbarheten.md)!

