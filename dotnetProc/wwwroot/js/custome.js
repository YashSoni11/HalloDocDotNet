// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const  preventNavigation = () => {
    history.pushState(null, null, window.location.href);
    window.onpopstate = function () {
        history.go(1);
    };
}

const handleNavChange = (id) => {

    let prevId = localStorage.getItem("ActiveLink");

    let prevEle = document.getElementById(prevId);

    prevEle.classList.remove("active");

    console.log(typeof id)
    let currEle = document.getElementById(id);
    console.log(currEle)
    currEle.classList.add("active");

    localStorage.setItem("ActiveLink", id);
}

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

            if (response.code == 401) {

               location.reload();
            } else {
   
             $("#tableContainer").html(response);
            localStorage.setItem("CurrentStatusType", Statusname);
            $(`#${id}`).addClass('ActiveStatus');
            DisPlayPagination()
            }

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

            if (response.code == 401) {

                location.reload();
            } else {

                $("#tableContainer").html(response);
                DisPlayPagination()
            }
        },
        error: function (err) {
            console.log(err)
        }
    })


}


//Action Url Functions

const GetEncounterCaseView = (id) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetEncounterFormStatus",
        data: { requestId: id },
        success: function (response) {
            console.log(response)
            if (response.isfinelized) {


                $("#EncounterCaseModal").modal("show");
                $("#EncounterCaseModalRequestInp").val(id);

            } else {

                window.location.href = `https://localhost:7008/Admindashboard/encounterform/${id}`
            }

        },
        error: function (err) {
            console.log(err)
        }
    })


}

const GetSendAgreementView = (id) => {


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetSendAgreementModal",
        data: { requestId: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#CancleCasePopup").html(response);

                $("#SendAgreementPopup").modal("show");
            }

        },
        error: function (err) {
            console.log(err)
        }
    })


}


const GetCancleCase = (id) => {




    console.log(id)
    $.ajax({
        method: "post",
        url: "/Admindashboard/GetCancleCaseView",
        data: { id: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#CancleCasePopup").html(response);

                $("#CancleCaseModal").addClass("show");
                $("#CancleCaseModal").css({ "display": "block", "backdrop-filter": "brightness(0.5)" });
            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetAssginCasePopup = (id) => {


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetAssginCaseView",
        data: { requestId: id },
        success: function (response) {
            if (response.code == 401) {

                location.reload();
            } else {

                $("#CancleCasePopup").html(response);

                $("#AssignCasePopUp").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetBlockCasePopup = (id) => {


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetBlockCaseView",
        data: { requestId: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#CancleCasePopup").html(response);

                $("#BlockCaseModal").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetViewCase = (id) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetRequestClientInfoFromRequestId",
        data: { id: id },
        success: function (response) {



            $("#pills-home").html(response);


        },
        error: function (err) {
            console.log(err)
        }
    })

}


const GetViewnotes = (id) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetRequestNotes",
        data: { id: id },
        success: function (response) {


            $("#pills-home").html(response);


        },
        error: function (err) {
            console.log(err)
        }
    })
}


const GetTransferCasePopup = (id) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetTransferCaseView",
        data: { id: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#CancleCasePopup").html(response);

                $("#TransferCaseModal").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetClearCasePopup = (id) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetClearCaseModal",
        data: { id: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#CancleCasePopup").html(response);

                $("#ClearCasePopup").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const ClearCase = () => {


    let requestId = $("#ClearCaseReqId").val();

    $.ajax({
        method: "post",
        url: "/Admindashboard/ClearRequest",
        data: { id: requestId },
        success: function (response) {

            if (response.code == 401) {
                location.reload();
            }
            else {

                toastr.remove();

                if ('@TempData["ShowPositiveNotification"]' != '') {
                    toastr.success('@TempData["ShowPositiveNotification"]');
                }
                else if ('@TempData["ShowNegativeNotification"]' != '') {
                    toastr.error('@TempData["ShowNegativeNotification"]')
                }
            }


        },
        error: function (err) {
            console.log(err)
        }
    })
}

//const toggleActionsUrls = (bodyId, btnId) => {


//    var collapseButton = document.getElementById(`${btnId}`);
//    var collapseExample = document.getElementById(`${bodyId}`);


//    console.log(collapseButton.getAttribute("aria-expanded"))

//    if (collapseButton.getAttribute("aria-expanded") == "true") {
//        console.log(1)
//        collapseExample.classList.remove("show");
//        collapseButton.setAttribute("aria-expanded", "false");
//    } else {
//        console.log(2)
//        collapseExample.classList.add("show");
//        collapseButton.setAttribute("aria-expanded", "true");
//    }




//}


//Dashboard Buttons Functions



const GetSendLinkPopUp = () => {




    $.ajax({
        method: "post",
        url: "/Admindashboard/GetSendLinkView",
        success: function (response) {

            if (response.code == 401) {

                location.reload();


            } else {

                $("#CancleCasePopup").html(response);


                $.validator.unobtrusive.parse($("#SendLinkForm"));

                const input = document.querySelector("#phonInput");
                window.intlTelInput(input, {
                    utilsScript: "https://cdn.jsdelivr.net/npm/intl-tel-input@18.2.1/build/js/utils.js",
                });

                $("#SendLinkPopUp").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })


}
const SendRequstLink = () => {


    $("#SendLinkForm").validate();

    var response = $("#SendLinkForm").valid();
    console.log(response)
    if (response == false) {
        console.log(90)
        return;
    }
}


//Common functions

const GetPhysicianByRegions = () => {


    let regionId = $("#sl").val();

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetPhysicianByRegion",
        data: { regionId: regionId },
        success: function (response) {


            var PhysicianData = JSON.parse(response);



            let PhysicianDropdown = $("#PhysicianInp")

            PhysicianDropdown.empty();

            PhysicianDropdown.append($("<option></option>").text("Physicians").attr("selected", "true"));


            $.each(PhysicianData, function (index, physician) {
                PhysicianDropdown.append($("<option></option>").text(physician.Firstname + " " + physician.Lastname).val(physician.Physicianid));
            })



        },
        error: function (err) {
            console.log(err)
        }
    })
}

const GetVendors = (target) => {


    console.log(target)

    let professsionId = $("#ProfessionInp").val();

    console.log(professsionId);

    $.ajax({

        method: "post",
        url: "/Admindashboard/GetVendorsByProfession",
        data: { id: professsionId },
        success: function (response) {



            var VendorsData = JSON.parse(response);

            console.log(VendorsData);

            let BusinessDropdown = $("#BusinessInp")



            BusinessDropdown.empty();




            $.each(VendorsData, function (index, vendor) {
                BusinessDropdown.append($("<option></option>").text(vendor.Vendorname).val(vendor.Vendorid));
            })


        },
        error: function () {
            console.log("Error Inside Order Page!");
        }


    })
}


const GetVendorDetails = () => {


    let vendorId = $("#BusinessInp").val();


    $.ajax({

        method: "post",
        url: "/Admindashboard/GetVendorDetails",
        data: { id: vendorId },
        success: function (response) {




            $("#BusinessContact").val(response.businesscontact)
            $("#VendorEmail").val(response.email)
            $("#VendorFax").val(response.faxnumber)




        },
        error: function () {
            console.log("Error Inside Order Page!");
        }


    })


}


///User-Dashboard Functionality

const editDetails = () => {
    console.log("iuhio")
    $(".tbDisabled").removeAttr("disabled");
    $(".editMode").css("display", "block");
    $(".editBtn").css("display", "none");


}

const cancleEdit = () => {

    $(".editBtn").css("display", "block");
    $(".editMode").css("display", "none");
    $(".tbDisabled").attr("disabled", "true")

}


const showLocation = () => {


   


    var zip = $("#zip").val();
    var city = $("#city").val();
    var state = $("#state").val();
    var street = $("#street").val();

    if (city == '' || state == "" || street == "") {
        toastr.error("Please enter  valid location!");
        return;
    }

    console.log(address)

    var address = street + ',' + city + ',' + state + ',' + zip

    var mapsUrl = "https://www.google.com/maps/search/?api=1&query=" + encodeURIComponent(address);


    window.open(mapsUrl, "_blank");
}