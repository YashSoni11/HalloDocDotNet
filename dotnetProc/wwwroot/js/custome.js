// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    var theme = localStorage.getItem('currentTheme')

    var bodyElement = $('body');
    var togglebtn = $('#themeIcon')

    if (theme == 'dark') {



        if (!bodyElement.hasClass('dark-theme')) {
            $('body').addClass('dark-theme')
            togglebtn.addClass('bi bi-sun')
            togglebtn.removeClass('bi bi-moon')
        }

    } else if (theme == 'light') {


        if (bodyElement.hasClass('dark-theme')) {

            togglebtn.removeClass('bi bi-sun')
            togglebtn.addClass('bi bi-moon')
            $('body').removeClass('dark-theme')
        }
    }


})


const dofocus = (id) => {
    console.log(id)
    $('.emailInput').css('border-color', 'transparent')
    $(`#${id}`).css('border', '1px solid #0dcaf0')
}

const doblur = (id) => {

    $(`#${id}`).css('border', '1px solid #dee2e6')
}

const showModal = () => {

    $('#modalBtn').click();
}
showModal();






const changeTheme = () => {




    var bodyElement = $('body');
    var togglebtn = $('#themeIcon')

    if (bodyElement.hasClass('dark-theme')) {

        togglebtn.removeClass('bi bi-sun')
        togglebtn.addClass('bi bi-moon')
        bodyElement.removeClass('dark-theme')
        localStorage.setItem('currentTheme', 'light')
    }
    else if (!bodyElement.hasClass('dark-theme')) {
        togglebtn.addClass('bi bi-sun')
        togglebtn.removeClass('bi bi-moon')
        bodyElement.addClass('dark-theme')
        localStorage.setItem('currentTheme', 'dark')

    }




}


const setUrl = ( url,  id) => {

    console.log(url, id);



}
