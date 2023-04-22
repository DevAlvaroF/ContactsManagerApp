using Entities;
namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(CountryResponse)) return false;

            return this.CountryName == ((CountryResponse)obj).CountryName && this.CountryId == ((CountryResponse)obj).CountryId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// Converts Country object to CountryResponse type via extension
    /// </summary>
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryReponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName,
            };
        }
    }
}
