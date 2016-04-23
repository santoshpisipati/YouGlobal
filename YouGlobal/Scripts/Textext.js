﻿/**
 * jQuery TextExt Plugin
 * http://alexgorbatchev.com/textext
 *
 * @version 1.1.0
 * @copyright Copyright (C) 2011 Alex Gorbatchev. All rights reserved.
 * @license MIT License
 */
(function (a, b) {
    function c() { }
    function d() { }
    function e() { }
    function x(a, b) {
        typeof b == "string" && (b = b.split("."));
        var c = b.join(".").replace(/\.(\w)/g, function (a, b) {
            return b.toUpperCase()
        }),
            d = b.shift(),
            e;
        return typeof (e = a[c]) != h ? e = e : typeof (e = a[d]) != h && b.length > 0 && (e = x(e, b)), e
    }
    function y() {
        function e(a, d) {
            c.bind(a, function () {
                return d.apply(b, arguments)
            })
        }
        var a = g.apply(arguments),
            b = this,
            c = a.length === 1 ? b : a.shift(),
            d;
        a = a[0] || {};
        for (d in a) e(d, a[d])
    }
    function z(a, b) {
        return {
            input: a,
            form: b
        }
    }
    var f = (JSON || {}).stringify,
        g = Array.prototype.slice,
        h = "undefined",
        i = "item.manager",
        j = "plugins",
        k = "ext",
        l = "html.wrap",
        m = "html.hidden",
        n = "keys",
        o = "preInvalidate",
        q = "postInvalidate",
        r = "getFormData",
        s = "setFormData",
        t = "setInputData",
        u = "postInit",
        v = "ready",
        w = {
            itemManager: d,
            plugins: [],
            ext: {},
            html: {
                wrap: '<div class="text-core"><div class="text-wrap"/></div>',
                hidden: '<input type="hidden" />'
            },
            keys: {
                8: "backspace",
                9: "tab",
                13: "enter!",
                27: "escape!",
                37: "left",
                38: "up!",
                39: "right",
                40: "down!",
                46: "delete",
                108: "numpadEnter"
            }
        };
    if (!f) throw new Error("JSON.stringify() not found");
    p = d.prototype, p.init = function (a) { }, p.filter = function (a, b) {
        var c = [],
            d, e;
        for (d = 0; d < a.length; d++) e = a[d], this.itemContains(e, b) && c.push(e);
        return c
    }, p.itemContains = function (a, b) {
        return this.itemToString(a).toLowerCase().indexOf(b.toLowerCase()) == 0
    }, p.stringToItem = function (a) {
        return a
    }, p.itemToString = function (a) {
        return a
    }, p.compareItems = function (a, b) {
        return a == b
    }, p = c.prototype, p.init = function (b, c) {
        var d = this,
            e, f, g;
        d._defaults = a.extend({}, w), d._opts = c || {}, d._plugins = {}, d._itemManager = f = new (d.opts(i)), b = a(b), g = a(d.opts(l)), e = a(d.opts(m)), b.wrap(g).keydown(function (a) {
            return d.onKeyDown(a)
        }).keyup(function (a) {
            return d.onKeyUp(a)
        }).data("textext", d), a(d).data({
            hiddenInput: e,
            wrapElement: b.parents(".text-wrap").first(),
            input: b
        }), e.attr("name", b.attr("name")), b.attr("name", null), e.insertAfter(b), a.extend(!0, f, d.opts(k + ".item.manager")), a.extend(!0, d, d.opts(k + ".*"), d.opts(k + ".core")), d.originalWidth = b.outerWidth(), d.invalidateBounds(), f.init(d), d.initPatches(), d.initPlugins(d.opts(j), a.fn.textext.plugins), d.on({
            setFormData: d.onSetFormData,
            getFormData: d.onGetFormData,
            setInputData: d.onSetInputData,
            anyKeyUp: d.onAnyKeyUp
        }), d.trigger(u), d.trigger(v), d.getFormData(0)
    }, p.initPatches = function () {
        var b = [],
            c = a.fn.textext.patches,
            d;
        for (d in c) b.push(d);
        this.initPlugins(b, c)
    }, p.initPlugins = function (b, c) {
        var d = this,
            e, f, g, h = [],
            i;
        typeof b == "string" && (b = b.split(/\s*,\s*|\s+/g));
        for (i = 0; i < b.length; i++) f = b[i], g = c[f], g && (d._plugins[f] = g = new g, h.push(g), a.extend(!0, g, d.opts(k + ".*"), d.opts(k + "." + f)));
        h.sort(function (a, b) {
            return a = a.initPriority(), b = b.initPriority(), a === b ? 0 : a < b ? 1 : -1
        });
        for (i = 0; i < h.length; i++) h[i].init(d)
    }, p.hasPlugin = function (a) {
        return !!this._plugins[a]
    }, p.on = y, p.bind = function (a, b) {
        this.input().bind(a, b)
    }, p.trigger = function () {
        var a = arguments;
        this.input().trigger(a[0], g.call(a, 1))
    }, p.itemManager = function () {
        return this._itemManager
    }, p.input = function () {
        return a(this).data("input")
    }, p.opts = function (a) {
        var b = x(this._opts, a);
        return typeof b == "undefined" ? x(this._defaults, a) : b
    }, p.wrapElement = function () {
        return a(this).data("wrapElement")
    }, p.invalidateBounds = function () {
        var a = this,
            b = a.input(),
            c = a.wrapElement(),
            d = c.parent(),
            e = a.originalWidth,
            f;
        a.trigger(o), f = b.outerHeight(), b.width(e), c.width(e).height(f), d.height(f), a.trigger(q)
    }, p.focusInput = function () {
        this.input()[0].focus()
    }, p.serializeData = f, p.hiddenInput = function (b) {
        return a(this).data("hiddenInput")
    }, p.getWeightedEventResponse = function (a, b) {
        var c = this,
            d = {},
            e = 0;
        c.trigger(a, d, b);
        for (var f in d) e = Math.max(e, f);
        return d[e]
    }, p.getFormData = function (a) {
        var b = this,
            c = b.getWeightedEventResponse(r, a);
        b.trigger(s, c.form), b.trigger(t, c.input)
    }, p.onAnyKeyUp = function (a, b) {
        this.getFormData(b)
    }, p.onSetInputData = function (a, b) {
        this.input().val(b)
    }, p.onSetFormData = function (a, b) {
        var c = this;
        c.hiddenInput().val(c.serializeData(b))
    }, p.onGetFormData = function (a, b) {
        var c = this.input().val();
        b[0] = z(c, c)
    }, a(["Down", "Up"]).each(function () {
        var a = this.toString();
        p["onKey" + a] = function (b) {
            var c = this,
                d = c.opts(n)[b.keyCode],
                e = !0;
            return d && (e = d.substr(-1) != "!", d = d.replace("!", ""), c.trigger(d + "Key" + a), a == "Up" && c._lastKeyDown == b.keyCode && (c._lastKeyDown = null, c.trigger(d + "KeyPress")), a == "Down" && (c._lastKeyDown = b.keyCode)), c.trigger("anyKey" + a, b.keyCode), e
        }
    }), p = e.prototype, p.on = y, p.formDataObject = z, p.init = function (a) {
        throw new Error("Not implemented")
    }, p.baseInit = function (b, c) {
        var d = this;
        b._defaults = a.extend(!0, b._defaults, c), d._core = b, d._timers = {}
    }, p.startTimer = function (a, b, c) {
        var d = this;
        d.stopTimer(a), d._timers[a] = setTimeout(function () {
            delete d._timers[a], c.apply(d)
        }, b * 1e3)
    }, p.stopTimer = function (a) {
        clearTimeout(this._timers[a])
    }, p.core = function () {
        return this._core
    }, p.opts = function (a) {
        return this.core().opts(a)
    }, p.itemManager = function () {
        return this.core().itemManager()
    }, p.input = function () {
        return this.core().input()
    }, p.val = function (a) {
        var b = this.input();
        if (typeof a === h) return b.val();
        b.val(a)
    }, p.trigger = function () {
        var a = this.core();
        a.trigger.apply(a, arguments)
    }, p.bind = function (a, b) {
        this.core().bind(a, b)
    }, p.initPriority = function () {
        return 0
    };
    var A = !1,
        B = a.fn.textext = function (b) {
            var d;
            return !A && (d = a.fn.textext.css) != null && (a("head").append("<style>" + d + "</style>"), A = !0), this.map(function () {
                var d = a(this);
                if (b == null) return d.data("textext");
                var e = new c;
                return e.init(d, b), d.data("textext", e), e.input()[0]
            })
        };
    B.addPlugin = function (a, b) {
        B.plugins[a] = b, b.prototype = new B.TextExtPlugin
    }, B.addPatch = function (a, b) {
        B.patches[a] = b, b.prototype = new B.TextExtPlugin
    }, B.TextExt = c, B.TextExtPlugin = e, B.ItemManager = d, B.plugins = {}, B.patches = {}
})(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtIE9Patches = b, a.fn.textext.addPatch("ie9", b);
    var c = b.prototype;
    c.init = function (a) {
        if (navigator.userAgent.indexOf("MSIE 9") == -1) return;
        var b = this;
        a.on({
            postInvalidate: b.onPostInvalidate
        })
    }, c.onPostInvalidate = function () {
        var a = this,
            b = a.input(),
            c = b.val();
        b.val(Math.random()), b.val(c)
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtAjax = b, a.fn.textext.addPlugin("ajax", b);
    var c = b.prototype,
        d = "ajax.data.callback",
        e = "ajax.cache.results",
        f = "ajax.loading.delay",
        g = "ajax.loading.message",
        h = "ajax.type.delay",
        i = "setSuggestions",
        j = "showDropdown",
        k = "loading",
        l = {
            ajax: {
                typeDelay: .5,
                loadingMessage: "Loading...",
                loadingDelay: .5,
                cacheResults: !1,
                dataCallback: null
            }
        };
    c.init = function (a) {
        var b = this;
        b.baseInit(a, l), b.on({
            getSuggestions: b.onGetSuggestions
        }), b._suggestions = null
    }, c.load = function (b) {
        var c = this,
            e = c.opts(d) ||
            function (a) {
                return {
                    q: a
                }
            },
            f;
        f = a.extend(!0, {
            data: e(b),
            success: function (a) {
                c.onComplete(a, b)
            },
            error: function (a, c) {
                console.error(c, b)
            }
        }, c.opts("ajax")), a.ajax(f)
    }, c.onComplete = function (a, b) {
        var c = this,
            d = a;
        c.dontShowLoading(), c.opts(e) == !0 && (c._suggestions = a, d = c.itemManager().filter(a, b)), c.trigger(i, {
            result: d
        })
    }, c.dontShowLoading = function () {
        this.stopTimer(k)
    }, c.showLoading = function () {
        var a = this;
        a.dontShowLoading(), a.startTimer(k, a.opts(f), function () {
            a.trigger(j, function (b) {
                b.clearItems();
                var c = b.addDropdownItem(a.opts(g));
                c.addClass("text-loading")
            })
        })
    }, c.onGetSuggestions = function (a, b) {
        var c = this,
            d = c._suggestions,
            f = (b || {}).query || "";
        if (d && c.opts(e) === !0) return c.onComplete(d, f);
        c.startTimer("ajax", c.opts(h), function () {
            c.showLoading(), c.load(f)
        })
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtArrow = b, a.fn.textext.addPlugin("arrow", b);
    var c = b.prototype,
        d = "html.arrow",
        e = {
            html: {
                arrow: '<div class="text-arrow"/>'
            }
        };
    c.init = function (b) {
        var c = this,
            f;
        c.baseInit(b, e), c._arrow = f = a(c.opts(d)), c.core().wrapElement().append(f), f.bind("click", function (a) {
            c.onArrowClick(a)
        })
    }, c.onArrowClick = function (a) {
        this.trigger("toggleDropdown"), this.core().focusInput()
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtAutocomplete = b, a.fn.textext.addPlugin("autocomplete", b);
    var c = b.prototype,
        d = ".",
        e = "text-selected",
        f = d + e,
        g = "text-suggestion",
        h = d + g,
        i = "autocomplete.enabled",
        j = "autocomplete.dropdown.position",
        k = "autocomplete.dropdown.maxHeight",
        l = "autocomplete.render",
        m = "html.dropdown",
        n = "html.suggestion",
        o = "hideDropdown",
        p = "showDropdown",
        q = "getSuggestions",
        r = "getFormData",
        s = "toggleDropdown",
        t = "above",
        u = "below",
        v = {
            autocomplete: {
                enabled: !0,
                dropdown: {
                    position: u,
                    maxHeight: "100px"
                }
            },
            html: {
                dropdown: '<div class="text-dropdown"><div class="text-list"/></div>',
                suggestion: '<div class="text-suggestion"><span class="text-label"/></div>'
            }
        };
    c.init = function (b) {
        var c = this;
        c.baseInit(b, v);
        var d = c.input(),
            e;
        c.opts(i) === !0 && (c.on({
            blur: c.onBlur,
            anyKeyUp: c.onAnyKeyUp,
            deleteKeyUp: c.onAnyKeyUp,
            backspaceKeyPress: c.onBackspaceKeyPress,
            enterKeyPress: c.onEnterKeyPress,
            escapeKeyPress: c.onEscapeKeyPress,
            setSuggestions: c.onSetSuggestions,
            showDropdown: c.onShowDropdown,
            hideDropdown: c.onHideDropdown,
            toggleDropdown: c.onToggleDropdown,
            postInvalidate: c.positionDropdown,
            getFormData: c.onGetFormData,
            downKeyDown: c.onDownKeyDown,
            upKeyDown: c.onUpKeyDown
        }), e = a(c.opts(m)), e.insertAfter(d), c.on(e, {
            mouseover: c.onMouseOver,
            click: c.onClick
        }), e.css("maxHeight", c.opts(k)).addClass("text-position-" + c.opts(j)), a(c).data("container", e), c.positionDropdown())
    }, c.containerElement = function () {
        return a(this).data("container")
    }, c.onMouseOver = function (b) {
        var c = this,
            d = a(b.target);
        d.is(h) && (c.clearSelected(), d.addClass(e))
    }, c.onClick = function (b) {
        var c = this,
            d = a(b.target);
        d.is(h) && c.selectFromDropdown()
    }, c.onBlur = function (a) {
        var b = this;
        b.isDropdownVisible() && setTimeout(function () {
            b.trigger(o)
        }, 100)
    }, c.onBackspaceKeyPress = function (a) {
        var b = this,
            c = b.val().length > 0;
        (c || b.isDropdownVisible()) && b.getSuggestions()
    }, c.onAnyKeyUp = function (a, b) {
        var c = this,
            d = c.opts("keys." + b) != null;
        c.val().length > 0 && !d && c.getSuggestions()
    }, c.onDownKeyDown = function (a) {
        var b = this;
        b.isDropdownVisible() ? b.toggleNextSuggestion() : b.getSuggestions()
    }, c.onUpKeyDown = function (a) {
        this.togglePreviousSuggestion()
    }, c.onEnterKeyPress = function (a) {
        var b = this;
        b.isDropdownVisible() && b.selectFromDropdown()
    }, c.onEscapeKeyPress = function (a) {
        var b = this;
        b.isDropdownVisible() && b.trigger(o)
    }, c.positionDropdown = function () {
        var a = this,
            b = a.containerElement(),
            c = a.opts(j),
            d = a.core().wrapElement().outerHeight(),
            e = {};
        e[c === t ? "bottom" : "top"] = d + "px", b.css(e)
    }, c.suggestionElements = function () {
        return this.containerElement().find(h)
    }, c.setSelectedSuggestion = function (b) {
        if (!b) return;
        var c = this,
            d = c.suggestionElements(),
            f = d.first(),
            h, i;
        c.clearSelected();
        for (i = 0; i < d.length; i++) {
            h = a(d[i]);
            if (c.itemManager().compareItems(h.data(g), b)) {
                f = h.addClass(e);
                break
            }
        }
        f.addClass(e), c.scrollSuggestionIntoView(f)
    }, c.selectedSuggestionElement = function () {
        return this.suggestionElements().filter(f).first()
    }, c.isDropdownVisible = function () {
        return this.containerElement().is(":visible") === !0
    }, c.onGetFormData = function (a, b, c) {
        var d = this,
            e = d.val(),
            f = e,
            g = e;
        b[100] = d.formDataObject(f, g)
    }, c.initPriority = function () {
        return 200
    }, c.onHideDropdown = function (a) {
        this.hideDropdown()
    }, c.onToggleDropdown = function (a) {
        var b = this;
        b.trigger(b.containerElement().is(":visible") ? o : p)
    }, c.onShowDropdown = function (b, c) {
        var d = this,
            e = d.selectedSuggestionElement().data(g),
            f = d._suggestions;
        if (!f) return d.trigger(q);
        a.isFunction(c) ? c(d) : (d.renderSuggestions(d._suggestions), d.toggleNextSuggestion()), d.showDropdown(d.containerElement()), d.setSelectedSuggestion(e)
    }, c.onSetSuggestions = function (a, b) {
        var c = this,
            d = c._suggestions = b.result;
        b.showHideDropdown !== !1 && c.trigger(d === null || d.length === 0 ? o : p)
    }, c.getSuggestions = function () {
        var a = this,
            b = a.val();
        if (a._previousInputValue == b) return;
        b == "" && (current = null), a._previousInputValue = b, a.trigger(q, {
            query: b
        })
    }, c.clearItems = function () {
        this.containerElement().find(".text-list").children().remove()
    }, c.renderSuggestions = function (b) {
        var c = this;
        c.clearItems(), a.each(b || [], function (a, b) {
            c.addSuggestion(b)
        })
    }, c.showDropdown = function () {
        this.containerElement().show()
    }, c.hideDropdown = function () {
        var a = this,
            b = a.containerElement();
        a._previousInputValue = null, b.hide()
    }, c.addSuggestion = function (a) {
        var b = this,
            c = b.opts(l),
            d = b.addDropdownItem(c ? c.call(b, a) : b.itemManager().itemToString(a));
        d.data(g, a)
    }, c.addDropdownItem = function (b) {
        var c = this,
            d = c.containerElement().find(".text-list"),
            e = a(c.opts(n));
        return e.find(".text-label").html(b), d.append(e), e
    }, c.clearSelected = function () {
        this.suggestionElements().removeClass(e)
    }, c.toggleNextSuggestion = function () {
        var a = this,
            b = a.selectedSuggestionElement(),
            c;
        b.length > 0 ? (c = b.next(), c.length > 0 && b.removeClass(e)) : c = a.suggestionElements().first(), c.addClass(e), a.scrollSuggestionIntoView(c)
    }, c.togglePreviousSuggestion = function () {
        var a = this,
            b = a.selectedSuggestionElement(),
            c = b.prev();
        if (c.length == 0) return;
        a.clearSelected(), c.addClass(e), a.scrollSuggestionIntoView(c)
    }, c.scrollSuggestionIntoView = function (a) {
        var b = a.outerHeight(),
            c = this.containerElement(),
            d = c.innerHeight(),
            e = c.scrollTop(),
            f = (a.position() || {}).top,
            g = null,
            h = parseInt(c.css("paddingTop"));
        if (f == null) return;
        f + b > d && (g = f + e + b - d + h), f < 0 && (g = f + e - h), g != null && c.scrollTop(g)
    }, c.selectFromDropdown = function () {
        var a = this,
            b = a.selectedSuggestionElement().data(g);
        b && (a.val(a.itemManager().itemToString(b)), a.core().getFormData()), a.trigger(o)
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtFilter = b, a.fn.textext.addPlugin("filter", b);
    var c = b.prototype,
        d = "filter.enabled",
        e = "filter.items",
        f = {
            filter: {
                enabled: !0,
                items: null
            }
        };
    c.init = function (a) {
        var b = this;
        b.baseInit(a, f), b.on({
            getFormData: b.onGetFormData,
            isTagAllowed: b.onIsTagAllowed,
            setSuggestions: b.onSetSuggestions
        }), b._suggestions = null
    }, c.onGetFormData = function (a, b, c) {
        var d = this,
            e = d.val(),
            f = e,
            g = "";
        d.core().hasPlugin("tags") || (d.isValueAllowed(f) && (g = e), b[300] = d.formDataObject(f, g))
    }, c.isValueAllowed = function (a) {
        var b = this,
            c = b.opts("filterItems") || b._suggestions || [],
            e = b.itemManager(),
            f = !b.opts(d),
            g;
        for (g = 0; g < c.length && !f; g++) e.compareItems(a, c[g]) && (f = !0);
        return f
    }, c.onIsTagAllowed = function (a, b) {
        b.result = this.isValueAllowed(b.tag)
    }, c.onSetSuggestions = function (a, b) {
        this._suggestions = b.result
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtFocus = b, a.fn.textext.addPlugin("focus", b);
    var c = b.prototype,
        d = "html.focus",
        e = {
            html: {
                focus: '<div class="text-focus"/>'
            }
        };
    c.init = function (a) {
        var b = this;
        b.baseInit(a, e), b.core().wrapElement().append(b.opts(d)), b.on({
            blur: b.onBlur,
            focus: b.onFocus
        }), b._timeoutId = 0
    }, c.onBlur = function (a) {
        var b = this;
        clearTimeout(b._timeoutId), b._timeoutId = setTimeout(function () {
            b.getFocus().hide()
        }, 100)
    }, c.onFocus = function (a) {
        var b = this;
        clearTimeout(b._timeoutId), b.getFocus().show()
    }, c.getFocus = function () {
        return this.core().wrapElement().find(".text-focus")
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtPrompt = b, a.fn.textext.addPlugin("prompt", b);
    var c = b.prototype,
        d = "text-hide-prompt",
        e = "prompt",
        f = "html.prompt",
        g = {
            prompt: "Awaiting input...",
            html: {
                prompt: '<div class="text-prompt"/>'
            }
        };
    c.init = function (b) {
        var c = this,
            d;
        c.baseInit(b, g), d = a(c.opts(f)), a(c).data("container", d), c.core().wrapElement().append(d), c.setPrompt(c.opts(e)), c.val().length > 0 && c.hidePrompt(), c.on({
            blur: c.onBlur,
            focus: c.onFocus,
            postInvalidate: c.onPostInvalidate,
            postInit: c.onPostInit
        })
    }, c.onPostInit = function (a) {
        this.invalidateBounds()
    }, c.onPostInvalidate = function (a) {
        this.invalidateBounds()
    }, c.invalidateBounds = function () {
        var a = this,
            b = a.input();
        a.containerElement().css({
            paddingLeft: b.css("paddingLeft"),
            paddingTop: b.css("paddingTop")
        })
    }, c.onBlur = function (a) {
        var b = this;
        b.startTimer("prompt", .1, function () {
            b.val().length === 0 && b.showPrompt()
        })
    }, c.showPrompt = function () {
        this.containerElement().removeClass(d)
    }, c.hidePrompt = function () {
        this.stopTimer("prompt"), this.containerElement().addClass(d)
    }, c.onFocus = function (a) {
        this.hidePrompt()
    }, c.setPrompt = function (a) {
        this.containerElement().text(a)
    }, c.containerElement = function () {
        return a(this).data("container")
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtSuggestions = b, a.fn.textext.addPlugin("suggestions", b);
    var c = b.prototype,
        d = "suggestions",
        e = {
            suggestions: null
        };
    c.init = function (a) {
        var b = this;
        b.baseInit(a, e), b.on({
            getSuggestions: b.onGetSuggestions,
            postInit: b.onPostInit
        })
    }, c.setSuggestions = function (a, b) {
        this.trigger("setSuggestions", {
            result: a,
            showHideDropdown: b != !1
        })
    }, c.onPostInit = function (a) {
        var b = this;
        b.setSuggestions(b.opts(d), !1)
    }, c.onGetSuggestions = function (a, b) {
        var c = this,
            e = c.opts(d);
        e.sort(), c.setSuggestions(c.itemManager().filter(e, b.query))
    }
}(jQuery), function (a) {
    function b() { }
    a.fn.textext.TextExtTags = b, a.fn.textext.addPlugin("tags", b);
    var c = b.prototype,
        d = ".",
        e = "text-tags-on-top",
        f = d + e,
        g = "text-tag",
        h = d + g,
        i = "text-tags",
        j = d + i,
        k = "tags.enabled",
        l = "tags.items",
        m = "html.tag",
        n = "html.tags",
        o = "isTagAllowed",
        p = {
            tags: {
                enabled: !0,
                items: null
            },
            html: {
                tags: '<div class="text-tags"/>',
                tag: '<div class="text-tag"><div class="text-button"><span class="text-label"/><a class="text-remove"/></div></div>'
            }
        };
    c.init = function (b) {
        this.baseInit(b, p);
        var c = this,
            d = c.input(),
            e;
        c.opts(k) && (e = a(c.opts(n)), d.after(e), a(c).data("container", e), c.on({
            enterKeyPress: c.onEnterKeyPress,
            backspaceKeyDown: c.onBackspaceKeyDown,
            preInvalidate: c.onPreInvalidate,
            postInit: c.onPostInit,
            getFormData: c.onGetFormData
        }), c.on(e, {
            click: c.onClick,
            mousemove: c.onContainerMouseMove
        }), c.on(d, {
            mousemove: c.onInputMouseMove
        })), c._originalPadding = {
            left: parseInt(d.css("paddingLeft") || 0),
            top: parseInt(d.css("paddingTop") || 0)
        }, c._paddingBox = {
            left: 0,
            top: 0
        }, c.updateFormCache()
    }, c.containerElement = function () {
        return a(this).data("container")
    }, c.onPostInit = function (a) {
        var b = this;
        b.addTags(b.opts(l))
    }, c.onGetFormData = function (a, b, c) {
        var d = this,
            e = c === 13 ? "" : d.val(),
            f = d._formData;
        b[200] = d.formDataObject(e, f)
    }, c.initPriority = function () {
        return 100
    }, c.onInputMouseMove = function (a) {
        this.toggleZIndex(a)
    }, c.onContainerMouseMove = function (a) {
        this.toggleZIndex(a)
    }, c.onBackspaceKeyDown = function (a) {
        var b = this,
            c = b.tagElements().last();
        b.val().length == 0 && b.removeTag(c)
    }, c.onPreInvalidate = function (a) {
        var b = this,
            c = b.tagElements().last(),
            d = c.position();
        c.length > 0 ? d.left += c.innerWidth() : d = b._originalPadding, b._paddingBox = d, b.input().css({
            paddingLeft: d.left,
            paddingTop: d.top
        })
    }, c.onClick = function (b) {
        var c = this,
            d = a(b.target),
            e = 0;
        d.is(j) ? e = 1 : d.is(".text-remove") && (c.removeTag(d.parents(h + ":first")), e = 1), e && c.core().focusInput()
    }, c.onEnterKeyPress = function (a) {
        var b = this,
            c = b.val(),
            d = b.itemManager().stringToItem(c);
        b.isTagAllowed(d) && (b.addTags([d]), b.core().focusInput())
    }, c.updateFormCache = function () {
        var b = this,
            c = [];
        b.tagElements().each(function () {
            c.push(a(this).data(g))
        }), b._formData = c
    }, c.toggleZIndex = function (a) {
        var b = this,
            c = b.input().offset(),
            d = a.clientX - c.left,
            g = a.clientY - c.top,
            h = b._paddingBox,
            i = b.containerElement(),
            j = i.is(f),
            k = d > h.left && g > h.top;
        (!j && !k || j && k) && i[(j ? "remove" : "add") + "Class"](e)
    }, c.tagElements = function () {
        return this.containerElement().find(h)
    }, c.isTagAllowed = function (a) {
        var b = {
            tag: a,
            result: !0
        };
        return this.trigger(o, b), b.result === !0
    }, c.addTags = function (a) {
        if (!a || a.length == 0) return;
        var b = this,
            c = b.core(),
            d = b.containerElement(),
            e, f;
        for (e = 0; e < a.length; e++) f = a[e], f && b.isTagAllowed(f) && d.append(b.renderTag(f));
        b.updateFormCache(), c.getFormData(), c.invalidateBounds()
    }, c.getTagElement = function (b) {
        var c = this,
            d = c.tagElements(),
            e, f;
        for (e = 0; e < d.length, f = a(d[e]) ; e++) if (c.itemManager().compareItems(f.data(g), b)) return f
    }, c.removeTag = function (b) {
        var c = this,
            d = c.core(),
            e;
        b instanceof a ? (e = b, b = b.data(g)) : e = c.getTagElement(b), e.remove(), c.updateFormCache(), d.getFormData(), d.invalidateBounds()
    }, c.renderTag = function (b) {
        var c = this,
            d = a(c.opts(m));
        return d.find(".text-label").text(c.itemManager().itemToString(b)), d.data(g, b), d
    }
}(jQuery);
$.fn.textext.css = '.text-core{position:relative;background:#fff;}\n.text-core .text-wrap{position:absolute;}\n.text-core .text-wrap textarea,.text-core .text-wrap input{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;-webkit-border-radius:0;-moz-border-radius:0;border-radius:0;border:1px solid #CCC;outline:none;resize:none;position:absolute;z-index:1;background:none;overflow:hidden;margin:0;padding:4px;white-space:nowrap;font:13px "lucida grande",tahoma,verdana,arial,sans-serif;line-height:18px;height:auto}\n.text-core .text-wrap .text-arrow{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:absolute;top:0;right:0;width:22px;height:23px;background:url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAOAQMAAADHWqTrAAAAA3NCSVQICAjb4U/gAAAABlBMVEX///8yXJnt8Ns4AAAACXBIWXMAAAsSAAALEgHS3X78AAAAHHRFWHRTb2Z0d2FyZQBBZG9iZSBGaXJld29ya3MgQ1MzmNZGAwAAABpJREFUCJljYEAF/xsY6hkY7BgYZBgYOFBkADkdAmFDagYFAAAAAElFTkSuQmCC") 50% 50% no-repeat;cursor:pointer;z-index:2}\n.text-core .text-wrap .text-dropdown{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;padding:0;position:absolute;z-index:3;background:#fff;border:1px solid #CCC;width:100%;max-height:100px;padding:1px;font:13px "lucida grande",tahoma,verdana,arial,sans-serif;display:none;overflow-x:hidden;overflow-y:auto;}\n.text-core .text-wrap .text-dropdown.text-position-below{margin-top:1px}\n.text-core .text-wrap .text-dropdown.text-position-above{margin-bottom:1px}\n.text-core .text-wrap .text-dropdown .text-list .text-suggestion{padding:4px;cursor:pointer;}\n.text-core .text-wrap .text-dropdown .text-list .text-suggestion em{font-style:normal;text-decoration:underline}\n.text-core .text-wrap .text-dropdown .text-list .text-suggestion.text-selected{color:#fff;background:#6d84b4}\n.text-core .text-wrap .text-focus{-webkit-box-shadow:0 0 6px #6d84b4;-moz-box-shadow:0 0 6px #6d84b4;box-shadow:0 0 6px #6d84b4;position:absolute;width:100%;height:auto;display:none;}\n.text-core .text-wrap .text-focus.text-show-focus{display:block}\n.text-core .text-wrap .text-prompt{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:absolute;width:100%;height:auto;margin:1px 0 0 2px;font:13px "lucida grande",tahoma,verdana,arial,sans-serif;color:#c0c0c0;overflow:hidden;white-space:pre;}\n.text-core .text-wrap .text-prompt.text-hide-prompt{display:none}\n.text-core .text-wrap .text-tags{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:absolute;width:100%;height:auto;padding:3px 35px 3px 3px;cursor:text;}\n.text-core .text-wrap .text-tags.text-tags-on-top{z-index:2}\n.text-core .text-wrap .text-tags .text-tag{float:left;}\n.text-core .text-wrap .text-tags .text-tag .text-button{-webkit-border-radius:2px;-moz-border-radius:2px;border-radius:2px;-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;position:relative;float:left;border:1px solid #CCC;background:#FEFEFE;color:#000;padding:1px 22px 0 8px;margin:0 2px 2px 0;cursor:pointer;height:21px;font:13px "lucida grande",tahoma,verdana,arial,sans-serif;}\n.text-core .text-wrap .text-tags .text-tag .text-button a.text-remove{position:absolute;right:3px;top:4px;display:block;width:11px;height:11px;background:url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAsAAAAhCAYAAAAPm1F2AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAALEgAACxIB0t1+/AAAAB50RVh0U29mdHdhcmUAQWRvYmUgRmlyZXdvcmtzIENTNS4xqx9I6wAAAQ5JREFUOI2dlD0WwiAQhCc8L6HHgAPoASwtSYvX8BrQxtIyveYA8RppLO1jE+LwE8lzms2yH8MCj1QoaBzH+VuUYNYMS213UlvDRamtUbXb5ZyPHuDoxwGgip3ipfvGuGzPz+vZ/coDONdzFuYCO6ramQQG0DJIE1oPBBvM6e9LqaS2FwD7FWwnVoIAsOc2Xn1jDlyd8pfPBRVOBHA8cc/3yCmQqt0jcY4LuTyAF3pOYS6wI48LAm4MUrx5JthgSQJAt5LtNgAUgEMBBIC3AL2xgo58dEPfhE9wygef89FtCeC49UwltR1pQrK2qr9vNr7uRTCBF3pOYS6wI4/zdQ8MUpxPI9hgSQL0Xyio/QBt54DzsHQx6gAAAABJRU5ErkJggg==") 0 0 no-repeat;}\n.text-core .text-wrap .text-tags .text-tag .text-button a.text-remove:hover{background-position:0 -11px}\n.text-core .text-wrap .text-tags .text-tag .text-button a.text-remove:active{background-position:0 -22px}\n';
