using System;

namespace CarLookup.Contracts.Requests
{
    /// <summary>
    /// Request model for creating/updating a new car model
    /// </summary>
    public class CarModelRequest
    {
        /// <summary>
        /// Foreign key to the associated car make
        /// </summary>
        public Guid MakeId { get; set; }

        /// <summary>
        /// Name of the model
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Year of production for this model
        /// </summary>
        public int ModelYear { get; set; }
    }
}
