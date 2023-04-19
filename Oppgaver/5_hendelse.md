# Incident Response Team (IRT) tar kontakt!

<details>
<summary>Incident Response Team (IRT) er...</summary>
... teamet som håndterer hendelser i organisasjonen. De har gjerne oversikt over alle systemer og deres logger. Blir et system angrepet, er det de som undersøker hva som har skjedd. 
</details><br>

Denne oppgaven kommer i 3 steg. Det er lurt å gjør stegene etter hverandre og deploye til prod mellom hver gang. Dette er en litt mer komplisert oppgave enn tidligere, ikke nøl med å spørre om hjelp!

---

### Steg 1: Hvem gjør kallet? (15 minutter)
Incident Response Teamet (IRT) har tatt kontakt! 
De ser at det er unormalt mange kall mot endepunktene for nedlasting av filer, 
men de forstår ikke hva som skjer fordi loggmeldingene er uklare!

Dere får i oppgave å gjøre loggene mer tydelige slik at IRT-teamet får undersøkt hva som foregår.
IRT ber dere rette opp i loggmeldingen ["Downloaded: {Attachment}"](/RootsPrescription/Controllers/InvoiceController.cs#L64) i filen InvoiceContoller.cs. De har ett krav. **De trenger å vite hvilken bruker som gjør kallet**.

Legg til endringene i produksjonsmiljøet. Fortsett på steg 2, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

<details>
<summary>Kodetips!💡</summary>
For å hente ut brukeren i kode kan dere legge inn følgende snutt over loggmeldingen:

```csharp
string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
UserDTO authuser = _dbservice.GetUserByUsername(authusername);
```

Dere kan også se på funksjonen over, `GetMyInvoices()` for inspirasjon. [Linje 43](/RootsPrescription/Controllers/InvoiceController.cs#L43) viser hvordan dere kan logge et brukernavn.
</details><br>

---

### Steg 2: Er det en uautorisert bruker som gjør kallet? (15 minutter)
Dere ser i loggene at det er noen uten bruker som henter filer! Å nei!
Det betyr at vi har samme sårbarhet som i den store hendelsen. 
IRT ber oss legge til informasjon i loggene. Kravet er å logge **forsøk på kall av brukere som ikke er autorisert til å gjøre kallet**.

Et eksempel på en som ikke er autorisert er en bruker som er autentisert (innlogget), men ikke eier PDFen som kallet prøver å hente ut.

Legg til endringene i produksjonsmiljøet. Fortsett på steg 3, men husk å sjekke loggene i Splunk når endringene er deployet til produksjonsmiljøet.

<details>
<summary>Kodetips!💡</summary>
For å logge dersom brukeren som er logget inn ikke er eieren til en faktura, kan dere bruke følgende kode:

```csharp
InvoiceDTO invoice = _dbservice.GetInvoice(id)
if (invoice == null || invoice.OwnerId != authuser.Id)
{
    _logger.LogWarning("");  // Din loggmelding
    return Unauthorized();  // Returner med statuskode 401
}
```

</details><br>

---

### Steg 3: Prøver noen å hente en fil som ikke finnes? (15 minutter)
Esra i utviklingsteamet spør IRT om de ikke burde logge forsøk på filer som ikke finnes. 
IRT er _helt_ enig! De gir dere kravet å logge **forsøk på å laste ned filer som ikke finnes, og hvem som gjør kallet**.

Legg til endringene i produksjonsmiljøet. Sjekk loggene i Splunk. Når det er klart kan dere gå videre.

<details>
<summary>Kodetips!💡</summary>
Første del av if-setningen under viser når filen ikke finnes. Inne i den blokken må dere logge hvem som gjør kallet som i steg 1. 

```csharp
if (stream == null)  // file does not exist
{
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

### Steg 4: Oppsummer valgene dere har tatt i oppsummeringspresentasjonen (15 minutter)
Bruk litt tid på å oppsummere valgene dere har gjort. Disse presenteres på oppsummeringsdagen.

## Neste oppgave
Neste oppgave er en [bonusoppgave](./6_fiks_s%C3%A5rbarheten.md)!

