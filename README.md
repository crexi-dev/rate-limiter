**Rate-limiting pattern**

Rate limiting involves restricting the number of requests that can be made by a client.
A client is identified with an access token, which is used for every request to a resource.
To prevent abuse of the server, APIs enforce rate-limiting techniques.
Based on the client, the rate-limiting application can decide whether to allow the request to go through or not.
The client makes an API call to a particular resource; the server checks whether the request for this client is within the limit.
If the request is within the limit, then the request goes through.
Otherwise, the API call is restricted.

Some examples of request-limiting rules (you could imagine any others)
* X requests per timespan;
* a certain timespan passed since the last call;
* for US-based tokens, we use X requests per timespan, for EU-based - certain timespan passed since the last call.

The goal is to design a class(-es) that manage rate limits for every provided API resource by a set of provided *configurable and extendable* rules. For example, for one resource you could configure the limiter to use Rule A, for another one - Rule B, for a third one - both A + B, etc. Any combinations of rules are possible, keep this fact in mind when designing the classes.

Use simple in-memory data structures to store the data; don't rely on a particular database. Do not prepare any complex environment,
a class library with a set of tests is more than enough. Don't worry about the API itself, including auth token generation - there is no real API environment required.
For simplicity, you can implement the API resource as a simple C# method accepting a user token, and at the very beginning of the method, you set up your classes and ask whether further execution is allowed for this particular callee.

There is a Test Project set up for you to use. You are welcome to create your own test project and use whatever test runner you would like.   

You are welcome to ask any questions regarding the requirements - treat us as product owners/analysts/whoever who knows the business.
Should you have any questions or concerns, submit them as a [GitHub issue](https://github.com/crexi-dev/rate-limiter/issues).

You should [fork](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) the project, and [create a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request-from-a-fork) once you are finished.

Good luck!

//test 2
