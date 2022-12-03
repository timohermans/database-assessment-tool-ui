import { defineConfig } from 'vite';
import glob from 'glob';

// vite.config.js
export default defineConfig({
    build: {
        manifest: true,
        outDir: 'wwwroot/js',
        assetsDir: '',
        sourcemap: true,
        modulePreload: {
            polyfill: false
        },
        rollupOptions: {
            // overwrite default .html entry
            input: glob.sync('Scripts/**/*.ts')
        }
    }
})
