window.CreateQrCode = () => {
	const qrCode = document.getElementById("qrCode");
	if (qrCode) {
		const uri = qrCode.getAttribute('data-url');
		new QRCode(qrCode, {
			text: uri,
			width: 200,
			height: 200,
			correctLevel: QRCode.CorrectLevel.H
		});
	}
};