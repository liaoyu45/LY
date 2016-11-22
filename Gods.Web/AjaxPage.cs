using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace Gods.Web {
    public abstract class AjaxPage : Page {

        public AjaxPage() : this("ajax", null) { }

        public AjaxPage(string ajax, string mime) : base() {
            ajaxKey = ajax;
            mimeKey = mime;
            Load += WebHubPage_Load;
        }

        void WebHubPage_Load(object sender, EventArgs e) {
            if (IsPostBack) {
                return;
            }
            try {
                if (Initiate()) {
                    return;
                }
            } catch {
            }
            OnError(EventArgs.Empty);
        }

        protected abstract Func<object, string> Serializer { get; }

        protected virtual bool Initiate() => true;

        private string ajaxKey;
        private string mimeKey;

        public override void ProcessRequest(HttpContext context) {
            if (string.IsNullOrWhiteSpace(ajaxKey) || string.IsNullOrWhiteSpace(context.Request[ajaxKey])) {
                base.ProcessRequest(context);
                return;
            }
            bool ok;
            var result = tryAjax(out ok);
            if (ok) {
                if (result is byte[]) {
                    if (!string.IsNullOrWhiteSpace(mimeKey)) {
                        context.Response.ContentType = MimeMapping.GetMimeMapping('.' + context.Request[mimeKey]);
                    }
                    context.Response.BinaryWrite(result as byte[]);
                    return;
                }
            }
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
            var m = GetType().GetMethods((System.Reflection.BindingFlags)256 - 1).FirstOrDefault(mi => mi.Name == name);
            if (m?.GetParameters().Length > 0) {
                ok = false;
                return $"Valid server method {{{name}}} does not exist.";
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
    }
    public abstract class HttpValidationAttribute : Attribute {
        protected HttpContext Context { get; } = HttpContext.Current;
        protected HttpResponse Response { get; } = HttpContext.Current?.Response;
        protected HttpRequest Request { get; } = HttpContext.Current?.Request;
        protected HttpSessionState Session { get; } = HttpContext.Current?.Session;
        public abstract string Validate();
    }
}
