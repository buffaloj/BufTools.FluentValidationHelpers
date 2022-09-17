using FluentValidation;
using FluentValidation.Extensions.Tests.Models;
using FluentValidation.Extensions.Tests.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiValidator.Tests
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
        public async Task MultiValidator_WithPassingValidator_Succeeds()
        {
            await _target.With<ExampleModelValidator>(_exampleModel)
                         .ValidateAsync();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task MultiValidator_WithFailingValidator_Throws()
        {
            _exampleModel.ExampleString = null;

            await _target.With<ExampleModelValidator>(_exampleModel)
                         .ValidateAsync();
        }
    }
}