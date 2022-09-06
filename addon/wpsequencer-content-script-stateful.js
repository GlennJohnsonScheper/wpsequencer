/*
 * C:\A\Exe\2021FirefoxAddons\wpsequencer\wpsequencer-content-script-stateful.js
 */

// old stuff, keeping it a while:

function PasteTwoTextsAndSubmit(s1, s2) {
	document.body.style.border = "15px solid lightblue";
	console.log("PasteTwoTextsAndSubmit() entered");
	var form = document.querySelector("form");
	if(form != null) {
		// console.log("found form");
		var txt1 = form.querySelector("input[placeholder='e.g. Jon Snow']");
		var txt2 = form.querySelector("input[placeholder='City, State, or ZIP']");
		var btn = form.querySelector("button[id='wp-search']");
		
		// allow for this alternate button after the first time:
		if(btn == null)
		{
			btn = form.querySelector("button[class='btn search-icon mr-3']");
		}

		if(txt1 != null && txt2 != null && btn != null) {
			console.log("Okay, found txt1, txt2, btn");

			console.log("about to set txt1 to " + s1);

			txt1.dispatchEvent(new Event('focus'));
			txt1.value = s1;
			txt1.dispatchEvent(new Event('input'));
			txt1.dispatchEvent(new Event('change'));
			txt1.dispatchEvent(new Event('blur'));


			console.log("about to set txt2 to " + s2);

			txt2.dispatchEvent(new Event('focus'));
			txt2.value = s2;
			txt2.dispatchEvent(new Event('input'));
			txt2.dispatchEvent(new Event('change'));
			txt2.dispatchEvent(new Event('blur'));

			console.log("Ready to submit...");
			
			btn.click();
		}
		else
		{
			console.log("Error, did not find txt1, txt2, btn");		
		}
	}
	console.log("PasteTwoTextsAndSubmit() exited");
	document.body.style.border = "15px solid darkblue";
}

function ScrapeAndSendAnyLandlines(){
	document.body.style.border = "15px solid gold";
	console.log("ScrapeAndSendAnyLandlines() entered");

	var phones = "?"; // failure reply meaning no landlines found
	
	var div = document.querySelector("div[id='landline']");
	if(div != null) {
		console.log("found div[id='landline']");
		// There may be 0, 1 or 2 anchors, like this:
		//  href="/phone/1-512-894-4028" 
		var nodeList = div.querySelectorAll("a");
		if(nodeList.length >= 1)
		{
			console.log("found a1");
			// set the first phone over the "?"
			var url = nodeList[0].getAttribute("href");
			if(url.startsWith("/phone/1-"))
			{
				console.log("I like a1");
				phones = url.replace("/phone/1-", "");				
			}
		}
		if(nodeList.length >= 2)
		{
			console.log("found a2");
			// append the second phone after a space
			var url = nodeList[1].getAttribute("href");
			if(url.startsWith("/phone/1-"))
			{
				console.log("I like a2");
				// THIS IS NOT THE ACTIVE CODE, LOOK DOWN BELOW!
				// I had a weird phone in #1 that spoiled the
				// MySQL save due to a space in front of #2.
				if(phones == "?") {
					phones = url.replace("/phone/1-", "");				
				}
				else {
					phones = phones + " " + url.replace("/phone/1-", "");				
				}
			}
		}
		myPort.postMessage({cmd:"phones", param:phones});
	}
	else
	{
		console.log("cannot find div[id='landline']");		
	}
	
	if(phones != "?")
	{
		// success
		document.body.style.border = "15px solid goldenrod";
	}
	else
	{
		// failure
		document.body.style.border = "15px solid crimson";
		myPort.postMessage({cmd:"phones", param:"?"});
	}
};

/*
 * What an awesome new day dawning for scraping web pages.
 * The CS has a period timer to examine current situation,
 * just like a human looking at page, and to take actions.
 */

// console.log messages from this CS appear in the webpage's console.

console.log("wpsequencer-content-script-stateful.js entered");
document.body.style.border = "15px solid yellow";

// CS part of unvarnished code steal from example:
// https://developer.mozilla.org/en-US/docs/Mozilla/Add-ons/WebExtensions/API/runtime/connect

// content-script.js

var myPort = browser.runtime.connect({name:"port-from-cs"});

myPort.onMessage.addListener(function(m) {
	console.log("CS received from BS: cmd=(" + m.cmd + "), param=(" + m.param + ")");
	
	// These original commands are still useful if script is stuck:
	
	if(m.cmd == "blue") // PRESS: 1. BLUE BUTTOn, 2. GOLD BUTTON
	{
		// document.body.style.border = "15px solid blue";
		var nameLocn = m.param.split('/');
		PasteTwoTextsAndSubmit(nameLocn[0], nameLocn[1]);
	}

	if(m.cmd == "gold") // PRESS: 1. BLUE BUTTOn, 2. GOLD BUTTON
	{
		// document.body.style.border = "15px solid gold";
		ScrapeAndSendAnyLandlines();
	}

	
	// After CS sent: myPort.postMessage({cmd:"need", param:"nameLocnAge"});
	// Process the expected C# native app response of the same name:
	if(m.cmd == "nameLocnAge")
	{
		// Now, I expect param to contain 3 segments, with 2 slashes:
		var nameLocnAge = m.param.split('/');
		queryName = nameLocnAge[0];

		// Take off certain suffixes:
		if(queryName.endsWith(" SR")
		|| queryName.endsWith(" JR")
		) {
			queryName = queryName.substring(0, queryName.length-3);
		}
		else if(queryName.endsWith(" III")
		) {
			queryName = queryName.substring(0, queryName.length-4);
		}
		
		// make a couple simple alternatives to queryName:
		var qns = queryName.split(" ");
		if(qns.length >= 3) {
			// likely I have a (first, middle, last) or more...
			var tail = qns.slice(2); // this omits the first two items
			// make alternative 4 first, before I change middle name:
			queryName4 = ""; // in case middle name is not one letter.
			queryName4Regex = null;
			if(qns[1].length == 1) {
				// alternative 4 = expand middle initial to match any name
				queryName4 = qns[0] + " " + qns[1] + "[A-Z]* " + tail.join(' ');
				queryName4Regex = new RegExp(queryName4);
			}
			// alternative 2 = shorten middle name to a letter
			qns[1] = "" + qns[1].charAt(0);
			queryName2 = qns.join(' ');
			// alternative 3 = omit middle name
			queryName3 = qns[0] + " " + tail.join(' ');
		}
		
		// console.log("QN1: " + queryName);
		// console.log("QN2: " + queryName2);
		// console.log("QN3: " + queryName3);
		// console.log("QN4: " + queryName4);

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

// Here is a new experiment, to have the CS drive the process.
// 1. Have a state variable describing the steps in the sequence.
// 2. Periodically examine the DOM for some condition to proceed.
// 3. When needed, send a query to C# for the next query strings.
// 4. I will need some rules to solve which person to choose.
// - Temporarily, the process could stop there until I click one.
// 5. When available, send the phones back to the C# to save SQL.

function RecognizeInitialPage() {
	var form = document.querySelector("form");
	if(form != null) {
		var txt1 = form.querySelector("input[placeholder='e.g. Jon Snow']");
		var txt2 = form.querySelector("input[placeholder='City, State, or ZIP']");
		var btn = form.querySelector("button[id='wp-search']");
		// allow for this alternate button after the first time:
		if(btn == null)
		{
			btn = form.querySelector("button[class='btn search-icon mr-3']");
		}
		if(txt1 != null && txt2 != null && btn != null) {
			return true;
		}
	}
	return false;
}

function InstallNameText(s1) {
	var form = document.querySelector("form");
	if(form != null) {
		var txt1 = form.querySelector("input[placeholder='e.g. Jon Snow']");
		if(txt1 != null) {
			txt1.dispatchEvent(new Event('focus'));
			txt1.value = s1;
			txt1.dispatchEvent(new Event('input'));
			txt1.dispatchEvent(new Event('change'));
			txt1.dispatchEvent(new Event('blur'));
			return true;
		}
	}
	return false;
}

function InstallLocnText(s2) {
	var form = document.querySelector("form");
	if(form != null) {
		var txt2 = form.querySelector("input[placeholder='City, State, or ZIP']");
		if(txt2 != null) {
			txt2.dispatchEvent(new Event('focus'));
			txt2.value = s2;
			txt2.dispatchEvent(new Event('input'));
			txt2.dispatchEvent(new Event('change'));
			txt2.dispatchEvent(new Event('blur'));
			return true;
		}
	}
	return false;
}

function ClickSubmitButton() {
	var form = document.querySelector("form");
	if(form != null) {
		var btn = form.querySelector("button[id='wp-search']");
		// allow for this alternate button after the first time:
		if(btn == null)
		{
			btn = form.querySelector("button[class='btn search-icon mr-3']");
		}
		if(btn != null) {
			btn.click();
			return true;
		}
	}
	return false;
}

function RecognizeSubmissionResults() {
	var div = document.querySelector("div[id='desktop-search-filters']");
	if(div != null) {
		return true;
	}	
	return false;	
}

function RecognizeResultsPageByMonitorCheckBox() {
	var label = document.querySelector("label[id='report-monitor-switch']");
	if(label != null) {
		var inputX = label.querySelector("input[type='checkbox']");
		if(inputX != null) {
			return true;
			// inputX.click(); // raises a modal for user to choose.
		}
	}
	return false;
}

function RaiseThePopupDialog() {
	var label = document.querySelector("label[id='report-monitor-switch']");
	if(label != null) {
		var inputX = label.querySelector("input[type='checkbox']");
		if(inputX != null) {
			inputX.click(); // raises a modal for user to choose.
		}
	}
}

function RecognizeTheStopMonitoringButtonOfPopup() {
	var btn = document.querySelector("button[class='btn btn--large btn--outlined w-100 primary--text']");
	if(btn != null) {
		return true;
	}	
	return false;
}


function ClickTheStopMonitoringButtonOfPopup() {
	var btn = document.querySelector("button[class='btn btn--large btn--outlined w-100 primary--text']");
	if(btn != null) {
		btn.click();
	}	
}

function RecognizeLandlineResults(){
	var div = document.querySelector("div[id='landline']");
	if(div != null) {
		// There should be 1 or 2 anchors, like this:
		//  href="/phone/1-512-894-4028" 
		var nodeList = div.querySelectorAll("a");
		return nodeList.length;
	}
	return 0;
}

function SendBackLandlineResults(){
	var div = document.querySelector("div[id='landline']");
	if(div != null) {
		// There may be 0, 1 or 2 anchors, like this:
		//  href="/phone/1-512-894-4028" 
		var phones = "";
		var nodeList = div.querySelectorAll("a");
		if(nodeList.length >= 1)
		{
			var url = nodeList[0].getAttribute("href");
			if(url.startsWith("/phone/1-"))
			{
				phones = url.replace("/phone/1-", "");				
			}
			// This was a weird 0- instead of 1- case:
			if(url.startsWith("/phone/0-"))
			{
				phones = url.replace("/phone/0-", "");				
			}
		}
		if(nodeList.length >= 2)
		{
			// append the second phone after a space
			var url = nodeList[1].getAttribute("href");
			if(url.startsWith("/phone/1-"))
			{
				phones = phones + " " + url.replace("/phone/1-", "");				
			}
			if(url.startsWith("/phone/0-"))
			{
				phones = phones + " " + url.replace("/phone/0-", "");				
			}
		}
		// just in case of a similar gotcha,
		phones = phones.trim();
		myPort.postMessage({cmd:"phones", param:phones});
	}
	else
	{
		myPort.postMessage({cmd:"phones", param:"?"}); // meaning, none found
	}
}

var selectedImgNodeToClick = null;

function AbleToApplyChoiceHeuristics(){
	console.log("AbleToApplyChoiceHeuristics() entered.");

	// I found the perfect CSS selector: a[href^="/name/"] to select
	// every <a> element whose href attr. value begins with "/name/"
	//
	// But now, I have a more precise selector:
	// The <a> of interest to me also have one of these two classes:
	// Line 2335: attr: /class/ = /pos-rl cs-p _2mGd /
	// Line 2609: attr: /class/ = /pos-rl cs-p _2mGd _3LNC/
	
	var nodeList = document.querySelectorAll("a[class^='pos-rl cs-p _2mGd']");
	myPort.postMessage({cmd:"nodeListlength", param:nodeList.length});
	// that much was good, I get N nodes...
	for(var i = 0; i < nodeList.length; i++) {
		// omit contents that hang CS....
		// oops, wrong case of L in nodeList!

		// Within each big <a> node covering a large rectangle,
		// find these two subelements:
		// 1. The age: I need just the immediate child text of a div:
		var div = nodeList[i].querySelector("div[class='_2MbI subtitle-1']");
		
		// from that I must trim various whitespaces to get,
		// e.g., "40s" or "80+" to compare to the passed age.
		// 2. The IMG to click:
		var img = nodeList[i].querySelector("img[alt='phone number']");

		if(div != null && img != null) {
			// What a mess getting a little #text child!
			// Rather, see match in a whole wad of text.
			var yes1 = 0;
			var yes2 = 0;

			// The queryAge string was perfected in C# (lowercase s).
			if(div.textContent.includes(queryAge)) {
				console.log("Matching queryAge[#" + i + "] = [" + queryAge + "]");				
				yes1 = 1;
			}
			
			// again, look in a whole wad of text of <a>.
			var nlituc = nodeList[i].textContent.toUpperCase();
			
			// The queryName string was sent from C# as uppercase.
			if(nlituc.includes(queryName)) {
				console.log("Matching queryName[#" + i + "] = [" + queryName + "]");				
				yes2 = 1;
			}
			
			if(nlituc.includes(queryName2)) {
				console.log("Matching queryName2[#" + i + "] = [" + queryName2 + "]");				
				yes2 = 1;
			}
			
			if(nlituc.includes(queryName3)) {
				console.log("Matching queryName3[#" + i + "] = [" + queryName3 + "]");				
				yes2 = 1;
			}
			
			if(queryName4Regex != null
			&& nlituc.match(queryName4Regex)) {
				console.log("Matching queryName4(Regex)[#" + i + "] = [" + queryName4 + "]");				
				yes2 = 1;
			}
			
			if(yes1 && yes2) {
				// VOILA! QED! Click on the img to take the next step!
				console.log("VOILA! QED! Selecting img #" + i);
				selectedImgNodeToClick = img;
				// Soon, not yet ... img.click();
				// stop looking at others...
				console.log("AbleToApplyChoiceHeuristics() exited (1).");
				return 1;
			}
		}
	}
	console.log("AbleToApplyChoiceHeuristics() exited (0).");
	return 0;
}



var state = "initial";
//var state = "stopper";

var six = ["00", "33", "66", "99", "cc", "ff"];


var queryName = "";
var queryName2 = ""; // simple alternative 2
var queryName3 = ""; // simple alternative 3
var queryName4 = ""; // complex regexp alt 4
var queryName4Regex = ""; // complex regexp alt 4
var queryLocn = "";
var queryAge = "";
var gotQuery = false;

var gotSaved = false;

var ms0LetUser = 0;
var ms0SvrDelay = 0;
var msRandomTimeout = 0;

function PeriodicProcess() {
	var msExtraDelay = 100; // default minimum state change rate
	var red = 1 + Math.floor(Math.random() * 4); // midlin 1-4
	var grn = 1 + Math.floor(Math.random() * 4); // midlin 1-4
	var blu = 1 + Math.floor(Math.random() * 4); // midlin 1-4
	var randomColor = "#" + six[red] + six[grn] + six[blu];
	document.body.style.border = "15px solid " + randomColor;
	
	console.log("entering " + state);
	switch(state) {
		
		case "initial":
			if(RecognizeInitialPage()) {
				state = "requestQueryParams";
			}
			break;

		case "requestQueryParams":
			queryName = "";
			queryName2 = "";
			queryName3 = "";
			queryLocn = "";
			queryAge = "";
			gotQuery = false; // disarm beforehand
			myPort.postMessage({cmd:"need", param:"nameLocnAge"});
			state = "awaitQueryParams";
			break;
		
		case "awaitQueryParams":
			// WHEN RX PACKET COMES, THIS WILL BE SET:
			if(gotQuery) {
				state = "haveQueryParams";
				gotQuery = false; // disarm for next time.
			}
			break;
		
		case "haveQueryParams":
			state = "InstallTxt1QueryParam";
			break;
		
		case "InstallTxt1QueryParam":
			InstallNameText(queryName);
			state = "InstallTxt2QueryParam";
			break;
		
		case "InstallTxt2QueryParam":
			InstallLocnText(queryLocn);
			state = "ClickSubmitButton";
			break;
		
		case "ClickSubmitButton":
			ClickSubmitButton();
			// First, start a delay for server to reply before I act:
			// otherwise, when starting over from a failed search,
			// the current/old page already shows clues to proceed.
			ms0SvrDelay = Date.now();
			state = "someDelayForServerToReply";
			break;
		
		case "someDelayForServerToReply":
			msExtraDelay = 1000;
			ms1SvrDelay = Date.now();
			// 4 sec is not enough. 8 sec? still narrow. 12.
			if(ms1SvrDelay - ms0SvrDelay > 12000)
				state = "recognizeSubmissionResults";
			break;
		
		case "recognizeSubmissionResults":
			if(RecognizeSubmissionResults()) {
				selectedImgNodeToClick = null;
				if(AbleToApplyChoiceHeuristics()) {
					state = "addonHasChoosen";
					// pretend extra human consideration happened here.
					msExtraDelay = Math.floor(Math.random() * 3000 + Math.random() * 4000);
					document.body.style.border = "15px dotted black"; // different dots FYI
				}
				else {
					// original plan left me in the loop:
					state = "letUserChoose";
					// Ok, go there, but start a delay to act:
					ms0LetUser = Date.now();
					// also a random timeout:
					msRandomTimeout = 10000 + Math.random() * 30000;
				}
console.log("at 4");
			}
			else {
				// I had one result get stuck at this switch case,
				// saying "Sorry, we didn't find any results for"

				var sorry = document.querySelector("div[class='display-1 mb-8']");

				// Oops! Here was my new code hang.
				// I was not testing for null first!
				if(sorry != null
				&& sorry.textContent.includes(
				"Sorry, we didn't find any results for")) {
					// this is another place I need to escape from,
					// to the initial state...
					// as elsewhere, not so good... state = "initial";
					// This state should find none, and save "?" nicely:
					state = "processAnyLandlines";
				}
			}
			break;
		
		case "addonHasChoosen":
			// Addon has chosen, but I delayed the click until now:
			document.body.style.border = "15px solid white"; // after addon made a choice
			if(selectedImgNodeToClick != null) {
				console.log("VOILA! QED! Clicking on that IMG!");
				selectedImgNodeToClick.click();
				selectedImgNodeToClick = null;
			}
			// otherwise, a variation on letUserChoose case.
			msExtraDelay = 100; // undo slow during AbleToApplyChoiceHeuristics
			// The next page is already coming. Watch for it:
			if(RecognizeResultsPageByMonitorCheckBox()) {
				state = "raiseThePopupDialog";
			}			
			break;

		case "letUserChoose":
console.log("at 10");
			msExtraDelay = 1000; // slower during "letUserChoose"
console.log("at 11");
			document.body.style.border = "15px dashed black"; // user choice required
console.log("at 12");
			// while in this state, if/when the human
			// user chooses some choice, and advances
			// to next page, recognize that page and
			// make the appropriate automatic actions:
			if(RecognizeResultsPageByMonitorCheckBox()) {
console.log("at 13");
				msExtraDelay = 100; // undo slow during "letUserChoose"
				document.body.style.border = "15px solid white"; // after user made a choice
				state = "raiseThePopupDialog";
console.log("at 14");
			} else {
console.log("at 15");
				// in the long absence of human user acting,
				// save the no-phones answer and start over.
				var ms1LetUser = Date.now();
				// 25 seconds?
				if(ms1LetUser - ms0LetUser > msRandomTimeout) {
console.log("at 16");
					document.body.style.border = "15px dotted yellow"; // call my attention
					console.log("Acting in the absence of human agency.");
					// nah, not so good... myPort.postMessage({cmd:"phones", param:"?"}); // meaning, none found
					// nah, not so good... state = "initial";
					// This state should find none, and save "?" nicely:
					state = "processAnyLandlines";
console.log("at 17");
				}
console.log("at 18");
			}
			
console.log("at 19");
			break;

		case "raiseThePopupDialog":
			RaiseThePopupDialog();
			// msExtraDelay = 500; // after calling RaiseThePopupDialog()
			state = "recognizeTheStopButton";
			break;

		case "recognizeTheStopButton":
			if(RecognizeTheStopMonitoringButtonOfPopup()){
				state = "ClickTheStopButton";
			}
			break;

		case "ClickTheStopButton":
			ClickTheStopMonitoringButtonOfPopup();
			// msExtraDelay = 500; // after calling ClickTheStopMonitoringButtonOfPopup()
			state = "analyzeForLandlines";
			break;

		case "analyzeForLandlines":
			switch(RecognizeLandlineResults()) {
				case 0:
					document.body.style.border = "15px dotted red"; // red for zero
					msExtraDelay = 500; // after finding zero results
					break;
				case 1:
					document.body.style.border = "15px dotted green"; // green for 1
					msExtraDelay = 1000; // after finding one result
					break;
				case 2:
					document.body.style.border = "15px dotted blue"; // blue for 2
					msExtraDelay = 1500; // after finding two results
					break;
			}
			state = "processAnyLandlines";
			break;

		case "processAnyLandlines":
			gotSaved = false; // disarm beforehand
			SendBackLandlineResults();
			// msExtraDelay = 500; // after SendBackLandlineResults
			state = "awaitNativeAppGotSaved";
			break;

		case "awaitNativeAppGotSaved":
			msExtraDelay = 1500; // during lengthy awaitNativeAppGotSaved
			if(gotSaved) {
				msExtraDelay = 100; // after awaitNativeAppGotSaved confirmed
				gotSaved = false; // disarm afterwards
				state = "initial";
			}
			break;

		case "stopper":
			msExtraDelay = 5000; // during stopper, if ever used
			document.body.style.border = "15px dashed red"; // error, user action required
			// state = "stopper";
			break;
	}
	console.log("exiting " + state);

	myPort.postMessage({cmd:"state", param:state});
	setTimeout(PeriodicProcess, msExtraDelay + Math.floor(Math.random() * 500 + Math.random() * 500));  // random from 0 to 1 sec, avg = 1/2 second
}

setTimeout(PeriodicProcess, 1000);


document.body.style.border = "15px solid lightsalmon";
console.log("wpsequencer-content-script-stateful.js exited");
