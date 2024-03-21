var gulp = require('gulp')
var sass = require('gulp-sass')(require('node-sass'));
var autoprefixer = require('gulp-autoprefixer')
var minCss = require('gulp-minify-css')
var rename = require('gulp-rename')

var config = {
    srcWatch: 'Styles/**/*.scss',
    srcScss: 'Styles/styles.scss',
    destCss: 'wwwroot/css'
}

gulp.task('sass', function (cb) {
    gulp.src(config.srcScss)

        // output non-minified CSS file
        .pipe(sass({
            outputStyle: 'expanded'
        }).on('error', sass.logError))
        .pipe(autoprefixer())
        .pipe(gulp.dest(config.destCss))

        // output the minified version
        .pipe(minCss())
        .pipe(rename({ extname: '.min.css' }))
        .pipe(gulp.dest(config.destCss))

    cb()
})

gulp.task(
    'watch',
    gulp.series('sass', function (cb) {
        gulp.watch(config.srcWatch, gulp.series('sass'));
        cb();
    })
);