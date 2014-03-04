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
		var progress, request;

		/** Updates the progress meter. 
		 * Shows the progress meter if it is hidden.
		 * @param {XMLHttpRequestProgressEvent} e
		 * @this {XMLHttpRequest}
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

		/** Processes the GTFS data.
		 * @param {XMLHttpRequestProgressEvent} e
		 * @this {XMLHttpRequest}
		 */
		function handleResponse(e) {
			var gtfs;
			progress.hidden = true;
			if (e.target.status === 500) {
				alert(e.target.response);
				console.error(e.target.statusText);
			} else {
				gtfs = e.target.response;
				console.log("gtfs", gtfs);
			}

		}

		progress = document.getElementById("agencyProgress");

		// Create the HTTP request for the selected agency's GTFS data.
		request = new XMLHttpRequest();
		request.responseType = "json";
		request.addEventListener("loadstart", updateProgress);
		request.addEventListener("progress", updateProgress);
		request.addEventListener("loadend", handleResponse);
		request.open("GET", ["api/feed/", document.getElementById("agencySelect").value].join(""));
		request.send();

		// Returning false so the form does not actually get submitted (prevents page from reloading).
		return false;
	};
}());