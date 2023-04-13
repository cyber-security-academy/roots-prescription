# Roots Apotek - Microservice for resepter

Hendelsen og microservicen er beskrevet på https://github.com/cyber-security-academy/roots-prescription.

Denne oppgaven handler om å teste hvordan APIet fungerer.

APIet er beregnet for maskiner. Men noe som heter Open API (tidligere "Swagger") lar et menneske lettere får oversikt, og teste et REST API.

## 1) Lokaliser ditt API

- Gå til https://xxx.xxx.xxx/swagger
- User `System`-kontrolleren, kjør Execute på `/System/Ping`, og du bør motta en *response* `-- CHANGE ME--`.

## 2) Logg på
- Åpne `/Login/Login`, logg inn med en av:
  - Brukere: `ada`, `bob`, `camilla`   *[!!! `ada` har p.t. ingen faktura eller resepter. Det skal endres!!!]*
  - Passord: `csa+admin`  &nbsp; &nbsp; *[??? skal endres til `SuperhemmeligFellespassord` ??? ]*

## 3) Last ned en resept
- Finn info om *dine* resepter: `/Prescription/GetMyPrescriptions`
- Finn ID til en resept, og bruk denne for å laste ned den vha `/Prescription/GetPdf`

*[!!!Legg inn bilde av 'Download file' fra Swagger!!!]*

## 4) Last ned en faktura
- Som over, men her har visst utviklerne glemt noe. Man benytter fortsatt `filename` som parameter. Ikke en ID. 
- Se om du 


### Neste oppgave
Når dere har lastet ned en PDF kan dere går videre til [neste oppgave](./2_les-logger-i-Splunk.md)!

