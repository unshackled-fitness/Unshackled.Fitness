var postcss = require('gulp-postcss');
var gulp = require('gulp');
var sass = require('gulp-sass')(require('node-sass'));
var autoprefixer = require('autoprefixer');
var cssnano = require('cssnano');
const rename = require('gulp-rename');

var config = {
    srcWatch: 'Styles/**/*.scss',
    srcScss: 'Styles/styles.scss',
    destCss: 'wwwroot/css'
}

var plugins = [
	autoprefixer(),
	cssnano(),
];

gulp.task('sass', function (cb) {
    gulp.src(config.srcScss)

        // output non-minified CSS file
        .pipe(sass({
            outputStyle: 'expanded'
		}).on('error', sass.logError))
		.pipe(postcss([autoprefixer()]))
		.pipe(gulp.dest(config.destCss))
		.pipe(postcss([cssnano()]))
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