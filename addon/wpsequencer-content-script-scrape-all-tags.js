/*
 * C:\A\Exe\2021FirefoxAddons\wpsequencer\wpsequencer-content-script-scrape-all-tags.js
 */

console.log("wpsequencer-content-script-scrape-all-tags.js entered");
document.body.style.border = "15px solid red"

// Revised to use the bi-di port:


// CS part of unvarnished code steal from example:
// https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API/runtime/connect

// content-script.js

var myPort = browser.runtime.connect({name:"port-from-cs"});

myPort.onMessage.addListener(function(m) {
	console.log("CS received from BS: cmd=(" + m.cmd + "), param=(" + m.param + ")");
	// After CS sent: myPort.postMessage({cmd:"need", param:"nameLocnAge"});
	// Process the expected C# native app response of the same name:
	if(m.cmd == "nameLocnAge")
	{
		// Now, I expect param to contain 3 segments, with 2 slashes:
		var nameLocnAge = m.param.split('/');
		queryName = nameLocnAge[0];
		queryLocn = nameLocnAge[1];
		queryAge = nameLocnAge[2];
		gotQuery = true; // triggers next action
	}
	if(m.cmd == "phonesGotSaved")
	{
		// Now, I expect param to contain 3 segments, with 2 slashes:
		gotSaved = true; // triggers next action
	}
});

myPort.postMessage({cmd:"greeting", param:"hello from content script"});

var collectedContent = "";

function recursiveAppendContentOfNode(node) {
	if (node.nodeType === Node.TEXT_NODE
	) {
		if(/^\s+$/.test(node.textContent)) {
			// omit pure whitespaces
		} else {
			collectedContent = (collectedContent + "\r\nTEXT: /" + node.textContent + "/\r\n" );			
		}
	} else if(
		node.nodeType != Node.ELEMENT_NODE // e.g., comment nodes, many other types
		||
		typeof node.tagName === 'undefined' // CYA
	){	
	  // 1st ignore
	}
    else
	{
		var lcTagName = node.tagName.toLowerCase();
		if(
			lcTagName == "script"
			||
			lcTagName == "code"
			||
			lcTagName == "li-icon"
			||
			lcTagName == "meta"
			||
			lcTagName == "link"
			||
			lcTagName == "style"
			||
			lcTagName == "svg"
		) {
			// 2nd ignore
		}
		else {
			collectedContent = (collectedContent + "\r\n<" + lcTagName + ">\r\n");
			if(node.hasAttributes()) {
				var attrs = node.attributes;
				for(var i = 0; i < attrs.length; i++) {
					collectedContent += "\r\nattr: /" + attrs[i].name + "/ = /" + attrs[i].value + "/\r\n";
				}
			}
			for (let i = 0; i < node.childNodes.length; i++) {
				recursiveAppendContentOfNode(node.childNodes[i]);
			}
			collectedContent = (collectedContent + "\r\n</" + lcTagName + ">\r\n");
		}
	}
}

function processCurrentWebpage() {
	document.body.style.border = "15px solid gray";
	console.log("processCurrentWebpage() entered");
	
	// OLD WAY:
	// Now as C# uses Newtonsoft.json, send complex object:
	// var dict = {}; // create an empty object literal
	
	collectedContent = "";
	
	// collectedContent = (collectedContent + "\r\n ===== document.head =====\r\n");
	// recursiveAppendContentOfNode(document.head);
	
	collectedContent = (collectedContent + "\r\n ===== document.body =====\r\n");
	recursiveAppendContentOfNode(document.body);

	// Revised to use the bi-di port:
	myPort.postMessage({cmd:"capture", param:collectedContent});

	// OLD WAY:
	// transmit the collected content (as JSON).
	// dict.capture = collectedContent; // The C# app expects "capture"
	// browser.runtime.sendMessage(dict);
	
	console.log("processCurrentWebpage() exited");
	document.body.style.border = "15px solid gold";
}

document.body.style.border = "15px solid orange"
	
// Because this CS.js is invoked manually by a click,
// Now Call it! ... without complex timeouts, etc.
processCurrentWebpage();
console.log("wpsequencer-content-script-scrape-all-tags.js exited");
