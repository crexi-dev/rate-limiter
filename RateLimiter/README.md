### Description on the Approach
- Assuming that rules are fixed and variables are sent in from either configuration file or database.
- If the rules are not fixed and pure data driven, the way classes are implemented will be more sophisticated at the mercy of available data.
- If the rules are fixed, we can create classes to help configure rules on startup. We can even use flient syntax.
- RateLimitService.cs is just to show how we can manage rules. The way this is implemented can all be changed depending on how we are using this.