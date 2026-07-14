window.easyBlazorBulma = {
    /**
     * Applies a style to the specified element.
     * @param {any} id The id of the element to apply the style to.
     * @param {any} key The name of the style to apply.
     * @param {any} value The value to assign to the style.
     */
    ApplyStyle: function (id, key, value) {
        var element = document.getElementById(id);

        if (element === null)
            return false;

        element.style[key] = value;
        return true;
    },

    /**
     * Navigates to the previous URL.
     */
    Back: function () {
        window.history.back();
    },

    /**
     * Navigates to the next URL.
     */
    Forward: function () {
        window.history.forward();
    },

    /**
     * Tests to see whether local storage is available in the current context.
     * @returns {boolean} true when local storage is available, otherwise false.
     */
    HasStorage: function () {
        try {
            window.localStorage.setItem('test', 'test');
            window.localStorage.removeItem('test');

            return true;
        }
        catch (e) {
            return false;
        }
    },

    /**
     * Checks to see whether the operating system is using a dark mode theme.
     * @returns {boolean} true when the operating system is in dark mode, otherwise false.
     */
    IsOsDarkMode: function () {
        try {
            return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        }
        catch (e) {
            return false;
        }
    },

    /**
     * Checks for and returns the local storage item with the specified name.
     * @param {string} name The name of the storage item to return.
     * @returns {string} The value of the requested local storage item when found, otherwise an empty string.
     */
    ReadStorage: function (name) {
        if (this.HasStorage()) {
            var value = window.localStorage.getItem(name);
            return value;
        }
        else {
            return '';
        }
    },

    /**
     * Enables or disables the stylesheet with the provided identity value if found.
     * @param {string} id The identity of the DOM element representing the stylesheet to update.
     * @param {boolean} enable Specifies which action to take. True to enable, false to disable.
     * @param {boolean} clearMediaText Specifies whether to clear the media property.
     * @returns {boolean} true when the operation is successful, false if the element cannot be found.
     */
    ToggleStyleSheet: function (id, enable, clearMediaText) {
        var element = document.getElementById(id);

        if (element === null)
            return false;

        element.disabled = enable === false;

        if (clearMediaText === true)
            element.media = '';

        return true;
    },

    /**
     * Creates or updates a local storage item with the provided values.
     * @param {string} name The text identifier for the storage item.
     * @param {string} value The value to store in the item.
     * @returns {boolean} true when the write operation was successful.
     */
    WriteStorage: function (name, value) {
        if (this.HasStorage()) {
            window.localStorage.setItem(name, value);
            return true;
        }

        return false;
    },

    /**
     * A map to track the loading state of scripts by their URL.
     * Each entry in the map corresponds to a script URL and its associated Promise that resolves when the script is loaded and the expected global symbol is available.
     */
    _scriptLoads: new Map(),

    /**
     * Ensures a script is loaded once and waits for a global symbol to become available.
     * @param {string} url The script URL to load.
     * @param {string} globalName The expected global symbol exposed by the script.
     * @param {number} timeoutMs Maximum wait time in milliseconds.
     * @returns {Promise<boolean>} true when the script/global is available; otherwise false.
     */
    EnsureScript: async function (url, globalName, timeoutMs) {
        if (!url || !globalName)
            return false;

        if (typeof window[globalName] !== "undefined")
            return true;

        var pending = this._scriptLoads.get(url);

        if (pending)
            return await pending;

        var timeout = timeoutMs ?? 1000;
        var self = this;

        var loadTask = new Promise(resolve => {
            var isDone = false;
            var startedAt = Date.now();

            // Wait for the global symbol to become available or for the timeout to expire.
            var finish = function (success) {
                if (isDone)
                    return;

                isDone = true;
                resolve(success);
            };

            // Check for the global symbol every 25ms until it is found or the timeout expires.
            var checkReady = function () {
                if (typeof window[globalName] !== "undefined") {
                    finish(true);
                    return;
                }

                if ((Date.now() - startedAt) >= timeout) {
                    console.error("Timed out waiting for script global.", {
                        url: url,
                        globalName: globalName,
                        timeoutMs: timeout
                    });
                    finish(false);
                    return;
                }

                window.setTimeout(checkReady, 25);
            };

            // If the script is already present, no need to add it again.
            var script = document.querySelector('script[src="' + url + '"]');

            // If not present, create a new script element and append it to the document head.
            if (!script) {
                script = document.createElement("script");
                script.src = url;
                script.async = true;
                script.defer = true;
                document.head.appendChild(script);
            }

            script.addEventListener("load", checkReady, { once: true });
            script.addEventListener("error", () => {
                console.error("Failed to load script.", {
                    url: url,
                    globalName: globalName
                });

                finish(false);
            }, { once: true });

            checkReady();
        }).then(success => {
            if (success === false)
                self._scriptLoads.delete(url);

            return success;
        });

        self._scriptLoads.set(url, loadTask);
        return await loadTask;
    },

    /**
     * Wrapper functions around Masonry.js instances.
     */
    Masonry: {
        _instances: new Map(),
        _observers: new Map(),

        /**
         * Ensures the Masonry script is loaded and available.
         * @param {number} timeoutMs Maximum wait time in milliseconds.
         * @returns {Promise<boolean>} true when Masonry is available; otherwise false.
         */
        EnsureScript: async function (timeoutMs) {
            if (typeof window.Masonry !== "undefined")
                return true;

            return await window.easyBlazorBulma.EnsureScript("_content/Easy.Blazor.Bulma/js/masonry.pkgd.min.js", "Masonry", timeoutMs ?? 1000);
        },

        /**
         * Initializes a Masonry.js instance on the element with the specified identity value and options.
         * @param {any} id The identity value of the DOM element to initialize the Masonry.js instance on.
         * @param {any} options The options to initialize the Masonry.js instance with.
         * @returns {boolean} true when the instance is successfully initialized, otherwise false.
         */
        Initialize: function (id, options) {
            var element = document.getElementById(id);

            if (element === null || typeof Masonry === "undefined")
                return false;

            this.Destroy(id);

            var instance = new Masonry(element, options || {});
            this._instances.set(id, instance);
            return true;
        },

        /**
         * Triggers a layout on the Masonry.js instance with the specified identity value.
         * @param {any} id The identity value of the Masonry.js instance to trigger a layout on.
         * @returns {boolean} true when the layout is successfully triggered, otherwise false.
         */
        Layout: function (id) {
            var instance = this._instances.get(id);

            if (!instance)
                return false;

            instance.layout();
            return true;
        },

        /**
         * Triggers a reload of items on the Masonry.js instance with the specified identity value.
         * @param {any} id The identity value of the Masonry.js instance to trigger a reload of items on.
         * @returns {boolean} true when the reload of items is successfully triggered, otherwise false.
         */
        ReloadItems: function (id) {
            var instance = this._instances.get(id);

            if (!instance)
                return false;

            instance.reloadItems();
            return true;
        },

        /**
         * Updates options for an existing Masonry.js instance.
         * @param {any} id The identity value of the Masonry.js instance to update.
         * @param {any} options The options to apply.
         * @param {boolean} relayout When true, triggers layout after applying options.
         * @returns {boolean} true when options are applied, otherwise false.
         */
        UpdateOptions: function (id, options, relayout) {
            var instance = this._instances.get(id);

            if (!instance)
                return false;

            instance.option(options || {});

            // Re-read item elements/sizes after option changes (e.g. column width updates)
            // before running layout to avoid stale positioning.
            instance.reloadItems();

            if (relayout !== false)
                instance.layout();

            return true;
        },

        /**
         * Destroys the Masonry.js instance with the specified identity value.
         * @param {any} id The identity value of the Masonry.js instance to destroy.
         * @returns {boolean} true when the instance is successfully destroyed, otherwise false.
         */
        Destroy: function (id) {
            var instance = this._instances.get(id);

            if (!instance)
                return false;

            instance.destroy();
            this._instances.delete(id);
            return true;
        },

        /**
         * Observes the element with the specified identity value for intersection changes and invokes the provided .NET method when the element becomes visible.
         * @param {any} sentinelId The identity value of the DOM element to observe for intersection changes.
         * @param {any} dotNetRef The reference to the .NET object containing the method to invoke when the observed element becomes visible.
         * @param {any} threshold The threshold value to use for the IntersectionObserver. Defaults to 0.1 if not provided.
         * @param {any} rootMargin The root margin value to use for the IntersectionObserver. Defaults to "0px" if not provided.
         * @returns {boolean} true when the observer is successfully created and observing the specified element, otherwise false.
         */
        ObserveInfiniteScroll: function (sentinelId, dotNetRef, threshold, rootMargin) {
            var sentinel = document.getElementById(sentinelId);

            if (sentinel === null || dotNetRef === null || typeof IntersectionObserver === "undefined")
                return false;

            this.UnobserveInfiniteScroll(sentinelId);

            var observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting)
                        dotNetRef.invokeMethodAsync("OnSentinelVisible");
                });
            }, {
                threshold: threshold ?? 0.1,
                rootMargin: rootMargin ?? "0px"
            });

            observer.observe(sentinel);
            this._observers.set(sentinelId, observer);
            return true;
        },

        /**
         * Stops observing the element with the specified identity value for intersection changes.
         * @param {any} sentinelId The identity value of the DOM element to stop observing for intersection changes.
         * @returns {boolean} true when the observer is successfully stopped and removed, otherwise false.
         */
        UnobserveInfiniteScroll: function (sentinelId) {
            var observer = this._observers.get(sentinelId);

            if (!observer)
                return false;

            observer.disconnect();
            this._observers.delete(sentinelId);
            return true;
        },
    }
}
