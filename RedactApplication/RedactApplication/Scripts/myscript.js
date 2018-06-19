$(document).ready(function(){ 
	$(".collapse").click(function () {

        $(this).parent().children().toggle();
        $(this).toggle();
        if($(this).children('i').text() == "+"){
			$(this).children('i').text('-');
		}
        else{
			$(this).children('i').text('+');
		}
    });
	
	//************************************************************
	
});