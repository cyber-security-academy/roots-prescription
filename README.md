# Roots Apotek - e-resept

Velkommen til dag to av kurset. I dag tar dere rollen som utviklere! 
Applikasjonen dere ble presentert for på Dag 1 skal vi nå utvikle videre på.

Først, la oss gå gjennom hva som har skjedd.
Roots Apotek fikk en melding om at deres resepter var på avveie. Kilden var en av microservicene 'e-resept' for nedlasting av PDFer for resepter og faktura. De fant ut at det var mulig å for innloggede brukere å laste ned andres resepter gjennom URLen `/Prescription/GetPdf`. 

Årsaken til sårbarheten ble identifisert. Sårbarheten lå på i funksjonen [GetPdf](RootsPrescription/Controllers/PrescriptionController.cs#L51-L85) i controlleren `PrescriptionController.cs`. Hullet er nå tettet og dere kan anta at funksjonen er sikker.

Etter sikkerhetsgjennomgangen er det avdekket at loggene må ryddes opp i. Dette er dere i Sopra Steria hyret inn for å løse.

## Første oppgave
Dere kan begynne på [oppgaven om trusselmodellering](Oppgaver/0_trusselmodellering.md). Vi anbefaler at dere løser én og én oppgave, i den rekkefølgen som oppgavetekstene legger opp til.  

> Alle oppgavene er lagt opp slik at man kun trenger nettleseren for å løse dem.
> Vanligvis vil utviklere bruke en teksteditor som Visual Studio Code eller
> IntelliJ, men det trenger dere ikke i dag.
>
> Oppgavetekstene skal være klar nok til å ikke trenge utviklererfaring. Noter
> at det er vanlig å synes det er litt forvirrende og det er helt OK å trenge
> hjelp. Spør oss om dere lurer på noe!


## Arkitektur
Roots Apoteks har eksportert en mappe med PDFer (resepter og faktura) fra det gamle fagsystemet sitt slik kunden kan få tilgang til sine dokumenter. PDFene ligger lagert i en beskyttet mappe som kun en systembrukeren har tilgang til. 

De har laget en mikroservice *e-resept* som tilbyr resepter og fakturakopi til kundene på internett.

Microservicen for resepter består av 3 *controllere* (Login, Prescriptions og Invoice) som tilbyr URLer, og 2 *servicer* (FileStorage og Database), og som snakker med back-end.



```mermaid
graph RL;
Login("<b><u>Login Controller</u></b><br/>/Login/Login")
Invoice("<b><u>Invoice</u></b><br/>/Invoice/GetMyInvoices<br/>/Invoice/GetInvoicePDF")
Prescription("<b><u>Prescription</u></b><br/>/Prescription/GetMyPrescriptions<br/>/Prescription/GetPDF")

DB("Database Service")
Login-->DB
Invoice-->DB
Prescription-->DB

File("FileStorage Service")
Invoice-->File
Prescription-->File

```

# Ordbok
På samme måte når slakteren setter ulike navn på de ulike delene av grisen når den slaktes, setter programmerer ulike navne på de ulike delene inne i et system.

Her er en liten kort forklaring på noen av dem slik de er brukt hos Roots Apotek
- *Endepunkt* - Et fancy ord for URL (men vanligvis for URLer der det skjer noe på servren når noen besøker URLen)
- *API* - En samling URLer (*endepunkter*) som er strukturert slik at maskiner lettere kan snakke sammen. (*Application programming interface* eller *grensesnitt*)
- *Controller* - En del av systemet som har regler om hvordan ting skal utføres. *Endepunkter* samles gjerne i en eller flere *controllere*
- *Service* - I denne sammenhengen er *service* den delen av systemet som "leverer tjenester" til *controllerne*. F.eks. henter og skriver data fra databasen, henter filer fra mapper, e.l.
- *Microservice* - Har ingenting med *service* å gjøre. Nå zoomer vi ut noen hakk, og ser alle IT-tjenestene til hele Roots Apotek. De kunne ha samlet alle diisse en én stor maskin, eller de kan dele tjeneste opp i mindre maskiner som snakker sammen. Disse småmaskinene kalles *micro*-servicer.
- *Front-end* - Samme som resepsjonen, salgslokalet og utstillinger som kundene ser
- *Back-end* - Samme som *backoffice*

