/*
 * C:\A\Exe\2021FirefoxAddons\wpsequencer\wpsequencer-background.js
 */

// This BS background script will:
// Make definitions which will remain after BS run exits;
// Open a persistent bi-directional port to C# native app.
// It will also run the C# native app until addon unloads.
// That BS-C# happens immediately upon loading the addon.

// Upon a first, or any later, operator click on addon icon:
// Upload/Execute a CS content script to current active tab,
// CS script opens a persistent bi-directional port with BS.
// I think CS script remains active across Whitepages pages.


// console.log messages from this BS appear in the ADDON's Inspect console.
console.log("wpsequencer-background.js entered");


// ====================================================
// This section pertains to an Addon-Click uploading CS:
// ====================================================


// generic promises->error routine, unused:

// function onError(error) {
// 	console.log(`Error: ${error}`);
// }

// A more specific, robust promises->error routine:
// Display any error from executing content script.

function onCSError(error) {
	console.log(`CS Error: ${error}`);
	console.log("error.name = " + error.name);
	console.log("error.message = " + error.message);
	console.log("error.fileName = " + error.fileName);
	console.log("error.lineNumber = " + error.lineNumber);
	console.log("error.columnNumber = " + error.columnNumber);
}

// Otherwise, this promises->success routine

function onActiveCSExecuted(tab) {
	console.log("CS has executed in active tab:")
}

// During BS execution, while loading the addon,
// add a listener for future addon-icon-clicks.

browser.browserAction.onClicked.addListener(() => {
	console.log("BS heard your Click on addon icon")

	// Upload/execute the CS in current tab:
	var executingCS = browser.tabs.executeScript({
		file: "/wpsequencer-content-script-stateful.js"
		// file: "/wpsequencer-content-script-scrape-all-tags.js"
	});
	executingCS.then(onActiveCSExecuted, onCSError);
});


// ====================================================
// This section pertains to connecting to native C# app:
// ====================================================

// During BS execution, while loading the addon,
// immediately connect to the "wpsequencer" C# native app.

var nativePort = browser.runtime.connectNative("com.changethistoyourname.wpsequencer");


// During BS execution, while loading the addon,
// add a listener for messages from the native C# app.

nativePort.onMessage.addListener((m) => {
	// Serialized JSON text will be a dict {} with all class member name(s):
	// E.g., thus after 4 count bytes: ---{"cmd":"btn1","param":"clckd"}---.
	console.log("BS received from C#: cmd=(" + m.cmd + "), param=(" + m.param + ")");

	// forward these messages to the current CS, if any.
	portFromCS.postMessage(m);
});


// ====================================================
// This section pertains to connecting the CS to the BS:
// ====================================================


// BS part of unvarnished code steal from example:
// https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API/runtime/connect

// background-script.js

// define port and func for the next act below:

var portFromCS;

function connectedFromCS(p) {
  portFromCS = p;
  portFromCS.onMessage.addListener(function(m) {
	console.log("BS received from CS: cmd=(" + m.cmd + "), param=(" + m.param + ")");
	// forward these messages to the native C# app port...
	nativePort.postMessage(m);
  });
  // Don't. This comes faster than the C# app has form up:
  // portFromCS.postMessage({cmd:"greeting", param:"hi there content script!"});
}

// During BS execution, while loading the addon,
// add above listener for messages from any future CS.

browser.runtime.onConnect.addListener(connectedFromCS);

console.log("wpsequencer-background.js exited");
