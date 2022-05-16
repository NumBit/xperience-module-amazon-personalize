using System;
using System.Collections.Generic;

namespace Kentico.Xperience.AmazonPersonalize.Admin
{
    /// <summary>
    /// Represents configuration of individual page types and how they are mapped to the Amazon Personalize dataset.
    /// </summary>
    public class PageTypeMappings
    {
        /// <summary>
        /// Gets the set of page cultures to be included in the Amazon Personalize dataset.
        /// Empty set means that all cultures are included.
        /// </summary>
        /// <remarks>
        /// Not all cultures might be suitable for the Amazon Personalize recommendation engine.
        /// </remarks>
        public ISet<string> IncludedCultures { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// Gets an ordered collection of field mappings for the individual page types.
        /// </summary>
        public IDictionary<string, IList<FieldMapping>> Mappings { get; } = new Dictionary<string, IList<FieldMapping>>(StringComparer.InvariantCultureIgnoreCase);
    }
}
