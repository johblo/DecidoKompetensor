$(document).ready(function () {
    // on resize
    hideFooter();
    jQuery(window).resize(function (e) {
        fixFooterPosition();
    });
    // on load
    fixFooterPosition();
    showFooter();

    $("#s4-leftpanel-content ul.root>li.static>a").css("margin-left", "9px");
    $("#s4-leftpanel-content ul.root>li.static").css("background", "url('/_layouts/images/Menu-right.gif') no-repeat 5px 7px");
    $("#s4-leftpanel-content ul.root>li.static>ul.static>li.selected").parent().parent().css("background", "url('/_layouts/images/Menu1.gif') no-repeat 5px 7px");
    $("#s4-leftpanel-content ul.root>li.static").css("border-bottom", "1px Solid #8CB2CE");

    $("#s4-leftpanel-content li.static>ul.static").each(function () {
        $(this).hide();
    });

    $("#s4-leftpanel-content ul.root>li.static>a").click(function (ev) {
        //ev.preventDefault();                
        var child = $(this).parent().children('ul');
        $("#s4-leftpanel-content li.static>ul.static").each(function () {
            $(this).hide("slow");
        });
        child.toggle("slow");
    });
    //disable heading click            
    $("#s4-leftpanel-content ul.root>li.static>a").toggle(
function () { },
function () { }
);
    $("#s4-leftpanel-content ul.root>li.static>ul.static>li.selected").parent().show();

}); 



 function fixFooterPosition() {
    var ribbonH = jQuery("#s4-ribbonrow").height();
    var footerH = jQuery("#footer").height();
    var windowHeight = jQuery(window).height();
    var h = windowHeight - footerH - ribbonH;
    var bodyContainer = jQuery("#s4-bodyContainer");
    if (h >= bodyContainer.height()) {
        bodyContainer.height(h);
    }
    else 
    {
        //reset height – important on resizing  
        bodyContainer.css("height","auto");
    }

 }

 function hideFooter() {
     $("#footer").css("display", "none");
 }
 function showFooter() {
     $("#footer").css("display", "inline");
 }