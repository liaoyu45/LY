(function () {
	"use strict";
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
				var notice = ename === "notice" ? "notice" : "notice" + ename;
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
					obj[notice] = function () {
						for (var i = 0; i < obj[store].length; i++) {
							try {
								obj[store][i].apply(obj, arguments);
							} catch (e) {
								console.log(e.message);
							}
						}
					};
				} else {
					obj[notice] = obj[fullname];
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
				fakeGIF: function (ele, delay) {
					var imgs = arguments[2].map(e=> {
						var i = document.createElement("img");
						i.style.display = "none";
						i.src = e;
						return ele.appendChild(i);
					});
					var last = imgs[imgs.length - 1];
					var id = 0;
					var i = 0;
					function start() {
						i = setInterval(() => {
							last.style.display = "none";
							id++;
							if (id === imgs.length) {
								id = 0;
							}
							(last = imgs[id]).style.display = "block";
						}, delay);
					}
					start();
					return {
						stop: function () {
							clearInterval(i);
							i = 0;
						},
						start: function () {
							if (!i) {
								start();
							}
						},
						element: ele
					}
				},
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
				dragable: function (ops) {
					var x0, y0, log = { move: [], element: ops.element };
					god.addEventListener(ops, "start", "stop", "move");
					function returnFalse() {
						return false;
					}
					function move(e) {
						var s = getComputedStyle(ops.element);
						log.newElement.style.left = parseFloat(s.left) + e.clientX - x0 + "px";
						log.newElement.style.top = parseFloat(s.top) + e.clientY - y0 + "px";
						log.move.push(e);
						ops.noticemove(log);
					}
					function stop(e) {
						window.removeEventListener("mouseup", stop);
						window.removeEventListener("touchend", stop);
						window.removeEventListener("blur", stop);
						window.removeEventListener("mousemove", move);
						window.removeEventListener("touchmove", move);
						ops.trigger.addEventListener("touchstart", start);
						ops.trigger.addEventListener("mousedown", start);
						document.removeEventListener("dragstart", returnFalse);
						log.stop = e;
						if (ops.noticestop(log)) {
							log.element.style.top = log.newElement.style.top;
							log.element.style.left = log.newElement.style.left;
						}
						log.newElement.remove();
					}
					function start(e) {
						if (e.button) {
							return;
						}
						document.addEventListener("dragstart", returnFalse);
						x0 = e.clientX;
						y0 = e.clientY;
						ops.trigger.removeEventListener("touchstart", start);
						ops.trigger.removeEventListener("mousedown", start);
						window.addEventListener("touchmove", move);
						window.addEventListener("mousemove", move);
						window.addEventListener("blur", stop);
						window.addEventListener("mouseup", stop);
						window.addEventListener("touchend", stop);
						log.start = e;
						ops.noticestart(log);
						log.newElement = ops.element.cloneNode(1);
						log.newElement.removeAttribute("id");
						log.newElement.style.display = "block";
						ops.element.parentElement.appendChild(log.newElement);
					}
					ops.trigger.addEventListener("mousedown", start);
					ops.trigger.addEventListener("touchstart", start);
					return {
						undragable: function () {
							ops.trigger.removeEventListener("mousedown", start);
						},
						keep: function () {
							log.element.style.left = log.newElement.style.left;
							log.element.style.top = log.newElement.style.top;
						}
					}
				},
				watchDrag: function (ops) {
					function stop(e) {
						window.removeEventListener("mouseup", stop);
						window.removeEventListener("mousemove", ops.move);
						window.addEventListener("mousedown", start);
						ops.stop(e);
					}
					function start(e) {
						if (e.button) {
							return;
						}
						ops.start(e);
						release();
						window.addEventListener("mousemove", ops.move);
						window.addEventListener("mouseup", stop);
					}
					window.addEventListener("mousedown", start);
					function release() {
						window.removeEventListener("mousedown", start);
					}
					return {
						release: release,
					};
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
				setCenter: function (ele, always) {
					function resize() {
						ele.style.left = (window.outerWidth - ele.clientWidth) / 2 + "px";
						ele.style.top = (window.outerHeight - ele.clientHeight) / 2 + "px";
					}
					resize();
					if (always) {
						window.addEventListener("resize", resize);
						return function () {
							window.removeEventListener("resize", resize);
						}
					}
				}
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
		this.makeFunction = function (func) {
			return typeof func === "function" ? func : this.emptyFunction;
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
					}
					if (reg.test(rgb)) {
						var aNum = rgb.replace(/#/, "").split("");
						if (aNum.length === 6) {
							return rgb;
						}
						if (aNum.length === 3) {
							var numHex = "#";
							for (var i = 0; i < aNum.length; i += 1) {
								numHex += (aNum[i] + aNum[i]);
							}
							return numHex;
						}
					}
					return rgb;
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
						return sColorChange;
					}
					return hex;
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
				};
				this.cutGragient = function (c0, c1, p) {
					function toRGBA(str) {
						if (typeof str !== "string") {
							return str;
						}
						if (str.toLowerCase().startsWith("rgb")) {
							str = str.split('(')[1].split(')')[0].split(',');
							return [parseInt(str[0]), parseInt(str[1]), parseInt(str[2]), str.length === 4 ? parseFloat(str[3]) : 1.0];
						}
						str = str.replace("#", "");
						if (str.length === 3) {
							str = str[0] + str[0] + str[1] + str[1] + str[2] + str[2];
						}
						return [parseInt("0x" + str.substr(0, 2)), parseInt("0x" + str.substr(2, 2)), parseInt("0x" + str.substr(4, 2)), 1];
					}
					var str = typeof c0 === "string" || typeof c0 === "string";
					c0 = toRGBA(c0);
					c1 = toRGBA(c1);
					var r = [];
					for (var i = 0; i < 4; i++) {
						r.push(c0[i] + parseInt((c1[i] - c0[i]) / p));
					}
					return str ? `rgba(${r.join(',')})` : r;
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
							c(_t, a.filter(function (ii) { return ii !== arri; }));
						}
					}
				})([], arr, m);
				return r;
			}
			function getOpacity(width, height, fx, fy, dx, dy) {
				if (dx < 0 || dy < 0 || dx > width || dy > height) {
					return 0;
				}
				function getK(x, y, xx, yy) {
					return (yy - y) / (xx - x);
				}
				var k = getK(dx, dy, fx, fy);
				var kk = getK(0, 0, fx, fx);
				var kkk = getK(width, 0, fx, fy);
				var kkkk = getK(width, height, fx, fy);
				var kkkkk = getK(0, height, fx, fy);
				if ((k > kk || k < kkk) && dy < fy) {
					return Math.abs(dy / fy);
				}
				if (k > kkk && k < kkkk && dx > fx) {
					return Math.abs((width - dx) / (width - fx));
				}
				if ((k > kkkk || k < kkkkk) && dy > fy) {
					return Math.abs((height - dy) / (height - fy));
				}
				return Math.abs(dx / fx);
			}
			return {
				combination: combination, permutation: permutation, getOpacity: getOpacity
			};
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
			return {
				moveArray: moveArray, removeItem: removeItem
			};
		})();
	};
	this.god = new GodThere();
}).call(this);
document.head.innerHTML += '<style type="text/css">@keyframes slide2topPosition{to{bottom:256px;display:none;}}@keyframes slide2topColor{to{color:rgba(0,0,0,0);background-color:rgba(0,0,0,0);border-color:rgba(0,0,0,0);}}.toast,.toast_slide2top{bottom:0;position:fixed;width:100%;text-align:center;z-index:1111;animation:slide2topPosition 2s cubic-bezier(0,1,0.5,1) 1 normal;}.toast .toastInner,.toast_slide2top .toastInner{padding:5px;display:inline-block;background-color:#393939;color:#a4a4a4;border-radius:5px;animation:slide2topColor 2s ease 1 normal;}</style>';
