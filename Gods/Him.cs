﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Gods {
    public static class Him {
        /// <summary>
        /// 扩展 DirectoryInfo.EnumerateFiles 的 searchPattern。
        /// </summary>
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo source, params string[] searchPatterns) {
            return searchPatterns.SelectMany(p => source.EnumerateFiles("*." + p));
        }

        /// <summary>
        /// 扩展 Directory.EnumerateFiles 的 searchPattern。
        /// </summary>
        public static IEnumerable<string> EnumerateFiles(string folder, params string[] searchPatterns) =>
            searchPatterns.SelectMany(p => Directory.EnumerateFiles(folder, "*." + p));

        public static IEnumerable<MethodBase> GetCallers(Type type) {
            var types = new StackTrace().GetFrames().Select(f => f.GetMethod());
            var indexes = types.Select((t, i) => t.DeclaringType == type ? i + 1 : 0).Where(i => i > 0);
            var result = types.Where((t, i) => indexes.Contains(i));
            return result;
        }

        /// <summary>
        /// 返回包含自身的继承链。
        /// </summary>
        /// <param name="self">起始类，从此开始向上寻找。</param>
        /// <param name="target">目标类，终止于这里。</param>
        /// <returns>如果起始类不继承目标类，返回值的长度为空。</returns>
        public static IEnumerable<Type> GetBases(Type self, Type target) {
            if (self.IsSubclassOf(target)) {
                yield return self;
                foreach (var t in GetBases(self.BaseType, target)) {
                    yield return t;
                }
            } else {
                if (self == target) {
                    yield return target;
                }
            }
        }

        public static string Remove(this string source, params char[] chars) {
            return source.Except(chars).ToString(null);//TODO Except(regex)
        }

        public static string ToString<T>(this IEnumerable<T> source, object spliter) {
            var b = new StringBuilder();
            foreach (var c in source) {
                b.Append(c);
                b.Append(spliter);
            }
            return b.ToString();
        }

        public static IPAddress GetLocalIP() {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
                if (ip.AddressFamily.ToString() == "InterNetwork") {
                    return ip;
                }
            }
            return null;
        }

        public static Dictionary<int, string> TryAll(Action action, params Action[] actions) {
            var dict = new Dictionary<int, string>();
            try {
                action();
            } catch (Exception e) {
                dict.Add(0, e.Message);
            }
            for (int i = 0; i < actions.Length; i++) {
                try {
                    actions[i]();
                } catch (Exception e) {
                    dict.Add(i + 1, e.Message);
                }
            }
            return dict;
        }

        public static bool Assert(Logic.IfElse assert) {
            return assert.Assert(Logic.IfElseResult.TC0);
        }

        public static T TryGet<T>(Func<T> func, T ifError = default(T)) {
            try {
                return func();
            } catch {
                return ifError;
            }
        }

        public static void ForEach<T>(Action<T> action, T t, params T[] list) {
            action(t);
            foreach (var tt in list) {
                action(tt);
            }
        }

        /// <summary>
        /// 执行结果等同于<see cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>。
        /// </summary>
        public static bool Any<T>(Func<T, bool> func, T t, params T[] list) {
            if (func(t)) {
                return true;
            }
            foreach (var tt in list) {
                if (func(tt)) {
                    return true;
                }
            }
            return false;
        }
    }
}