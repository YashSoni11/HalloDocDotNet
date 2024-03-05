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


/*Validations*/


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




/*Pagination*/


var allrows;
let start_idx;
let end_idx;
let rowsnum = 5;

const DisPlayPagination = () => {

    $("tr").addClass("Same");

    allrows = $(".Same");

    $("tbody").empty();

    start_idx = 0;
    end_idx = allrows.length > rowsnum ? (rowsnum - 1) : allrows.length;
   

    diPlayIntialRows(allrows, start_idx, end_idx);


    disPlayPageNumbers(rowsnum);

}


const disPlayPageNumbers = (RowsPerPage) => {

    $(".PageNumbers").empty();

    let pages = allrows.length / RowsPerPage;

    if (allrows.length % RowsPerPage != 0) {
        pages += 1;
    }

    $(".PageNumbers").append("<li class='page-item'> <a class='page-link' onclick='GetPreviousRows()'>Previous</a></li>")

    for (let i = 1; i <= pages; i++) {

        $(".PageNumbers").append("<li value='" + i + "' class='page-item'><a class='page-link' onclick=\"GetNumberedRows('" + i + "')\">" + i + "</a></li>");

    }

    $(".PageNumbers").append("<li class='page-item'> <a class='page-link' onclick='GetNextRows()'>Next</a></li>")


}

const diPlayIntialRows = (allrows, start_idx, end_idx) => {


    for (let i = start_idx; i <= end_idx; i++) {

        $("tbody").append(allrows[i]);
    }

}


const GetNextRows = () => {

    $("tbody").empty();

    start_idx = end_idx + 1;
    end_idx = allrows.length - start_idx + 1 > rowsnum ? start_idx + (rowsnum - 1) : allrows.length;


    for (let i = start_idx; i <= end_idx; i++) {

        $("tbody").append(allrows[i]);
    }


}


const GetPreviousRows = () => {

    $("tbody").empty();


    end_idx = start_idx - 1;
    start_idx = start_idx - rowsnum < 0 ? 0 : start_idx - rowsnum;



    for (let i = start_idx; i <= end_idx; i++) {

        $("tbody").append(allrows[i]);
    }

}


const GetNumberedRows = (numb) => {

    $("tbody").empty();


    let numbint = parseInt(numb);

    if (numbint == 1) {
        start_idx = 0

    } else {

        start_idx = (numbint - 1) * rowsnum;
    }
    console.log(start_idx)
    end_idx = start_idx + (rowsnum - 1) > allrows.length ? allrows.length : start_idx + (rowsnum - 1);


    for (let i = start_idx; i <= end_idx; i++) {

        $("tbody").append(allrows[i]);
    }


}

const changeRowsPerPage = (target) => {

    rowsnum = target.value;
    console.log(rowsnum)
    DisPlayPagination();
}

//#Deshboard Filters

const getStatusWiseRequests = (statusarray, id, Statusname) => {


    $("#searchinp").val('');
    $("#DashRegionSelector").val('All Regions');


    var newstring = "(" + Statusname + ")"
    $(".StatusName").text(newstring)




    let i = 1;
    $('.big-btn').each(function () {

        var borderColor = $(this).removeClass(`big-btn${i}`);
        $(this).children().removeClass(`big-btn${i}`);

        $(this).removeClass("ActiveStatus");

        i++;

    });
    $(`#${id}`).addClass(`${id}`);

    $(`#${id}`).children().addClass(`${id}`);




    $.ajax({
        method: "post",
        url: "/Admindashboard/GetStatuswiseRequests",
        data: { StatusArray: statusarray },
        success: function (response) {


            $("#tableContainer").html(response);
            localStorage.setItem("CurrentStatusType", Statusname);
            $(`#${id}`).addClass('ActiveStatus');

            DisPlayPagination()
        },
        error: function (err) {
            console.log(err)
        }
    })

}


const GetFiltteredRequests = (type = '') => {

    let region = $("#DashRegionSelector").val();

    if (region == "All Regions") {
        region = ''
    }

    let searchString = $("#searchinp").val();
    let statusarray = [];
    let StatusBtns = $(".big-btn");

    for (let i = 1; i < StatusBtns.length + 1; i++) {



        if ($(`#big-btn${i}`).hasClass('ActiveStatus')) {



            if (i == 1) {                     /*New*/
                statusarray = ['1'];
            } else if (i == 2) {              /*Unpaid*/
                statusarray = ['2']
            } else if (i == 3) {              /*Active*/
                statusarray = ['3', '4'];
            } else if (i == 4) {              /*Conclude*/
                statusarray = ['5']
            } else if (i == 5) {               /*ToClose*/
                statusarray = ['6', '7', '8']
            } else if (i == 6) {                /*Pending*/
                statusarray = ['9']
            }

            break;
        }





    }

    let RequestorTypeBtns = $(".ReqType-Heading");


    if (type != '') {
        for (let i = 0; i < RequestorTypeBtns.length; i++) {

            $(`#ReqType-Heading${i}`).removeClass("ActiveRequestorType")


        }
        $(`#ReqType-Heading${type}`).addClass("ActiveRequestorType")
    }
    else {



        for (let i = 0; i < RequestorTypeBtns.length; i++) {


            if ($(`#ReqType-Heading${i}`).hasClass('ActiveRequestorType')) {

                type = i;
                break;

            }

        }

    }




    $.ajax({
        method: "post",
        url: "/Admindashboard/GetRequestorTypeWiseRequests",
        data: { type: type, StatusArray: statusarray, region: region, Name: searchString },
        success: function (response) {


            $("#tableContainer").html(response);
            DisPlayPagination()

        },
        error: function (err) {
            console.log(err)
        }
    })


}


