# Fiks loggmeldingen 

Hjelp! Personvern-ekspertene i Roots Apotek har sagt at vi ikke kan logge personnummer. 
Vi logger jo personnummer i filen [InvoiceController](/RootsPrescription/Controllers/InvoiceController.cs#L43)!

Deres oppgave er å fikse loggmeldingen slik at dere unngår logging av personnummer. 
Deploy endringen til produksjonsmiljøet. 
Når endringen er live i Splunk, kan dere gå videre til [neste oppgave](./5_hendelse.md).

> Tips! Deploy endringene til produksjonsmiljøet slik som i forrige oppgave.

### Diskusjonsoppgave til oppsummeringsdagen (10-15 minutter)
Diskuter hvorfor man ikke burde logge personnummer eller annen persondata. Kan man man logge persondata? I så fall, når? 

Dette er et vanskelig tema og vi forventer ikke et riktig juridisk svar. Da må dere bruke alt for lang tid! Hold dere til 10-15 minutter for å komme med et forslag til det dere tror er riktig som vi kan diskutere i plenum på oppsummeringsdagen.
