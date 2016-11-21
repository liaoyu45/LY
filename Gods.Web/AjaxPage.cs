using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace WebThings {
    public abstract class AjaxPage : Page {

        public AjaxPage() : this("ajax") { }

        public AjaxPage(string ajax) : base() {
            ajaxKey = ajax;
            Load += WebHubPage_Load;
        }

        void WebHubPage_Load(object sender, EventArgs e) {
            if (IsPostBack) {
                return;
            }
            Gods.Him.Assert(Initiate, () => OnError(EventArgs.Empty));
        }

        protected abstract Func<object, string> Serializer { get; }

        protected virtual bool Initiate() => true;

        private string ajaxKey;

        public override void ProcessRequest(HttpContext context) {
            if (!string.IsNullOrWhiteSpace(ajaxKey)) {
                if (!string.IsNullOrWhiteSpace(context.Request[ajaxKey])) {
                    bool ok;
                    var result = tryAjax(out ok);
                    Serializer(new { ok, result });
                    return;
                }
            }
            base.ProcessRequest(context);
        }

        private object tryAjax(out bool ok) {
            var context = HttpContext.Current;
            var mName = (context.Request[ajaxKey] ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(mName)) {
                ok = false;
                return "try to call an ajax method, but didn't pass a method name.";
            }
            var m = GetType().GetMethod(mName);
            if (m == null) {
                ok = false;
                return $"no method like {mName} exists";
            }
            var validators = m.GetCustomAttributes(typeof(HttpValidationAttribute), true) as HttpValidationAttribute[];
            foreach (var v in validators) {
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

        public void WriteOk(HttpResponse r, object result) {
            Write(r, true, result);
        }
        public void WriteError(HttpResponse r, object result) {
            Write(r, false, result);
        }
        void Write(HttpResponse r, bool ok, object result) {
            if (r == null) {
                return;
            }
            if (ok) {
                if (result is byte[]) {
                    //r.ContentType = "application/json";
                    r.BinaryWrite(result as byte[]);
                    r.End();
                    return;
                }
            }
            r.ContentType = "application/json";
            r.Write(Serializer(new { ok, result }));
            //var response = JsonConvert.SerializeObject(new {
            //    ok = ok,
            //    result = result,
            //});
            //r.Write(response);
            r.End();
        }
    }
    public abstract class HttpValidationAttribute : Attribute {
        public string this[string key] {
            get { return HttpContext.Current.Request[key]; }
        }

        public string ErrorMessage { get; set; }

        public bool ThrowError { get; set; }

        public abstract string Validate();
    }
}
