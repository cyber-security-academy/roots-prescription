# Introduksjonsoppgave: Les logger i Splunk (10-15 minutter)

Vi har blitt ansatt til å rette opp i loggene. Da må vi først gjøre oss kjent med 
logg-verktøyet som kunden bruker, Splunk.

La oss starte med å lese loggene til applikasjonen vår. 

1. Gå til https://splunk.ca2.datasnok.no/en-GB/
2. Logg inn med brukernavn `groupN` og passord `unloving backspace mooing puma`. Bytt `N` med gruppenummeret dere fikk utdelt.
3. Trykk på "Search & Reporting" på venstre side, et grønt ikon
4. Skriv `index="group_N"` i søkefeltet. Bytt `N` med gruppenummeret dere har fått utdelt.
5. Du skal nå se loggene til din applikasjon. Får du ikke opp noen ting, si ifra til kursholderne. 

### Søk
Nå som dere har oppe loggene, er det på tide å gjøre litt forskjellige søk. 
Test disse søkene og lek dere litt i Splunk.

* `index="group_N" `
* `index="group_N" Resept`
* `index="group_N" Fakt*`

### Oversikt
Se "Fields" på venstre side, klikk på `MessageTemplate` og `RenderedMessage` for å se oversikt over antallet meldinger.
* `MessageTemplate` viser en oversikt over *typer* loggmeldinger 
* `RenderedMessage` viser loggnmeldinger med alle detaljene fylt ut

### Real-time
For å slippe å trykke 'Søk' hele tiden for å se nye loggmeldinger, kan man endre til real-time oppdatering til venstere for Søkeknappen.  \
![image](https://user-images.githubusercontent.com/4437745/230628582-4c503ee0-f3df-4ca0-b5e3-8ffe92484ba3.png)

### Avvikere
For å identifisere avvik i loggene (tegn på unormal aktivitet), så kan man bruke Splunk for å finne *sjeldne verdier*.

Klikk på `RenderedMessages`.  I popup-vinduet som kommer opp, så kan man under `Reports` velge `Rare values`.

Ignorer grafen, men fokuser på den grå tabellen under. Den er sortert med de færreste forekomstene øverst. 


## Neste oppgave
Det er mye man kan filtrere på og sjekke i Splunk, men vi kan ikke bruke hele dagen på dette. Du kan nå gå videre til neste oppgave, [første kodeendring](./3_ping.md)!

[Gå tilbake til forrige oppgave](./1_swagger.md)
