# Ping! Noens første pull request?

<details>
<summary>En "pull request" er...</summary>
... en metode å be noen se over endringer i et prosjekt for å få det inn i produksjonsmiljøet. Dette kalles "code review" eller "kodegjennomgang", en av de viktigste aktivitetene i sikker utvikling.
</details><br>

Dere skal nå gjøre første endring i applikasjonen. Dette er en enkel endring
for å gjøre dere litt varme i trøya slik at dere kan det grunnleggende. 

1. Gå til filen [SystemController.cs](/RootsPrescription/Controllers/SystemController.cs#L32) i GitHub. Der er loggmeldingen `--CHANGEME--`. Den skal dere endre til hva dere vil. Pass på å kun endre teksten mellom hermetegnene (`"`), ellers kan koden feile (det går helt fint om den feiler, men det tar litt lenger tid å løse oppgaven).
2. Nederst på siden, under "Commit changes", skriver dere en kort melding om hvilken endring som er gjort, huker av på "Create a new branch for this commit and start a pull request", og velger "Propose changes". Dere blir redirigert til en ny side hvor dere velger "Create pull request".
3. Kuult! Nå er det sikkert noen av dere som har laget deres første **pull request** 😎
4. Nå kan en av de andre i gruppa se over kodeendringen og godkjenne den ved trykke på "Files" og "Review> Approve". 
5. Gratulerer! Dere har nå gjort deres første kodeendring i produksjons-miljøet! Gå til "Actions"-tabben øverst på siden i GitHub. Der ser dere en GitHub workflow som ble trigget automatisk da kodeendringen ble godkjent. Når den blir grønn, gå til neste steg. Dersom workflowen feiler, ta kontakt med en av kursholderne.
6. Sjekk endringen i Splunk. Først, gå til `/swagger`. Gå deretter til "System"-seksjonen og åpne `/System/Ping`. Trykk "Execute" og dere har nå gjort et kall som skal vises i Splunk med den nye meldingen.


### Neste oppgave
Det er bra dere er litt varme i trøya nå, for personvernekspertene i Roots Apotek har en [oppgave til dere](./4_fiks-loggmeldingen.md)!

