_spBodyOnLoadFunctionNames.push("OnPageLoad");

function OnPageLoad() {

adjustFooter();
window.onresize = adjustFooter;

}

function adjustFooter(){

	var viewportHeight;
	var bodyHeight;
	var footerHeight;
	var ribbonHeight;
	
	if (typeof window.innerHeight != 'undefined')
	{
	     viewportHeight = window.innerHeight
	}
	else
	{
		viewportHeight = document.documentElement.clientHeight;
	}
	
	bodyHeight = document.getElementById("s4-bodyContainer").clientHeight;
	footerHeight = document.getElementById("footer").clientHeight;
	ribbonHeight = document.getElementById("s4-ribbonrow").clientHeight;
	
	if(viewportHeight > (bodyHeight + footerHeight))
	{	
		var x = viewportHeight - (bodyHeight + footerHeight + ribbonHeight);
		var y = x + bodyHeight;
		document.getElementById("s4-bodyContainer").style.height = y + "px";
	}
//alert("bh=" +bodyHeight + "| vph=" + viewportHeight + "| fh=" + footerHeight); 
}