using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BufTools.Extensions.XmlComments;
using BufTools.Extensions.XmlComments.Models;
using BufTools.ObjectCreation.FromXmlComments;
using System.Collections;
using BufTools.FluentValidation.TestByExample.Extensions;

namespace BufTools.FluentValidation.TestByExample
{
    public class TestValidatorsByExample
    {
        private IEnumerable<Type> BasicTypes => new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(int?),
            typeof(float),
            typeof(float?),
            typeof(short),
            typeof(short?),
            typeof(long),
            typeof(long?),
            typeof(decimal),
            typeof(decimal?),
            typeof(DateTime),
            typeof(DateTime?)
        };

        private List<string> Empty => new List<string>();
        private List<string> Error(string error) => new List<string> { error };

        public async Task<IEnumerable<string>> GetValidationErrors(IServiceCollection services, IEnumerable<Type> validatorTypes)
        {
            var provider = services.BuildServiceProvider();
            var assemblies = validatorTypes.Select(t => t.Assembly).Distinct();

            var xmlDocs = LoadXmlDocs(assemblies);
            var loadingErrors = xmlDocs.Where(d => d.Value == null).Select(d => $"Could not find XML docs for type={d.Key.FullName}").ToList();
            var validationErrors = await validatorTypes.SelectManyAsync(async type => await GetErrorsForValidator(type, provider, xmlDocs[type.Assembly]));

            var allErrors = loadingErrors.Concat(validationErrors);
            return allErrors;
        }

        private IDictionary<Assembly, IDictionary<string, MemberDoc>> LoadXmlDocs(IEnumerable<Assembly> assemblies)
        {
            return assemblies.ToDictionary(a => a, a => a.LoadXmlDocumentation());
        }

        private async Task<IEnumerable<string>> GetErrorsForValidator(Type validatorType, ServiceProvider provider, IDictionary<string, MemberDoc> xmlDocs)
        {
            if (xmlDocs == null)
                return Empty;

            if (!validatorType.IsAssignableTo(typeof(IValidator)))
                return Error($"{validatorType.Name} can only be applied to classes of type {typeof(IValidator).Name}.");

            var classDoc = xmlDocs.GetDocumentation(validatorType);
            if (classDoc == null)
                return Error($"{validatorType} does not have any XML documentation.");

            var typeToValidate = GetTypeToValidate(validatorType);
            if (typeToValidate == null)
                return Error($"{validatorType} - Could not determine the type of object the validator validates.");

            var validator = provider.GetService(validatorType) as IValidator;
            if (validator == null)
                return Error($"{validatorType} could not be instantiated. Please check that the validators dependencies are registerd in the service container.");

            if (BasicTypes.Any(t => typeToValidate.IsAssignableTo(t)))
                return await GetEnforcementErrors(validator, xmlDocs);

            return await GetObjectErrors(validator, typeToValidate, provider, xmlDocs);
        }

        private Type GetTypeToValidate(Type validatorType)
        {
            var interfaceTest = new Func<Type, Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>) ? i.GetGenericArguments().Single() : null);

            var innerType = interfaceTest(validatorType);
            if (innerType != null)
            {
                return innerType;
            }

            foreach (var i in validatorType.GetInterfaces())
            {
                innerType = interfaceTest(i);
                if (innerType != null)
                {
                    return innerType;
                }
            }
            return null;
        }

        private async Task<IEnumerable<string>> GetObjectErrors(IValidator validator, Type typeToValidate, ServiceProvider provider, IDictionary<string, MemberDoc> xmlDocs)
        {
            if (validator == null)
                return Empty;

            var classDoc = xmlDocs.GetDocumentation(validator.GetType());
            if (classDoc == null)
            {
                return Error($"{validator.GetType().Name} does not have any XML documentation.");
            }

            var mother = new ObjectMother(provider);

            var errors = new List<string>();
            var instance = mother.Birth(typeToValidate, (t, err) => errors.Add($"{t.Name} - {err}"));
            if (instance == null)
                errors.Insert(0, $"{typeToValidate} could not be instantiated and hydrated. Please check that the validators dependencies are registerd in the service container.");

            if (errors.Any())
                return errors;

            var context = GetObjectContext(instance);
            var results = await validator.ValidateAsync(context);

            if (!results.IsValid)
                return results.Errors.Select(e => $"{typeToValidate} - {e.ErrorMessage}");

            foreach (var property in typeToValidate.GetProperties())
            {
                //if (_ignoreAttributes.Intersect(property.GetCustomAttributes().Select(a => a.GetType())).Any())
                //    continue;
                var doc = xmlDocs.GetDocumentation(property);
                if (doc == null)
                {
                    errors.Add($"{typeToValidate.Name}.{property.Name} - No XML documentation.");
                    continue;
                }

                if (doc.FailValues.Any())
                {
                    foreach (var fail in doc.FailValues)
                    {
                        var orgVal = property.GetValue(instance, null);

                        var newVal = GetValue(fail, property.PropertyType);
                        property.SetValue(instance, newVal);
                        context = GetObjectContext(instance);
                        results = await validator.ValidateAsync(context);
                        if (results.IsValid)
                            errors.Add($"{validator.GetType().Name} did not fail with '{fail}'");

                        property.SetValue(instance, orgVal);
                    }
                }
                else
                    errors.Add($"{typeToValidate.Name} does not have any fail values in the XML documentation.");

                foreach (var pass in doc.PassValues)
                {
                    var orgVal = property.GetValue(instance, null);

                    var newVal = GetValue(pass, property.PropertyType);
                    property.SetValue(instance, newVal);
                    context = GetObjectContext(instance);
                    results = await validator.ValidateAsync(context);
                    if (!results.IsValid)
                        errors.Add($"{validator.GetType().Name} did not pass with '{pass}'");

                    property.SetValue(instance, orgVal);
                }
            }

            return errors;
        }

        private object GetValue(string value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                type = type.GetGenericArguments()[0];

                var innerObject = GetValue(value, type);// Birth(type);

                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(type);
                var list = Activator.CreateInstance(constructedListType) as IList;
                list?.Add(innerObject);
                return list;
            }

            if (type == typeof(string))
                return value;

            if (type == typeof(int?))
                return string.IsNullOrEmpty(value) ? default(int?) : int.Parse(value);
            if (type == typeof(int))
                return int.Parse(value);

            if (type == typeof(float?))
                return string.IsNullOrEmpty(value) ? default(float?) : float.Parse(value);
            if (type == typeof(float))
                return float.Parse(value);

            if (type == typeof(decimal?))
                return string.IsNullOrEmpty(value) ? default(decimal?) : decimal.Parse(value);
            if (type == typeof(decimal))
                return decimal.Parse(value);

            if (type == typeof(short?))
                return string.IsNullOrEmpty(value) ? default(short?) : short.Parse(value);
            if (type == typeof(short))
                return short.Parse(value);

            if (type == typeof(long?))
                return string.IsNullOrEmpty(value) ? default(long?) : long.Parse(value);
            if (type == typeof(long))
                return long.Parse(value);

            if (type == typeof(DateTime?))
                return string.IsNullOrEmpty(value) ? default(DateTime?) : DateTime.Parse(value);
            if (type == typeof(DateTime))
                return string.IsNullOrEmpty(value) ? DateTime.MinValue : DateTime.Parse(value);

            if (type == typeof(bool?))
                return string.IsNullOrEmpty(value) ? default(long?) : long.Parse(value);
            if (type == typeof(bool))
                return bool.Parse(value);

            if (type.IsEnum)
                return Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            return null;// Birth(type);
        }

        private IValidationContext GetObjectContext(object objectToValidate)
        {
            var type = objectToValidate.GetType();

            var method = GetType().GetMethod(nameof(GetObjectContext));
            var make = method.MakeGenericMethod(type);

            var makeContext = GetType().GetMethod(nameof(GetObjectContext)).MakeGenericMethod(type);
            return makeContext.Invoke(null, new object[] { objectToValidate }) as IValidationContext;
        }

        public static IValidationContext GetObjectContext<T>(object objectToValidate)
        {
            return new ValidationContext<T>((T)objectToValidate);
        }

        private async Task<IEnumerable<string>> GetEnforcementErrors(IValidator validator, IDictionary<string, MemberDoc> xmlDocs)
        {
            if (validator == null)
                return Empty;

            var classDoc = xmlDocs.GetDocumentation(validator.GetType());
            if (classDoc == null)
            {
                return Error($"Validator of type {validator.GetType().Name} does not have any XML documentation.");
            }

            var errors = new List<string>();
            if (classDoc.FailValues.Any())
            {
                foreach (var fail in classDoc.FailValues)
                {
                    if (!GetContext(fail, validator, ref errors, out var context))
                        continue;

                    var result = await validator.ValidateAsync(context);
                    if (result.IsValid)
                        errors.Add($"{validator.GetType().Name} did not fail with '{fail}'");
                }
            }
            else
                errors.Add($"{validator.GetType().Name} does not have any fail values in the class XML documentation.");

            if (classDoc.PassValues.Any())
            {
                foreach (var pass in classDoc.PassValues)
                {
                    if (!GetContext(pass, validator, ref errors, out var context))
                        continue;

                    var result = await validator.ValidateAsync(context);
                    if (!result.IsValid)
                        errors.Add($"{validator.GetType().Name} did not pass with '{pass}' (Errors: {result.Errors})");
                }

            }
            else
                errors.Add($"{validator.GetType().Name} does not have any pass values in the class XML documentation.");
            return errors;
        }

        private bool GetContext(string value, IValidator validator, ref List<string> errors, out IValidationContext context)
        {
            try
            {
                context = GetContext(value, validator);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                context = null;
                return false;
            }
        }

        private IValidationContext GetContext(string value, IValidator validator)
        {
            if (validator.CanValidateInstancesOfType(typeof(string)))
                return new ValidationContext<string>(value);

            if (validator.CanValidateInstancesOfType(typeof(int?)))
                return new ValidationContext<int?>(string.IsNullOrWhiteSpace(value) ? default(int?) : int.Parse(value));
            if (validator.CanValidateInstancesOfType(typeof(int)))
                return new ValidationContext<int>(int.Parse(value));

            if (validator.CanValidateInstancesOfType(typeof(float?)))
                return new ValidationContext<float?>(string.IsNullOrWhiteSpace(value) ? default(float?) : float.Parse(value));
            if (validator.CanValidateInstancesOfType(typeof(float)))
                return new ValidationContext<float>(float.Parse(value));

            if (validator.CanValidateInstancesOfType(typeof(short?)))
                return new ValidationContext<short?>(string.IsNullOrWhiteSpace(value) ? default(short?) : short.Parse(value));
            if (validator.CanValidateInstancesOfType(typeof(short)))
                return new ValidationContext<short>(short.Parse(value));

            if (validator.CanValidateInstancesOfType(typeof(long?)))
                return new ValidationContext<long?>(string.IsNullOrWhiteSpace(value) ? default(long?) : long.Parse(value));
            if (validator.CanValidateInstancesOfType(typeof(long)))
                return new ValidationContext<long>(long.Parse(value));

            if (validator.CanValidateInstancesOfType(typeof(decimal?)))
                return new ValidationContext<decimal?>(string.IsNullOrWhiteSpace(value) ? default(decimal?) : decimal.Parse(value));
            if (validator.CanValidateInstancesOfType(typeof(decimal)))
                return new ValidationContext<decimal>(decimal.Parse(value));

            if (validator.CanValidateInstancesOfType(typeof(DateTime?)))
                return new ValidationContext<DateTime?>(string.IsNullOrWhiteSpace(value) ? default(DateTime?) : DateTime.Parse(value));
            if (validator.CanValidateInstancesOfType(typeof(DateTime)))
                return new ValidationContext<DateTime>(DateTime.Parse(value));

            return null;
        }
    }
}
