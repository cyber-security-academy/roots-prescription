# Roots Apotek - Microservice for resepter

Velkommen til dag to av kurset. I dag tar dere rollen som utviklere! 
Applikasjonen dere ble presentert for på dag 1 skal vi nå utvikle videre på.

Først, la oss gå gjennom hva som har skjedd.
Roots Apotek fikk en melding om at deres resepter var på avveie. Kilden var en av microservicene for nedlasting av PDFer for resept og faktura. De fant ut at det var mulig å for innloggede brukere å laste ned andres resepter gjennom URLen `/Prescription/GetPdf`. 

Årsaken til sårbarheten ble identifisert. Sårbarheten lå på i funksjonen [GetPdf](RootsPrescription/Controllers/PrescriptionController.cs#L51-L85) i controlleren `PrescriptionController.cs`. Hullet er nå tettet og dere kan anta at funksjonen er sikker.

Etter en sikkerhetsgjennomgang er det avdekket at loggene må ryddes opp i. Dette er dere i Sopra Steria hyret inn for å løse.

Dere kan begynne på [oppgaven om trusselmodellering](Oppgaver/0_trusselmodellering.md). Vi anbefaler å løse en og en oppgave, i rekkefølgen som oppgaveteksten linker til.  

> Alle oppgavene er lagt opp slik at man kun trenger nettleseren for å løse dem.
> Vanligvis vil utviklere bruke en teksteditor som Visual Studio Code eller
> IntelliJ, men det trenger dere ikke i dag.
>
> Oppgavetekstene skal være klar nok til å ikke trenge utviklererfaring. Noter
> at det er vanlig å synes det er litt forvirrende og det er helt OK å trenge
> hjelp. Spør oss om dere lurer på noe!


## Arkitektur
Roots Apoteks har eksportert en mappe med PDFer (resepter og faktura) fra fagsystemet, til en beskyttet mappe som kun en servicebrukeren har tilgang til. 
De har laget en mikroservice som tilbyr reseptene og fakturakopi til kundene på nettet.

Microservicen for resepter består av 3 *controllere* (Login, Prescriptions og Invoice) som tilbyr URLer, og 2 *servicer* (FileStorage og Database), og som snakker med backend.


Dataflyt er som følger:
```mermaid
graph LR;
Login("<b><u>Login</u></b><br/>/Login/Login<br/>/Login/Logout")
Invoice("<b><u>Invoice</u></b><br/>/Invoice/GetMyInvoices<br/>/Invoice/GetInvoicePDF")
Prescription("<b><u>Prescription</u></b><br/>/Prescription/GetMyPrescriptions<br/>/Prescription/GetPDF")

DB("Database")
DB-->Login
DB-->Invoice
DB-->Prescription

File("FileStorage")
File-->Invoice
File-->Prescription

```

