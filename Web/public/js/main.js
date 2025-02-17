


jQuery(document).ready(function($) {

    'use strict';

        $(".Modern-Slider").slick({
            autoplay:true,
            speed:1000,
            slidesToShow:1,
            slidesToScroll:1,
            pauseOnHover:false,
            dots:true,
            fade: true,
            pauseOnDotsHover:true,
            cssEase:'linear',
           // fade:true,
            draggable:false,
            prevArrow:'<button class="PrevArrow"></button>',
            nextArrow:'<button class="NextArrow"></button>', 
          });

        $('#nav-toggle').on('click', function (event) {
            event.preventDefault();
            $('#main-nav').toggleClass("open");
        });


        $('.tabgroup > div').hide();
            $('.tabgroup > div:first-of-type').show();
            $('.tabs a').click(function(e){
              e.preventDefault();
                var $this = $(this),
                tabgroup = '#'+$this.parents('.tabs').data('tabgroup'),
                others = $this.closest('li').siblings().children('a'),
                target = $this.attr('href');
            others.removeClass('active');
            $this.addClass('active');
            $(tabgroup).children('div').hide();
            $(target).show();
          
        });



        $(".box-video").click(function(){
          $('iframe',this)[0].src += "&amp;autoplay=1";
          $(this).addClass('open');
        });

        $('.owl-carousel').owlCarousel({
            loop:true,
            margin:30,
            responsiveClass:true,
            responsive:{
                0:{
                    items:1,
                    nav:true
                },
                600:{
                    items:2,
                    nav:false
                },
                1000:{
                    items:3,
                    nav:true,
                    loop:false
                }
            }
        });
        $(function() {
            console.log('123');
            $( "#dialog-4" ).dialog({
                
                autoOpen: false,
                maxHeight:  500,
                width:600,
                modal: true,
            //  buttons: {
            //     OK: function() {$(this).dialog("close");}
            //  },
            });
            $( "#click" ).click(function() {
                $( "#survey" ).show();
                // $( "#dialog-4" ).dialog( "open" );
            });
        });


        var contentSection = $('.content-section, .main-banner');
        var navigation = $('nav');
        
        //when a nav link is clicked, smooth scroll to the section
        navigation.on('click', 'a', function(event){
            event.preventDefault(); //prevents previous event
            smoothScroll($(this.hash));
        });
        
        //update navigation on scroll...
        $(window).on('scroll', function(){
            updateNavigation();
        });
        //...and when the page starts
        updateNavigation();
        
        /////FUNCTIONS
        function updateNavigation(){
            contentSection.each(function(){
                var sectionName = $(this).attr('id');
                if (sectionName === 'featured') sectionName = 'class';
                var navigationMatch = $('nav a[href="#' + sectionName + '"]');
                navigationMatch.addClass('active-section');
            });
        }
        function smoothScroll(target){
            $('body,html').animate({
                scrollTop: target.offset().top
            }, 800);
        }


        $('.button a[href*=#]').on('click', function(e) {
          e.preventDefault();
          $('html, body').animate({ scrollTop: $($(this).attr('href')).offset().top -0 }, 500, 'linear');
        });
        
        
        
       
        
});

function toggleMember( id ) {
    $("#bar_"+id).slideToggle("slow");
    
    $("#member_"+id).hide();
    
}

function goto($name){
    document.location = $name ;
    
}
function gotopdf($name){
    //document.location = $name ;
    window.open(
        $name,
        '_blank' // <- This is what makes it open in a new window.
    );
}

function check( class_id,i ){
    event.stopPropagation();
    var enter = prompt("確定要刪除此課程的話，請輸入'" + class_id + "'");
    console.log(enter)
    if(enter==class_id){
        console.log("HERE")
        showLoading();
        document.getElementById("delete_class_"+i).submit();
        alert("刪除成功");
    }else if(enter==null){
        alert("取消刪除");
    }else {
        alert("輸入錯誤");
    }
}

async function search_class(class_id, table_i,class_name,id){
    console.log("js SUCESS")
    console.log(class_id)
    await $.ajax({
        url: '/class_member_search',
        type: "POST",
        data:{
            class_id:class_id
        },
        dataType: "json",
        success:function(data){
            
            if(data.length>0){
                var str1 = '<th id ="asd"colspan="3">成員名單</th>';
                var str2 = '<td id="td"></td>';
                
                var tbody = $('#member_'+table_i+' tbody');
                $('#member_'+table_i+' thead th').empty();
                $('#member_'+table_i+' thead th').append("成員名單");
                tbody.empty();
                for (var j = 0; j < Math.ceil(data.length/3); j++) {
                    tbody.append('<tr></tr>');
                    var tr = tbody.children("tr").last();
                    for(var i = 0;i<3;i++){
                        var cnt = j*3+i;
                        if (cnt >= data.length ){
                            break;
                        }
                        console.log(data[cnt].member_name);
                        tr.append('<td class="'+data[cnt].member_name+'_'+class_id+'" onclick="deleteMember(\''+data[cnt].member_name+'\',\''+class_id+'\',\''+class_name+'\',\''+data.length+'\',\''+table_i+'\',\''+id+'\')">'+data[cnt].member_name+'\#'+data[cnt].pin+'</td>');
                        console.log("第" + cnt + "次");
                        //$("#member_"+id).slideToggle("slow");
                    }
                }
                $("#loading_"+id).hide();
                $("#member_"+id).show();
                
               
            }else{
                $('#member_'+table_i+' thead th').empty();
                $('#member_'+table_i+' thead th').append("目前沒有成員");
                $("#loading_"+id).hide();
                $("#member_"+id).show();
            }
        }
    });  
    console.log('end')    
}
var loopload = {};
function run_setInterval(class_id, table_i,class_name,id){
    if(!(class_id in loopload)){
        
        $("#loading_"+id).show();
        
        loopload[class_id] = setInterval(function(){search_class(class_id, table_i,class_name,id)},3000);
        
    }else{
        clearInterval(loopload[class_id]);
        delete loopload[class_id];
    }
}
async function deleteMember(member_id, class_id ,class_name,length,table_i,id) {
    var check = confirm("你確定要把\"" + member_id + "\"這位同學踢出課堂 : \"" + class_name + "\"嗎?")
    if(check){

        clearInterval(loopload[class_id]);
        delete loopload[class_id];

        $("#loading2_"+id).show();
        await $.ajax({
            type: 'POST',
            url: '/class_member_delete' ,
            data: {
                member_id:member_id,
                class_id:class_id
            } ,
            dataType: "json",
            success: function(data){
                
                
            }
        });
        $("."+member_id+'_'+class_id).fadeOut();
        $("#loading2_"+id).hide();
        loopload[class_id] = setInterval(function(){search_class(class_id, table_i,class_name,id)},3000);
        
    }


    
}
function playAudio(text) {
    event.stopPropagation();
    var msg = new SpeechSynthesisUtterance(text);
    msg.lang = 'en-US';
    msg.voiceURI = 'native';
    msg.rate = 0.6; // 0.1 to 10
    window.speechSynthesis.speak(msg);
}

function alwayson(){
    $("#ap").prop('checked', true);
}

function checkOne(){
    var allTrue = true;
    $("#survey .select-group--right input").each(function () {
        if (!this.checked) {
            allTrue = false;
        }
    })

    if (allTrue) {
        $("#survey .select-group--left input").prop('checked', true);
    }
    else {
        $("#survey .select-group--left input").prop('checked', false);
    }
}

function checkAll(){
    if ($("#survey .select-group--left input").is(':checked')) {
        $("#survey .select-group--right input").prop('checked', true);
    }
    else {
        $("#survey .select-group--right input").prop('checked', false);
        $("#ap").prop('checked', true);
    }
}

function closeSurvey() {
    $( "#survey" ).hide();
}

function showLoading() {
    $("#loading").show();
}