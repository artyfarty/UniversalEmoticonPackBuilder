// ==UserScript==
// @name           Gmail Smiley Extender : __PACKNAME__
// @description    Add extra emojii to your Gmail chat!
// @author         1nfected & __PACKAUTHOR__
// @version        __PACKVERSION__
// @namespace      1nfected
// @license        CC by-nc-sa http://creativecommons.org/licenses/by-nc-sa/3.0/

// @include        http://mail.google.com/*
// @include        https://mail.google.com/*
// ==/UserScript==


(function() {

// -------- USER CONFIGURABLE OPTIONS ------- //

// User defined CUSTOM SMILEYS
/* Declare your custom smileys here in the following format:

	var customSmileys = [
		[<REGEXP>, <FULL_PATH_TO_SMILEY>],
		[<REGEXP>, <FULL_PATH_TO_SMILEY>]
	];
	 
	EXAMPLE:
	var customSmileys = [
		[/:lol:/      ,'http://www.example.com/lol.gif'],
		[/:roflmao:/  ,'http://example.com/smileys/roflmao.png'],
		[/lmao/		  ,'https://ex.ample.org/laugingmyassoff.jpg']
	];
*/

var customSmileys;

// ------ END USER CONFIGURABLE OPTIONS ------ //

// -------- DONT EDIT BELOW THIS LINE -------- //

try { if(self != window.top) { return; } }
catch(e) { return; }

testGM();

var smileys, smileyURL;

var version = '0.5';
var scriptid = 77439;

// Test for Greasemonkey API & adapt accordingly
function testGM() {
	const LOG_PREFIX = 'Gmail Smiley Extender: ';
	const LOG = true;
	const DEBUG = false;
	isGM = typeof GM_getValue != 'undefined' && typeof GM_getValue('a', 'b') != 'undefined';
	log = isGM ? function(msg) { if(LOG) GM_log(msg) } : window.opera ? function(msg) { if(LOG) opera.postError(LOG_PREFIX+msg); } : function(msg) { try { if(LOG) console.info(LOG_PREFIX+msg); } catch(e) {} }
	debug = function(msg) { if(LOG && DEBUG) log('** Debug: '+msg+' **') }
}


// All in one function to get elements
function $(q,root,single,context) {
	root = root || document;
	context = context || root;
	if(q[0] == '#') return root.getElementById(q.substr(1));
	if(q.match(/^[\/*]|^\.[\/\.]/)) {
		if(single) return root.evaluate(q,context,null,9,null).singleNodeValue;
		var arr = []; var xpr = root.evaluate(q,context,null,7,null);
		for(var i = 0, len = xpr.snapshotLength; i < len; i++) arr.push(xpr.snapshotItem(i));
		return arr;
	}
	if(q[0] == '.') {
		if(single) return root.getElementsByClassName(q.substr(1))[0];
		return root.getElementsByClassName(q.substr(1));
	}
	if(single) return root.getElementsByTagName(q)[0];
	return root.getElementsByTagName(q);
}

function addStyle(css) {
	var head = $('head')[0];
	if(!head) return;
	var style = document.createElement('style');
	style.type = 'text/css';
	style.appendChild(document.createTextNode(css));
	head.appendChild(style);
}

// Waits for a given set of Elements to load. Takes a callback as argument which is called if all the elements are found.
// mode == 0 : callback only if all the elements are found. (DEFAULT)
// mode == 1 : callback even if none of the elements are found.
// mode == 2 : callback immed if any single element is found.
function $W(Q,callback,mode,t) {
	t = t || 1; mode = mode || 0;
	var arr = Q instanceof Array;
	var l = arr ? Q.length : 1;
	var matches = [];
	for(var i = 0; i < l; i++) {
		var O = arr ? Q[i] : Q;
		var q = O.q || O, r = O.r, s = O.s, c = O.c;
		var T = O.t || 10, I = O.i || 250, N = O.n, F = O.f;
		var match = $(q,r,s,c);
		if(match && match.length == 0) match = null;
		if(match) { matches.push(match); if(mode == 2) { break; } }
		else {
			if(i !== (l-1) && mode == 2) { continue; }
			if(t >= T) {
				if(F) log(F);
				if(mode !== 1)
					return;
			}
			else {
				if(N) debug(t+' - '+N+' in '+t*I+'ms...');
				window.setTimeout(function(){$W(Q,callback,mode,++t)},t*I);
				return;
			}
		}
	}
	if(typeof callback == 'function') {
		if(matches.length == 1) matches = matches[0];
		if(matches.length == 0) matches = null;
		callback(matches);
	}
}

window.addEventListener('load', init, false);

function init() {
	$W({q:'.no',t:20,i:150,r:document,s:true,n:'Finding root element...',f:'Failed to find root element!'},chatHook,2);

	smileys = [
		__PACKDATA__
	];
	smileyURL = '__PACKURL__';
	
	for(var i = smileys.length-1; i >= 0; i--) {
		smileys[i][0] = new RegExp(addslashes(smileys[i][0]), "gim");
		smileys[i][1] = smileyURL+smileys[i][1];
	}
	
	addStyle(".smileyext{margin-top:-2px;vertical-align:middle}");
}

function chatHook(match) {
	match.addEventListener('DOMNodeInserted',function(event) {
		var source = event.target;
		if(source.className == 'km' || source.className == 'kl' || source.className == 'Z8Dgfe') {
			insertSmiley(source);
		}
	},false);
	
	log('Smiley Extender started.');
}

function insertSmiley(node) {
	for(var i = smileys.length-1; i >= 0; i--) {
		var smileyRegex = smileys[i][0];
		var smileyImg = smileys[i][1];
		if (node.innerHTML.match(smileyRegex)) {
			node.innerHTML = node.innerHTML.replace(smileyRegex,' <img class="smileyext" src="'+smileyImg+'"> ');
			
			node.addEventListener('DOMSubtreeModified',function(event) {
				insertSmiley(node);
			},false);
		}
	}
}

function addslashes( str ) {    // Quote string with slashes
    //return str.replace('/(["\'\])/g', "\\$1").replace('/\0/g', "\\0");
	return str.replace(/([\\\(\)\[\]\+\-\*])/g, "\\$1");
}


})();