(function () {
    "use strict"
    function GodThere() {
        this.initiatedTime = new Date();
        this.modes = {
            coding: location.href.length === 11,
            debugging: location.href.indexOf("localhost") > 0,
        };
        Object.defineProperty(this, "emptyFunction", {
            get: function () {
                return function () { };
            }
        });
        this.removeEventListener = function (obj, ename, func) {
            var store = obj[this.addEventListener.pre + ename];
            if (!store) {
                return;
            }
            var i = store.indexOf(func);
            if (i < 0) {
                return;
            }
            obj[this.addEventListener.pre + ename].splice(i, 1);
        };
        this.addEventListener = function (obj, enames) {
            if (arguments.length < 2) {
                return;
            }
            if (!this.addEventListener.pre) {
                this.addEventListener.pre = "god_" + Math.random() * Number.MAX_VALUE;
            }
            if (typeof arguments[1] === "string") {
                var ename = arguments[1];
                var fullname = "on" + ename;
                if (!(fullname in obj)) {
                    var store = this.addEventListener.pre + ename;
                    if (!(store in obj)) {
                        obj[store] = [];
                    }
                    Object.defineProperty(obj, fullname, {
                        set: function (v) {
                            obj[store].push(v);
                        }
                    });
                    var notice = ename === "notice" ? "notice" : "notice" + ename;
                    obj[notice] = function () {
                        for (var i = 0; i < obj[store].length; i++) {
                            try {
                                obj[store][i].apply(obj, arguments);
                            } catch (e) {
                                console.log(e.message);
                            }
                        }
                    };
                }
            }
            if (typeof arguments[2] === "string") {
                var narguments = [obj];
                for (var i = 2; i < arguments.length; i++) {
                    narguments.push(arguments[i]);
                }
                this.addEventListener.apply(this, narguments);
            }
        };
        this.toDefault = function (v, dv) {
            if (typeof v === "undefined" || v === null) {
                return dv;
            }
            return v;
        };
        this.window = (function () {
            return {
                queryString: function (item) {
                    var svalue = location.search.match(new RegExp("[\?\&]" + item + "=([^\&]*)(\&?)", "i"));
                    return svalue ? svalue[1] : svalue;
                },
                allSuffixesImg: function (img, filename) {
                    var suffixes = ["jpg", "bmp", "gif", "png"];
                    var i = 0;
                    function assign() {
                        if (i === suffixes.length) {
                            return;
                        }
                        img.src = god.formatString("{0}.{1}", filename, suffixes[i] + "?r=" + Math.random());
                        i++;
                    };
                    img.onerror = assign;
                    assign();
                },
                fromTemplate: function (templateSelector, outterTagName) {
                    var template = document.body.querySelector(templateSelector);
                    if (!outterTagName) {
                        outterTagName = template.dataset.tagName;
                    }
                    var parent = document.createElement(outterTagName);
                    parent.innerHTML = template.textContent.trim();
                    return parent.firstElementChild;
                },
                browser: {
                    versions: (function () {
                        var u = navigator.userAgent, app = navigator.appVersion;
                        return {//移动终端浏览器版本信息 
                            trident: u.indexOf('Trident') > -1, //IE内核
                            presto: u.indexOf('Presto') > -1, //opera内核
                            webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                            gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') === -1, //火狐内核
                            ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                            android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或者uc浏览器
                            iPhone: u.indexOf('iPhone') > -1 || u.indexOf('Mac') > -1, //是否为iPhone或者QQHD浏览器
                            iPad: u.indexOf('iPad') > -1, //是否iPad
                            webApp: u.indexOf('Safari') === -1 //是否web应该程序，没有头部与底部
                        };
                    })(),
                    language: (navigator.browserLanguage || navigator.language).toLowerCase()
                },
                mobile: (function () {
                    return !!navigator.userAgent.match(/Mobile/);
                })(),
                dragable: function (ele, trigger) {
                    var _trigger = (function () {
                        var r;
                        if (typeof trigger === "string") {
                            r = ele.querySelector(trigger);
                        } else {
                            if (trigger instanceof HTMLElement) {
                                r = trigger;
                            }
                        }
                        return r;
                    })();
                    if (!_trigger) {
                        return;
                    }
                    var events = {};
                    events.start = {};
                    _trigger.addEventListener("mousedown", function start(e) {
                        var os = getComputedStyle(ele);
                        ele.dataset.x = parseFloat(os.left) || 0;
                        ele.dataset.y = parseFloat(os.top) || 0;
                        ele.dataset.ox = e.clientX;
                        ele.dataset.oy = e.clientY;
                        window.addEventListener("mousemove", move);
                        window.addEventListener("blur", release);
                        window.addEventListener("mouseup", release);
                    });
                    function move(e) {
                        ele.style.left = parseFloat(ele.dataset.x) + e.clientX - parseFloat(ele.dataset.ox) + "px";
                        ele.style.top = parseFloat(ele.dataset.y) + e.clientY - parseFloat(ele.dataset.oy) + "px";
                        document.title = ele.style.top + "  " + ele.style.left;
                    }
                    function release() {
                        window.removeEventListener("blur", release);
                        window.removeEventListener("mouseup", release);
                        window.removeEventListener("mousemove", move);
                        moving = false;
                    }
                },
                toast: function (message, duration, later) {
                    if (!new String(message).length) return;
                    var Toast = function () {
                        this.p = document.createElement("p");
                        this.s = document.createElement("span");

                        this.p.className = "toast";
                        this.s.className = "toastInner";

                        this.p.appendChild(this.s);

                        this.showMessage = function (message, duration) {
                            this.s.innerHTML = message;
                            document.body.appendChild(this.p);
                            setTimeout(function (e) {
                                document.body.removeChild(e.p);
                                god.safe(later)();
                            }, duration, this);
                        }
                    }
                    new Toast().showMessage(message, duration || 1024);
                },
            };
            return a;
        })();
        this.formatString = function () {
            if (arguments.length === 0)
                return null;
            var str = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
                str = str.replace(re, arguments[i]);
            }
            return str;
        };
        this.formatTime = function (time) {
            time = time || new Date();
            return this.formatString("{0}/{1}/{2}", time.getHours(), time.getMinutes(), time.getSeconds());
        },
        this.trim = function (str, side, replace) {
            if (god.modes.coding) {
                str = ""; side = true; replace = [];
            }
            if (typeof replace === "string") {
                replace = replace.split("");
            }
            if (typeof side === "boolean") {
                return god.trim(str, side, replace);
            }
            return god.trim(str, true, side) + god.trim(str, false, side);
        };
        this.getTypeName = function (obj) {
            if (typeof obj === "undefined") return undefined;
            var name = obj.constructor.toString().trim();
            var match = name.match(/[^\s]+?(?=\s*\()/);
            return match.length ? match[0] : "";
        };
        this.idIndexFile = (function () {
            function idIndexFileInner() {
                var self = this;
                //this.folder = this.format = "";
                //this.id = 0;
                //this.suffixes = [];
                //this.canceled = false;
                this.reset = function () {
                    self.folder = "";
                    self.format = "{0}_{1}";
                    self.id = 0;
                    self.suffixes = [];
                    this.canceled = true;
                    return self;
                };
                this.reset();
                this.initiate = function (folder, id, format, suffixes) {
                    this.folder = folder;
                    this.id = id;
                    if (typeof format === "string" && format.indexOf("{0}") > -1 && format.indexOf("{1}") > -1) {
                        this.format = format;
                    }
                    this.suffixes = [];
                    for (var i = 3; i < arguments.length; i++) {
                        this.suffixes.push(arguments[i]);
                    }
                    if (!self.suffixes.length) {
                        self.suffixes = ["jpg", "jpeg", "bmp", "png", "gif"];
                    }
                    this.canceled = false;
                    return this;
                };
                function iiData(index, suffixIndex, onSuccess, onFail, picture) {
                    this.index = index;
                    this.suffixIndex = suffixIndex;
                    this.onSuccess = onSuccess;
                    this.onFail = onFail;
                    this.id = 0;
                    this.folder = "";
                    this.format = "";
                    this.suffixes = [];
                    this.nextIndex = function (callback) {
                        this.index++;
                        this.suffixIndex = 0;
                        god.safe(callback)(this);
                    };
                    this.nextSuffix = function (has, hasNot) {
                        var result = this.suffixIndex < this.suffixes.length - 1;
                        if (result) {
                            this.suffixIndex++;
                            god.safe(has)(this);
                        } else {
                            god.safe(hasNot)(this);
                        }
                        return result;
                    };
                    this.getPath = function () {
                        var prename = god.formatString(this.format, this.id, this.index);
                        var result = god.formatString("{0}/{1}.{2}", this.folder, prename, this.suffixes[this.suffixIndex]);
                        return result;
                    };
                }
                function create_iiData(oldArguments) {
                    if (god.getTypeName(oldArguments[0]) === "iiData") {
                        if (god.modes.coding) {
                            return new iiData();
                        }
                        return oldArguments[0];
                    }
                    var index, suffixIndex, onSuccess, onFail;
                    if (!oldArguments[1]) {
                        index = suffixIndex = 0;
                        onSuccess = oldArguments[0];
                    } else if (!oldArguments[2]) {
                        index = oldArguments[0];
                        suffixIndex = 0;
                        onSuccess = oldArguments[1];
                    } else {
                        index = oldArguments[0];
                        suffixIndex = oldArguments[1];
                        onSuccess = oldArguments[2];
                        onFail = oldArguments[3];
                    }
                    var result = new iiData(index, suffixIndex, onSuccess, onFail);
                    result.id = self.id;
                    result.folder = self.folder;
                    result.format = self.format;
                    result.suffixes = self.suffixes;
                    return result;
                }
                this.loadImgs = function (index, onSuccess, onFail) {
                    if (god.modes.coding) {
                        if (typeof index === "function") {
                            index(new Image());
                            onSuccess(new Image());
                        } else {
                            onSuccess(new Image());
                            onFail(new Image());
                        }
                    }
                    if (self.canceled) {
                        return;
                    }
                    var r = create_iiData(arguments);
                    var img = document.createElement("img");
                    img.onerror = function () {
                        r.nextSuffix(self.loadImgs, function () {
                            self.reset();
                            god.safe(r.onFail)(r.index);
                        });
                    };
                    img.onload = function (e) {
                        god.safe(r.onSuccess)(e.target);
                        r.nextIndex(self.loadImgs);
                    };
                    img.src = r.getPath();
                }
            }
            return new idIndexFileInner();
        })();
        this.random = function (exclude) {
            function innerClass() {
                function randomString(len, chars) {
                    if (typeof exclude === "string") {
                        for (var i = 0; i < exclude.length; i++) {
                            chars = chars.replace(exclude[i], "");
                        }
                    }
                    var max = chars.length;
                    var result = "";
                    for (i = 0; i < len; i++) {
                        result += chars.charAt(Math.floor(Math.random() * max));
                    }
                    return result;
                }
                var fieldStart = "_$ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                var keyboard = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()`-=[]\;',./~_+{}|:\"<>?";
                this.keyboard = function (len) {
                    return randomString(len, keyboard);
                };
                this.fieldName = function (len) {
                    return randomString(1, fieldStart) + randomString(len - 1, keyboard);
                };
                this.number = function (len) {
                    return parseInt("0x" + randomString(1, "01234566789ABCDEF"));
                };
                this.color = function () {
                    return "#" + ("00000" + ((Math.random() * 16777215 + 0.5) >> 0).toString(16)).slice(-6);
                }
            };
            return new innerClass();
        };
        this.safe = function (func, thisArg) {
            if (!thisArg) {
                thisArg = window;
            }
            if (typeof func !== "function") {
                console.log("not a function");
                return this.emptyFunction;
            }
            return function () {
                try {
                    return func.apply(thisArg, arguments);
                } catch (e) {
                    console.log(e.message);
                }
            };
        };
        this.removeItem = function (arr, filter) {
            for (var i = arr.length - 1; i >= 0; i--) {
                if (filter.call(arr[i], i)) {
                    arr.splice(i, 1);
                }
            }
        };
        this.color = (function () {
            var innerClass = function () {
                var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/;//http://www.zhangxinxu.com/wordpress/2010/03/javascript-hex-rgb-hsl-color-convert/
                this.rgb2hex = function (rgb) {
                    if (/^(rgb|RGB)/.test(rgb)) {
                        var aColor = rgb.replace(/(?:\(|\)|rgb|RGB)*/g, "").split(",");
                        var strHex = "#";
                        for (var i = 0; i < aColor.length; i++) {
                            var hex = Number(aColor[i]).toString(16);
                            if (hex === "0") {
                                hex += hex;
                            }
                            strHex += hex;
                        }
                        if (strHex.length !== 7) {
                            strHex = rgb;
                        }
                        return strHex;
                    } else if (reg.test(rgb)) {
                        var aNum = rgb.replace(/#/, "").split("");
                        if (aNum.length === 6) {
                            return rgb;
                        } else if (aNum.length === 3) {
                            var numHex = "#";
                            for (var i = 0; i < aNum.length; i += 1) {
                                numHex += (aNum[i] + aNum[i]);
                            }
                            return numHex;
                        }
                    } else {
                        return rgb;
                    }
                };
                this.hex2rgb = function (hex) {
                    hex = hex.toLowerCase();
                    if (hex && reg.test(hex)) {
                        if (hex.length === 4) {
                            var sColorNew = "#";
                            for (var i = 1; i < 4; i += 1) {
                                sColorNew += hex.slice(i, i + 1).concat(hex.slice(i, i + 1));
                            }
                            hex = sColorNew;
                        }
                        var sColorChange = [];
                        for (var i = 1; i < 7; i += 2) {
                            sColorChange.push(parseInt("0x" + hex.slice(i, i + 2)));
                        }
                        return "RGB(" + sColorChange.join(",") + ")";
                    } else {
                        return hex;
                    }
                };
                this.hsl2rgb = function (h, s, l) {//http://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion/
                    var r, g, b;
                    if (s === 0) {
                        r = g = b = l;
                    } else {
                        var hue2rgb = function hue2rgb(p, q, t) {
                            if (t < 0) t += 1;
                            if (t > 1) t -= 1;
                            if (t < 1 / 6) return p + (q - p) * 6 * t;
                            if (t < 1 / 2) return q;
                            if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
                            return p;
                        }
                        var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                        var p = 2 * l - q;
                        r = hue2rgb(p, q, h + 1 / 3);
                        g = hue2rgb(p, q, h);
                        b = hue2rgb(p, q, h - 1 / 3);
                    }

                    return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
                };
                this.rgb2hsl = function rgbToHsl(r, g, b) {
                    r /= 255, g /= 255, b /= 255;
                    var max = Math.max(r, g, b), min = Math.min(r, g, b);
                    var h, s, l = (max + min) / 2;
                    if (max === min) {
                        h = s = 0;
                    } else {
                        var d = max - min;
                        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
                        switch (max) {
                            case r: h = (g - b) / d + (g < b ? 6 : 0); break;
                            case g: h = (b - r) / d + 2; break;
                            case b: h = (r - g) / d + 4; break;
                        }
                        h /= 6;
                    }
                    return [h, s, l];
                }
            };
            return new innerClass();
        })();
        this.math = (function () {
            //get m from n, return indexes array, max index is n.
            function combination(n, m) {
                var r = [];
                var arr = [];
                for (var i = 0; i < n; i++) {
                    arr.push(i);
                }
                (function f(t, a, n) {
                    for (var i = 0, l = a.length; i <= l - n; i++) {
                        var _t = t.concat(a[i]);
                        if (_t.length === m) {
                            r.push(_t);
                        } else {
                            f(_t, a.slice(i + 1), n - 1);
                        }
                    }
                })([], arr, m);
                return r;
            }
            function permutation(n, m) {
                var arr = [];
                for (var i = 0; i < n; i++) {
                    arr.push(i);
                }
                var r = [];
                (function c(t, a) {
                    for (var i = 0; i < a.length; i++) {
                        var arri = a[i];
                        var _t = t.concat(arri);
                        if (_t.length === m) {
                            r.push(_t);
                        } else {
                            c(_t, a.filter(function (ii) { return ii != arri; }));
                        }
                    }
                })([], arr, m);
                return r;
            }

            return { combination: combination, permutation: permutation };
        })(),
        this.arr = (function () {
            function moveArray(distance, arr, instance) {
                function innerMove(distance) {
                    var tag = "uqpoirewuqwpeiruqwre";
                    var front = 0, end = 0, endAdded = 0, frontAdded = 0;
                    if (this[tag]) {
                        front = this.front;
                        end = this.end;
                        frontAdded = this.frontAdded;
                        endAdded = this.endAdded;
                        arr = this.arr;
                    }
                    var d = Math.abs(distance);
                    if (distance > 0) {
                        for (var i = 0; i < d - end; i++) {
                            arr.push(instance(arr[arr.length - 1], false));
                            endAdded += 1;
                        }
                    } else {
                        for (var i = 0; i < d - front; i++) {
                            arr.unshift(instance(arr[0], true));
                            frontAdded += 1;
                        }
                    }
                    front += distance;
                    end -= distance;
                    if (front < 0) {
                        front = 0;
                    }
                    if (end < 0) {
                        end = 0;
                    }
                    this.end = end;
                    this.front = front;
                    this.endAdded = endAdded;
                    this.frontAdded = frontAdded;
                    this.arr = arr;
                    if (!this.instance) {
                        this.instance = instance;
                    }
                    if (!this.moveArray) {
                        this.moveArray = innerMove;
                    }
                    if (!this.orignal) {
                        this.orignal = arr;
                    }
                    this.current = arr.slice(front, front + arr.length);
                    this[tag] = true;
                    return this;
                }
                return new innerMove(distance, arr, instance);
            }
            function removeItem(arr, filter) {
                for (var i = arr.length - 1; i >= 0; i--) {
                    if (filter.call(arr[i], i)) {
                        arr.splice(i, 1);
                    }
                }
            }
            return { moveArray: moveArray, removeItem: removeItem };
        })();
    };
    this.god = new GodThere();
}).call(this);
document.head.innerHTML += '<style type="text/css">@keyframes slide2topPosition{to{bottom:256px;display:none;}}@keyframes slide2topColor{to{color:rgba(0,0,0,0);background-color:rgba(0,0,0,0);border-color:rgba(0,0,0,0);}}.toast,.toast_slide2top{bottom:0;position:fixed;width:100%;text-align:center;z-index:1111;animation:slide2topPosition 2s cubic-bezier(0,1,0.5,1) 1 normal;}.toast .toastInner,.toast_slide2top .toastInner{padding:5px;display:inline-block;background-color:#393939;color:#a4a4a4;border-radius:5px;animation:slide2topColor 2s ease 1 normal;}</style>';
