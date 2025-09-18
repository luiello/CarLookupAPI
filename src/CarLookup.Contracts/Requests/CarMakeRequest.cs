namespace CarLookup.Contracts.Requests
{
    /// <summary>
    /// Rrequest model for creating/updating a new car make
    /// </summary>
    public class CarMakeRequest
    {
        /// <summary>
        /// Name of the car make
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Country where the brand is headquartered
        /// </summary>
        public string CountryOfOrigin { get; set; } = string.Empty;
    }
}
