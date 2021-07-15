using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorldCities.Angular.Data.Models
{
    [Table("Countries")]
    public class Country
    {
        #region Constructor
        public Country()
        {

        }
        #endregion

        #region Properties
        //The unique id and primary key for this Country
        [Key]
        [Required]
        public int Id { get; set; }

        //Country name (in UTF8 format)
        public string Name { get; set; }

        //Country code (in ISO 3166-1 ALPHA-2 format)
        [JsonPropertyName("iso2")]
        public string ISO2 { get; set; }

        //Country code (in ISO 3166-1 ALPHA-3 format)
        [JsonPropertyName("iso3")]
        public string ISO3 { get; set; }
        #endregion

        #region Client-side properties
        //The number of cities related to this country.
        [NotMapped]
        public int TotCities
        {
            get
            {
                return (Cities != null)
                    ? Cities.Count
                    : _TotCities;
            }

            set { _TotCities = value; }
        }

        private int _TotCities = 0;
        #endregion

        #region Navigation Properties
        //A list containing all the cities related to this country.
        [JsonIgnore]
        public virtual List<City> Cities { get; set; }
        #endregion
    }

    /*[Table("Countries")]
    public class Country
    {
        #region Constructor
        public Country()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// The unique id and primary key for this Country
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Country name (in UTF8 format)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country code (in ISO 3166-1 ALPHA-2 format)
        /// </summary>
        [JsonPropertyName("iso2")]
        public string ISO2 { get; set; }

        /// <summary>
        /// Country code (in ISO 3166-1 ALPHA-3 format)
        /// </summary>
        [JsonPropertyName("iso3")]
        public string ISO3 { get; set; }
        #endregion

        #region Navigation Properties
        /// <summary>
        /// A list containing all the cities related to this country.
        /// </summary>
        public virtual List<City> Cities { get; set; }
        #endregion
    }*/
}