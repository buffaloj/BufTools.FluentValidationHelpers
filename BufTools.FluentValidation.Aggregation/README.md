# MultiValidator
Allows validating multiple properties with multiple FluentValidators in one function call.

```cs
await validator.For(request).Use<RequestValidator>()
               .For(endDate).Use<EndDateValidator>()
               .ForOptional(requestFilter).Use<RequestFilterValidator>()
               .ValidateAsync();
```

# Getting Started

1. Add this when you register your services at startup:
```cs
builder.Services.AddMultiValidation();
```

2. Inject MultiValidator into a class and use it:
```cs
public class MyClass
{
    private readonly MultiValidator validator;
	
    public MyClass(MultiValidator validator) => this.validator = validator;
	
    public async Task DoUserTaskAsync(string name, DateTime dob, UserFilter filter = null)
    {
        await validator.For(name).Use<NameValidator>()
                       .For(dob).Use<DobValidator>()
                       .ForOptional(filter).Use<UserFilterValidator>()
                       .ValidateAsync();
    }
}
```