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
     * Loads a JavaScript file and appends to the end of document.body.
     * @param {string} uri The URI of the script to load.
     * @param {string} type The name of a type contained in the script to test for after loading completes.
     * @param {number} timeout The duration in milliseconds to wait for the type to be available in the browser window.
     */
    LoadScript: async function (uri, type, timeout) {
        // Get preloaded scripts
        if (IsLoaderInitialized === false) {
            var scripts = document.getElementsByTagName("script");

            for (var i = 0; i < scripts.length; i++) {
                LoadedScripts.set(this.GetFileName(scripts[i].src), true);
            }

            IsLoaderInitialized = true;
        }

        // Check if previously loaded
        var file = this.GetFileName(uri);

        if (LoadedScripts.has(file) && LoadedScripts.get(file) === true)
            return true;

        // Load
        var result = await new Promise(resolve => {
            var script = document.createElement('script');

            script.type = "text/javascript";

            script.onload = () => {
                LoadedScripts.set(file, true);
                resolve(true);
            };

            script.onerror = () => {
                console.error(`Failed to load script: ${uri}.`);

                LoadedScripts.set(file, false);
                resolve(false);
            };

            document.body.appendChild(script);
            script.src = uri;
        });

        if (result === false || typeof (type) === 'undefined' || type === null)
            return result;

        // Test if type exists
        if (typeof (timeout) === 'undefined' || timeout === null)
            timeout = 1000;

        var start = Date.now();

        do {
            result = typeof window[type] === 'function';

            if (result === true)
                break;

            await new Promise(resolve => setTimeout(resolve, 50));
        } while ((Date.now() - start) < timeout);

        return result;
    },

    /**
     * Extracts the file name from the provided path.
     * @param {string} path The path to extract the file name from.
     */
    GetFileName: function (path) {
        return path.split('\\').pop().split('/').pop().split('?').shift();
    },

    /**
     * Wrapper functions around Masonry.js instances.
     */
    Masonry: {
        _instances: new Map(),
        _observers: new Map(),

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
         * Refreshes the Masonry.js instance with the specified identity value by reloading items and triggering a layout.
         * @param {any} id The identity value of the Masonry.js instance to trigger a layout on.
         * @returns true when the container is successfully refreshed, otherwise false.
         */
        Refresh: function (id) {
            var instance = this._instances.get(id);

            if (!instance)
                return false;

            instance.reloadItems();
            instance.layout();

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
            instance.reloadItems();

            if (relayout === true)
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
         * @param {any} interop The reference to the .NET object containing the method to invoke when the observed element becomes visible.
         * @param {any} threshold The threshold value to use for the IntersectionObserver. Defaults to 0.1 if not provided.
         * @param {any} rootMargin The root margin value to use for the IntersectionObserver. Defaults to "0px" if not provided.
         * @returns {boolean} true when the observer is successfully created and observing the specified element, otherwise false.
         */
        ObserveInfiniteScroll: function (sentinelId, interop, threshold, rootMargin) {
            var sentinel = document.getElementById(sentinelId);

            if (sentinel === null || interop === null || typeof IntersectionObserver === "undefined")
                return false;

            this.UnobserveInfiniteScroll(sentinelId);

            var observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (!entry.isIntersecting)
                        return;

                    interop.invokeMethodAsync("OnSentinelVisible").catch(error => {
                        console.debug("Infinite scroll callback (OnSentinelVisible) failed.", {
                            sentinelId: sentinelId,
                            error: error
                        });
                    });
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

/**
 * Stores a list of JavaScript files that have been loaded.
 */
const LoadedScripts = new Map();

/**
 * Indicates whether the DOM has been scanned for any user loaded scripts.
 */
let IsLoaderInitialized = false;
