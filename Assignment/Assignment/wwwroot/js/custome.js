// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




const GetFillteredTaskk = (currentPage, IsPageAction, totalPages = 0) => {


    

    if (IsPageAction && (currentPage <= 0 || currentPage > totalPages)) {

        return;
    }


    let serachString = $("#searchinp").val();

    let rowsPerPage = $("#rowsPerPage").val();

    if (rowsPerPage == null || rowsPerPage == undefined) {
        rowsPerPage = 2;
    }


    $.ajax({

        url: "/Home/GetFilterredTask",
        method: "post",
        data: { currentPage: currentPage, Name: serachString, rowsPerPage: rowsPerPage },
        success: function (response) {



            $("#TaskTableContainer").html(response);



        },
        error: function (err) {
            console.log(err)
        }

    })


}

const GetTaskDetails = () => {


  

    $.ajax({

        url: "/Home/GetFilterredTask",
        method: "post",
        success: function (response) {
          
            $("#TaskTableContainer").html(response);
        },
        error: function (err) {
            console.log(err)
        }

    })

}



const GetCategoriesByName = (target) => {

    let Name = target.value;

    console.log(Name,"kk");

    $.ajax({

        url: "/Home/GetCatagoriesList",
        method: "post",
        data: {name:Name},
        success: function (response) {


            var CategoryData = JSON.parse(response);

            console.log(CategoryData.length)

            if (CategoryData.length == 0) {

                $("#CategoryDrop").css("display", "none");

                return;
            }

            console.log(CategoryData)

            let CategoryDropDown = $("#CategoryDrop")

            CategoryDropDown.empty();

            //PhysicianDropdown.append($("<option></option>").text("Physicians").attr("selected", "true"));


            $.each(CategoryData, function (index, category) {
                CategoryDropDown.append($("<li></li>").text(category.Name));
            })

            console.log($("#CategoryList"))
            $("#CategoryDrop").css("display", "block");

           
             
        },
        error: function (err) {
            console.log(err)
        }

    })

}



const GetAddTaskView = () => {




    $.ajax({

        url: "/Home/AddTaskView",
        method: "post",
        success: function (response) {

          

            $("#ActionModalContaier").html(response);

            $.validator.unobtrusive.parse($("#AddTaskForm"));


            $("#AddTaskModal").modal("show")
        },
        error: function (err) {
            console.log(err)
        }

    })

}

const GetEditTaskView = (id) => {




    $.ajax({

        url: "/Home/EditTaskView",
        method: "post",
        data: {id:id},
        success: function (response) {



            $("#ActionModalContaier").html(response);

            $.validator.unobtrusive.parse($("#EditTaskForm"));


            $("#EditTaskModal").modal("show")
        },
        error: function (err) {
            console.log(err)
        }

    })

}
