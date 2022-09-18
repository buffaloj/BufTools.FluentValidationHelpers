using FluentValidation.Extensions.Tests.Models;
using FluentValidation.Extensions.Tests.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentValidation.Extensions.Tests
{
    [TestClass]
    public class MultivalidatorTests
    {
        private readonly MultiValidator _validator;

        private readonly ExampleModel _exampleModel = ExampleModel.Example();

        public MultivalidatorTests()
        {
            _validator = new MultiValidator();
        }

        [TestMethod]
        public async Task ValidateAsync_WithPassingValidator_Succeeds()
        {
            await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task ValidateAsync_WithFailingValidator_Throws()
        {
            _exampleModel.ExampleString = null;

            await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
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
                await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                                .Check(model2).Using<ExampleModelValidator>()
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
            ExampleModel? nullModel = default(ExampleModel);
            await _validator.CheckOptional(nullModel).Using<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ValidationBuilder_WithOptionalNullObjectToValidate_Succeeds()
        {
            ExampleModel? nullModel = default(ExampleModel);
            await _validator.Check(_exampleModel).Using<ExampleModelValidator>()
                            .CheckOptional(nullModel).Using<ExampleModelValidator>()
                            .ValidateAsync();

            Assert.IsTrue(true);
        }
    }
}