using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GottaMix.Extensions
{
    /// <summary>
    /// 拡張クラス
    /// </summary>
    static class IEnumerableExtensions
    {
        /// <summary>
        /// ReadOnlyCollectionへの変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> self) => new ReadOnlyCollection<T>(self.ToList());
    }
}
