export default {
  entry: 'dist/index.js',
  dest: 'dist/bundles/studiox-ng2-module.umd.js',
  sourceMap: false,
  format: 'umd',
  moduleName: 'ng.studioxModule',
  globals: {
    '@angular/core': 'ng.core'
  }
}