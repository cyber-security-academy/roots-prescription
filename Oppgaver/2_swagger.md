# Roots Apotek - Microservice for resepter (5-10 minutter)

Denne oppgaven handler om å teste hvordan APIet fungerer. APIet er beregnet for maskiner, men noe som heter OpenAPI (tidligere "Swagger") lar et menneske lettere får oversikt og teste et REST API. Det skal vi gjøre nå!

### URLen til APIet deres
Hver gruppe har sitt eget API med egen URL. Under er URLen til applikasjonen deres, husk å bytte med deres gruppenummer. 

**https://csa-gr[gruppenummer]-app.azurewebsites.net/** *Husk å bytte ut `[GRUPPENUMMER]`, f.eks. `csa-gr9-app`*

> Hver gang vi refererer til APIet og en tekst som inneholder en slash (/), mener vi URLen deres med denne teksten bak. F.eks. `/Prescription/GetPdf` blir til "https://csa-gr9-app.azurewebsites.net/Prescription/GetPdf".

### 1) Lokaliser ditt API
- Gå til https://csa-gr[GRUPPENUMMER]-app.azurewebsites.net/swagger
- User `System`-kontrolleren, kjør Execute på `/System/Ping`, og du bør motta en *response* `-- CHANGE ME--`.

### 2) Logg på
- Åpne `/Login/Login`, logg inn med en av:
  - Brukere: `ada`, `bob`, `camilla`, `dilan` 
  - Passord: `csa+admin`  
- Hvis du klarte å logge på, vil du få en slik respons i Swagger:

  ![image](https://user-images.githubusercontent.com/4437745/231814625-7bad51d0-ed19-4efb-897e-149d8fae0bd5.png)



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
