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
    let prevSideEle = document.getElementById(`${prevId}1`)

    prevEle.classList.remove("active");
    prevSideEle.classList.remove("active");

   
    let currEle = document.getElementById(id);
    let currSideEle = document.getElementById(`${id}1`);
    
    currEle.classList.add("active");
    currSideEle.classList.add("active");

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



const makeBold = (id) => {


    let radBtn = $(".rdInput");


   

    //radBtn.each(function (index, inp) {

    //    inp.checked = false;
    //})


    //document.getElementById(`${id}`).checked = true;


    $('.rdLabel').css({ 'background-color': 'white', 'color': '#0dcaf0' })


    if ($(`#${id}`)[0].checked) {
        $(`#${id}`).next('label').css({ 'background-color': '#0dcaf0', 'color': 'white' })

    }



}


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



//<----------------------------------------------------Checks------------------------------->//

const checkAll = (ClassName, target) => {

    console.log(ClassName)

    var checks = $(`.${ClassName}`)

    for (let i = 0; i < checks.length; i++) {
        console.log(checks[i].checked)
        if (target.checked == false && checks[i].checked == false) {
            continue;
        }
        checks[i].click();
    }

}




/*Validations*/


const checkEmailAvailibility = (target) => {


    var email = target.value;
    console.log(email)
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

const chekcRegionAvailibility = (id) => {


    let region = $(`#${id}`).val();
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

                $("#AlertModalContent").text(response.negativeMsg)

            } else {
                $("#AlertModalContent").text(response.positiveMsg)

            }
                $("#regionModalBtn").click();
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
        data: { StatusArray: statusarray,currentPage:1 },
        success: function (response) {

            if (response.code == 401) {

               location.reload();
            } else {
   
             $("#tableContainer").html(response);
            localStorage.setItem("CurrentStatusType", Statusname);
            $(`#${id}`).addClass('ActiveStatus');
            //DisPlayPagination()
            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}


const GetAllCurrentStatusRequests = () => {

    let StatusBtns = $(".big-btn");


    for (let i = 1; i < StatusBtns.length + 1; i++) {



        if ($(`#big-btn${i}`).hasClass('ActiveStatus')) {



            if (i == 1) {                     /*New*/
                statusarray = ['1'];
            } else if (i == 2) {              /*Unpaid*/
                statusarray = ['9']
            } else if (i == 3) {              /*Active*/
                statusarray = ['3', '4'];
            } else if (i == 4) {              /*Conclude*/
                statusarray = ['5']
            } else if (i == 5) {               /*ToClose*/
                statusarray = ['6', '7', '8']
            } else if (i == 6) {                /*Pending*/
                statusarray = ['2']
            }

            break;
        }





    }




    $.ajax({
        method: "post",
        url: "/Admindashboard/GetStatuswiseRequests",
        data: { StatusArray: statusarray, currentPage: 1 },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#tableContainer").html(response);
             
               
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}


const GetFiltteredRequests = (currentPage,totalPages,isPageAction, type) => {



    console.log(currentPage, totalPages, isPageAction)
    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }

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
                statusarray = ['9']
            } else if (i == 3) {              /*Active*/
                statusarray = ['3', '4'];
            } else if (i == 4) {              /*Conclude*/
                statusarray = ['5']
            } else if (i == 5) {               /*ToClose*/
                statusarray = ['6', '7', '8']
            } else if (i == 6) {                /*Pending*/
                statusarray = ['2']
            }

            break;
        }





    }

    let RequestorTypeBtns = $(".ReqType-Heading");


    if (type != '') {
        console.log("hiee",type)

        for (let i = 0; i < RequestorTypeBtns.length; i++) {

            $(`#ReqType-Heading${i}`).removeClass("ActiveRequestorType")


        }
        $(`#ReqType-Heading${type}`).addClass("ActiveRequestorType")
    }
    else {

        console.log("hi",type)

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
        data: { type: type, StatusArray: statusarray, region: region, Name: searchString, currentPage: currentPage },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#tableContainer").html(response);
                $("#searchinp").val(searchString);
                //$("DashRegionSelector").append($("<option selected></option>").text(physician.Firstname + " " + physician.Lastname).val(region))
                
                //DisPlayPagination()
            }
        },
        error: function (err) {
            console.log(err)
        }
    })


}


//Action Url Functions

const GetEncounterCaseView = (id,modalId,IsAdmin) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetEncounterFormStatus",
        data: { requestId: id },
        success: function (response) {
            console.log(response)
            if (IsAdmin == false && response.status == 3) {
                $("#TypeOfCareModal").modal("show");
                $("#TypeCareRequestId").val(id);
            }
            else if (response.isfinelized) {


                $(`#${modalId}`).modal("show");
                $("#EncounterCaseModalRequestInp").val(id);

            } else {

                if (IsAdmin) {

                    window.location.href = `https://localhost:7008/Admin/encounterform/${id}`
                } else {
                    window.location.href = `https://localhost:7008/Provider/encounterform/${id}`

                }
            }

        },
        error: function (err) {
            console.log(err)
        }
    })


}

const GetSendAgreementView = (id, modalContainerId) => {


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetSendAgreementModal",
        data: { requestId: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $(`#${modalContainerId}`).html(response);

                $("#SendAgreementPopup").modal("show");
            }

        },
        error: function (err) {
            console.log(err)
        }
    })


}


const GetCancleCase = (id, modalContainerId) => {




    console.log(id)
    $.ajax({
        method: "post",
        url: "/Admindashboard/GetCancleCaseView",
        data: { id: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {
                $(`#${modalContainerId}`).html(response);

                $.validator.unobtrusive.parse($("#CancleCaseForm"));
                $("#CancleCaseModal").modal("show");
            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetAssginCasePopup = (id,modalContainerId) => {


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetAssginCaseView",
        data: { requestId: id },
        success: function (response) {
           
            if (response.code == 401) {

                location.reload();
            } else {

                $(`#${modalContainerId}`).html(response);

                $.validator.unobtrusive.parse($("#AssignCaseForm"));
                $("#AssignCasePopUp").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetBlockCasePopup = (id, modalContainerId) => {


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetBlockCaseView",
        data: { requestId: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $(`#${modalContainerId}`).html(response);

                $.validator.unobtrusive.parse($("#BlockCaseForm"));
                $("#BlockCaseModal").modal("show");

            }

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


const GetTransferCasePopup = (id, modalContainerId) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetTransferCaseView",
        data: { id: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $(`#${modalContainerId}`).html(response);

                $.validator.unobtrusive.parse($("#TransferCaseForm"));
                $("#TransferCaseModal").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetClearCasePopup = (id, modalContainerId) => {

    $.ajax({
        method: "post",
        url: "/Admindashboard/GetClearCaseModal",
        data: { id: id },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $(`#${modalContainerId}`).html(response);

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



const GetSendLinkPopUp = (modalContainerId) => {




    $.ajax({
        method: "post",
        url: "/Admindashboard/GetSendLinkView",
        success: function (response) {

            if (response.code == 401) {

                location.reload();


            } else {

                $(`#${modalContainerId}`).html(response);


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

            //PhysicianDropdown.append($("<option></option>").text("Physicians").attr("selected", "true"));


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


            if (response.code == 401) {

                location.reload();


            } else if (response.code == 403) {
                toastr.error(response.msg);
            }
            else {



                $("#BusinessContact").val(response.businesscontact)
                $("#VendorEmail").val(response.email)
                $("#VendorFax").val(response.faxnumber)

            }


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


const editDetails1 = () => {
    console.log("po")
    $(".tbDisabled1").removeAttr("disabled");
    $(".editMode1").css("display", "block");
    $(".editBtn1").css("display", "none");
}

const cancleEdit1 = () => {


    $(".editBtn1").css("display", "block");
    $(".editMode1").css("display", "none");
    $(".tbDisabled1").attr("disabled", "true")


}

const editDetails2 = () => {
    $(".tbDisabled2").removeAttr("disabled");
    $(".editMode2").css("display", "block");
    $(".editBtn2").css("display", "none");
}

const cancleEdit2 = () => {


    $(".editBtn2").css("display", "block");
    $(".editMode2").css("display", "none");
    $(".tbDisabled2").attr("disabled", "true")


}

const editDetails3 = () => {
    $(".tbDisabled3").removeAttr("disabled");
    $(".editMode3").css("display", "block");
    $(".editBtn3").css("display", "none");
}

const cancleEdit3 = () => {


    $(".editBtn3").css("display", "block");
    $(".editMode3").css("display", "none");
    $(".tbDisabled3").attr("disabled", "true")


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

//-------------------------Scheduling-------------------------------//

const GetNextDayWiseShiftTableView = (date, month, year,dateheading) => {



        var regionId = $("#shiftRegionSelector").val();

    $.ajax({

        url: "/Provider/GetDayWiseShiftTable",
        method: "post",
        data: { date: date, month: month, year: year, regionId: regionId },
        success: function (response) {

            $("#ShiftTableContainer").html(response);

            var activeModeBtn = localStorage.getItem("ActiveTableMode");
            var activeBtn = document.getElementById(`${activeModeBtn}`);
            activeBtn.classList.remove("btn-outline-info", "text-info", "lightInfo-btn");
            activeBtn.classList.add("btn-info", "text-white", "darkInfo-btn");

            $("#DateHeader").text(dateheading)

            localStorage.setItem("currentDate", date);
            localStorage.setItem("currentMonth", month);
            localStorage.setItem("currentYear", year);
        },
        error: function (err) {
            console.log(err);
        }

    })
}

const GetNextWeekWiseShiftTableView = (date, month, year,dateHeading) => {




    var regionId = $("#shiftRegionSelector").val();

    $.ajax({

        url: "/Provider/GetWeekWiseShiftTableView",
        method: "post",
        data: { date: date, month: month, year: year, regionId: regionId },
        success: function (response) {

            $("#ShiftTableContainer").html(response);


            var activeModeBtn = localStorage.getItem("ActiveTableMode");
            var activeBtn = document.getElementById(`${activeModeBtn}`);
            activeBtn.classList.remove("btn-outline-info", "text-info", "lightInfo-btn");
            activeBtn.classList.add("btn-info", "text-white", "darkInfo-btn");

            $("#DateHeader").text(dateHeading)


            localStorage.setItem("currentDate", date);
            localStorage.setItem("currentMonth", month);
            localStorage.setItem("currentYear", year);
        },
        error: function (err) {
            console.log(err);
        }

    })
}


const GetNextMonthWiseShiftTableView = (month, year,dateHeading) => {

    var regionId = $("#shiftRegionSelector").val();

    $.ajax({

        url: "/Provider/GetMonthWiseShiftTableView",
        method: "post",
        data: { date: 1, month: month, year: year, regionId: regionId },
        success: function (response) {

            $("#ShiftTableContainer").html(response);


            var activeModeBtn = localStorage.getItem("ActiveTableMode");
            var activeBtn = document.getElementById(`${activeModeBtn}`);
            activeBtn.classList.remove("btn-outline-info", "text-info", "lightInfo-btn");
            activeBtn.classList.add("btn-info", "text-white", "darkInfo-btn");

            $("#DateHeader").text(dateHeading)


            localStorage.setItem("currentDate", 1);
            localStorage.setItem("currentMonth", month);
            localStorage.setItem("currentYear", year);
        },
        error: function (err) {
            console.log(err);
        }

    })
}


const GetAllShiftViewModal = (date, month, year) => {
   
    var regionId = $("#shiftRegionSelector").val();

    $.ajax({
        method: "post",
        url: "/Provider/GetDayWiseAllShiftView",
        data: { date: date, month: month, year: year, regionId: regionId },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {
                
                $("#AllShiftViewContainer").html(response);

          
                $("#AllShift").modal("show");

            }

        },
        error: function (err) {
            console.log(err)
        }
    })
}


const GetRegionsWiseShiftTable = () => {



    var TableMode = localStorage.getItem("TableMode");


    var Controller = '';

    if (TableMode == "Day") {
        Controller = "GetDayWiseShiftTable"
    } else if (TableMode == "Week") {
        Controller = "GetWeekWiseShiftTableView"

    } else if (TableMode == "Month") {
        Controller = "GetMonthWiseShiftTableView"
    }

    var date = localStorage.getItem("currentDate");
    var month = localStorage.getItem("currentMonth");
    var year = localStorage.getItem("currentYear");
    var regionId = $("#shiftRegionSelector").val();


    $.ajax({

        url: `/Provider/${Controller}`,
        method: "post",
        data: { date: date, month: month, year: year, regionId: regionId },
        success: function (response) {

            $("#ShiftTableContainer").html(response);

            var activeModeBtn = localStorage.getItem("ActiveTableMode");
            var activeBtn = document.getElementById(`${activeModeBtn}`);
            activeBtn.classList.remove("btn-outline-info","text-info","lightInfo-btn");
            activeBtn.classList.add("btn-info","text-white","darkInfo-btn");

        },
        error: function (err) {
            console.log(err);
        }

    })
}

const GetShiftModalView = () => {

    $.ajax({
        method: "post",
        url: "/Provider/GetCreateShiftView",
        data: {},
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {
                
                $("#ShiftModalContainer").html(response);

               

                $.validator.unobtrusive.parse($("#CreateShiftForm"));





                $("#ShiftModal").modal("show")

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetProviderShiftModalView = () => {

    $.ajax({
        method: "post",
        url: "/ProviderDashboard/GetProviderCreateShiftView",
        data: {},
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#ShiftModalContainer").html(response);

                $.validator.unobtrusive.parse($("#ProviderCreateShiftForm"));
                $("#ShiftModal").modal("show")

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetViewShiftModel = (shiftDetailId) => {



    $.ajax({
        method: "post",
        url: "/Provider/GetViewShiftModel",
        data: { shiftdetailId: shiftDetailId },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else if (response.IsPastShift) {
                return;
            }
            else {

                $("#ShiftModalContainer").html(response);
                $.validator.unobtrusive.parse($("#EditShiftModalForm")); 
                $("#ViewShiftModal").modal("show")

            }

        },
        error: function (err) {
            console.log(err)
        }
    })
}  

const GetAccountAccessTableView = (currentPage, isPageAction, totalPages = 0) => {

    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction, currentPage, totalPages)
        return;
    }


    $.ajax({
        method: "post",
        url: "/Provider/GetAccountAccessTableView",
        data: { currentPage: currentPage },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#AccountAccessTableContainer").html(response);

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const GetUserAccessTableView = (currentPage, isPageAction, totalPages = 0) => {


    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction, currentPage, totalPages)
        return;
    }

    let roleId = document.getElementById("userAccessRoleSelector").value;


    $.ajax({
        method: "post",
        url: "/Provider/GetUserAccessTableView",
        data: { roleId: roleId, currentPage: currentPage },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#UserAccessTableContainer").html(response);

            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}

const ShiftApproveAction = (le) => {


  
    if ($(window).width() <= 600) {
        $("#mb-ApproveInp").val(1);
        $("#mb-RequestShiftTableForm").submit();
    } else {
        $("#ApproveInp").val(1);
        $("#RequestShiftTableForm").submit();
       
    }

}

const ShiftDeleteAction = () => {

    

    if ($(window).width() <= 600) {
        $("#mb-ApproveInp").val(2);
        $("#mb-RequestShiftTableForm").submit();
    } else {
        $("#ApproveInp").val(2);
        $("#RequestShiftTableForm").submit();
    }


}
const handleRepeatAction = () => {



    if ($("#RepeatSwitch")[0].checked == true) {

        $(".tbDisabled").removeAttr("disabled");
    } else {
        $(".tbDisabled").attr("disabled", "true")

    }

}

const SubmitShiftForm = () => {


    let start = $("#StartTime").val();
    let end = $("#EndTime").val();


   

    if (end < start) {
        console.log(0)
        toastr.warning("Please Enter Valid Time");
    }  else {
        console.log(2);
        $("#CreateShiftForm").submit();
    }

}

const CreateShift = () => {

    if ($("#RepeatSwitch")[0].checked == true) {

        if ($("#DaysInputs")[0].checked == false) {

            $("#dayValidation").text("Please Select Minimum 1 Day.")
        } else {
            console.log("9")

            SubmitShiftForm()
            //$("#CreateShiftForm").submit();
            $("#dayValidation").text("")


        }
    } else {

        $("#dayValidation").text("")

        SubmitShiftForm()

        //$("#CreateShiftForm").submit();
    }
}



const ProviderCreatShift = () => {

    if ($("#RepeatSwitch")[0].checked == true) {

        if ($("#DaysInputs")[0].checked == false) {

            $("#dayValidation").text("Please Select Minimum 1 Day.")
        } else {
            console.log("9")
            $("#ProviderCreateShiftForm").submit();
            $("#dayValidation").text("")


        }
    } else {

        $("#dayValidation").text("")

        $("#CreateShiftForm").submit();
    }

}

const hadleDaysCheck = () => {



    if ($("#DaysInputs")[0].checked == false) {
        $("#DaysInputs").click();
    }





}

const GetProvidersByRegion = (currentPage,totalPages,isPageAction) => {

    let region = $("#ProviderRegionSelector").val();


    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }


    $.ajax({
        method: "post",
        url: "/Admindashboard/GetProvidersByRegions",
        data: { regionId: region, currentPage: currentPage },
        success: function (response) {


            if (response.code == 401) {

                location.reload();


            } else {

               
                $("#ProviderTableContainer").html(response);
            }
        },
        error: function (err) {
            console.log(err)
        }
    })



}



const GetRequestedShiftTableView = (regionId, currentPage, isPageAction, totalPages = 0) => {
   
    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }

    $.ajax({

        url: "/Provider/RequestedShiftTableView",
        method: "post",
        data: { regionId: regionId, currentPage: currentPage },
        success: function (response) {

            $("#RequestedShiftTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })
}


const GetVendorsTableView = (currentPage, isPageAction, totalPages = 0) => {


    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }


    let vendorName = document.getElementById("VendorSearchString").value
    let HealthProfessionId = document.getElementById("PreosfessionId").value;

    console.log(vendorName, HealthProfessionId);

    $.ajax({

        url: "/Provider/GetVendorsTableView",
        method: "post",
        data: { vendorName: vendorName, HealthProfessionId: HealthProfessionId, currentPage: currentPage },
        success: function (response) {

            $("#VendorTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })
}



const GetSearchRecordsTableView = (currentPage, isPageAction, totalPages = 0) => {


    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        return;
    }


    let Status = $("#sr-status").val();
    let patientname = $("#sr-patientName").val();
    let RequestType = $("#sr-requestType").val();
    let FromDate = $("#sr-FromDate").val();
    let toDate = $("#sr-ToDate").val();
    let ProviderName = $("#sr-ProviderName").val();
    let Email = $("#sr-Email").val();
    let phonumber = $("#sr-phoneNumber").val();

    console.log(Email)
    $.ajax({

        url: "/Provider/GetSearchRecordsTableView",
        method: "post",
        data: {
            currentPage: currentPage,   Status: Status, PatientName: patientname, RequestType: RequestType, FromDate: FromDate, ToDate: toDate, ProviderName: ProviderName, Email
                : Email, Phone: phonumber
        },
        success: function (response) {

            $("#SearchRecordsTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })

    

}


const ExportSearchRecordsData = () => {


    let Status = $("#sr-status").val();
    let patientname = $("#sr-patientName").val();
    let RequestType = $("#sr-requestType").val();
    let FromDate = $("#sr-FromDate").val();
    let toDate = $("#sr-ToDate").val();
    let ProviderName = $("#sr-ProviderName").val();
    let Email = $("#sr-Email").val();
    let phonumber = $("#sr-phoneNumber").val();


    $.ajax({

        url: "/Provider/ExportSearchRecordsData",
        method: "post",
        data: {
            currentPage: 1, Status: Status, PatientName: patientname, RequestType: RequestType, FromDate: FromDate, ToDate: toDate, ProviderName: ProviderName, Emai
                : Email, Phone: phonumber
        },
        success: function (response) {


            var filename = "table_data.xlsx";
            var link = document.createElement('a');
            link.href = response.url;
            link.download = filename;
            link.click();
            toastr.success("File Exported Successfully.")

           


        },
        error: function (err) {
            console.log(err);
        }

    })

}


const GetEmailLogsTableView = (currentPage, isPageAction,totalPages = 0) => {



    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        return;
    }

    let ReciverName = $("#el-ReciverName").val();
    let RoleId = $("#el-Role").val();
    let EmailId = $("#el-email").val();
    let CreateDate = $("#el-createDate").val();
    let SentDate = $("#el-SentDate").val();




    $.ajax({

        url: "/Provider/GetEmailLogsTableView",
        method: "post",
        data: {
            currentPage: currentPage, ReciverName: ReciverName, RoleId: RoleId, EmailId: EmailId, CreateDate: CreateDate, SentDate: SentDate
        },
        success: function (response) {

            $("#EmailLogsTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })



}

const GetPatientHistoryTableView = (currentPage,isPageAction,totalPages) => {


    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        return;
    }

    let FirstName = $("#ph-Firstname").val();
    let LastName = $("#ph-Lastname").val();
    let Email = $("#ph-Email").val();
    let Phone = $("#ph-Phone").val();
    




    $.ajax({

        url: "/Provider/GetPatientHistoryTableView",
        method: "post",
        data: {
            currentPage: currentPage, FirstName: FirstName, LastName: LastName, Email: Email, Phone: Phone
        },
        success: function (response) {

            $("#PatientHistoryTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })



}

const handleNotificationChange = () => {

    if ($("#ProviderChangeSaveBtn").css("display") == "none") {
        $("#ProviderChangeSaveBtn").css("display", "block");
    }
}



            //<----------------------------------Provider Dashboar------------------------>>



const GetStatuswiseProviderRequests = (statusarray, id, Statusname) => {


    $("#searchinp").val('');
   


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
        url: "/ProviderDashboard/GetStatuswiseProviderRequests",
        data: { StatusArray: statusarray, currentPage: 1 },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#tableContainer").html(response);
                localStorage.setItem("CurrentStatusType", Statusname);
                $(`#${id}`).addClass('ActiveStatus');
                //DisPlayPagination()
            }

        },
        error: function (err) {
            console.log(err)
        }
    })

}


const GetRequestorTypeWiseProviderRequests = (currentPage, totalPages, isPageAction, type) => {

    console.log(currentPage, totalPages, isPageAction)
    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }

  

    let searchString = $("#searchinp").val();
    let statusarray = [];
    let StatusBtns = $(".big-btn");

    for (let i = 1; i < StatusBtns.length + 1; i++) {



        if ($(`#big-btn${i}`).hasClass('ActiveStatus')) {



            if (i == 1) {                     /*New*/
                statusarray = ['1'];
            } else if (i == 2) {              /*Unpaid*/
                statusarray = ['9']
            } else if (i == 3) {              /*Active*/
                statusarray = ['3', '4'];
            } else if (i == 4) {              /*Conclude*/
                statusarray = ['5']
            } else if (i == 5) {               /*ToClose*/
                statusarray = ['6', '7', '8']
            } else if (i == 6) {                /*Pending*/
                statusarray = ['2']
            }

            break;
        }





    }

    let RequestorTypeBtns = $(".ReqType-Heading");


    if (type != '') {
        console.log("hiee", type)

        for (let i = 0; i < RequestorTypeBtns.length; i++) {

            $(`#ReqType-Heading${i}`).removeClass("ActiveRequestorType")


        }
        $(`#ReqType-Heading${type}`).addClass("ActiveRequestorType")
    }
    else {

        console.log("hi", type)

        for (let i = 0; i < RequestorTypeBtns.length; i++) {


            if ($(`#ReqType-Heading${i}`).hasClass('ActiveRequestorType')) {

                type = i;
                break;

            }

        }

    }




    $.ajax({
        method: "post",
        url: "/ProviderDashboard/GetRequestorTypeWiseProviderRequests",
        data: { type: type, StatusArray: statusarray, Name: searchString, currentPage: currentPage },
        success: function (response) {

            if (response.code == 401) {

                location.reload();
            } else {

                $("#tableContainer").html(response);
                $("#searchinp").val(searchString);
                //$("DashRegionSelector").append($("<option selected></option>").text(physician.Firstname + " " + physician.Lastname).val(region))

                //DisPlayPagination()
            }
        },
        error: function (err) {
            console.log(err)
        }
    })


}

const GetNextMonthWiseProviderShiftTableView = (month, year, dateHeading) => {

  

    $.ajax({

        url: "/ProviderDashboard/GetMonthWiseProviderShiftTableView",
        method: "post",
        data: { date: 1, month: month, year: year},
        success: function (response) {

            $("#ShiftTableContainer").html(response);



            $("#DateHeader").text(dateHeading)


            localStorage.setItem("currentDate", 1);
            localStorage.setItem("currentMonth", month);
            localStorage.setItem("currentYear", year);
        },
        error: function (err) {
            console.log(err);
        }

    })
}

const GetTransferAdminModalView = (id) => {




    $("#TransferToAdminModalRequestId").val(id);

    $('#TransferToAdminModal').modal('show')
}

const MarkUploadedFiles = (id) => {

    console.log("hii", id, $(`#${id}`)[0].checked)

    if ($(`#${id}`)[0].checked == false) {
        console.log("hii")
       $(`#${id}`).click();
    }

    //console.log($(`#${id}`),"oo")


}


const GetPatientExploreTableView = (currentPage,isPageAction  ,userId,totalPages) => {

    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }


    $.ajax({

        url: "/Provider/GetPatientExploreTableView",
        method: "post",
        data: { currentPage: currentPage,userId:userId },
        success: function (response) {

            $("#PatientExploreTableContainer").html(response);



          


          
        },
        error: function (err) {
            console.log(err);
        }

    })


     
}


const GetBlockHistoryTableView = (currentPage, isPageAction, totalPages = 0 ) => {


    if (isPageAction && (currentPage <= 0 || currentPage > totalPages)) {
        console.log(isPageAction)
        return;
    }


    let Name = $("#br-PatientName").val();
    let CreateDate = $("#br-createDate").val();
    let Email = $("#br-Email").val();
    let Phone = $("#br-Phone").val();


    $.ajax({

        url: "/Provider/GetBlockHistoryTableView",
        method: "post",
        data: { currentPage: currentPage, Name: Name, CreateDate: CreateDate, Email: Email, Phone: Phone },
        success: function (response) {

            $("#BlockHistoryTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })

}

const GetProviderOnCallData = (regionId) => {


 


    $.ajax({

        url: "/Provider/GetProvidersOnCallData",
        method: "post",
        data: { RegionId: regionId },
        success: function (response) {

            $("#ProviderOnCallTableContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })

}



const GetShiftTimeSheetsDetails = (date) => {



    $.ajax({

        url: "/Invoicing/GetShiftTimeSheetsDetails",
        method: "post",
        data: { StartDate: date },
        success: function (response) {

            $("#TimeSheetDetailsContainer").html(response);
           


        },
        error: function (err) {
            console.log(err);
        }

    })


    GetTimeReibursmentDetails(date);


}


const GetTimeReibursmentDetails = (date) => {



    $.ajax({

        url: "/Invoicing/GetTimeSheetReibursmentDetails",
        method: "post",
        data: { currentPage:1, StartDate: date },
        success: function (response) {

            $("#VendorTableContainer").html(response);
            $("#AdminTimeSheetReibursmentTable").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })




}


const GetAdminTimeSheetView = () => {



    let physicianId = $("#TimeSheetPhysicianInp").val();
   
    let startDate = $("#TimeSheetStartDate").val();



   
    $.ajax({

        url: "/Invoicing/GetAdminTimeSheetTableView",
        method: "post",
        data: { physicianId: physicianId, StartDate: startDate },
        success: function (response) {

               

            if (response.isApproved) {


                GetAdminShiftTimeSheetsDetails(startDate, physicianId);

            } else {
                $("#PendingTimeSheetContainer").html(response);

            }








        },
        error: function (err) {
            console.log(err);
        }

    })



}

const GetAdminShiftTimeSheetsDetails = (startDate, physicianId) => {



    $.ajax({

        url: "/Admindashboard/GetAdminShiftTimeSheetsDetails",
        method: "post",
        data: { physicianId: physicianId, StartDate: startDate },
        success: function (response) {

         
            $("#PendingTimeSheetContainer").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })

    GetAdminTimeReibursmentDetails(startDate, physicianId);
}


const GetAdminTimeReibursmentDetails = (startDate, physicianId) => {



    $.ajax({

        url: "/Admindashboard/GetAdminTimeSheetReibursmentDetails",
        method: "post",
        data: { currentPage: 1, physicianId: physicianId, StartDate: startDate },
        success: function (response) {

          
            $("#AdminTimeSheetReibursmentTable").html(response);


        },
        error: function (err) {
            console.log(err);
        }

    })




}