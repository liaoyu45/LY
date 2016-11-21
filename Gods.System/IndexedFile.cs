using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Gods.System {
    /// <summary>
    /// 以固定格式标记同一文件夹中相的文件，并将其分组。
    /// </summary>
    [Guid(GUID)]
    public sealed class IndexedFile {
        /// <summary>
        /// “{0}”和“{1}”必须被包含且不能相临；其余字符为“[^\d{}\s]”。
        /// </summary>
        public const string FORMAT_REGEX = @"^[^\d{}\s]*[{]0}[^\d{}\s]+[{]1}[^\d{}\s]*$";

        private const string GUID = "C63A0348-40EC-4D93-B119-0806530D1CE7";
        private readonly string suffixesPattern;

        public IndexedFile(string folder, string format, string suffixes) {
            if (!Regex.IsMatch(format, FORMAT_REGEX)) {
                throw new ArgumentException(format);
            }
            if (suffixes.Split('|').Any(s => s.Intersect(Path.GetInvalidFileNameChars()).Any())) {
                throw new ArgumentException(suffixes);
            }
            this.Folder = folder;
            this.Format = format;
            this.Suffixes = suffixes.Split('|');
            this.suffixesPattern = $"(?=.({suffixes}))";
        }

        public string Folder { get; }
        /// <summary>
        /// 获取相关文件名的统一格式。参见 <see cref="FORMAT_REGEX"/>。
        /// </summary>
        public string Format { get; }
        /// <summary>
        /// 获取所有可选的文件后后缀。默认第一项。
        /// </summary>
        public string[] Suffixes { get; }

        public int LastIndexOf(int id) {
            const string index = @"\d+";
            var temp = string.Format(this.Format, id, index);
            var arr = temp.Split(new[] { index }, StringSplitOptions.RemoveEmptyEntries);
            var pre = temp.StartsWith(index) ? string.Empty : $"(?<={arr[0]})";
            var next = temp.EndsWith(index) ? string.Empty : $"(?={arr[1]})";
            var pattern = $"{pre}{index}{next}{this.suffixesPattern}";
            var files = from f in Him.EnumerateFiles(this.Folder, this.Suffixes)
                        let p = f.Split('/', '\\').Last()
                        where Regex.IsMatch(p, pattern)
                        let v = Regex.Match(p, pattern).Value
                        select int.Parse(v);
            var result = files.LastOrDefault();
            return result;
        }

        private string getFilePath(int id, int index, string suffix) {
            var prename = string.Format(this.Format, id, index);
            return $"{this.Folder}/{prename}.{suffix}";
        }

        private string GetFile(int id, int index) {
            foreach (var suffix in this.Suffixes) {
                var p = getFilePath(id, index, suffix);
                if (File.Exists(p)) {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// 使用默认的后缀添加一个文件。
        /// </summary>
        /// <param name="id">要添加的文件的分组 Id。</param>
        public void AddFile(int id, byte[] bytes) =>
            this.AddFile(id, bytes, this.Suffixes.First());

        /// <summary>
        /// 添加一个具有指定分组 Id 的文件。
        /// </summary>
        /// <param name="id">要添加的文件的分组 Id。</param>
        /// <param name="suffix">要添加的文件的后缀。必须属于 <see cref="Suffixes"/>。</param>
        /// <exception cref="FormatException">要添加的文件的后缀不属于 <see cref="Suffixes"/>。</exception>
        public void AddFile(int id, byte[] bytes, string suffix) {
            if (!this.Suffixes.Contains(suffix)) {
                throw new FormatException(suffix);
            }
            var nextIndex = this.LastIndexOf(id) + 1;
            var path = this.getFilePath(id, nextIndex, suffix);
            File.WriteAllBytes(path, bytes);
        }

        /// <summary>
        /// 删除具有指定下标的文件，如果其不是最大下标，则在成功删除后将具有最大下标的文件移动到被删除的文件的位置。
        /// </summary>
        /// <param name="index">要删除的文件下标。</param>
        /// <returns>成功返回 0，失败则返回没有被删除的文件下标。</returns>
        public int DeleteFile(int id, int index) {
            var file = this.GetFile(id, index);
            if (file == null) {
                return index;
            }
            try {
                File.Delete(file);
            } catch {
                return index;
            }
            var lastIndex = this.LastIndexOf(id);
            if (lastIndex > index) {
                var lastFile = this.GetFile(id, lastIndex);
                try {
                    File.Move(lastFile, file);
                } catch {
                    File.Copy(lastFile, file);
                    return lastIndex;
                }
            }
            return 0;
        }
    }
}