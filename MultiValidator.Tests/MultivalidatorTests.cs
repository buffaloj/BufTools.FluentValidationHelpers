using FluentValidation.Extensions.Tests.Models;
using FluentValidation.Extensions.Tests.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentValidation.Extensions.Tests
{
    [TestClass]
    public class MultivalidatorTests
    {
        private readonly MultiValidator _target;

        private readonly ExampleModel _exampleModel = ExampleModel.Example();

        public MultivalidatorTests()
        {
            _target = new MultiValidator();
        }

        [TestMethod]
        public async Task ValidateAsync_WithPassingValidator_Succeeds()
        {
            await _target.With<ExampleModelValidator, ExampleModel>(_exampleModel)
                         .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task ValidateAsync_WithFailingValidator_Throws()
        {
            _exampleModel.ExampleString = null;

            await _target.With<ExampleModelValidator, ExampleModel>(_exampleModel)
                         .ValidateAsync();
        }

        [TestMethod]
        public async Task ValidateAsync_WithMultipleFailingValidators_ConcatenatesErrors()
        {
            var model2 = ExampleModel.Example();

            _exampleModel.ExampleString = null;
            model2.ExampleString = null;

            try
            {
                await _target.With<ExampleModelValidator, ExampleModel>(_exampleModel)
                             .With<ExampleModelValidator, ExampleModel>(model2)
                             .ValidateAsync();

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
            await _target.WithOptional<ExampleModelValidator, ExampleModel>(null)
                         .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ValidationBuilder_WithOptionalNullObjectToValidate_Succeeds()
        {
            await _target.With<ExampleModelValidator, ExampleModel>(_exampleModel)
                         .WithOptional<ExampleModelValidator, ExampleModel>(null)
                         .ValidateAsync();

            Assert.IsTrue(true);
        }
    }
}