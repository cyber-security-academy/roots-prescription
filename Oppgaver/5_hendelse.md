# Hovedoppgave: Incident Response Team (IRT) tar kontakt!

<details>
<summary>Incident Response Team (IRT) er...</summary>
... teamet som håndterer hendelser i organisasjonen. De har gjerne oversikt over alle systemer og deres logger. Blir et system angrepet, er det de som undersøker hva som har skjedd. 
</details><br>

Denne oppgaven kommer i fire steg. Det er lurt å gjøre stegene etter hverandre og deploye til produksjon mellom hver gang. Dette er en litt mer komplisert oppgave enn tidligere, ikke nøl med å spørre om hjelp!

Har dere noen spørsmål eller vil diskutere noen unormale logger, skriv i Teams-kanalen til gruppen deres og tag IRT-teamet, Vegard Bakke, Cato Førrisdahl, Randi Stensli eller Even Frøyen.

---

### Steg 1: Mangelfulle logger (10 minutter)
Incident Response Teamet (IRT) har tatt kontakt! 

De ser at det er unormalt mange kall mot endepunktene for nedlasting av filer, 
men loggene til eResept gjenspeiler ikke denne trafikken! De mistenkter at eResept ikke logger *404 - Not found*.


Dere får i oppgave å logge alle forsøk som feiler, slik at IRT-teamet får undersøkt hva som foregår.

Legg til endringene i produksjonsmiljøet. Sjekk loggene i Splunk. Når det er klart kan dere gå videre.

<details>
<summary>Et lite hint!💡</summary>
Prøv dere fram om dere klarer å fremprovosere noen 404-meldinger. Finnes det mer enn én type?
</details><br>

<details>
<summary>Kodetips!💡</summary>
Metoden `GetInvoicePdf()` returnerer 404 Not found hvis en PDF det spørres etter ikke finnes. 

Første del av if-setningen under viser når filen ikke finnes. Inne i den blokken må dere logge hvilken URL som noen forsøker. Ser `PrescriptionController.cs` om dere er i tvil.

```csharp
if (stream == null)  // file does not exist
{
    // Deres loggmelding her
    return NotFound();
}
else  // file exists
{
    string attachmentname = Path.GetFileName(stream.Name);
    _logger.LogInformation("Downloaded: {Attachment}", attachmentname);

    // Respond to client
    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{attachmentname}\"");
    Response.Headers.Add("X-Content-Type-Options", "nosniff");
    return new FileStreamResult(stream, "application/pdf");
}
```

</details><br>

---

### Steg 2: Hva skjer egentlig? (10 minutter)
Ser dere unormal aktivitet i loggene? Å nei! 
Det er på tide at dere tar kontakt med IRT og forteller hva dere har sett!  Hva slags type angrep er Root Apotek utsatt for?

(Skriv i Teams-kanalen deres og tag noen fra IRT-teamet). 

> Her kan alle i gruppen forsøke å finne informasjon i Splunk. 
> 
> Er det én hendelse eller flere? Kanskje dere kan dele opp fordele søket.
>
> Er det *eieren* av fakturaen som henter ned dokumentet, eller har Root samme problem med fakturaene, som de hadde med reseptene tidligere?
>
> Er det et mønster i IP-adressene?



Legg til endringene i produksjonsmiljøet. Fortsett på neste steg, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

<details>
<summary>Kodetips!💡</summary>
Et eksempel på en  ikke-autorisert forespørsel, er en bruker som er innlogget (autentisert), men som ikke *eier* PDFen som personen prøver å hente ut.

For å blokkere. og logge dersom brukeren som er logget inn ikke er eieren til en faktura, kan dere bruke følgende kode:
    
```csharp
InvoiceDTO invoice = _dbservice.GetInvoice(filename);
if (invoice == null || invoice.OwnerId != authuser.Id)
{
    _logger.LogWarning("");  // Din loggmelding
    return Unauthorized();  // Returner med statuskode 401
}
```

</details><br>

---

### Steg 3: Hvem henter hva? (15 minutter)
Om dere ikke allerede har gjort det, ber IRT dere inkluderfe brukerinfo i loggmeldingen ["Downloaded: {Attachment}"](/RootsPrescription/Controllers/InvoiceController.cs#L60) i filen InvoiceContoller.cs. **De trenger å vite hvilken bruker som gjør kallet**.


<details>
<summary>Kodetips!💡</summary>
For å hente ut brukeren i kode kan dere legge inn følgende snutt over loggmeldingen:

```csharp
string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
UserDTO authuser = _dbservice.GetUserByUsername(authusername);
```

Dere kan også se på funksjonen over, `GetMyInvoices()` for inspirasjon. [Linje 43](/RootsPrescription/Controllers/InvoiceController.cs#L43) viser hvordan dere kan logge et brukernavn.

Hvis User er tom, kan det være at endepunktet ikke krever innlogging.  Ved å legge på `[Authorize]` vil .Net kreve at en bruker er pålogget. (Litt som `@PreAuthorize` i Java Spring.)
</details><br>

---

### Steg 4: Oppsummer valgene dere har tatt i oppsummeringspresentasjonen (15 minutter)
Bruk litt tid på å oppsummere valgene dere har gjort. Disse presenteres på oppsummeringsdagen.

Er det noe annet dere tenker dere kunne lagt til for at loggene blir enda bedre for hendelseshåndtering?

Fant dere noen sårbarheter? Hvor mange? Ta det med i oppsummeringen!


## Neste oppgave
Hvis dere rekker det, kan dere prøve å [fikse sårbarhetene](./6_fiks_s%C3%A5rbarheten.md) dere har avdekket!

[Gå tilbake til forrige oppgave](./4_fiks-loggmeldingen.md)
