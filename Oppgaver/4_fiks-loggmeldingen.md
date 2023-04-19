# Hovedoppgave: Fiks loggmeldingene

### Programmeringsoppgave (10-15 minutter)
Hjelp! Personvernombudet i Roots Apotek har sagt at vi ikke kan logge persondata. 
Vi logger jo bl.a. personnummer i loggmeldingen `User logged in:` i filen [LoginController](/RootsPrescription/Controllers/LoginController.cs)! 

1. Finn loggmeldingen i Splunk
2. Fjern personnummer fra loggmeldingen i koden og deploy endringen til produksjonsmiljøet 

> Tips! Deploy endringene til produksjonsmiljøet slik som i forrige oppgave.

### Diskusjonsoppgave til oppsummeringsdagen (10-15 minutter)
Diskuter hvorfor man ikke burde logge personnummer eller annen persondata. Kan man man logge persondata? I så fall, når? 
  
Dette er et vanskelig tema og vi forventer ikke et riktig juridisk svar. Da må dere bruke alt for lang tid! Hold dere til 10-15 minutter for å komme med et forslag til det dere tror er riktig som vi kan diskutere i plenum på oppsummeringsdagen.

### Neste oppgave
Når endringen er live i Splunk, kan dere gå videre til [neste oppgave](./5_hendelse.md).

[Gå tilbake til forrige oppgave](./3_ping.md)
