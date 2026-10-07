# ERP Phase 1 — API .NET

Solution API pour les modules Achats, Stock, Ventes & Facturation.
Architecture : `ERP.Domain` (entites) / `ERP.Infrastructure` (EF Core + DbContext) / `ERP.Api` (endpoints, vertical slice par module).

## Prerequis

- .NET 8 SDK
- SQL Server (instance `DESKTOP-O7CGU5J\SQLSERVER2022`, base `ERP_Phase1` deja creee via `erp_creation_bd.sql`)

## Demarrage

```bash
cd ERP
dotnet restore
dotnet build
cd src/ERP.Api
dotnet run
```

L'API demarre sur `https://localhost:xxxx` (le port exact s'affiche dans la console).
Swagger est disponible sur `/swagger`.

## Verifier la connexion a la base

Ouvrir dans le navigateur (ou Swagger) :

```
GET /api/health/db
```

Reponse attendue :

```json
{ "connecteALaBase": true, "nombreCategories": 2 }
```

Si `connecteALaBase` est `false`, verifier la chaine de connexion dans `src/ERP.Api/appsettings.json`
(nom de l'instance SQL Server, droits Windows sur la base).

## Premier module de test

`GET /api/categories`, `GET /api/categories/{id}`, `POST /api/categories` — permet de valider
tout le pipeline Angular (plus tard) -> API -> EF Core -> SQL Server avant de construire les
modules metier (Fournisseurs, Commandes, Stock, Ventes...).

## Prochaines etapes

1. Valider que `dotnet run` + `/api/health/db` fonctionnent sur ta machine.
2. Ajouter les features suivantes sous `ERP.Api/Features/` (une vertical slice par cas d'utilisation) :
   `Fournisseurs`, `CommandesFournisseur`, `Stock`, `Clients`, `Devis`, `CommandesClient`, `Factures`.
3. Initialiser le workspace Angular et consommer `/api/categories` en premier test d'integration.
