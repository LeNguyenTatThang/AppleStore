function checkFormPassword() {
    var checkActive = document.getElementById('changePassword');
    if (checkActive.checked) {
        $('.form-password').removeClass('hidden');
    } else {
        $('.form-password').addClass('hidden');
    }
}

function customEventEditor() {
    //$(document).on("click", "#btnSubmit", function (e) {
    //    var editorContent = $(".ql-editor").html();
    //    $("#content").val(editorContent);

    //});
    var editorContent = $(".ql-editor").html();
    $("#content").val(editorContent);
}