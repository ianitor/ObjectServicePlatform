window.addEventListener("load", function () {

	function explode() {
		var a = document.querySelector("a.PostLogoutRedirectUri");
		if (a) {
			window.location = a.href;
		}
	}
	setTimeout(explode, 3000);
});
