var charts = new Object();

window.setupChart = (id, config) => {
	var context = document.getElementById(id).getContext('2d');

	if (typeof charts[id] !== 'undefined') {
		charts[id].destroy();
	}

	charts[id] = new Chart(context, config);
}

window.loadData = (id, datasets) => {
	if (typeof charts[id] !== 'undefined') {
		charts[id].data.datasets = datasets;
		charts[id].options.scales.y.min = 0;
		charts[id].update();
	}
}