var charts = new Object();

window.setupChart = (id, config) => {
	var context = document.getElementById(id).getContext('2d');

	if (typeof charts[id] !== 'undefined') {
		charts[id].destroy();
	}
		
	charts[id] = new Chart(context, config);
}

window.loadData = (id, datasets, formatTime) => {
	if (typeof charts[id] !== 'undefined') {
		charts[id].data.datasets = datasets;

		if (formatTime) {
			charts[id].options.scales.y.ticks.stepSize = 60;
			charts[id].options.scales.y.ticks.callback = formatSecsAsMins;
			charts[id].options.plugins.tooltip.callbacks.label = formatSecsAsMinsLabel;
		}

		charts[id].update();
	}
}

window.formatSecsAsMinsLabel = (context) => {
	return context.dataset.label + ': ' + formatSecsAsMins(context.parsed.y)
}

window.formatSecsAsMins = (secs) => {
	var totalSeconds = Math.round(secs);
	var hours = Math.floor(totalSeconds / 3600);
	totalSeconds %= 3600;
	var minutes = Math.floor(totalSeconds / 60);
	var seconds = totalSeconds % 60;
	return [hours, minutes, seconds]
		.map(v => v < 10 ? "0" + v : v)
		.filter((v, i) => v !== "00" || i > 0)
		.join(":")
}