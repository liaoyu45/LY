/// <reference path="god.js" />
god.f12js = function (obj, name, page) {
    var panel = god.f12js.fromTemplate("panel");
    var container = panel.querySelector("[data-container]");
    var allPs = [];
    var removed = [];
    function addItems() {
        container.innerHTML = "";
        var start = parseInt(panel.querySelector("input[type=range][data-start]").value);
        var end = start + parseInt(panel.querySelector("input[type=range][data-size]").value);
        var initiatePs = [];
        var hasPs = [];
        for (var i = 0; i < allPs.length; i++) {
            if (allPs[i] in obj) {
                hasPs.push(allPs[i]);
            }
        }
        for (var i = start; i < end; i++) {
            initiatePs.push(hasPs[i]);
        }
        for (var i in initiatePs) {
            var p = initiatePs[i];
            addItem(p);
        }
    }
    function getLength() {
        var i = 0;
        allPs.forEach(function (v) {
            if (v in obj) {
                i++;
            }
        });
        return i;
    }
    function resetPager() {
        var size = panel.querySelector("input[type=range][data-size]");
        var start = panel.querySelector("input[type=range][data-start]");
        var i = getLength();
        var max = Math.min(page, i);
        if (size.value > max) {
            size.value = max;
        }
        size.max = max;
        start.max = i - parseInt(size.value);
    }
    function initiate() {
        var size = panel.querySelector("input[type=range][data-size]");
        var start = panel.querySelector("input[type=range][data-start]");
        size.max = size.value = page;
        size.min = 1;
        start.min = 0;
        start.value = 0;
        start.max = allPs.length - page;
        var length = god.f12js.layers.length;
        panel.style.zIndex = length;
        if (length) {
            var s = getComputedStyle(god.f12js.layers[length - 1]);
            panel.style.top = parseFloat(s.top) + top + "px";
            panel.style.left = parseFloat(s.left) + top + "px";
        } else {
            panel.style.top = 0 + "px";
            panel.style.left = 0 + "px";
        }
        if (!god.f12js.layers.length) {
            window.addEventListener("keydown", keydownHide);
        }
        start.addEventListener("change", addItems);
        size.addEventListener("change", function () {
            start.max = getLength() - parseInt(size.value);
            start.value = 0;
            addItems();
        });
        panel.querySelector("[data-close]").addEventListener("click", function () {
            for (var i = god.f12js.layers.length - 1; i >= 0; i--) {
                var isCurrent = god.f12js.layers[i] === panel;
                removeLast();
                if (isCurrent) {
                    return;
                }
            }
        });
    }
    function addItem(i) {
        if (!(i in obj)) {
            return;
        }
        var v = obj[i];
        var t = typeof v;
        var isValueType = ["number", "boolean", "string"].indexOf(t) > -1;
        var item = god.f12js.fromTemplate("item", "tbody");
        item.dataset.f12item = true;
        container.appendChild(item);
        item.querySelector("[data-type]").innerHTML = t;
        var dataName = item.querySelector("[data-name]");
        dataName.innerHTML = i;
        dataName.dataset.name = i;
        dataName.addEventListener("click", removeItem);
        var vE = item.querySelector("[data-orignal]");
        vE.innerText = isValueType ? v : "(click to show more)";
        vE.dataset.name = i;
        if (isValueType) {
            vE.dataset.orignal = v;
            vE.dataset.type = t;
        }
        vE.addEventListener("click", showActions);
    }
    function showActions() {
        if (hasLocked()) {
            return;
        }
        if (this.dataset.orignal) {
            var self = this;
            var name = this.dataset.name;
            var orignal = this.dataset.orignal;
            var modify = god.f12js.fromTemplate("modify");
            modify.dataset.modifying = true;
            var title  = modify.querySelector("[data-title]");
            title.innerHTML = "modify: " + name;
            modify.querySelector("[data-orignal]").innerText = orignal;
            god.window.dragable(modify, title);
            title.onselectstart = function () {
                return false;
            };
            panel.firstElementChild.appendChild(modify);
            var dataNew = modify.querySelector("[data-new]");
            dataNew.focus();
            modify.querySelector("[data-modify]").addEventListener("click", function () {
                var nv = dataNew.value;
                var ot = typeof orignal;
                if (ot === "boolean") {
                    nv = !!nv;
                }
                if (ot === "number") {
                    if (isNaN(nv)) {
                        return;
                    } else {
                        nv = parseFloat(nv);
                    }
                }
                obj[name] = nv;
                if (ot === "string") {
                    nv = nv.replace("\n", "");
                }
                till_f12item(self).querySelector("[data-orignal]").innerText = nv;
            });
            modify.querySelector("[data-reset]").addEventListener("click", function () {
                obj[name] = orignal;
                till_f12item(self).querySelector("[data-orignal]").innerText = orignal;
            });
            modify.querySelector("[data-close]").addEventListener("click", function () {
                panel.firstElementChild.removeChild(modify);
            });
        } else {
            god.f12js(obj[this.dataset.name], this.dataset.name, page);
        }
    }
    function till_f12item(self) {
        if (self.parentElement.dataset.f12item) {
            return self.parentElement;
        } else {
            return till_f12item(self.parentElement);
        }
    }
    function removeItem() {
        if (hasLocked()) {
            return;
        }
        var p = till_f12item(this);
        p.parentElement.removeChild(p);
        var name = this.dataset.name;
        removed[name] = obj[name];
        delete obj[name];
        var r = god.f12js.fromTemplate("removedItem");
        if ("input" in r) {
            r.value = name;
        } else {
            r.querySelector("input").value = name;
        }
        r.dataset.name = name;
        resetPager();
        addItems();
        r.addEventListener("click", function () {
            var sibling = r.nextElementSibling || r.previousElementSibling;
            if (sibling) {
                sibling.querySelector("input").focus();
            }
            this.parentElement.removeChild(this);
            obj[this.dataset.name] = removed[this.dataset.name];
            resetPager();
            addItems();
        });
        panel.querySelector("[data-removed]").appendChild(r);
    }
    function hasLocked() {
        if (!god.f12js.layers.length) {
            return true;
        }
        var last = god.f12js.layers[god.f12js.layers.length - 1];
        var m = last.querySelector("[data-modifying]");
        if (m) {
            m.querySelector("[data-new]").focus();
            return true;
        }
        if (last !== panel) {
            last.querySelector("[data-tab0]").focus();
            return true;
        }
        return false;
    }
    function removeLast() {
        var index = god.f12js.layers.length - 1;
        var last = god.f12js.layers[index];
        var m = last.querySelector("[data-modifying]");
        if (m) {
            m.parentElement.removeChild(m);
            return;
        }
        document.body.removeChild(last);
        god.f12js.layers.splice(index, 1);
        if (!god.f12js.layers.length) {
            window.removeEventListener("keydown", keydownHide);
        }
    }
    function keydownHide(e) {
        if (e.keyCode !== 27) {
            return;
        }
        removeLast();
    }
    function prepare() {
        for (var i in obj) {
            allPs.push(i);
        }
        return allPs.length;
    }
    function titleText() {
        panel.dataset.title = name;
        var title = panel.querySelector("[data-title]");
        if (!god.window.browser.versions.mobile) {
            god.window.dragable(panel, title);
        }
        title.innerHTML = "Debugger: ";
        title.onselectstart = function () {
            return false;
        };
        if (god.f12js.layers.length) {
            title.innerHTML += god.f12js.layers[0].dataset.title;
            for (var i = 1; i < god.f12js.layers.length; i++) {
                title.innerHTML += '.' + god.f12js.layers[i].dataset.title;
            }
        } else {
            title.innerHTML += name;
        }
    }
    function addPanel() {
        god.f12js.layers.push(panel);
        document.body.appendChild(panel);
    }
    function main() {
        if (!prepare()) {
            return;
        }
        initiate();
        addItems();
        addPanel();
        titleText();
    }
    main();
};
god.f12js.fromTemplate = function (id, parent) {
    var tt = document.getElementById(id);
    var tttt = document.createElement(parent || "div");
    tttt.innerHTML = tt.textContent;
    return tttt.firstElementChild;

}
god.f12js.layers = [];