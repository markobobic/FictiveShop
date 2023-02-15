namespace FictiveShop.Core.Exceptions
{
    public class ValidatingException : Exception
    {
        public ValidatingException(string message)
            : this(message, Enumerable.Empty<Error>())
        {
        }

        public ValidatingException(string message, IEnumerable<Error> errors)
            : base(message)
        {
            this.Errors = errors;
        }

        public ValidatingException(IEnumerable<Error> errors)
        {
            this.Errors = errors;
        }

        public IEnumerable<Error> Errors { get; }

        [Serializable]
        public class Error
        {
            public Error(string propertyName, string errorMessage)
            {
                this.PropertyName = propertyName;
                this.ErrorMessage = errorMessage;
            }

            public string PropertyName { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}