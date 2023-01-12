# MultiValidator
Simplifies validating multiple properties with multiple FluentValidators into one function call.

```cs
await _validator.For(request).Use<RequestValidator>()
                .For(requestFilter).Use<RequestFilterValidator>()
                .ValidateAsync();
```
