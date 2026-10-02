// Project Remniscence Service Worker
const CACHE_NAME = 'rem-companion-v1';
const PRECACHE_URLS = [
  './index.html',
  './manifest.json',
  './icon.svg',
  './libs/three.min.js',
  './libs/OrbitControls.js',
  './libs/GLTFLoader.js'
];

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => {
      return cache.addAll(PRECACHE_URLS).catch((err) => console.warn('PWA Precache fallback:', err));
    }).then(() => self.skipWaiting())
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys().then((keys) => {
      return Promise.all(
        keys.filter((key) => key !== CACHE_NAME).map((key) => caches.delete(key))
      );
    }).then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', (event) => {
  // Let GLB models and textures fetch normally to respect cache-busting queries
  if (event.request.url.includes('/models/') || event.request.url.includes('/api/')) {
    return;
  }
  event.respondWith(
    caches.match(event.request).then((cachedResponse) => {
      if (cachedResponse) return cachedResponse;
      return fetch(event.request);
    })
  );
});
