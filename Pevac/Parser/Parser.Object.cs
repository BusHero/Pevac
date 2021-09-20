using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pevac
{
    public static partial class Parser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Parser<Func<T, T>?> ParseObject<T>(Func<string, Parser<Func<T, T>>> parserSelector)
        {
            return parserSelector switch
            {
                null => throw new ArgumentNullException(nameof(parserSelector)),
                not null => 
                    from _ in StartObjectToken
                from updaters in (from propertyName in PropertyName
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                                              from updater in parserSelector(propertyName)
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                                              select updater)
                                              .Many()
                            from __ in EndObjectToken
                            select new Func<T, T>(t => updaters.Aggregate(t, (data, updater) => updater(data)))
            }
            ;
        }
    }
}
