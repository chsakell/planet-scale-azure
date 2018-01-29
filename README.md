# Globally-distributed applications with Microsoft Azure

[![Build status](https://ci.appveyor.com/api/projects/status/n4p67xq9llkhiqn1/branch/master?svg=true)](https://ci.appveyor.com/project/chsakell/planet-scale-azure/branch/master)

This is the repository associated with the e-book [Globally-distributed applications with Microsoft Azure](https://leanpub.com/globally-distributed-applications-with-microsoft-azure)

![Globally-distributed applications with Microsoft Azure](/Online.Store/wwwroot/images/app/schema-architecture.png?raw=true "Globally-distributed applications with Microsoft Azure")

## Architecture

`Online.Store` is a web application designed to scale across multiple regions over the globe. It does that by leveraging the following `Azure Services`:

* [Azure CDN](https://azure.microsoft.com/en-us/services/cdn/)
* [Azure Traffic Manager](https://azure.microsoft.com/en-us/services/traffic-manager/)
* [Azure Storage](https://azure.microsoft.com/en-us/services/storage/?v=16.50)
* [Azure Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction)
* [Azure Search](https://azure.microsoft.com/en-us/services/search/)
* [Azure Redis Cache](https://azure.microsoft.com/en-us/services/cache/)
* [Service Bus](https://azure.microsoft.com/en-us/services/service-bus/)
* [Azure SQL Databases](https://azure.microsoft.com/en-us/services/sql-database/)
* [SQL Active Geo-Replication](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-geo-replication-overview)
* [Azure Active Directory B2C](https://azure.microsoft.com/en-us/services/active-directory-b2c/)

Users can be authenticated either using [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) or [Azure Active Directory B2C](https://azure.microsoft.com/en-us/services/active-directory-b2c/)

## The book

The first 4 parts describe the features for each `Azure Service` used and their role in the design of the `Online.Store` application. It provides step by step instructions to configure `Online.Store` application settings and secret keys so that you can deploy it all over the globe. The final chapter explains the `PowerShell` scripts that you can use to automate processes in a globally distributed application *(resource provisioning, releases or rolling back updates)*

![Globally-distributed applications with Microsoft Azure](/Online.Store/wwwroot/images/app/schema-replicas.png?raw=true "Globally-distributed applications with Microsoft Azure")

## Continuous Integration & Delivery

`Continuous Intragration & Delivery` is based in [AppVeyor](https://www.appveyor.com/) which is totally free to use for open-source projects 

![Globally-distributed applications with Microsoft Azure](/Online.Store/wwwroot/images/app/schema-traffic-manager-current-status.png?raw=true "Globally-distributed applications with Microsoft Azure")

## Online.Store project

`Online.Store` is a **cross-platform** application built with `.NET Core` and `Angular`. In order to follow along with the book, create Azure Services and build the project you need the following:

1. An [Azure free account](https://azure.microsoft.com/en-us/free/). It's absolutely free, plus you get a $200 credit you can spend during the first 30 days. In a nutchell, to register a free account you need a Microsoft email and a credit cart. Don't worry your credit cart **will not be charged**. You can read more about the Azure free account [here](https://azure.microsoft.com/en-us/free/free-account-faq/)
1. Install [Microsoft - NET Core](https://www.microsoft.com/net/learn/get-started
)
2. Install [Node JS](https://nodejs.org/en/download/). Version **v6.11.2** and later should be good enough
3. Install [Git](https://git-scm.com/downloads)

In case you are in a `Windows` OS it is highly advised to install [Visual Studio 2017](https://www.visualstudio.com/downloads/). The project however can be built and run outside of Visual Studio as well *(using the .NET Core CLI)* 

<h2>License</h2>
Code released under the <a href="https://github.com/chsakell/planet-scale-azure/blob/master/licence" target="_blank"> MIT license</a>.