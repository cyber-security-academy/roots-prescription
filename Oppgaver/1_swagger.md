# Roots Apotek - Microservice for e-resept (5-10 minutter)

Microservicen 'e-resept' er et REST API, dvs en samling en URLer beregnet for andre datamaskiner.

Denne oppgaven handler om å teste hvordan APIet fungerer. 

APIet er som sagt beregnet for maskiner, men noe som heter OpenAPI (tidligere "Swagger") lar et menneske lettere få oversikt og teste et REST API. Det skal vi gjøre nå!

### URLen til APIet deres
Hver gruppe har sitt eget API med egen URL. Under er URLen til applikasjonen deres, husk å bytte med deres gruppenummer. 

**https://csa-gr[GRUPPENUMMER]-app.azurewebsites.net/** *Husk å bytte ut `[GRUPPENUMMER]`, f.eks. `csa-gr9-app`*

> Hver gang vi refererer til APIet og en tekst som inneholder en slash (/), mener vi URLen deres med denne teksten bak. F.eks. `/Prescription/GetPdf` blir til "https://csa-gr9-app.azurewebsites.net/Prescription/GetPdf" for gruppe 9.

### 1) Lokaliser ditt API
- Gå til https://csa-gr[GRUPPENUMMER]-app.azurewebsites.net/swagger
- User `System`-kontrolleren, kjør Execute på `/System/Ping`, og du bør motta en *respons* `-- CHANGE ME--`.

### 2) Logg på
- Åpne `/Login/Login`, logg inn med en av:
  - Brukere: `ada`, `bob`, `camilla`, `dilan` , `eira`
  - Passord: `csa+admin`  *(Likt for alle brukere)*
- Hvis du klarte å logge på, vil du få en slik respons i Swagger:

  ![image](https://user-images.githubusercontent.com/4437745/231814625-7bad51d0-ed19-4efb-897e-149d8fae0bd5.png)
  
  <details>
    <summary>Respons-tokenet eyJhbG...</summary>
    Den lange responsen her (som starter med 'eyJhbG...') kalles et `token`, og fungerer som en tivolibillett. Den slipper deg inn på anlegget, og gir deg lov til å kjøre noen av karusellene, man kanskje ikke alle. Kanskje får dere ulike farger på billettene avhengig av alder eller høyde, slik at hver karusell kan lett sjekke billetten uten å ringe billettselgeren. (Token som starter med `ey` er normalt av typen JWT token, for dem som synes dette var nyttig kunnskap. JWT token inneholder informasjon om brukeren, og kan leses i klartekst, f.eks. på jwt.io, men det en en avsporing i fra dette kurset. : )
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
