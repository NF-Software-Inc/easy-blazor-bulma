window.easyBlazorBulma = {
    /**
     * Adds a new URI parameter.
     * @param {string} name The name of the parameter to add.
     * @param {string} value The value to assign to the parameter.
     */
    AddUriParameter: function (name, value) {
        var parameters = new URLSearchParams(window.location.search);
        parameters.set(name, value);

        var updated = window.location.origin + window.location.pathname + '?' + parameters;
        window.history.replaceState(null, '', updated);
    },

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
     * Checks and requests permission to access a camera.
     */
    CheckCameraPermission: function () {
        if (this.HasCamera() === false)
            return false;

        return navigator.mediaDevices.getUserMedia({ video: true, audio: false })
            .then(() => true)
            .catch(() => false);
    },

    /**
     * Checks and requests permission to display desktop notifications to the user.
     * @param {boolean} reset Requests permission again when already denied.
     */
    CheckNotificationPermission: async function (reset) {
        if (this.HasNotification() === false)
            return false;
        else if (Notification.permission === 'granted')
            return true;
        else if (reset !== true && Notification.permission === "denied")
            return false;

        var permission = await Notification.requestPermission();
        return permission === 'granted';
    },

    /**
     * Compresses a Base64 image to a JPEG with specified quality and resizes to the greater of the provided width or height.
     * @param {string} base64String The Base64 image string.
     * @param {int} maxWidth The width to resize to in px (if width > height).
     * @param {int} maxHeight The height to resize to in px (if height > width).
     * @param {double} quality Value from 0 to 1 that represents the compression level.
     */
    CompressAndResize: function (base64String, maxWidth, maxHeight, quality) {
        return new Promise((resolve, reject) => {
            if (!base64String || typeof base64String !== 'string' || !base64String.startsWith('data:image/'))
                return reject(new Error("Input is not a valid Base64 image data URL string."));

            const image = new Image();

            image.onload = () => {
                let width = image.width;
                let height = image.height;

                if (width > height && width > maxWidth) {
                    height = Math.round(height * (maxWidth / width));
                    width = maxWidth;
                } else if (height > maxHeight) {
                    width = Math.round(width * (maxHeight / height));
                    height = maxHeight;
                }

                const canvas = document.createElement('canvas');

                canvas.width = width;
                canvas.height = height;

                const ctx = canvas.getContext('2d');

                ctx.drawImage(image, 0, 0, width, height);
                resolve(canvas.toDataURL('image/jpeg', quality));
            };

            image.onerror = (e) => reject(new Error(`Failed to load image from Base64 string for resizing.: ${e.message}`));
            image.src = base64String;
        });
    },

    /**
     * Function to decrypt text using AES-GCM.
     * @param {any} text The payload to decrypt.
     * @param {any} password  The secret to decrypt the payload data with.
     * @returns A plain text decrypted string.
     */
    Decrypt: async function (text, password) {
        var combined = new Uint8Array(Uint8Array.from(atob(text), c => c.charCodeAt(0)).buffer);
        var salt = combined.slice(0, 16);
        var iv = combined.slice(16, 28);
        var encrypted = combined.slice(28);
        var key = await this.GenerateKey(password, salt);
        var decrypted = await crypto.subtle.decrypt({ name: 'AES-GCM', iv: iv }, key, encrypted);

        var decoder = new TextDecoder();
        return decoder.decode(decrypted);
    },

    /**
     * Deletes the specified cache if it exists.
     * @param {string} cache The cache to delete.
     * @returns {boolean} Indicated whether the cache was deleted successfully.
     */
    DeleteCache: async function (cache) {
        var exists = await window.caches.has(cache);

        if (exists === true) {
            var success = await window.caches.delete(cache);

            return success;
        }
        else {
            return true;
        }
    },

    /**
     * Checks for and removes the local storage item with the specified name.
     * @param {string} name The name of the storage item to remove.
     */
    DeleteStorage: function (name) {
        if (this.HasStorage()) {
            window.localStorage.removeItem(name);
            return true;
        }

        return false;
    },

    /**
     * Displays a notification to the user.
     * @param {string} title The value to set as the notification header.
     * @param {string} message The value to set as the notification text.
     * @param {string} url The URL to open when a notification is clicked.
     */
    DisplayNotification: function (title, message, url) {
        if (this.HasNotification() === false || this.CheckNotificationPermission() === false)
            return false;

        var notification = new Notification(title, {
            icon: 'favicon.ico',
            body: message.length > 200 ? message.substring(0, 200) + '...' : message
        });

        if (typeof (url) !== 'undefined') {
            notification.onclick = (event) => {
                event.preventDefault();
                window.open(url, '_blank');
            };
        }

        return true;
    },

    /**
     * Converts the provided stream to a blob in memory and downloads the file.
     * @param {string} name The name to assign the downloaded file.
     * @param {any} stream A reference to the stream to download.
     */
    DownloadFileFromStream: async function (name, stream) {
        var buffer = await stream.arrayBuffer();
        var blob = new Blob([buffer]);
        var url = URL.createObjectURL(blob);

        this.DownloadFileFromUrl(name, url);

        URL.revokeObjectURL(url);
    },

    /**
     * Converts the provided base-64 string and downloads the file.
     * @param {string} name The name to assign the downloaded file.
     * @param {string} bytesBase64 The base-64 string containing the file data.
     */
    DownloadFileFromBase64: function (name, bytesBase64) {
        var element = document.createElement('a');

        element.href = `data:application/octet-stream;base64,${bytesBase64}`;
        element.download = name;
        element.click();

        element.remove();
    },

    /**
     * Downloads the file at the specified URL.
     * @param {string} name The name to assign the downloaded file.
     * @param {string} url The URL to download a file from.
     */
    DownloadFileFromUrl: function (name, url) {
        var element = document.createElement('a');

        element.href = url;
        element.download = name;
        element.click();

        element.remove();
    },

    /**
     * Function to encrypt text using AES-GCM.
     * @param {any} text The payload to encrypt.
     * @param {any} password The secret to secure encrypted data with.
     * @returns A base64 encrypted string.
     */
    Encrypt: async function (text, password) {
        var encoder = new TextEncoder();
        var salt = crypto.getRandomValues(new Uint8Array(16));
        var key = await this.GenerateKey(password, salt);
        var iv = crypto.getRandomValues(new Uint8Array(12));
        var encrypted = await crypto.subtle.encrypt({ name: 'AES-GCM', iv: iv }, key, encoder.encode(text));

        var combined = new Uint8Array([...salt, ...iv, ...new Uint8Array(encrypted)]);
        return btoa(String.fromCharCode(...new Uint8Array(combined.buffer)));
    },

    /**
     * Moves the focussed element to the specified id tag or first element with the autofocus attribute.
     * @param {string} id The identity tag of the element to focus.
     * @returns {boolean} true when an element is matched, otherwise false.
     */
    FocusElement: function (id) {
        var element = (typeof (id) !== 'undefined' && id !== null && id.length > 0) ? document.getElementById(id) : document.querySelector('[autofocus]');

        if (element === null)
            return false;

        element.focus();
        return true;
    },

    /**
     * Navigates to the next URL.
     */
    Forward: function () {
        window.history.forward();
    },

    /**
     * Function to generate an AES-GCM key from a password.
     * @param {string} password The secret to secure encrypted data with.
     * @param {any} salt The salt value to apply to the key.
     * @returns an encryption key.
     */
    GenerateKey: async function (password, salt) {
        var encoder = new TextEncoder();
        var material = await crypto.subtle.importKey('raw', encoder.encode(password), 'PBKDF2', false, ['deriveBits', 'deriveKey']);

        return crypto.subtle.deriveKey(
            { name: 'PBKDF2', salt: salt, iterations: 100000, hash: 'SHA-256' },
            material,
            { name: 'AES-GCM', length: 256 },
            false,
            ['encrypt', 'decrypt']
        );
    },

    /**
     * Returns a list of browser application caches.
     */
    GetCaches: async function () {
        var list = await window.caches.keys();
        return list;
    },

    /**
     * Extracts the file name from the provided path.
     * @param {string} path The path to extract the file name from.
     */
    GetFileName: function (path) {
        return path.split('\\').pop().split('/').pop().split('?').shift();
    },

    /**
     * Returns the x and y scroll positions the browser currently is at.
     */
    GetScroll: function () {
        return [Math.round(window.scrollX), Math.round(window.scrollY)];
    },

    /**
     * Returns the number of bytes remaining for IndexedDB storage.
     */
    GetStorageBytes: async function () {
        if ('storage' in navigator) {
            var estimate = await navigator.storage.estimate();
            return (estimate.quota ?? 0) - (estimate.usage ?? 0);
        }

        return 0;
    },

    /**
     * Returns a list of keys that are currently in the local storage.
     */
    GetStorageKeys: function () {
        if (this.HasStorage()) {
            return Object.keys(window.localStorage);
        }
        else {
            return [];
        }
    },

    /**
     * Gets the current URI parameters.
     * @returns {string} The current URI parameters starting at the ?.
     */
    GetUriParameters: function () {
        return window.location.search;
    },

    /**
     * Gets the current page title.
     * @returns {string} The current page title.
     */
    GetWindowTitle: function () {
        return document.title;
    },

    /**
     * Returns the current width of the screen in pixels.
     */
    GetWindowWidth: function () {
        return window.innerWidth;
    },

    /**
     *  Returns the current height of the screen in pixels.
     */
    GetWindowHeight: function () {
        return window.innerHeight;
    },

    /**
     * Returns whether the browser supports taking photos from a camera.
     */
    HasCamera: function () {
        if ('mediaDevices' in navigator && 'getUserMedia' in navigator.mediaDevices)
            return true;
        else
            return false;
    },

    /**
     * Tests to see whether IndexedDB is available in the current context.
     * @returns {boolean} true when IndexedDB is available, otherwise false.
     */
    HasIndexedDb: function () {
        if ('indexedDB' in window)
            return true;
        else
            return false;
    },

    /**
     * Returns whether the browser supports desktop notifications.
     */
    HasNotification: function () {
        if ('Notification' in window)
            return true;
        else
            return false;
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
     * Accepts a content stream and loads it as a blob into the element with the specified id value.
     * @param {string} id The id of the element to load the data into.
     * @param {string} mime The MIME type of the content to load.
     * @param {any} stream A stream containing the data.
     */
    LoadBlob: async function (id, mime, stream) {
        var buffer = await stream.arrayBuffer();
        var blob = mime === null ? new Blob([buffer]) : new Blob([buffer], { type: mime });
        var url = URL.createObjectURL(blob);
        var element = document.getElementById(id);

        if (element !== null) {
            element.onload = () => URL.revokeObjectURL(url);
            element.src = url;
        }
    },

    /**
     * Loads the specified image from the database.
     * @param {string} name The database to read from.
     * @param {string} store The table in the database to read from.
     * @param {number} version The version number of the database.
     * @param {string} id The key the image is stored under.
     * @returns {boolean} true when successful, otherwise false.
     */
    LoadBlobFromIndexedDb: async function (name, store, version, id) {
        var blob = await this.ReadFromIndexedDb(name, store, version, id);

        if (typeof (blob) === 'undefined')
            return false;

        var url = URL.createObjectURL(blob);
        var element = document.getElementById(id);

        if (element !== null) {
            element.onload = () => URL.revokeObjectURL(url);
            element.src = url;
        }

        return true;
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
     * Opens or creates a database with the provided name.
     * @param {string} name The name of the database to open.
     * @param {string} store Creates a table in the database during upgrades.
     * @param {number} version The version number of the database.
     * @returns {IDBDatabase} The database object.
     */
    OpenIndexedDb: async function (name, store, version) {
        if (typeof (version) === 'undefined')
            version = 1;

        var request = indexedDB.open(name, version);

        var promise = new Promise(resolve => {
            request.onupgradeneeded = (event) => {
                var db = event.target.result;

                if (db.objectStoreNames.contains(store) === false)
                    db.createObjectStore(store);
            };

            request.onsuccess = () => resolve(request.result);
            request.onerror = () => {
                console.error('Failed to open IndexedDb.');
                resolve(null);
            };
        });

        try {
            return await promise;
        }
        catch (e) {
            console.error(e);
            return null;
        }
    },

    /**
     * Displays the print preview dialog.
     */
    PrintPreview: function () {
        window.print();
    },

    /**
     * Checks for and returns the cookie value with the specified name.
     * @param {string} name The name of the cookie value to return.
     * @returns {string} The value of the requested cookie when found, otherwise an empty string.
     */
    ReadCookie: function (name) {
        // Ensure there is at least 1 cookie
        if (document.cookie.length === 0)
            return '';

        // Check for and return coookie
        var i, cookie, value;
        var cookies = document.cookie.split(';')

        for (i = 0; i < cookies.length; i++) {
            cookie = cookies[i].substring(0, cookies[i].indexOf('=')).replace(/^\s+|\s+$/g, '');
            value = cookies[i].substring(cookies[i].indexOf('=') + 1);

            if (cookie === name)
                return decodeURIComponent(value.replace(/%(?![0-9A-Fa-f]{2})/g, '%25'));
        }

        // Not found
        return '';
    },

    /**
     * Reads and returns text from the system clipboard.
     * @returns {string} The text from the clipboard when available, otherwise an empty string when the clipboard is empty or access is denied.
     */
    ReadClipboard: async function () {
        try {
            return await navigator.clipboard.readText();
        } catch (e) {
            return '';
        }
    },

    /**
     * Returns the specified object from the database.
     * @param {string} name The database to read from.
     * @param {string} store The table in the database to read from.
     * @param {number} version The version number of the database.
     * @param {string} id The key the object is stored under.
     * @returns {any} The requested object.
     */
    ReadFromIndexedDb: async function (name, store, version, id) {
        if (this.HasIndexedDb() == false)
            return null;

        var database = await this.OpenIndexedDb(name, store, version);

        if (database === null || typeof (database) === 'undefined')
            return null;

        var request = database.transaction(store, 'readonly')
            .objectStore(store)
            .get(id);

        var promise = new Promise(resolve => {
            request.onsuccess = () => {
                resolve(request.result);
            };

            request.onerror = () => {
                resolve(null);
            };
        });

        var result = await promise;

        if (result === null || typeof (result) === 'undefined')
            return null;
        else
            return result.data;
    },

    /**
     * Checks for and returns the history state for the current URI.
     * @returns {any} The text of the history state as an object.
     */
    ReadHistoryState: function () {
        return window.history.state;
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
     * Reloads the application.
     */
    Reload: function () {
        window.location.reload();
    },

    /**
     * Saves the provided stream to the database.
     * @param {string} name The database to save to.
     * @param {string} store The table in the database to save to.
     * @param {number} version The version number of the database.
     * @param {string} id The key to store the object under.
     * @param {any} stream A stream containing the image data.
     * @returns {boolean} true when successful, otherwise false.
     */
    SaveBlobToIndexedDb: async function (name, store, version, id, stream) {
        var available = await this.GetStorageBytes();

        if (available < 10 * 1024 * 1024)
            return false;

        var arrayBuffer = await stream.arrayBuffer();
        var blob = new Blob([arrayBuffer]);

        return await this.SaveToIndexedDb(name, store, version, id, blob);
    },

    /**
     * Saves the provided object to the database.
     * @param {string} name The database to save to.
     * @param {string} store The table in the database to save to.
     * @param {number} version The version number of the database.
     * @param {string} id The key to store the object under.
     * @param {any} data The object to store in the database.
     * @returns {boolean} true when successful, otherwise false.
     */
    SaveToIndexedDb: async function (name, store, version, id, data) {
        if (this.HasIndexedDb() == false)
            return false;

        var database = await this.OpenIndexedDb(name, store, version);

        if (database === null || typeof (database) === 'undefined')
            return false;

        var transaction = database.transaction(store, 'readwrite');

        return await new Promise(resolve => {
            transaction.oncomplete = () => resolve(true);
            transaction.onerror = () => resolve(false);

            transaction.objectStore(store).put({ timestamp: Date.now(), data: data }, id);
        });
    },

    /**
     * Selects the inner text of the specified element.
     * @param {string} id The identity tag of the element to select.
     * @returns {boolean} true when an element is matched, otherwise false.
     */
    SelectElement: function (id) {
        var element = document.getElementById(id);

        if (element === null)
            return false;

        element.select();
        return true;
    },

    /**
     * Scrolls the browser window to the provided coordinates.
     * @param {any} x The x coordinate to scroll to.
     * @param {any} y The y coordinate to scroll to.
     */
    SetScroll: function (x, y) {
        window.scroll(x, y);
    },

    /**
     * Starts a camera stream on the specified video element.
     * @param {string} element The id value of the element to link the camera stream to.
     */
    StartCamera: function (element) {
        if (this.HasCamera() === false)
            return false;

        var video = document.getElementById(element);

        if (video === null)
            return false;

        return navigator.mediaDevices.getUserMedia({
            video: {
                facingMode: "environment",
                width: { ideal: 4096 },
                height: { ideal: 2160 }
            },
            audio: false
        })
        .then(stream => {
            video.srcObject = stream;
            video.play();
            return true;
        })
        .catch(() => false);
    },

    /**
     * Stops an active camera stream on the specified video element.
     * @param {string} element The id value of the element to stop the camera stream on.
     */
    StopCamera: function (element) {
        if (this.HasCamera() === false)
            return false;

        var video = document.getElementById(element);

        if (video === null)
            return false;

        var stream = video.srcObject;

        if (stream === null)
            return true;

        stream.getTracks().forEach(track => {
            if (track.readyState == 'live')
                track.stop();
        });

        video.srcObject = null;
        return true;
    },

    /**
     * Takes a photo on the active camera stream and returns the result as a base64 string.
     * @param {string} videoElement The id value of the element the stream is running on.
     * @param {string} canvasElement The id value of a canvas element to process the image.
     * @param {string} imageElement The id value of an image element to preview the image.
     * @param {string} mimeType The mime type to save the image data as.
     */
    TakePhoto: async function (videoElement, canvasElement, imageElement, mimeType) {
        if (this.HasCamera() === false)
            return null;

        var canvas = document.getElementById(canvasElement);
        var image = document.getElementById(imageElement);
        var video = document.getElementById(videoElement);

        if (canvas === null || video === null)
            return null;

        var context = canvas.getContext("2d");

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        context.drawImage(video, 0, 0, video.videoWidth, video.videoHeight);

        return await new Promise(resolve => {
            var data = canvas.toDataURL(mimeType);

            if (image !== null)
                image.src = data;

            resolve(data);
        });
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
     * Removes the source attribute from the specified element to unload the content.
     * @param {any} id The id of the element to unload.
     */
    UnloadBlob: function (id) {
        var element = document.getElementById(id);

        if (element !== null)
            element.removeAttribute('src');
    },

    /**
     * Updates the current URI to the provided value without adding a state entry.
     * @param {string} uri The value to update the URI to.
     */
    UpdateCurrentUri: function (uri) {
        window.history.pushState(null, '', uri);
    },

    /**
     * Writes text to the system clipboard.
     * @param {string} text The text to write to the clipboard.
     * @returns {boolean} true when the write operation was successful.
     */
    WriteClipboard: async function (text) {
        if (text === null || text === undefined)
            return false;

        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (e) {
            return false;
        }
    },

    /**
     * Creates or replaces a cookie with the provided 'name'.
     * @param {string} name The text identifier for the cookie.
     * @param {any} value The value to store in the cookie.
     * @param {number} validFor The duration in milliseconds the cookie is valid for.
     * @param {string} path A web path root to restrict the cookie to.
     * @returns {boolean} true when the write operation was successful.
     */
    WriteCookie: function (name, value, validFor, path) {
        var expires;

        // Add expiry
        if (typeof (validFor) !== 'undefined' && validFor > 0) {
            var date = new Date();
            date.setTime(date.getTime() + validFor);
            expires = '; expires=' + date.toGMTString();
        }
        else {
            expires = '';
        }

        // Add path
        if (typeof (path) !== 'undefined' && path.length > 0)
            path = '/' + path;
        else
            path = '/';

        // Write cookie
        document.cookie = `${name}=${value}${expires}; path=${path}`;
        return true;
    },

    /**
     * Creates or updates a history state with the provided value.
     * @param {string} value The serialized data to store in the history.
     */
    WriteHistoryState: function (value) {
        window.history.replaceState(value, '', window.location.href);
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
