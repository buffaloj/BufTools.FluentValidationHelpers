namespace MultiValidation.Tests.Models
{
    public class ExampleModel
    {
        public string ExampleString { get; set; }
        public int? ExampleInt { get; set; }
        public float? ExampleFloat { get; set; }
        public bool? ExampleBool { get; set; }

        public static ExampleModel Example()
        {
            return new ExampleModel
            {
                ExampleString = "Example Text!",
                ExampleInt = 8,
                ExampleFloat = 20.62f,
                ExampleBool = true
            };
        }
    }
}
