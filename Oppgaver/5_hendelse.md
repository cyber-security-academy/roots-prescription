# Incident Response Team (IRT) tar kontakt!

<details>
<summary>Incident Response Team (IRT) er...</summary>
... teamet som håndterer hendelser i organisasjonen. De har gjerne oversikt over alle systemer og deres logger. Blir et system angrepet, er det de som undersøker hva som har skjedd. 
</details><br>

Denne oppgaven kommer i 3 steg. Det er lurt å gjør stegene etter hverandre og deploye til prod mellom hver gang. Dette er en litt mer komplisert oppgave enn tidligere, ikke nøl med å spørre om hjelp!

---

### 🪵 Steg 1: Hvem gjør kallet?
Incident Response Teamet (IRT) har tatt kontakt! 
De ser at det er unormalt mange kall mot endepunktene for nedlasting av filer, 
men de forstår ikke hva som skjer fordi loggmeldingene er uklare!

Dere får i oppgave å gjøre loggene mer tydelige slik at IRT-teamet får undersøkt hva som foregår.
IRT ber dere rette opp i [denne loggmeldingen](/RootsPrescription/Controllers/InvoiceController.cs#L65). De har ett krav **de trenger å vite hvilken bruker som gjør kallet**.

> Tips!💡 Sjekk funksjonen over, GetMyInvoices, på [linje 33 og 34](/RootsPrescription/Controllers/InvoiceController.cs#L33-L34) for hvordan dere kan hente ut brukernavn. Linje 43 viser hvordan dere kan bruke det i en loggmelding

Legg til endringene i produksjonsmiljøet. Fortsett på steg 2, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

---

### 🪵 Steg 2: Er det en uautorisert bruker som gjør kallet?
Dere ser i loggene at det er noen uten bruker som henter filer! Å nei!
Det betyr at vi har samme sårbarhet som i den store hendelsen. 
IRT ber oss legge til informasjon i loggene. Kravet er å logge **forsøk på kall av brukere som ikke er autorisert til å gjøre kallet**.

Et eksempel på en som ikke er autorisert er en bruker som er autentisert (innlogget), men ikke eier PDFen.

> Tips!💡 Sjekk linje 61-67 i funksjonen som allerede er fikset på, [GetPDF i PrescriptionController.cs](/RootsPrescription/Controllers/PrescriptionController.cs#61-67). 

Legg til endringene i produksjonsmiljøet. Fortsett på steg 3, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

---

### 🪵 Steg 3: Prøver noen å hente en fil som ikke finnes?
* forsøk på å laste ned filer som ikke finnes, og hvem som gjør kallet

Legg til endringene i produksjonsmiljøet. Sjekk loggene i Splunk. Når det er klart, gå til neste oppgave, [bonusoppgaven](./6_fiks_s%C3%A5rbarheten.md)!
