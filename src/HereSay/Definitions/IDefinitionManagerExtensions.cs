using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;

namespace HereSay.Definitions
{
    public static class IDefinitionManagerExtensions
    {
        /// <summary>
        /// Gets an array of <see cref="Type"/>s that are assignable from the given type with the 
        /// specified <see cref="IDefinitionManager"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to find other types that are 
        /// assignable from this type.</param>
        /// <returns>An array of <see cref="Type"/>s which the given type is assignable from.</returns>
        public static Type[] GetTypesAssignableFrom(this IDefinitionManager definitions, Type type)
        {
            Type[] result = definitions.GetDefinitions()
                .Where(definition => type.IsAssignableFrom(definition.ItemType)
                    && type.IsAssignableFrom(definition.ItemType))
                .Select(definition => definition.ItemType)
                .ToArray();

            return result;
        }
    }
}
