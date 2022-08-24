# Final source for the test. I included 3 api test endpoints:
http://localhost:1111/api/consignments
http://localhost:2222/api/consignments
http://localhost:3333/api/consignments

## How to run:
Please start 3 web apis (Api1, Api2, Api3 projects) first and the ApiRequestsTest.Entry and then press any key to run the request
If you're using rider (probably applicable for Visual Studio), there's a Run all profile that you can just hit run.

## Solution structure:
**ApiRequestsTest.Entry:** Startup project that includes hosting, configuration and DI for the application.
This project has almost no business logic in it so I wont test this project

**ApiRequestsTest.Application:** The main business logic of the app. Each Api has it own class and implement the same interface `IPackageProvider` to unify the process of `GetAuthenticationCredential`, `SerializeInput` and `RequestDealWithRetry`.
The `HttpClient` dependency of each ApiProvider has a retry logic 3 times and  there will be a slight delay after each failure (**ConsignmentRetryPolicy**).
There's also a `PackageAggregator` that exec each provider and select the best price. In addition, there's also an timeout mechanism (10 sec) to prevent one api call take too long to response.
All PackageAggregator and ApiProvider are heavily unit tested. I believe that they have 100% coverage

**ApiRequestsTest.Domain:** This layer is for business domain model, right now it just have the main app input and 3 api models. There's not much logic so this does not have unit tests.

**ApiRequestsTest.Application.Tests:** Test project for the **ApiRequestsTest.Application**