# Sky, o store sky - i din evige form
# aldri stille - aldri det samme

Skyløsninger er kult det. Men hver gang vi tar fram dette prosjektet må det tilpasses til siste års endringer.

Her er en oppskrift over det jeg gjorde i 2025. Håper det kan være nytte i 2026.

## Upgrade process 2025

Kjørte .Net 6.0, som var på tide å oppgradere til siste Long-Term-Support .Net versjon:
 - Installerte `.NET Upgrade Assistant` from https://marketplace.visualstudio.com/
 - Upgraded RootPrescription from .Net 6.0 to .Net 8.0
   -  Høyreklikk på prosjektet i Solution Explorer
     - Select 'Upgrade', In-place-project
 - OG oppdater`Dockerfile`, til å bruke samme versjon:
    - `mcr.microsoft.com/dotnet/aspnet:8.0 AS base`
    - `mcr.microsoft.com/dotnet/sdk:8.0 AS build`
