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
    }
}
