(function () {
	"use strict";

	/** @typedef {AgencyData}
	 * @property {string} name
	 * @property {string} dataexchange_id
	 */

	var agenciesRequest = new XMLHttpRequest();
	agenciesRequest.addEventListener("load", function () {
		var data, select, frag;
		if (this.status === 200) {
			data = this.response.data;
			// Create the document fragment that the options will be added to.
			frag = document.createDocumentFragment();
			// Create an option for each agency in WA and add to document fragment.
			data.forEach(function (agency) {
				var option;
				if (agency.state === "Washington") {
					option = document.createElement("option");
					option.value = agency.dataexchange_id;
					option.textContent = agency.name;
					frag.appendChild(option);
				}
			});
			select = document.getElementById("agencySelect");
			select.appendChild(frag);
		} else {
			console.error(this);
		}
	});
	agenciesRequest.responseType = "json";
	agenciesRequest.open("GET", "/api/agencies");
	agenciesRequest.send();

	document.getElementById("agencySelect").addEventListener("change", function (e) {
		var select = e.target;
		console.log(select.value);
		document.getElementById("agencySubmit").disabled = false;
	});

	// Setup agency submission.
	document.forms.agencyForm.onsubmit = function () {
		/**
		 * @param {XMLHttpRequestProgressEvent} e
		 */
		function updateProgress(e) {
			if (progress.hidden) {
				progress.hidden = false;
			}
			if (e.lengthComputable) {
				progress.max = 1;
				progress.value = e.position / e.total;
			} else {
				progress.max = 1;
				progress.value = 0;
			}
			console.log("progress event", e);
		}

		/**
		 * @param {XMLHttpRequestProgressEvent} e
		 */
		function handleResponse(e) {
			var gtfs;
			gtfs = e.target.response;
			console.log("gtfs", gtfs);
			progress.hidden = true;
		}

		var progress = document.getElementById("agencyProgress");

		var request = new XMLHttpRequest();
		request.responseType = "json";
		request.addEventListener("loadstart", updateProgress);
		request.addEventListener("progress", updateProgress);
		request.addEventListener("loadend", handleResponse);
		request.open("GET", ["api/feed/", document.getElementById("agencySelect").value].join(""));
		request.send();

		return false;
	};
}());