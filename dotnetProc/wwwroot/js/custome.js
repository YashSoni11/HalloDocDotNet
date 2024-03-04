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


//const dofocus = (id) => {
//    console.log(id)
//    $('.emailInput').css('border-color', 'transparent')
//    $(`#${id}`).css('border', '1px solid #0dcaf0')
//}

//const doblur = (id) => {

//    $(`#${id}`).css('border', '1px solid #dee2e6')
//}

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

const checkEmailAvailibility = () => {


    var email = $(".emailInp").val();
    if (email == '')
        return;


    $.ajax({

        url: "/Home/ckeckEmailAvailibility",
        method: "post",
        data: { email: email },
        success: function (response) {

            console.log(response)

            if (response.isEmailBLocked) {
                $("#AlertModalContent").text(response.modalMsg)
                $(".passFields").css("display", "none")
                $("#regionModalBtn").click();
            }

            if (response.code == 401) {
                $(".passFields").css("display", "none")

                toastr.warning(response.error)
            }
            else if (response.code == 402) {

                $(".passFields").css("display", "block")
            }



        },
        error: function () {
            console.log(2, email);
        }

    })



}


const ValidateForm = () => {



    let birthdate = $('.BirthdateInp').val()



    let currenttime = new Date().toISOString().split('T')[0]

    if (birthdate == null || birthdate > currenttime) {
        console.log(09)


        document.getElementById('BirthdayError').textContent = "Please Enter Valid Birthdate"

        return;
    }

}





const checkPhoneNumberAvailibility = () => {


    let phoneNumber = $("#phonInput").val();
    console.log(098)
    if (phoneNumber == "") {
        return;
    }


    $.ajax({

        url: "/Home/CheckNumberAvailibility",
        method: "post",
        data: { Phone: phoneNumber },
        success: function (response) {
            console.log(response)
            if (response.isPhone == true) {

                $("#AlertModalContent").text(response.modalMsg)

                $("#regionModalBtn").click();
            }
        },
        error: function () {
            console.log(2, email);
        }

    })

}


const chekcRegionAvailibility = () => {


    let region = $("#regionInp").val();
    console.log(region)
    if (region == "Region") {
        return;
    }


    $.ajax({

        url: "/Home/CheckRegionAvailibility",
        method: "post",
        data: { region: region },
        success: function (response) {
            console.log(response)
            if (response.response == false) {

                $("#AlertModalContent").text(response.modalMsg)

                $("#regionModalBtn").click();
            }
        },
        error: function () {
            console.log(2, email);
        }

    })

}


