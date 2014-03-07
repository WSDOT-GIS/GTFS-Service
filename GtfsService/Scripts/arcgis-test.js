/*global require, Terraformer, Gtfs*/
/*jshint browser:true*/
require(["esri/map", "esri/graphic", "esri/layers/GraphicsLayer", "esri/renderers/SimpleRenderer"], function (Map, Graphic, GraphicsLayer, SimpleRenderer) {
	"use strict";
	var map, agencySelect, importButton, gtfsProgressMeter;

	agencySelect = document.getElementById("agencySelect");
	importButton = document.getElementById("importGtfsButton");
	gtfsProgressMeter = document.getElementById("gtfsProgress");

	map = new Map("mapDiv", {
		basemap: "streets",
		center: [-120.80566406246835, 47.41322033015946],
		zoom: 7,
		showAttribution: true
	});

	/** 
	 * Populates the list of agencies.
	 */
	function populateAgenciesSelect(/*{XMLHttpRequestProgressEvent}*/ e) {
		var data, officialFrag, unofficialFrag, unofficialGroup, officialGroup;

		gtfsProgressMeter.hidden = true;

		if (e.target.response.data) {
			// Get the array of agencies.
			data = e.target.response.data;
			// Filter the array so that only those in WA remain.
			data = data.filter(function (v) {
				return v.state === "Washington";
			});
			// Add each agency to the select box.
			officialFrag = document.createDocumentFragment();
			unofficialFrag = document.createDocumentFragment();
			data.forEach(function (v) {
				var option = document.createElement("option");
				option.textContent = v.name;
				option.value = v.dataexchange_id;
				if (!v.is_official) {
					unofficialFrag.appendChild(option);
				} else {
					officialFrag.appendChild(option);
				}
			});
			
			unofficialGroup = agencySelect.querySelector("optgroup[label=Unofficial]");
			officialGroup = agencySelect.querySelector("optgroup[label=Official]");
			officialGroup.appendChild(officialFrag);
			unofficialGroup.appendChild(unofficialFrag);
			agencySelect.disabled = false;
		}
	}

	agencySelect.addEventListener("change", function (/*{Event}*/e) {
		var select = e.target;
		importButton.disabled = !select.value;
	});

	function updateProgressMeter(/*{ProgressEvent}*/ e) {
		var request = e.target;
		if (request.readyState === 3) { // LOADING
			if (e.lengthComputable) {
				gtfsProgressMeter.max = e.total;
				gtfsProgressMeter.value = e.loaded;
			}
		} else if (request.readyState === 4) { // DONE
			gtfsProgressMeter.removeAttribute("value");
			gtfsProgressMeter.removeAttribute("max");
			gtfsProgressMeter.hidden = true;
		} else {
			if (gtfsProgressMeter.hidden) {
				gtfsProgressMeter.hidden = false;
			}
		}
	}

	/** Creates layers for the Shapes and Stops in a GTFS data object.
	 * @param {Object} gtfs
	 * @param {string} agencyId
	 * @returns {Object.<string, Layer>}
	 */
	function createLayersFromGtfs(gtfs, agencyId) {
		var stopsLayer, shapesLayer;

		/** Converts a GeoJSON feature collection into an array of Graphics and then adds them to a layer.
		 * @param {GraphicsLayer} layer
		 * @param {Terraformer.FeatureCollection} features
		 * @param {string} type - "shape" or "stop"
		 * @return {Graphic[]}
		 */
		function addGraphicsFromGeoJsonFeatureCollection(layer, features) {
			var graphics = Terraformer.ArcGIS.convert(features);
			graphics.forEach(function (f) {
				var graphic = new Graphic(f);
				layer.add(graphic);
			});
			return graphics;
		}

		/** Creates a graphics layer
		 * @param {string} type - Either "shape" or "stop"
		 * @param {Terraformer.FeatureCollection} features
		 * @return {GraphicsLayer}
		 */
		function createLayer(type, features) {
			var suffix = type + "s", renderer;

			renderer = type === "stop" ? new SimpleRenderer({
				type: "simple",
				label: "",
				description: "",
				symbol: {
					color: [210, 105, 30, 191],
					size: 6,
					angle: 0,
					xoffset: 0,
					yoffset: 0,
					type: "esriSMS",
					style: "esriSMSCircle",
					outline: {
						color: [0, 0, 128, 255],
						width: 0,
						type: "esriSLS",
						style: "esriSLSSolid"
					}
				}
			}) : null;

			var layer = new GraphicsLayer({
				id: [agencyId, suffix].join("-"),
				className: ["gtfs", suffix].join("-"),
				styling: Boolean(renderer)
			});

			if (renderer) {
				layer.setRenderer(renderer);
			}
			addGraphicsFromGeoJsonFeatureCollection(layer, features, type);

			return layer;
		}

		stopsLayer = createLayer("stop", gtfs.Stops);
		shapesLayer = createLayer("shape", gtfs.Shapes);

		map.addLayer(shapesLayer);
		map.addLayer(stopsLayer);

		return {
			stopsLayer: stopsLayer,
			shapesLayer: shapesLayer
		};
	}
	
	/** Gets the GTFS data for the currently selected agency.
	 */
	function getGtfsData() {
		var agencyId, url, feedRequest;

		function handleFeedData(/*{XMLHttpRequestProgressEvent}*/ e) {
			var gtfs, layers;
			if (e.target.status === 200) {
				// Process the GTFS data if available.
				gtfs = e.target.response;
				gtfs = new Gtfs(gtfs);
				if (gtfs) {
					// Disable the option in the select for this agency so that its data can't be added a second time.
					document.querySelector("option[value=" + agencyId + "]").disabled = true;
					// Reset the select to the first element, "Select an agency...".
					agencySelect.selectedIndex = 0;
					importButton.disabled = true;

					gtfs = e.target.response;
					layers = createLayersFromGtfs(gtfs, agencyId);
				} else {
					alert('Server returned "OK" status, but no GTFS data.');
				}
			} else {
				alert("Error loading GTFS data");
			}
		}

		agencyId = agencySelect.value;
		url = "api/feed/" + agencyId;
		

		feedRequest = new XMLHttpRequest();
		feedRequest.responseType = "json";
		feedRequest.addEventListener("loadstart", updateProgressMeter);
		feedRequest.addEventListener("progress", updateProgressMeter);
		feedRequest.addEventListener("loadend", handleFeedData);
		feedRequest.addEventListener("loadend", updateProgressMeter);
		feedRequest.open("GET", url);
		feedRequest.send();
	}

	// Load agency data
	var agenciesRequest = new XMLHttpRequest();
	agenciesRequest.open("GET", "api/agencies");
	agenciesRequest.responseType = "json";
	agenciesRequest.addEventListener("progress", updateProgressMeter);
	agenciesRequest.addEventListener("loadend", populateAgenciesSelect);
	agenciesRequest.send();

	// Setup the import button click event.
	importButton.addEventListener("click", getGtfsData);
});
