# Rate Limiter - Fixed Window Algorithm

## Overview
This project implements a **Fixed Window Rate Limiter** to control API request rates per client within a given time window. It ensures **clean code, concurrency support, and scalability** while adhering to best coding practices.

## Features
- **Fixed Window Algorithm**: Limits requests within a fixed time window.
- **Concurrency & Scalability**: Supports multiple clients independently.
- **Extensibility**: Allows multiple rate-limiting rules.
- **Thread Safety**: Ensures correct behavior under concurrent requests.

## Installation & Usage
### **Prerequisites**
- .NET 6.0+
- NUnit for running tests

### **Running the Application**
Clone the repository and build the solution:
```sh
cd rate-limiter
dotnet build
```

## Test Cases
The test suite ensures **correct request blocking, time window resets, and concurrency handling**.

### **Test Coverage**
✔ Validates **request limits** per client  
✔ Ensures **time window resets correctly**  
✔ Supports **multiple clients independently**  
✔ Handles **concurrent and burst requests**  

### **Test Cases**
| **Test Name** | **Description** |
|--------------|----------------|
| `AllowRequest_AfterTimeWindow_ShouldResetCounter` | Ensures requests are allowed again **after the window resets**. |
| `AllowRequest_WithinLimit_ShouldReturnTrue` | Validates requests **within the allowed limit**. |
| `AllowRequest_ExceedsLimit_ShouldReturnFalse` | Ensures **requests beyond the limit** are blocked. |
| `AllowRequest_MultipleClients_ShouldBeTrackedIndividually` | Confirms **each client has independent rate limits**. |
| `AllowRequest_MultipleRules_AllRulesMustPass` | Tests **multiple rules running together**. |
| `AllowRequest_BurstRequests_ShouldBeBlocked` | Blocks **rapid successive requests**. |
| `AllowRequest_ConcurrentClients_ShouldNotInterfere` | Ensures **one client’s rate limit doesn’t affect another**. |
| `AllowRequest_ShortTimeWindow_ShouldResetQuickly` | Verifies short **time windows expire correctly**. |

## Running Tests
Run all tests:
```sh
dotnet test
```
Run a specific test:
```sh
dotnet test --filter "FullyQualifiedName=RateLimiter.Tests.RateLimiterTest.AllowRequest_AfterTimeWindow_ShouldResetCounter"
```

## Future Enhancements
- **Sliding Window Rate Limiter** for smoother rate enforcement.
- **Distributed rate limiting** using Redis.
- **Per-endpoint & per-user rules**.
