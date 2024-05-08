# Testing Validators by Example
Allows testing FluentValidators using example values in the XML comments of a class being tested

```cs
var tester = new TestValidatorsByExample();
var services = new ServiceCollection();
var types = services.AddSingletonClassesWithAttribute<TestValidatorByExampleAttribute>(AllAssemblies);

var errors = await _exampleTester.GetValidationErrors(services, types);
```

# Getting Started

1. Create a service collection and register all the classes and dependencies needed to test the validators
```cs
var services = new ServiceCollection();
var types = services.AddSingletonClassesWithAttribute<TestValidatorByExampleAttribute>(GetType().Assembly);
```

2. Check for errors.  An empty collection means no errors/
```cs
var errors = await _exampleTester.GetValidationErrors(services, types);
```