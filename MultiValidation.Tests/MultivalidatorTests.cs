using MultiValidation.Tests.Models;
using MultiValidation.Tests.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentValidation;

namespace MultiValidation.Tests
{
    [TestClass]
    public class MultiValidatorTests
    {
        private readonly MultiValidator _validator;

        private readonly ExampleModel _exampleModel = ExampleModel.Example();

        public MultiValidatorTests()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<ExampleModelValidator>();
            sc.AddSingleton<ExampleIntValidator>();
            sc.AddSingleton<ExampleNullableIntValidator>();
            var provider = sc.BuildServiceProvider();

            _validator = new MultiValidator(new ValidatorFactory(provider));
        }

        [TestMethod]
        public async Task ValidateAsync_WithPassingModel_Succeeds()
        {
            await _validator.For(_exampleModel).Use<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task WithValidateAsync_WithPassingModel_Succeeds()
        {
            await _validator.For(_exampleModel).Use<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task WithValidateAsync_WithNonReferenceType_Succeeds()
        {
            int i = 5;

            await _validator.For(i).Use<ExampleIntValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task WithValidateAsync_WithOptionalNullableNonReferenceType_Succeeds()
        {
            int? i = 5;

            await _validator.ForOptional(i).Use<ExampleNullableIntValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Validate_WithPassingModel_Succeeds()
        {
            _validator.For(_exampleModel).Use<ExampleModelValidator>()
                      .Validate();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task ValidateAsync_WithFailingModel_Throws()
        {
            _exampleModel.ExampleString = null;

            await _validator.For(_exampleModel).Use<ExampleModelValidator>()
                            .ValidateAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validate_WithFailingModel_Throws()
        {
            _exampleModel.ExampleString = null;

            _validator.For(_exampleModel).Use<ExampleModelValidator>()
                      .Validate();
        }

        [TestMethod]
        public async Task ValidateAsync_WithMultipleFailingModels_ConcatenatesErrors()
        {
            var model2 = ExampleModel.Example();

            _exampleModel.ExampleString = null;
            model2.ExampleString = null;

            try
            {
                await _validator.For(_exampleModel).Use<ExampleModelValidator>()
                                .For(model2).Use<ExampleModelValidator>()
                                .ValidateAsync();

                Assert.IsTrue(false);
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual(2, ex.Errors.Count());
            }
        }

        [TestMethod]
        public void Validate_WithMultipleFailingModels_ConcatenatesErrors()
        {
            var model2 = ExampleModel.Example();

            _exampleModel.ExampleString = null;
            model2.ExampleString = null;

            try
            {
                _validator.For(_exampleModel).Use<ExampleModelValidator>()
                          .For(model2).Use<ExampleModelValidator>()
                          .Validate();

                Assert.IsTrue(false);
            }
            catch (ValidationException ex)
            {
                Assert.AreEqual(2, ex.Errors.Count());
            }
        }

        [TestMethod]
        public async Task MultiValidator_WithOptionalNullObjectToValidate_Succeeds()
        {
            ExampleModel nullModel = null;
            await _validator.ForOptional(nullModel).Use<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ValidationBuilder_WithOptionalNullObjectToValidate_Succeeds()
        {
            ExampleModel nullModel = default;
            await _validator.ForOptional(_exampleModel).Use<ExampleModelValidator>()
                            .ForOptional(nullModel).Use<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }
    }
}