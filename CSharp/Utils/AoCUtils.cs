﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventOfCode.Utils
{
    /// <summary>
    /// General Advent of Code utility methods
    /// </summary>
    public static class AoCUtils
    {
        #region Static methods
        /// <summary>
        /// Combines input lines into sequences, separated by empty lines
        /// </summary>
        /// <param name="input">Input lines</param>
        /// <returns>An enumerable of the packed input</returns>
        public static IEnumerable<List<string>> CombineLines(string[] input)
        {
            List<string> pack = new();
            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (pack.Count is not 0)
                    {
                        yield return pack;
                        pack = new List<string>();
                    }
                }
                else
                {
                    pack.Add(line);
                }
            }

            if (pack.Count is not 0)
            {
                yield return pack;
            }
        }

        /// <summary>
        /// Logs the answer to Part 1 to the console and results file
        /// </summary>
        /// <param name="answer">Answer to log</param>
        public static void LogPart1(object answer) => Trace.WriteLine($"Part 1: {answer}");
        
        /// <summary>
        /// Logs the answer to Part 3 to the console and results file
        /// </summary>
        /// <param name="answer">Answer to log</param>
        public static void LogPart2(object answer) => Trace.WriteLine($"Part 2: {answer}");

        /// <summary>
        /// Iterates over all the permutations of the given array
        /// </summary>
        /// <typeparam name="T">Type of element in the array</typeparam>
        /// <param name="array">Array to get the permutations for</param>
        /// <returns>An enumerable returning all the permutations of the original array</returns>
        public static IEnumerable<T[]> Permutations<T>(T[] array)
        {
            static T[] Copy(T[] original, int size)
            {
                T[] copy = new T[original.Length];
                if (size is not 0)
                {
                    Buffer.BlockCopy(original, 0, copy, 0, size * original.Length);
                }
                else
                {
                    original.CopyTo(copy, 0);
                }

                return copy;
            }
            
            static IEnumerable<T[]> GetPermutations(T[] working, int k, int size)
            {
                if (k == working.Length - 1)
                {
                    T[] perm = Copy(working, size);
                    yield return perm;
                }
                else
                {
                    for (int i = k; i < working.Length; i++)
                    {
                        Swap(ref working[k], ref working[i]);
                        foreach (T[] perm in GetPermutations(working, k + 1, size))
                        {
                            yield return perm;
                        }
                        Swap(ref working[k], ref working[i]);
                    }
                }
            }

            int size = typeof(T).IsPrimitive ? GetSizeOfPrimitive<T>() : 0;
            return GetPermutations(Copy(array, size), 0, size);
        }

        /// <summary>
        /// Swaps two values in memory
        /// </summary>
        /// <typeparam name="T">Type of value to swap</typeparam>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Gets the size of the object in bytes for a given primitive type
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <returns>The size in int of the primitive type</returns>
        /// <exception cref="InvalidOperationException">If the type isn't a primitive</exception>
        public static int GetSizeOfPrimitive<T>()
        {
            Type type = typeof(T);
            if (!type.IsPrimitive) throw new InvalidOperationException($"Cannot get the size of a non primitive type {typeof(T).FullName}");

            //Manual overrides
            if (type == typeof(bool)) return 1;
            if (type == typeof(char)) return 2;
            
            //Normal behaviour
            return Marshal.SizeOf<T>();
        }
        #endregion
        
        #region Extension methods
        /// <summary>
        /// Transforms the range into an enumerable over it's start and end
        /// </summary>
        /// <returns>An enumerable over the entire Range</returns>
        /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
        public static IEnumerable<int> AsEnumerable(this Range range)
        {
            //Make sure the indices aren't from the end
            if (range.Start.IsFromEnd || range.End.IsFromEnd) throw new ArgumentException("Range indices cannot be from end for enumeration", nameof(range));
            
            return range.End.Value > range.Start.Value ? Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value) : Enumerable.Range(0, 0);
        }

        /// <summary>
        /// Range GetEnumerator method, allows foreach over a range as long as the indices are not from the end
        /// </summary>
        /// <returns>An enumerator over the entire range</returns>
        /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
        public static IEnumerator<int> GetEnumerator(this Range range) => range.AsEnumerable().GetEnumerator();
        #endregion
    }
}