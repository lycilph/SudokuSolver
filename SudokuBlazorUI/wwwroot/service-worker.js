// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', () => { });

// During production, this file is replaced with a file generated by the
// Blazor WebAssembly build process that enables offline support and pre-caching
// of application assets.