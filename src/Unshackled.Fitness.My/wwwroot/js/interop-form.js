window.InitalizeForm = () => {
	const forms = document.querySelectorAll('.disable-on-submit');
	for (var i = 0; i < forms.length; i++) {
		var form = forms[i];
		form.addEventListener("submit", function (e) {
			var btn = e.currentTarget.querySelector("button[type='submit']");
			if (btn) {
				btn.disabled = true;
			}
		});
	}
};

window.InitalizeForm();