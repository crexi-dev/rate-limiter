# A simple rate-limited Web API using ASP.NET Core 5.0

## Prerequisites
* ASP.NET Core in .NET 5.0
* Visual Studio with the **ASP.NET and web development** workload.

## Solution Structure

The solution is categorized into the following projects.
* **Web API** *(/src/RateLimiter.Api/)*
  * ***RateLimiter.Api***
      * The web application root project

* **Cache** *(/src/Cache/)*
  * ***RateLimiter.Cache***
    * Contains the code to manipulate items in the in-memory distributed cache.

* **Common** *(/src/Common/)*
  * ***RateLimiter.Common***
    * Contains the code for helper methods.

* **Filters** *(/src/Filters/)*
  * ***RateLimiter.Filters***
    * Contains the custom filters which allow code to run before or after specific stages in the request pipeline.
    
* **Models** *(/src/Models/)*
    * ***RateLimiter.Models.Contract***
      * Contains all the request and response DTOs
    * ***RateLimiter.Models.Domain***
      * Contains all the domain models

* **Tests** *(/src/Tests/)*
  * Contains the unit tests for the API layer, filters, and services. It uses the `NUnit` testing framework and `NSubstitute` mocking framework.
  * ***RateLimiter.Api.Tests***
  * ***RateLimiter.Filters.Tests***
  * ***RateLimiter.Services.Tests***

## Overview
This is an implementation of a rate limiter in ASP.NET Core that allows for rate-limiting with a sliding window. There are several canonical algorithms for rate limiting, but for this project, I decided to draw inspiration from the [Token bucket](https://en.wikipedia.org/wiki/Token_bucket) algorithm, while leveraging a sliding window approach. I’ve incorporated an in-memory `SortedSet<T>` to keep track of all request timestamps and to calculate whether certain parameters are exceeded. For simplicity and testing purposes, I've bootstrapped the sample Weather Forecast application from Microsoft. It provides a **single** endpoint that returns a random forecast for the upcoming days.

| API | Description | Request body | Response body |
| ------------- | ------------- | ------------- | ------------- |
| GET /weatherforecast | Gets the current weather | None | Array of forecast items

JSON similar to the following example is returned:

```json
[
    {
        "date": "2019-07-16T19:04:05.7257911-06:00",
        "temperatureC": 52,
        "temperatureF": 125,
        "summary": "Mild"
    },
    {
        "date": "2019-07-17T19:04:05.7258461-06:00", 
        "temperatureC": 36, 
        "temperatureF": 96, 
        "summary": "Warm"
    }, 
    {
        "date": "2019-07-18T19:04:05.7258467-06:00", 
        "temperatureC": 39, 
        "temperatureF": 102, 
        "summary": "Cool"
    },
    ...
]
 ```

The service will throttle based on the following metrics over a given interval:

1. The number of requests (for instance, 100 requests per minute).
2. The size of data (for instance, 100 MBs per minute).
3. The cost of requests (for instance, 1000 request units per minute).

## Testing
* When using the [swagger portal]("https://localhost:54996/swagger"), be sure to add a Bearer token, otherwise the Authorization filter will not authorize the request.
![img.png](auth.jpg)
* In the `appsettings.json` file, you can artificially set your own throttle limits and given the period of time.

## ClientMetrics Properties
ClientMetrics contains the following properties:
* AccessKey:`string` – the user’s access token
* TotalRequests:`int` – the total number of requests accrued
* TotalSize:`long` – the total data size accrued in bytes
* ExpiresAt:`DateTime?` – the TTL expiration date
* Requests:`SortedSet<Request>` - an ordered set collection of the user’s requests (by timestamp)

## Workflow
The core logic can be summarized as the following:
1. Each user has a sorted set associated with them. The keys, in this case, will be the user’s access token, and the values cached will be of type `ClientMetrics`.
2. When a user makes a request, we first evict all elements in the sorted set which occurred in the previous interval. This is accomplished with the `ExceptWith(IEnumerable)` method.
3. If the number of requests in the set exceeds one of the threshold metrics (see above), the request is blocked and returns a `429` status code.
4. Otherwise, the request with the current timestamp is added to the set.
5. After the request is completed, we tally up the request metrics.
6. For each key in our cache, we set a TTL equal to the time interval. After the TTL has expired, the key will automatically be deleted.
> Note: if a request is blocked, it is not added to the set.

## Issues and suggestions
* There is one obvious caveat with this implementation. Our rate limiter cannot be shared across multiple processes, so it is not distributed by nature. One idea is to swap out the in-memory cache with a centralized key-value store, such as `Redis`. 
* One of the most documented problems with a centralized key-value store is the potential for race conditions in a high concurrency environment. In this case, we could leverage some type of lease management system to manage these uncoordinated processes. Our uncoordinated processes would then compete for these exclusive leases, which would grant them a certain amount of capacity.
* And finally, we can consider sending our requests to a messaging or stream processing system (i.e. Apache Kafka) that can control the flow of ingestion. We can leverage internal consumer services to read the messages off a topic at a fixed rate that's within our metric limits. Queuing or streaming these messages can act as a buffer to allow you to dequeue only the messages that can be processed in the given interval.