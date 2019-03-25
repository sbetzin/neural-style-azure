using System.Collections.Generic;
using System.Linq;

namespace NeuralStyle.ConsoleClient
{
    public static class EnumExtensions
    {
        public static IEnumerable<(T1, T2)> Product<T1, T2>(this IEnumerable<T1> list1, IEnumerable<T2> list2)
        {
            return list1.SelectMany(item1 => list2.Select(item2 => (item1, item2)));
        }

        public static List<(T In, TR Style)> GetCombinations<T, TR>(this List<T> allIn, List<TR> allStyles)
        {
            return allIn.SelectMany(inImage => allStyles, (inImage, styleImage) => (In: inImage, Style: styleImage)).ToList();
        }
    }
}