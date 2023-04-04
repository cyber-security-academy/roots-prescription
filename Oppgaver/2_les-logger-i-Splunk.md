# Kom i gang - les logger i Splunk (10-15 minutter)

Vi har blitt ansatt til å rette opp i loggene. Da må vi først gjøre oss kjent med 
logg-verktøyet som kunden bruker, Splunk.
La oss starte med å lese loggene til applikasjonen vår. 

Vi anbefaler at en i gruppa setter maskinen sin på storskjerm slik at vi jobber sammen som et team.

1. Gå til https://splunk.csa.datasnok.no
2. Logg inn med brukernavn og passord `utvikler:SuperHemmelig1337`
3. Trykk på "Search & Reporting" på venstre side, et grønt ikon
4. Skriv `* index="group_N"` i søkefeltet. Bytt `N` med gruppenummeret dere har fått utdelt.
5. Du skal nå se loggene til din applikasjon. Får du ikke opp noen ting, si ifra til kursholderne. 

Nå som dere har oppe loggene, er det på tide å gjøre litt forskjelige søk. 
Test disse søkene og lek dere litt i Splunk.

* `* index="group_N" Resept`
* `* index="group_N" Fakt*`
* `* index="group_N" `
* Se "fields" på venstre side, f.eks. `RenderedMessage`. Der sere antallet av hver loggmelding.

Det er mye man kan filtrere på og sjekke i Splunk, men vi kan ikke bruke hele dagen på det. 

### Neste oppgave

Du kan nå gå videre til neste oppgave, [første kodeendring](./3_ping.md)!

