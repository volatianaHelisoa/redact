$(document).ready(function () {
    var $errorMessageLicence = $("#errorMessageLicence");

    if (document.getElementById('erroFilevalue') != null && document.getElementById('erroFilevalue').value == 1) {
        $errorMessageLicence.show();
    }

    $errorMessageLicence.on("click", "button", function () {
        $errorMessageLicence.hide();
        document.getElementById('erroFilevalue').value = 0;
        document.location.href = "?errorType=0";
    });

    if (document.getElementById('upload') != null) {
        var fullPath = document.getElementById('upload').value;
        if (fullPath) {
            var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
            var filename = fullPath.substring(startIndex);
            if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                filename = filename.substring(1);
            }
            alert(filename);
        };

        $("#upload").change(function () {
            var fileName = $("#upload")[0].files[0].name;
            $('.file-detect').text((fileName));
            return false;
        });
    }
  

  
})