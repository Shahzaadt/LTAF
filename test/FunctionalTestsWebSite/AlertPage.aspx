<html>
<body>
    <form id="form1" runat="server">
    <input type="button" id="AlertButton" value="alert" onclick="showAlert('hi')" />
    <input type="button" id="ConfirmButton" value="confirm" onclick="getConfirmation('are you sure?')" />
    <input type="button" id="ShowText" value="ShowText" onclick="showText('This is some text')" />
    <span id="log"></span>
    <script>

function showText(text) {
    document.getElementById("log").innerHTML = text;
}

function showAlert(text){ 
    alert(text);
    document.getElementById("log").innerHTML = text;
}

function getConfirmation(text) {
    var answer = confirm(text);
    if(answer) {
        document.getElementById("log").innerHTML = "You said OK";
    } else {
        document.getElementById("log").innerHTML = "You said Cancel";
    }
}

    </script>

    </form>
</body>
</html>
