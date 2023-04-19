# Introduksjonsoppgave: Microservice for eResept (5-10 minutter)

Microservicen eResept er et REST API. Denne oppgaven handler om å teste hvordan APIet fungerer. 
APIet er beregnet for maskiner, men noe som heter OpenAPI (tidligere "Swagger") lar et menneske lettere få oversikt og teste et REST API. 
Det skal vi bruke nå!

### 1) Lokaliser din gruppe sitt API
- Gå til https://csa-gr[GRUPPENUMMER]-app.azurewebsites.net/swagger. *Husk å bytte ut `[GRUPPENUMMER]`, f.eks. `csa-gr9-app`*
- Gå til `System`-kontrolleren, kjør Execute på `/System/Ping`, og du bør motta en *respons* `-- CHANGE ME--`.

### 2) Logg på
- Åpne `/Login/Login`, logg inn med en av:
  - Brukere: `ada`, `bob`, `camilla`, `dilan` , `eira`
  - Passord: `Superhemmelig1337`  *(Likt for alle brukere)*
- Hvis du klarte å logge på, vil du få en slik respons i Swagger:

  ![image](https://user-images.githubusercontent.com/4437745/231814625-7bad51d0-ed19-4efb-897e-149d8fae0bd5.png)
  
  <details>
    <summary>Respons-tokenet eyJhbG...</summary>
    <p>Den lange responsen her (som starter med 'eyJhbG...') kalles et `token`, og fungerer som en tivolibillett. Den slipper deg inn på anlegget, og gir deg lov til å kjøre noen av karusellene, man kanskje ikke alle. </p><p>Kanskje får dere ulike farger på billettene avhengig av alder eller høyde. Hver billettkontrollør kan da lett sjekke om du får lov å kjøre karusellen uten å ringe billettselgeren for flere detaljer. </p><p><i>(Token som starter med `ey` er normalt av typen JWT token, for dem som synes dette var nyttig kunnskap. JWT token inneholder informasjon om brukeren, og kan leses i klartekst, f.eks. på jwt.io, men det en en avsporing i fra dette kurset. : )</i></p>
</details><br>

- Hent ut informasjon om brukeren med `/Login/CurrentUser`

### 3) Last ned en resept
- Finn info om *dine* resepter: `/Prescription/GetMyPrescriptions`
- Finn ID til en resept, og bruk denne for å laste ned den vha `/Prescription/GetPdf`
- Hvis du klarte å laste ned en PDF vil du få følgende respons:

  ![image](https://user-images.githubusercontent.com/4437745/231814072-8371b082-f4b5-4ef9-8a24-daa5f61c01f4.png)


### 4) Last ned en faktura
- Som over, men her har visst utviklerne glemt noe. Man benytter fortsatt `filename` som parameter. Ikke en ID. 
- Se om du kan hente ned en av fakturaene til brukeren 

## Neste oppgave
Når dere har lastet ned en PDF kan dere går videre til [neste oppgave](./2_les-logger-i-Splunk.md)!

[Gå tilbake til forrige oppgave](./0_trusselmodellering.md)
