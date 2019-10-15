**Rate-limiting pattern**

Rate limiting involves restricting the number of requests that can be made by a client.
A client is identified with an access token, which is used for every request to a resource. 
To prevent abuse of the server, APIs enforce rate-limiting techniques. 
Based on the client, the rate-limiting application can decide whether to allow the request to go through or not.
The client makes an API call to a particular resource; the server checks whether the request for this client is within the limit.
If the request is within the limit, then the request goes through.
Otherwise, the API call is restricted.

Some examples of request-limiting rules:
* X requests per timespan
* A certain timespan passed since the last call

The goal is to design a class(-es) that manage rate limits for every provided API resource by a set of provided extendable rules.
Think of API resource as a C# method, and at the very beginning of the method, you set up your classes 
and ask whether further execution is allowed for this particular callee.

Use simple in-memory data structures to store the data; don't rely on a particular database. Do not prepare any complex environment, 
a class library with a set of tests is more than enough. 

You are welcome to ask any questions regarding the requirements, treat us as product owners/analysts/whoever who knows the business.
Should you have any questions or concerns, submit them as a GitHub issue.

Send a pull request once you finished.

Good luck!
