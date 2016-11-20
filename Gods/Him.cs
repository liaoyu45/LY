using System;
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

        public static IEnumerable<MethodBase> GetCallers<T>() {
            var type = typeof(T);
            return GetCallers(type, t => true);
        }

        public static IEnumerable<MethodBase> GetCallers(Type type) {
            return GetCallers(type, t => true);
        }

        public static IEnumerable<MethodBase> GetCallers(Type type, Func<MethodBase, bool> filter) {
            var types = new StackTrace().GetFrames().Select(f => f.GetMethod());
            var indexes = types.Select((t, i) => t.DeclaringType == type ? i + 1 : 0).Where(i => i > 0);
            var result = types.Where((t, i) => indexes.Contains(i));
            return result.Where(filter);
        }

        public static IEnumerable<Type> GetBases<CT>() {
            return GetBases(typeof(CT), typeof(object));
        }

        public static IEnumerable<Type> GetBases<CT>(Type bt) {
            return GetBases(typeof(CT), bt);
        }

        public static IEnumerable<Type> GetBases(Type ct) {
            return GetBases(ct, typeof(object));
        }

        public static IEnumerable<Type> GetBases<CT, BT>() where CT : BT {
            return GetBases(typeof(CT), typeof(BT));
        }

        public static IEnumerable<Type> GetBases(Type ct, Type bt) {
            if (!ct.IsSubclassOf(bt)) {
                throw new ArgumentException();
            }
            yield return ct.BaseType;
            if (ct.BaseType != bt) {
                foreach (var t in GetBases(ct.BaseType, bt)) {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// 检查一个预期为等差数列的序列中的异常元素。
        /// </summary>
        /// <typeparam name="T">要可比较的类型。</typeparam>
        /// <param name="source">要检查的集合。</param>
        /// <param name="tolerance">此集合的预期公差。</param>
        /// <returns>返回预期为等差数列的序列中第一个异常元素，如果存在。</returns>
        public static T CheckArithmeticProgression<T>(this IEnumerable<T> source, int tolerance) where T : IComparable<T> {
            if (source?.Count() < 3) {
                throw new IndexOutOfRangeException("source?.Count() < 3");
            }
            var tor = source.GetEnumerator();
            if (tor.MoveNext()) {
                var previous = tor.Current;
                var i = 1;
                while (tor.MoveNext()) {
                    var current = tor.Current;
                    if (current.CompareTo(previous) != tolerance) {
                        return current;
                    }
                    i++;
                }
            }
            return default(T);
        }

        public static int? GetTolerance<T>(this IEnumerable<T> source) where T : IComparable<T> {
            if (source.Count() == 1) {
                return null;
            }
            var expected = source.Last().CompareTo(source.First()) / (source.Count() - 1);
            if (CheckArithmeticProgression(source, expected) != null) {
                return null;
            }
            return expected;
        }

        public static IEnumerable<object> ParseValues(this NameValueCollection source, Dictionary<string, Type> dict) {
            foreach (var p in dict) {
                var value = source[p.Key];
                var type = p.Value;
                if (type == typeof(string)) {
                    yield return value;
                } else if (type == typeof(int)) {
                    yield return int.Parse(value);
                } else if (type == typeof(double)) {
                    yield return double.Parse(value);
                } else if (type == typeof(float)) {
                    yield return float.Parse(value);
                } else if (type == typeof(DateTime)) {
                    yield return DateTime.Parse(value);
                } else if (type == typeof(short)) {
                    yield return short.Parse(value);
                } else if (type == typeof(long)) {
                    yield return long.Parse(value);
                } else if (type == typeof(decimal)) {
                    yield return decimal.Parse(value);
                } else if (type == typeof(bool)) {
                    yield return bool.Parse(value);
                } else {
                    yield return value;
                }
            }
        }

        public static string Remove(this string source, params char[] chars) {
            return source.Except(chars).ToString(null);
        }

        public static string ToString<T>(this IEnumerable<T> source, object spliter) {
            var b = new StringBuilder();
            foreach (var c in source) {
                b.Append(c);
                b.Append(spliter);
            }
            return b.ToString();
        }

        public static string TrimAnything(this string source, params char[] trimChars) {
            return (source ?? string.Empty).Trim(trimChars);
        }

        public static int ToInt32(this string source) {
            try {
                return int.Parse(source);
            } catch (Exception) {
                return 0;
            }
        }
        public static IPAddress GetLocalIP() {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
                if (ip.AddressFamily.ToString() == "InterNetwork") {
                    return ip;
                }
            }
            return null;
        }
        /// <summary>
        /// 找到指定了最上级父类的所有父类的GUID。
        /// </summary>
        /// <param name="type">以此子类开始向上寻找。</param>
        /// <param name="till">到此为止。</param>
        /// <returns>所有父类的GUID。</returns>
        public static IEnumerable<Guid> GUIDTill(this Type type, Type till) {
            if (type.IsSubclassOf(till)) {
                yield return type.GUID;
                foreach (var g in GUIDTill(type.BaseType, till)) {
                    yield return g;
                }
            } else {
                if (type == till) {
                    yield return till.GUID;
                }
            }
        }

        public static Dictionary<string, string> GetContentTypes { get; } = new Func<Dictionary<string, string>>(() => {
            var all = "|.*|application/octet-stream|.tif|image/tiff|.001|application/x-001|.301|application/x-301|.323|text/h323|.906|application/x-906|.907|drawing/907|.a11|application/x-a11|.acp|audio/x-mei-aac|.ai|application/postscript|.aif|audio/aiff|.aifc|audio/aiff|.aiff|audio/aiff|.anv|application/x-anv|.asa|text/asa|.asf|video/x-ms-asf|.asp|text/asp|.asx|video/x-ms-asf|.au|audio/basic|.avi|video/avi|.awf|application/vnd.adobe.workflow|.biz|text/xml|.bmp|application/x-bmp|.bot|application/x-bot|.c4t|application/x-c4t|.c90|application/x-c90|.cal|application/x-cals|.cat|application/vnd.ms-pki.seccat|.cdf|application/x-netcdf|.cdr|application/x-cdr|.cel|application/x-cel|.cer|application/x-x509-ca-cert|.cg4|application/x-g4|.cgm|application/x-cgm|.cit|application/x-cit|.class|java/*|.cml|text/xml|.cmp|application/x-cmp|.cmx|application/x-cmx|.cot|application/x-cot|.crl|application/pkix-crl|.crt|application/x-x509-ca-cert|.csi|application/x-csi|.css|text/css|.cut|application/x-cut|.dbf|application/x-dbf|.dbm|application/x-dbm|.dbx|application/x-dbx|.dcd|text/xml|.dcx|application/x-dcx|.der|application/x-x509-ca-cert|.dgn|application/x-dgn|.dib|application/x-dib|.dll|application/x-msdownload|.doc|application/msword|.dot|application/msword|.drw|application/x-drw|.dtd|text/xml|.dwf|Model/vnd.dwf|.dwf|application/x-dwf|.dwg|application/x-dwg|.dxb|application/x-dxb|.dxf|application/x-dxf|.edn|application/vnd.adobe.edn|.emf|application/x-emf|.eml|message/rfc822|.ent|text/xml|.epi|application/x-epi|.eps|application/x-ps|.eps|application/postscript|.etd|application/x-ebx|.exe|application/x-msdownload|.fax|image/fax|.fdf|application/vnd.fdf|.fif|application/fractals|.fo|text/xml|.frm|application/x-frm|.g4|application/x-g4|.gbr|application/x-gbr|.|application/x-|.gif|image/gif|.gl2|application/x-gl2|.gp4|application/x-gp4|.hgl|application/x-hgl|.hmr|application/x-hmr|.hpg|application/x-hpgl|.hpl|application/x-hpl|.hqx|application/mac-binhex40|.hrf|application/x-hrf|.hta|application/hta|.htc|text/x-component|.htm|text/html|.html|text/html|.htt|text/webviewhtml|.htx|text/html|.icb|application/x-icb|.ico|image/x-icon|.ico|application/x-ico|.iff|application/x-iff|.ig4|application/x-g4|.igs|application/x-igs|.iii|application/x-iphone|.img|application/x-img|.ins|application/x-internet-signup|.isp|application/x-internet-signup|.IVF|video/x-ivf|.java|java/*|.jfif|image/jpeg|.jpe|image/jpeg|.jpe|application/x-jpe|.jpeg|image/jpeg|.jpg|image/jpeg|.jpg|application/x-jpg|.js|application/x-javascript|.jsp|text/html|.la1|audio/x-liquid-file|.lar|application/x-laplayer-reg|.latex|application/x-latex|.lavs|audio/x-liquid-secure|.lbm|application/x-lbm|.lmsff|audio/x-la-lms|.ls|application/x-javascript|.ltr|application/x-ltr|.m1v|video/x-mpeg|.m2v|video/x-mpeg|.m3u|audio/mpegurl|.m4e|video/mpeg4|.mac|application/x-mac|.man|application/x-troff-man|.math|text/xml|.mdb|application/msaccess|.mdb|application/x-mdb|.mfp|application/x-shockwave-flash|.mht|message/rfc822|.mhtml|message/rfc822|.mi|application/x-mi|.mid|audio/mid|.midi|audio/mid|.mil|application/x-mil|.mml|text/xml|.mnd|audio/x-musicnet-download|.mns|audio/x-musicnet-stream|.mocha|application/x-javascript|.movie|video/x-sgi-movie|.mp1|audio/mp1|.mp2|audio/mp2|.mp2v|video/mpeg|.mp3|audio/mp3|.mp4|video/mpeg4|.mpa|video/x-mpg|.mpd|application/vnd.ms-project|.mpe|video/x-mpeg|.mpeg|video/mpg|.mpg|video/mpg|.mpga|audio/rn-mpeg|.mpp|application/vnd.ms-project|.mps|video/x-mpeg|.mpt|application/vnd.ms-project|.mpv|video/mpg|.mpv2|video/mpeg|.mpw|application/vnd.ms-project|.mpx|application/vnd.ms-project|.mtx|text/xml|.mxp|application/x-mmxp|.net|image/pnetvue|.nrf|application/x-nrf|.nws|message/rfc822|.odc|text/x-ms-odc|.out|application/x-out|.p10|application/pkcs10|.p12|application/x-pkcs12|.p7b|application/x-pkcs7-certificates|.p7c|application/pkcs7-mime|.p7m|application/pkcs7-mime|.p7r|application/x-pkcs7-certreqresp|.p7s|application/pkcs7-signature|.pc5|application/x-pc5|.pci|application/x-pci|.pcl|application/x-pcl|.pcx|application/x-pcx|.pdf|application/pdf|.pdf|application/pdf|.pdx|application/vnd.adobe.pdx|.pfx|application/x-pkcs12|.pgl|application/x-pgl|.pic|application/x-pic|.pko|application/vnd.ms-pki.pko|.pl|application/x-perl|.plg|text/html|.pls|audio/scpls|.plt|application/x-plt|.png|image/png|.png|application/x-png|.pot|application/vnd.ms-powerpoint|.ppa|application/vnd.ms-powerpoint|.ppm|application/x-ppm|.pps|application/vnd.ms-powerpoint|.ppt|application/vnd.ms-powerpoint|.ppt|application/x-ppt|.pr|application/x-pr|.prf|application/pics-rules|.prn|application/x-prn|.prt|application/x-prt|.ps|application/x-ps|.ps|application/postscript|.ptn|application/x-ptn|.pwz|application/vnd.ms-powerpoint|.r3t|text/vnd.rn-realtext3d|.ra|audio/vnd.rn-realaudio|.ram|audio/x-pn-realaudio|.ras|application/x-ras|.rat|application/rat-file|.rdf|text/xml|.rec|application/vnd.rn-recording|.red|application/x-red|.rgb|application/x-rgb|.rjs|application/vnd.rn-realsystem-rjs|.rjt|application/vnd.rn-realsystem-rjt|.rlc|application/x-rlc|.rle|application/x-rle|.rm|application/vnd.rn-realmedia|.rmf|application/vnd.adobe.rmf|.rmi|audio/mid|.rmj|application/vnd.rn-realsystem-rmj|.rmm|audio/x-pn-realaudio|.rmp|application/vnd.rn-rn_music_package|.rms|application/vnd.rn-realmedia-secure|.rmvb|application/vnd.rn-realmedia-vbr|.rmx|application/vnd.rn-realsystem-rmx|.rnx|application/vnd.rn-realplayer|.rp|image/vnd.rn-realpix|.rpm|audio/x-pn-realaudio-plugin|.rsml|application/vnd.rn-rsml|.rt|text/vnd.rn-realtext|.rtf|application/msword|.rtf|application/x-rtf|.rv|video/vnd.rn-realvideo|.sam|application/x-sam|.sat|application/x-sat|.sdp|application/sdp|.sdw|application/x-sdw|.sit|application/x-stuffit|.slb|application/x-slb|.sld|application/x-sld|.slk|drawing/x-slk|.smi|application/smil|.smil|application/smil|.smk|application/x-smk|.snd|audio/basic|.sol|text/plain|.sor|text/plain|.spc|application/x-pkcs7-certificates|.spl|application/futuresplash|.spp|text/xml|.ssm|application/streamingmedia|.sst|application/vnd.ms-pki.certstore|.stl|application/vnd.ms-pki.stl|.stm|text/html|.sty|application/x-sty|.svg|text/xml|.swf|application/x-shockwave-flash|.tdf|application/x-tdf|.tg4|application/x-tg4|.tga|application/x-tga|.tif|image/tiff|.tif|application/x-tif|.tiff|image/tiff|.tld|text/xml|.top|drawing/x-top|.torrent|application/x-bittorrent|.tsd|text/xml|.txt|text/plain|.uin|application/x-icq|.uls|text/iuls|.vcf|text/x-vcard|.vda|application/x-vda|.vdx|application/vnd.visio|.vml|text/xml|.vpg|application/x-vpeg005|.vsd|application/vnd.visio|.vsd|application/x-vsd|.vss|application/vnd.visio|.vst|application/vnd.visio|.vst|application/x-vst|.vsw|application/vnd.visio|.vsx|application/vnd.visio|.vtx|application/vnd.visio|.vxml|text/xml|.wav|audio/wav|.wax|audio/x-ms-wax|.wb1|application/x-wb1|.wb2|application/x-wb2|.wb3|application/x-wb3|.wbmp|image/vnd.wap.wbmp|.wiz|application/msword|.wk3|application/x-wk3|.wk4|application/x-wk4|.wkq|application/x-wkq|.wks|application/x-wks|.wm|video/x-ms-wm|.wma|audio/x-ms-wma|.wmd|application/x-ms-wmd|.wmf|application/x-wmf|.wml|text/vnd.wap.wml|.wmv|video/x-ms-wmv|.wmx|video/x-ms-wmx|.wmz|application/x-ms-wmz|.wp6|application/x-wp6|.wpd|application/x-wpd|.wpg|application/x-wpg|.wpl|application/vnd.ms-wpl|.wq1|application/x-wq1|.wr1|application/x-wr1|.wri|application/x-wri|.wrk|application/x-wrk|.ws|application/x-ws|.ws2|application/x-ws|.wsc|text/scriptlet|.wsdl|text/xml|.wvx|video/x-ms-wvx|.xdp|application/vnd.adobe.xdp|.xdr|text/xml|.xfd|application/vnd.adobe.xfd|.xfdf|application/vnd.adobe.xfdf|.xhtml|text/html|.xls|application/vnd.ms-excel|.xls|application/x-xls|.xlw|application/x-xlw|.xml|text/xml|.xpl|audio/scpls|.xq|text/xml|.xql|text/xml|.xquery|text/xml|.xsd|text/xml|.xsl|text/xml|.xslt|text/xml|.xwd|application/x-xwd|.x_b|application/x-x_b|.sis|application/vnd.symbian.install|.sisx|application/vnd.symbian.install|.x_t|application/x-x_t|.ipa|application/vnd.iphone|.apk|application/vnd.android.package-archive|.xap|application/x-silverlight-app|";
            var list = all.Trim('|').Split('|').ToList();
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < list.Count / 2; i += 2) {
                dict.Add(list[i], list[i + 1]);
            }
            return dict;
        })();
    }
}