(() => {
    const dbName = "adhihtan-pwa";
    const storeName = "state";
    const stateKey = "app";
    let wakeLock = null;

    const openDatabase = () => new Promise((resolve, reject) => {
        const request = indexedDB.open(dbName, 1);
        request.onupgradeneeded = () => {
            if (!request.result.objectStoreNames.contains(storeName)) {
                request.result.createObjectStore(storeName);
            }
        };
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });

    const transaction = async (mode, operation) => {
        const db = await openDatabase();
        try {
            return await new Promise((resolve, reject) => {
                const tx = db.transaction(storeName, mode);
                const request = operation(tx.objectStore(storeName));
                request.onsuccess = () => resolve(request.result);
                request.onerror = () => reject(request.error);
                tx.onerror = () => reject(tx.error);
            });
        } finally {
            db.close();
        }
    };

    const play = (path) => {
        const audio = new Audio(path);
        audio.volume = 0.7;
        audio.play().catch(() => {});
    };

    window.adhihtanApp = {
        loadState: async () => (await transaction("readonly", store => store.get(stateKey))) ?? null,
        saveState: async json => transaction("readwrite", store => store.put(json, stateKey)),
        downloadJson: (filename, json) => {
            const url = URL.createObjectURL(new Blob([json], { type: "application/json;charset=utf-8" }));
            const anchor = document.createElement("a");
            anchor.href = url;
            anchor.download = filename;
            anchor.click();
            setTimeout(() => URL.revokeObjectURL(url), 1000);
        },
        feedback: (mode, completed, alarm) => {
            if (mode === "sound" || mode === "sound_vibrate" || (completed && alarm)) {
                play(completed ? "assets/audio/dialog-click.mp3" : "assets/audio/soft-click.mp3");
            }
            if ((mode === "vibrate" || mode === "sound_vibrate" || (completed && alarm)) && navigator.vibrate) {
                navigator.vibrate(completed ? [45, 35, 90] : 18);
            }
        },
        share: async (title, text) => {
            if (navigator.share) {
                try {
                    await navigator.share({ title, text });
                    return;
                } catch (error) {
                    if (error?.name === "AbortError") return;
                }
            }
            await navigator.clipboard?.writeText(text);
        },
        setWakeLock: async enabled => {
            try {
                if (!enabled && wakeLock) {
                    await wakeLock.release();
                    wakeLock = null;
                } else if (enabled && !wakeLock && "wakeLock" in navigator) {
                    wakeLock = await navigator.wakeLock.request("screen");
                    wakeLock.addEventListener("release", () => wakeLock = null);
                }
            } catch {
                wakeLock = null;
            }
        },
        registerPwa: async () => {
            if (!("serviceWorker" in navigator)) return;
            try {
                const registration = await navigator.serviceWorker.register("service-worker.js", { updateViaCache: "none" });
                registration.update().catch(() => {});
            } catch {
                // Browsers can disable service workers in private or insecure contexts.
            }
        },
        setFontEncoding: encoding => window.adhihtanMyanmar?.setEncoding(encoding)
    };
})();
