using RateLimiterApi;

Auth auth = new Auth();
// clientOne allow only 2 valid request
Client clientOne = Auth.Login("pass1");

// clientTwo allow only 3 valid request
Client clientTwo = Auth.Login("pass2");

// clientThree allow only 4 valid request
Client clientThree = Auth.Login("pass3");


ResourceApi resourceApi = new ResourceApi();

// valid attempts
string clientOne_attempt_1 = resourceApi.ResourceOne(clientOne);
string clientOne_attempt_2 = resourceApi.ResourceOne(clientOne);
// invalid attempts
string clientOne_attempt_3 = resourceApi.ResourceOne(clientOne);
string clientOne_attempt_4 = resourceApi.ResourceOne(clientOne);

// valid attempts
string clientTwo_attempt_1 = resourceApi.ResourceOne(clientTwo);
string clientTwo_attempt_2 = resourceApi.ResourceOne(clientTwo);
string clientTwo_attempt_3 = resourceApi.ResourceOne(clientTwo);
// invalid attempts
string clientTwo_attempt_4 = resourceApi.ResourceOne(clientTwo);
string clientTwo_attempt_5 = resourceApi.ResourceOne(clientTwo);


// valid attempts
string clientThree_attempt_1 = resourceApi.ResourceOne(clientThree);
string clientThree_attempt_2 = resourceApi.ResourceOne(clientThree);
string clientThree_attempt_3 = resourceApi.ResourceOne(clientThree);
string clientThree_attempt_4 = resourceApi.ResourceOne(clientThree);
// invalid attempts
string clientThree_attempt_5 = resourceApi.ResourceOne(clientThree);
string clientThree_attempt_6 = resourceApi.ResourceOne(clientThree);