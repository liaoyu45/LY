using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Gods.Web {
    public abstract class AjaxPage : Page {
        #region script
        static readonly string SCRIPT = @"
        (function () {
            'use strict';
            function form2str(form) {
                var o = {};
                var s = form.getElementsByTagName('select');
                for (var i = 0; i < s.length; i++) {
                    var si = s[i];
                    var n = si.name;
                    if (!n) {
                        continue;
                    }
                    if (!si.multiple) {
                        o[n] = si.value;
                        continue;
                    }
                    o[n] = [];
                    var ops = si.options;
                    for (var j = 0; j < ops.length; j++) {
                        var oj = ops[j];
                        if (oj.selected) {
                            o[n].push(oj.value || oj.text);
                        }
                    }
                }
                var inputs = form.querySelectorAll('input');
                for (var i = 0; i < inputs.length; i++) {
                    var ri = inputs[i];
                    var n = ri.name;
                    if (!n) {
                        continue;
                    }
                    if (!(n in o)) {
                        if (ri.type === 'checkbox') {
                            o[n] = [];
                        } else {
                            o[n] = '';
                        }
                    }
                    switch (ri.type) {
                        case 'radio':
                            if (o[n]) {
                                continue;
                            }
                            if (ri.checked) {
                                o[n] = ri.value || 'on';
                            }
                            break;
                        case 'checkbox':
                            if (ri.checked) {
                                o[n].push(ri.value || 'on');
                            }
                            break;
                        default:
                            o[n] = ri.value;
                            break;
                    }
                }
                return obj2str(o);
            }
            function obj2str(obj) {
                var t = '';
                for (var i in obj) {
                    t += '&' + i + '=' + obj[i];
                }
                return t.length ? t.substr(1) : t;
            }
            this.$ajax = {
                send: function (form) {
                    var success, fail, error, complete;
                    var isForm = form.constructor === HTMLFormElement;
                    var a1 = arguments[1];
                    if (a1) {
                        if (typeof a1 === 'function') {
                            success = a1;
                        } else {
                            if (typeof a1.success === 'function') {
                                success = a1.success;
                            }
                            if (typeof a1.fail === 'function') {
                                fail = a1.fail;
                            }
                            if (typeof a1.error === 'function') {
                                error = a1.error;
                            }
                            if (typeof a1.complete === 'function') {
                                complete = a1.complete;
                            }
                        }
                    }
                    var action = isForm ? form.action.substr(form.action.lastIndexOf('/') + 1) : form;
                    var data;
                    if (isForm) {
                        data = form2str(form);
                    } else {
                        if (a1 && a1.data) {
                            data = obj2str(a1.data);
                        }
                    }
                    data += '&{0}=' + action;
                    if (data[0] === '&') {
                        data = data.substr(1);
                    }
                    var method = isForm ? form.method : a1.method;
                    var isPost = method === 'post';
                    var url = location.href.split('?')[0];
                    if (isPost) {
                        http.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                    } else {
                        url += '?' + data;
                    }
                    var http = new XMLHttpRequest();
                    http.open(method, url, true);
                    http.onreadystatechange = function (e) {
                        if (this.readyState !== 4) {
                            return;
                        }
                        if (this.status !== 200) {
                            if (error) {
                                error(this.status, this.statusText);
                            }
                            return;
                        }
                        var r = JSON.parse(this.responseText);
                        ((r.ok ? success : fail) || function () { })(r.result);
                        if (complete) {
                            complete(r);
                        }
                    }
                    http.send(isPost ? data : null);
                }
            };
        }).call(window);".Replace('\'', '\"');
        #endregion

        public AjaxPage() : this("ajax") { }

        public AjaxPage(string ajax) : base() {
            ajaxKey = ajax;
            Load += WebHubPage_Load;
        }

        protected abstract Func<object, string> Serializer { get; }

        private string ajaxKey;

        protected virtual bool Initiate() => true;

        void WebHubPage_Load(object sender, EventArgs e) {
            if (IsPostBack) {
                return;
            }
            for (var i = 0; i < this.Controls.Count; i++) {
                var body = this.Controls[i] as LiteralControl;
                if (body?.Text.Contains("<body>") == true) {
                    body.Text = body.Text.Replace("<body>", $@"<body>
    <script type='text/javascript'>{SCRIPT.Replace("{0}", this.ajaxKey)}
    </script>");
                    break;
                }
            }
            try {
                if (Initiate()) {
                    return;
                }
            } catch {
            }
            OnError(EventArgs.Empty);
        }

        public override void ProcessRequest(HttpContext context) {
            if (string.IsNullOrWhiteSpace(ajaxKey) || string.IsNullOrWhiteSpace(context.Request[ajaxKey])) {
                base.ProcessRequest(context);
                return;
            }
            bool ok;
            var result = tryAjax(out ok);
            try {
                var json = Serializer?.Invoke(new { ok, result });
                if (string.IsNullOrWhiteSpace(json)) {
                    return;
                }
                context.Response.ContentType = "application/json";
                context.Response.Write(json);
            } catch {
            }
        }

        private object tryAjax(out bool ok) {
            var context = HttpContext.Current;
            var name = context.Request[ajaxKey].Trim();
            var m = GetType().BaseType.GetMethods((System.Reflection.BindingFlags)63).FirstOrDefault(mi => mi.Name == name);
            if (m == null || m.GetParameters().Length > 0) {
                ok = false;
                return $@"Valid server method ""{name}"" does not exist.";
            }
            var vs = m.GetCustomAttributes(typeof(HttpValidationAttribute), true) as HttpValidationAttribute[];
            foreach (var v in vs) {
                try {
                    var r = v.Validate();
                    if (r != null) {
                        ok = false;
                        return r;
                    }
                } catch (Exception e) {
                    ok = false;
                    return e.Message;
                }
            }
            try {
                ok = true;
                return m.Invoke(this, null);
            } catch (Exception e) {
                ok = false;
                return e.Message;
            }
        }
    }
    public abstract class HttpValidationAttribute : Attribute {
        public abstract string Validate();
    }
}
