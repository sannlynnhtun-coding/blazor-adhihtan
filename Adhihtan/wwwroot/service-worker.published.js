self.importScripts("./service-worker-assets.js");

self.addEventListener("install", event => event.waitUntil(onInstall()));
self.addEventListener("activate", event => event.waitUntil(onActivate()));
self.addEventListener("fetch", event => event.respondWith(onFetch(event)));

const cacheNamePrefix = "adhihtan-offline-";
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [
    /\.dll$/, /\.pdb$/, /\.wasm/, /\.html$/, /\.js$/, /\.json$/, /\.css$/,
    /\.woff2?$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.mp3$/,
    /\.blat$/, /\.dat$/, /\.webmanifest$/
];
const offlineAssetsExclude = [/^service-worker\.js$/];
const baseUrl = new URL("/", self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall() {
    const requests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: "no-cache" }));

    const cache = await caches.open(cacheName);
    await cache.addAll(requests);
    await self.skipWaiting();
}

async function onActivate() {
    const keys = await caches.keys();
    await Promise.all(keys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
    await self.clients.claim();
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === "GET") {
        const shouldServeIndex = event.request.mode === "navigate"
            && !manifestUrlList.some(url => url === event.request.url);
        const request = shouldServeIndex ? "index.html" : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }

    return cachedResponse || fetch(event.request);
}
