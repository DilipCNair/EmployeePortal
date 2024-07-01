// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$(document).ready(function () {
//    // For BasicDetails form
//    $('#EditDetails').click(function () {
//        $('#BasicDetails input[type=text], #BasicDetails input[type=email], #BasicDetails select, #BasicDetails input[type=date]').prop('disabled', false);
//        $('#UpdateDetails').prop('disabled', false);
//        $('#EditDetails').prop('disabled', true);
//    });

//    // For Address form
//    $('#EditAddress').click(function () {
//        $('#Address input[type=text]').prop('disabled', false);
//        $('#UpdateAddress').prop('disabled', false);
//        $('#EditAddress').prop('disabled', true);
//    });
//});


//To enable the submit button when user checks the TandC checkbox in Signup
//$(document).ready(function ()
//{
//    $('#formcheck').change(function ()
//    {
//        // Check if the checkbox is checked
//        if ($(this).prop('checked'))
//        {
//            // If checked, enable the submit button
//            $('#submitButton').prop('disabled', false);
//        } else
//        {
//            // If unchecked, disable the submit button
//            $('#submitButton').prop('disabled', true);
//        }
//    });
//});


//// To enable notification in profile
//document.addEventListener('DOMContentLoaded', function () {
//    const toastLiveExample = document.getElementById('liveToast');
//    if (toastLiveExample) {
//        const toastBootstrap = new bootstrap.Toast(toastLiveExample);
//        toastBootstrap.show();
//    }
//});

// To enable update button when user clicks on edit button for address and
// basic details form
//$(document).ready(function () {
//    // For BasicDetails form
//    $('#EditDetails').click(function () {
//        if ($(this).text() === "Edit") {
//            $('#BasicDetails input[type=text], #BasicDetails input[type=email], #BasicDetails select, #BasicDetails input[type=date]').prop('disabled', false);
//            $('#UpdateDetails').prop('disabled', false);
//            $(this).text("Cancel");
//        } else {
//            location.reload(); // Reload the page to cancel editing
//        }
//    });

//    // For Address form
//    $('#EditAddress').click(function () {
//        if ($(this).text() === "Edit") {
//            $('#Address input[type=text]').prop('disabled', false);
//            $('#UpdateAddress').prop('disabled', false);
//            $(this).text("Cancel");
//        } else {
//            location.reload(); // Reload the page to cancel editing
//        }
//    });
//});

//// To ask a dialog when you try to remove a profile pic in profile
//$(document).ready(function () {
//    $("#RemovePicture").click(function () {
//        if (confirm("Are you sure you want to remove your profile picture?")) {
//            $("#RemovePictureForm").submit();
//        }
//    });
//});

// To load the image to the image control when user selects an image
//$(document).ready(function () {
//    $('#InputFile').change(function () {
//        var file = this.files[0];
//        var imageType = /^image\//;

//        if (!file || !imageType.test(file.type)) {
//            alert("Please select an image file.");
//            if ($('#InputFile').val() === '') {
//                $('#InputFile').val('');
//                $('#profilePic').attr('src', 'https://localhost/images/blank_profile_pic.png');
//            }
//            else {
//                $('#InputFile').val('');
//                $('#profilePic').attr('src', 'https://localhost/images/blank_profile_pic.png');
//            }
//            return;
//        } else {
//            var reader = new FileReader();

//            reader.onload = function (e) {
//                $('#profilePic').attr('src', e.target.result);
//            }

//            reader.readAsDataURL(file);
//        }
//    });
//});

function changeColor(link) {
    // Toggle between black and another color, for example red
    //if (link.style.color === 'black') {
    //    link.style.color = 'red';
    //} else {
    //    link.style.color = 'black';
    //}
    link.style.color = 
}






