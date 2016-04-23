function getRandomFlash() {
    return new SWFObject("/Content/flas/HomeLoadernew.swf", "HomeLoadernew", "651", "188", "8");
}
function checkemail(str) {
    return /^.+@.+\..{2,3}$/.test(str);
}

function checkphone(str) {
    return /^[ \+\-\.0-9\(\)]+$/.test(str);
}

//Job Alert
$('input#IndustrySearch').bind("keyup focus", function () {
    if ($('input#IndustrySearch').val().length > 0) {
        var indDiv = document.getElementById("indtreepostJob");
        indDiv.style.display = "block";
    }
    else {
        var indDiv = document.getElementById("indtreepostJob");
        indDiv.style.display = "none";
    }
});

$('input#search-empLocationpost').bind("keyup focus", function () {
    if ($('input#search-empLocationpost').val().length > 0) {
        var locDiv = document.getElementById("searchable-empLocationtree");
        locDiv.style.display = "block";
    }
    else {
        var locDiv = document.getElementById("searchable-empLocationtree");
        locDiv.style.display = "none";
    }
});

$('input#search-empOccuj').bind("keyup focus", function () {
    if ($('input#search-empOccuj').val().length > 0) {
        var occuDiv = document.getElementById("occutreepostJob");
        occuDiv.style.display = "block";
    }
    else {
        var occuDiv = document.getElementById("occutreepostJob");
        occuDiv.style.display = "none";
    }
});
//Job Alert

//PostJob
$('input#search-empIndustry').bind("keyup focus", function () {
    if ($('input#search-empIndustry').val().length > 0) {
        var indDiv = document.getElementById("indtreepostJob");
        indDiv.style.display = "block";
    }
    else {
        var indDiv = document.getElementById("indtreepostJob");
        indDiv.style.display = "none";
    }
});


$('input#search-empOccu').bind("keyup focus", function () {
    if ($('input#search-empOccu').val().length > 0) {
        var occuDiv = document.getElementById("occutreepostJob");
        occuDiv.style.display = "block";
    }
    else {
        var occuDiv = document.getElementById("occutreepostJob");
        occuDiv.style.display = "none";
    }
});
//PostJob

//Job Upload Cv
$('input#search-empText').bind("keyup focus", function () {
    if ($('input#search-empText').val().length > 0) {
        var empDiv = document.getElementById("emptreepostJob");
        empDiv.style.display = "block";
    }
    else {
        var empDiv = document.getElementById("emptreepostJob");
        empDiv.style.display = "none";
    }
});

$('input#search-empLoca').bind("keyup focus", function () {
    if ($('input#search-empLoca').val().length > 0) {
        var locDiv = document.getElementById("loctreepostJob");
        locDiv.style.display = "block";
    }
    else {
        var locDiv = document.getElementById("loctreepostJob");
        locDiv.style.display = "none";
    }
});

$('input#search-empOccu').bind("keyup focus", function () {
    if ($('input#search-empOccu').val().length > 0) {
        var occuDiv = document.getElementById("occutreepostJob");
        occuDiv.style.display = "block";
    }
    else {
        var occuDiv = document.getElementById("occutreepostJob");
        occuDiv.style.display = "none";
    }
});

$('input#search-empInd').keyup(function () {
    if ($('input#search-empInd').val().length > 0) {
        var indDiv = document.getElementById("indtreepostJob");
        indDiv.style.display = "block";
    }
    else {
        var indDiv = document.getElementById("indtreepostJob");
        indDiv.style.display = "none";
    }
});
//Job Upload Cv

//Job Search
$('textarea#search-term').bind("keyup focus", function () {
    if ($('textarea#search-term').val().length > 0) {
        var occupation = document.getElementById("search-termoccu");
        var industry = document.getElementById("searchable-empIndtree");
        var location = document.getElementById("searchable-empLocationtree");
        occupation.style.display = "block";
        industry.style.display = "none";
        location.style.display = "none";
    }
    else {
        var occupation = document.getElementById("search-termoccu");
        var industry = document.getElementById("searchable-empIndtree");
        var location = document.getElementById("searchable-empLocationtree");
        occupation.style.display = "none";
        industry.style.display = "none";
        location.style.display = "none";

    }
});

$('textarea#search-empIndtree').bind("keyup focus", function () {
    if ($('textarea#search-empIndtree').val().length > 0) {
        var occupation = document.getElementById("search-termoccu");
        var industry = document.getElementById("searchable-empIndtree");
        var location = document.getElementById("searchable-empLocationtree");
        occupation.style.display = "none";
        industry.style.display = "block";
        location.style.display = "none";
    }
    else {
        var occupation = document.getElementById("search-termoccu");
        var industry = document.getElementById("searchable-empIndtree");
        var location = document.getElementById("searchable-empLocationtree");
        occupation.style.display = "none";
        industry.style.display = "none";
        location.style.display = "none";

    }
});

$('input#search-empLocationsearch').bind("keyup focus", function () {
    if ($('input#search-empLocationsearch').val().length > 0) {
        var occupation = document.getElementById("search-termoccu");
        var industry = document.getElementById("searchable-empIndtree");
        var location = document.getElementById("searchable-empLocationtree");
        occupation.style.display = "none";
        industry.style.display = "none";
        location.style.display = "block";
    }
    else {
        var occupation = document.getElementById("search-termoccu");
        var industry = document.getElementById("searchable-empIndtree");
        var location = document.getElementById("searchable-empLocationtree");
        occupation.style.display = "none";
        industry.style.display = "none";
        location.style.display = "none";

    }
});
//Job Search

for (var i in CKEDITOR.instances) {
    CKEDITOR.instances[i].on('change', function (ev) {
        counter = document.getElementById('txtContentCounter');
        field = ev.editor.document.getBody().getText(); //e.editor.getData();
        if (field.trim()) {
            field_length = field.length;
            if (field_length <= 10000) {
                //Calculate remaining characters
                remaining_characters = 10000 - field_length;
                //Update the counter on the page
                counter.innerHTML = '<b>' + remaining_characters + '</b>';
            }
            else {
                remaining_characters = 10000 - field_length;
                //Update the counter on the page
                counter.innerHTML = '<b>' + remaining_characters + '</b>';
                //e.editor.setData(field.substring(0, 100));
                alert('Textbox allows 10000 characters only');
                //fld.focus();
                return false;
            }
        }
        else {
            counter.innerHTML = '<b>' + 10000 + '</b>';
        }
    });
}

$('#IsSeek').click(function () {
    var $check = $(this),
        $div = $check.parent();
    if ($(this).is(':checked')) {
        document.getElementById("IsEmp").required = false;
        document.getElementById("IsEmp").nextSibling.innerHTML = '';
        document.getElementById("IsCon").required = false;
        document.getElementById("IsCon").nextSibling.innerHTML = '';
    } else {
        document.getElementById("IsEmp").required = true;
        document.getElementById("IsEmp").nextSibling.innerHTML = '';
        document.getElementById("IsSeek").required = true;
        document.getElementById("IsSeek").nextSibling.innerHTML = '';
        document.getElementById("IsCon").required = true;
        document.getElementById("IsCon").nextSibling.innerHTML = '';
    }
});
$('#IsEmp').click(function () {
    var $check = $(this),
        $div = $check.parent();
    if ($(this).is(':checked')) {
        document.getElementById("IsSeek").required = false;
        document.getElementById("IsSeek").nextSibling.innerHTML = '';
        document.getElementById("IsCon").required = false;
        document.getElementById("IsCon").nextSibling.innerHTML = '';
    } else {
        document.getElementById("IsEmp").required = true;
        document.getElementById("IsEmp").nextSibling.innerHTML = '';
        document.getElementById("IsSeek").required = true;
        document.getElementById("IsSeek").nextSibling.innerHTML = '';
        document.getElementById("IsCon").required = true;
        document.getElementById("IsCon").nextSibling.innerHTML = '';
    }
});
$('#IsCon').click(function () {
    var $check = $(this),
        $div = $check.parent();
    if ($(this).is(':checked')) {
        document.getElementById("IsSeek").required = false;
        document.getElementById("IsSeek").nextSibling.innerHTML = '';
        document.getElementById("IsEmp").required = false;
        document.getElementById("IsEmp").nextSibling.innerHTML = '';
    } else {
        document.getElementById("IsEmp").required = true;
        document.getElementById("IsEmp").nextSibling.innerHTML = '';
        document.getElementById("IsSeek").required = true;
        document.getElementById("IsSeek").nextSibling.innerHTML = '';
        document.getElementById("IsCon").required = true;
        document.getElementById("IsCon").nextSibling.innerHTML = '';
    }
});

function countChar(val) {
    var $row = $(val).parents('tr');
    var len = val.value.length;
    if (len >= 500) {
        val.value = val.value.substring(0, 500);
    } else {
        $row.find('#charNum').text(500 - len + " Characters Left.");
    }
};

function countCharEss(val) {
    var len = val.value.length;
    if (len >= 500) {
        val.value = val.value.substring(0, 500);
    } else {
        $('#essCriteriaText').text(500 - len + " Characters Left.");
    }
};
function countJobContentChar(val) {
    var len = val.value.length;
    if (len >= 150) {
        val.value = val.value.substring(0, 150);
    } else {
        $('#JobContentChar').text(150 - len + " Characters Left.");
    }
};

function addDesirableCriteria() {
    var inputEl = document.getElementById("impcriteria");
    var DesirableCriteriaList = document.getElementById('DesirableCriteriaList');
    //var textareaEl = $('#DesirableCriteriaList');
    if (inputEl != null) {
        var newOption = document.createElement('option');
        newOption.value = inputEl.value; // The value that this option will have
        newOption.innerHTML = inputEl.value; // The displayed text inside of the <option> tags
        DesirableCriteriaList.appendChild(newOption);
        //$(textareaEl).val($(textareaEl).val() + $(inputEl).val() + "\r\n");
        //Cleaning input
        $(inputEl).val('');
    }
}

function addEssentialCriteria() {
    var inputEl = document.getElementById("esscriteria");
    var EssentialCriteriaList = document.getElementById('MustHaveList');
    if (inputEl != null) {
        var newOption = document.createElement('option');
        newOption.value = inputEl.value; // The value that this option will have
        newOption.innerHTML = inputEl.value; // The displayed text inside of the <option> tags
        var lst = "<option selected='true' value='" + newOption.value + "'> > " + newOption.value + "</option>";
        $(".havelist").append(lst);
        $(inputEl).val('');
    }
    $('#EssImpCriteria').dialog("close")
}

function addConfigEssenCriteria() {
    var table = document.getElementById('sortableEssen');
    if (table != null) {
        var tableRowCount = table.rows.length;
        for (var i = 0; i < tableRowCount; i++) {
            var rowDesc = $(table.rows.item(i).cells[1]).find('textarea').val();
            var newOption = document.createElement('option');
            var listbox = document.getElementById('tblNicetoHaveList');
            var lst = "<option selected='true' value='" + rowDesc + "'> > " + rowDesc + "</option>";
            $(".nicetohavelist").append(lst);
        }
        $('#EssConfCriteria').dialog("close")
    }
}

function addDuties() {
    var table = document.getElementById('JobDuties');
    if (table != null) {
        var tableRowCount = table.rows.length;
        for (var i = 0; i < tableRowCount; i++) {
            var rowDesc = $(table.rows.item(i).cells[1]).find('textarea').val();
            var newOption = document.createElement('option');
            var listbox = document.getElementById('DutiesList');
            var lst = "<option selected='true' value='" + rowDesc + "'> > " + rowDesc + "</option>";
            $(".dutiesListBox").append(lst);
        }
    }
}

function addRemuneration() {
    var table = document.getElementById('JobRemuneration');
    if (table != null) {
        var tableRowCount = table.rows.length;
        for (var i = 0; i < tableRowCount; i++) {
            var rowDesc = $(table.rows.item(i).cells[1]).find('textarea').val();
            var newOption = document.createElement('option');
            var listbox = document.getElementById('RemunerationList');
            var lst = "<option selected='true' value='" + rowDesc + "'> > " + rowDesc + "</option>";
            $(".remunerationListBox").append(lst);
        }
    }
}

function deleteSelect(listid) {
    var listb = document.getElementById(listid);
    var listbtype = document.getElementById('SelectedLocationTypeList');
    var listbid = document.getElementById('SelectedLocationIDList');
    var len = listb.options.length;
    for (var i = listb.options.length - 1 ; i >= 0 ; i--) {
        if (listb.options[i].selected == true) {
            listb.options.remove(i);
            listbtype.options.remove(i);
            listbid.options.remove(i);
        }
    }
}

function deleteSelectAlert(listid) {
    var listb = document.getElementById(listid);
    var len = listb.options.length;
    for (var i = listb.options.length - 1 ; i >= 0 ; i--) {
        if (listb.options[i].selected == true) {
            listb.options.remove(i);
        }
    }
}

function addRow(tableID) {
    var table = document.getElementById(tableID);
    if (table != null && table.rows != null) {
        var rowCount = table.rows.length;
        var row = table.insertRow(rowCount);
        var colCount = table.rows[0].cells.length;
        for (var i = 0; i < colCount; i++) {
            var newcell = row.insertCell(i);
            newcell.innerHTML = table.rows[0].cells[i].innerHTML;
            //alert(newcell.childNodes);
            for (s = 0; s < newcell.childNodes.length; s++) {
                switch (newcell.childNodes[s].type) {
                    case "textarea":
                        newcell.childNodes[s].value = "";
                        newcell.childNodes[3].value = "";
                        break;
                    case "checkbox":
                        newcell.childNodes[s].checked = false;
                        break;
                }
            }
        }
    }
}

function deleteRow(tableID) {
    try {
        var table = document.getElementById(tableID);
        if (table != null && table.rows != null) {
            var rowCount = table.rows.length;
            for (var i = 0; i < rowCount; i++) {
                var row = table.rows[i];
                var chkbox = row.cells[0].childNodes[0];
                if (null != chkbox && true == chkbox.checked) {
                    if (rowCount <= 1) {
                        alert("Cannot delete all the rows.");
                        break;
                    }
                    table.deleteRow(i);
                    rowCount--;
                    i--;
                }
            }
        }
    } catch (e) {
        alert(e);
    }
}

var configIndex = 1;
function insertConfigRow() {
    var table = document.getElementById("sortableConfig");
    var row = table.insertRow(table.rows.length);
    //var cell1 = row.insertCell(0);
    //var t1 = document.createElement("input");
    //t1.type = "checkbox";
    //t1.id = "txtconfNo" + configIndex;
    //cell1.appendChild(t1);
    var cell2 = row.insertCell(0);
    var t2 = document.createElement("input");
    t2.className = "form-control";
    t2.id = "txtconfDesc" + configIndex;
    cell2.appendChild(t2);
    var cell3 = row.insertCell(1);
    var t3 = document.createElement("input");
    t3.className = "form-control";
    t3.id = "txtConfAnsLength" + configIndex;
    cell3.appendChild(t3);
    configIndex++;
}

var essenIndex = 1;
function insertEssenRow() {
    var table = document.getElementById("sortableEssen");
    var row = table.insertRow(table.rows.length);
    var cell1 = row.insertCell(0);
    var t1 = document.createElement("input");
    t1.className = "form-control";
    t1.style.width = "24px";
    t1.type = "checkbox";
    t1.id = "txtEssNo" + essenIndex;
    cell1.appendChild(t1);
    var cell2 = row.insertCell(1);
    var t2 = document.createElement("textarea");
    t2.onkeyup = function () { countChar(this) };
    t2.rows = "4";
    t2.cols = "50";
    t2.maxLength = "500";
    t2.className = "form-control";
    t2.id = "txtEssDesc" + essenIndex;
    var t3 = document.createElement("div");
    t3.id = "charNum";
    t3.style.color = "white";
    cell2.appendChild(t2);
    cell2.appendChild(t3);
    essenIndex++;
}

var essenIndex = 1;
function insertDutiesRow() {
    var table = document.getElementById("JobDuties");
    var row = table.insertRow(table.rows.length);
    var cell1 = row.insertCell(0);
    var t1 = document.createElement("input");
    t1.className = "form-control";
    t1.style.width = "24px";
    t1.type = "checkbox";
    t1.id = "txtEssNo" + essenIndex;
    cell1.appendChild(t1);
    var cell2 = row.insertCell(1);
    var t2 = document.createElement("textarea");
    t2.onkeyup = function () { countChar(this) };
    t2.rows = "4";
    t2.cols = "50";
    t2.maxLength = "500";
    t2.className = "form-control";
    t2.id = "txtDuties" + essenIndex;
    var t3 = document.createElement("div");
    t3.id = "charNum";
    t3.style.color = "white";
    cell2.appendChild(t2);
    cell2.appendChild(t3);
    essenIndex++;
}

function deleteDutiesRow(tableID) {
    try {
        var table = document.getElementById(tableID);
        if (table != null && table.rows != null) {
            var rowCount = table.rows.length;
            for (var i = 0; i < rowCount; i++) {
                var row = table.rows[i];
                var chkbox = row.cells[0].childNodes[0];
                if (null != chkbox && true == chkbox.checked) {
                    if (rowCount <= 1) {
                        alert("Cannot delete all the rows.");
                        break;
                    }
                    table.deleteRow(i);
                    rowCount--;
                    i--;
                }
            }
        }
    } catch (e) {
        alert(e);
    }
}

var essenIndex = 1;
function insertRemunerationRow() {
    var table = document.getElementById("JobRemuneration");
    var row = table.insertRow(table.rows.length);
    var cell1 = row.insertCell(0);
    var t1 = document.createElement("input");
    t1.className = "form-control";
    t1.style.width = "24px";
    t1.type = "checkbox";
    t1.id = "txtEssNo" + essenIndex;
    cell1.appendChild(t1);
    var cell2 = row.insertCell(1);
    var t2 = document.createElement("textarea");
    t2.onkeyup = function () { countChar(this) };
    t2.rows = "4";
    t2.cols = "50";
    t2.maxLength = "500";
    t2.className = "form-control";
    t2.id = "txtRemuneration" + essenIndex;
    var t3 = document.createElement("div");
    t3.id = "charNum";
    t3.style.color = "white";
    cell2.appendChild(t2);
    cell2.appendChild(t3);
    essenIndex++;
}

function deleteRemunerationRow(tableID) {
    try {
        var table = document.getElementById(tableID);
        if (table != null && table.rows != null) {
            var rowCount = table.rows.length;
            for (var i = 0; i < rowCount; i++) {
                var row = table.rows[i];
                var chkbox = row.cells[0].childNodes[0];
                if (null != chkbox && true == chkbox.checked) {
                    if (rowCount <= 1) {
                        alert("Cannot delete all the rows.");
                        break;
                    }
                    table.deleteRow(i);
                    rowCount--;
                    i--;
                }
            }
        }
    } catch (e) {
        alert(e);
    }
}

function checkfileformat(str) {
    if (str && str.indexOf('.') != -1) {
        var fileExtension = str.substr(str.lastIndexOf('.')).toLowerCase();
        return (!((fileExtension != ".pdf") && (fileExtension != ".rtf") && (fileExtension != ".doc") && (fileExtension != ".docx") && (fileExtension != ".htm") && (fileExtension != ".html") && (fileExtension != ".ppt") && (fileExtension != ".txt") && (fileExtension != ".")));
    }
    return false;
}

function checkfileformatforAdlogic(str) {
    if (str && str.indexOf('.') != -1) {
        var fileExtension = str.substr(str.lastIndexOf('.')).toLowerCase();
        return (!((fileExtension != ".pdf") && (fileExtension != ".rtf") && (fileExtension != ".doc") && (fileExtension != ".txt") && (fileExtension != ".")));
    }
    return false;
}

function checkFormFields(check) {
    if (typeof (check) != 'undefined' && check) {
        if (typeof (check.firstName) != 'undefined' && check.firstName.value == "") {
            alert("The name field cannot be blank - please re-enter");
            //check.firstName.focus();
            return false;
        }
        else if (typeof (check.Name) != 'undefined' && check.Name.value == "") {
            alert("The name field cannot be blank - please re-enter");
            //check.Name.focus();
            return false;
        }
        else if (typeof (check.FirstName) != 'undefined' && check.FirstName.value == "") {
            alert("The first name field cannot be blank - please re-enter");
            //check.FirstName.focus();
            return false;
        }
        else if (typeof (check.LastName) != 'undefined' && check.LastName.value == "") {
            alert("The last name field cannot be blank - please re-enter");
            //check.LastName.focus();
            return false;
        }
        else if (typeof (check.SurName) != 'undefined' && check.SurName.value == "") {
            alert("The Family Name field cannot be blank - please re-enter");
            //check.SurName.focus();
            return false;
        }
        else if (typeof (check.Email) != 'undefined' && check.Email.value == "") {
            alert("The email field cannot be blank - please re-enter");
            //check.Email.focus();
            return false;
        }
        else if (typeof (check.Email) != 'undefined' && !checkemail(check.Email.value)) {
            alert("Please enter a valid email address");
            //check.Email.focus();
            return false;
        }
        else if (typeof (check.PhoneCode) != 'undefined' && check.PhoneCode.value == "") {
            alert("The country code field cannot be blank - please re-enter");
            //check.PhoneCode.focus();
            return false;
        }
        else if (typeof (check.ContactNumber) != 'undefined' && check.ContactNumber.value == "") {
            alert("The contact number field cannot be blank - please re-enter");
            //check.ContactNumber.focus();
            return false;
        }
        else if (typeof (check.ContactNumber) != 'undefined' && !checkphone(check.ContactNumber.value)) {
            alert("Please enter a valid contact number");
            //check.ContactNumber.focus();
            return false;
        }
        else if (typeof (check.selectOccupation) != 'undefined' && check.selectOccupation.value == "") {
            alert("Please select Occupation");
            //check.selectOccupation.focus();
            return false;
        }

        else if (typeof (check.IndustrySelect) != 'undefined' && check.IndustrySelect.selectedIndex == "-1") {
            alert("Please select Industry");
            //check.IndustrySelect.focus();
            return false;
        }

        else if (typeof (check.LocationSelect) != 'undefined' && check.LocationSelect.selectedIndex == "-1") {
            alert("Please select Location");
            //check.LocationSelect.focus();
            return false;
        }

        else if (typeof (check.WorkTypeSelect) != 'undefined' && check.WorkTypeSelect.selectedIndex == "-1") {
            alert("Please select Work type");
            //check.WorkTypeSelect.focus();
            return false;
        }

        else if (typeof (check.phone) != 'undefined' && check.phone.value == "") {
            alert("The phone field cannot be blank - please re-enter");
            //check.phone.focus();
            return false;
        }
        else if (typeof (check.phone) != 'undefined' && !checkphone(check.phone.value)) {
            alert("Please enter a valid phone number");
            //check.phone.focus();
            return false;
        }
        else if (typeof (check.contactNo) != 'undefined' && check.contactNo.value == "") {
            alert("The contact number field cannot be blank - please re-enter");
            //check.contactNo.focus();
            return false;
        }
        else if (typeof (check.contactNo) != 'undefined' && !checkphone(check.contactNo.value)) {
            alert("Please enter a valid contact number");
            //check.contactNo.focus();
            return false;
        }

        else if (typeof (check.ContactNumber) != 'undefined' && !checkphone(check.ContactNumber.value)) {
            alert("Please enter a valid contact number");
            //check.ContactNumber.focus();
            return false;
        }
        else if (typeof (check.mobileCell) != 'undefined' && check.mobileCell.value == "") {
            alert("The mobile/cell number field cannot be blank - please re-enter");
            //check.mobileCell.focus();
            return false;
        }
        else if (typeof (check.mobileCell) != 'undefined' && !checkphone(check.mobileCell.value)) {
            alert("Please enter a valid mobile/cell number");
            //check.mobileCell.focus();
            return false;
        }
        else if (typeof (check.email) != 'undefined' && check.email.value == "") {
            alert("The email field cannot be blank - please re-enter");
            //check.email.focus();
            return false;
        }

        else if (typeof (check.email) != 'undefined' && !checkemail(check.email.value)) {
            alert("Please enter a valid email address");
            //check.email.focus();
            return false;
        }

        else if (typeof (check.companyName) != 'undefined' && check.companyName.value == "") {
            alert("The company name field cannot be blank - please re-enter");
            //check.companyName.focus();
            return false;
        }
        else if (typeof (check.contactNo) != 'undefined' && check.contactNo.value == "") {
            alert("The mobile number field cannot be blank - please re-enter");
            //check.contactNo.focus();
            return false;
        }
        else if (typeof (check.contactNo) != 'undefined' && !checkphone(check.contactNo.value)) {
            alert("Please enter a valid mobile number");
            //check.contactNo.focus();
            return false;
        }
            //        else if (typeof (check.selectIndustry) != 'undefined' && check.selectIndustry.value == "") {
            //            alert("Please select the industry you are interested in");
            //            check.selectIndustry.focus();
            //            return false;
            //        }
        else if (typeof (check.IndustrySelect) != 'undefined' && check.IndustrySelect.length == 0) {
            alert("Please select the industry you are interested in");
            //check.IndustrySelect.focus();
            return false;
        }
            //        else if (typeof (check.selectIndustry) != 'undefined' && check.selectIndustry.value == "0" && typeof (check.otherIndustry) != 'undefined' && check.otherIndustry.value == "") {
            //            alert("Please enter the 'other' industry you are interested in");
            //            check.otherIndustry.focus();
            //            return false;
            //        }
        else if (typeof (check.selectOccupation) != 'undefined' && check.selectOccupation.value == "") {
            alert("Please select your Occupation category by typing at the Occupation field then selecting from the options available.");
            //check.selectOccupation.focus();
            return false;
        }
        else if (typeof (check.attachResume) != 'undefined' && check.attachResume.value == "") {
            alert("Please select your resume to upload");
            //check.attachResume.focus();
            return false;
        }
            //        else if (typeof (check.attachResume) != 'undefined' && !checkfileformat(check.attachResume.value)) {
            //            alert("Invalid resume file format");
            //            check.attachResume.focus();
            //            return false;
            //        }
        else if (typeof (check.attachment) != 'undefined' && check.attachment.value == "") {
            alert("Please select your resume to upload");
            // check.attachment.focus();
            return false;
        }
            //        else if (typeof (check.attachment) != 'undefined' && !checkfileformat(check.attachment.value)) {
            //            alert("Invalid resume file format");
            //            check.attachment.focus();
            //            return false;
            //        }
        else if (typeof (check.coverLetter) != 'undefined' && check.coverLetter.value == "") {
            alert("Please select your cover letter to upload");
            // check.coverLetter.focus();
            return false;
        }
            //        else if (typeof (check.coverLetter) != 'undefined' && !checkfileformat(check.coverLetter.value)) {
            //            alert("Invalid cover letter file format");
            //            check.coverLetter.focus();
            //            return false;
            //        }
        else if (typeof (check.additionalDocs) != 'undefined' && check.additionalDocs.value == "") {
            alert("Please select your additional documents to upload");
            //check.coverLetter.focus();
            return false;
        }
            //        else if (typeof (check.coverLetter) != 'undefined' && !checkfileformat(check.additionalDocs.value)) {
            //            alert("Invalid additional document file format");
            //            check.coverLetter.focus();
            //            return false;
            //        }
        else if (typeof (check.YourEmail) != 'undefined' && check.YourEmail.value == "") {
            alert("The from email field cannot be blank - please re-enter");
            //check.YourEmail.focus();
            return false;
        }
        else if (typeof (check.YourEmail) != 'undefined' && !checkemail(check.YourEmail.value)) {
            alert("Please enter a valid from email address");
            //check.YourEmail.focus();
            return false;
        }
        else if (typeof (check.FriendEmail) != 'undefined' && check.FriendEmail.value == "") {
            alert("The to email field cannot be blank - please re-enter");
            // check.FriendEmail.focus();
            return false;
        }
        else if (typeof (check.FriendEmail) != 'undefined' && !checkemail(check.FriendEmail.value)) {
            alert("Please enter a valid to email address");
            // check.FriendEmail.focus();
            return false;
        }
        else if (typeof (check.BirthDate) != 'undefined' && check.BirthDate.value == "") {
            alert("Please enter birth date");
            //check.BirthDate.focus();
            return false;
        }

        return true;
    }
    return true;
}

function yearvalidation1(id) {
    var fy = $('.CurrentFY').val();
    var ty = $('.CurrentTY').val();
    var fm = $('.CurrentFM').val();
    var tm = $('.CurrentTM').val();
    var fd = $('.CurrentFD').val();
    var td = $('.CurrentTD').val();
    var fdt;
    var todt;
    fd = fd == '' ? '01' : (fd < 10 ? '0' + fd : fd);
    td = td == '' ? fd : (td < 10 ? '0' + td : td);
    fdt = new Date(fy + '-' + (fm == '' ? '01' : ($('.CurrentFM').attr('selectedIndex') < 10 ? ('0' + $('.CurrentFM').attr('selectedIndex')) : $('.CurrentFM').attr('selectedIndex'))) + '-' + fd);
    todt = new Date(ty + '-' + (tm == '' ? '12' : ($('.CurrentTM').attr('selectedIndex') < 10 ? ('0' + $('.CurrentTM').attr('selectedIndex')) : $('.CurrentTM').attr('selectedIndex'))) + '-' + td);
    if (fy != '') {
        if (isNaN(fdt)) {
            alert('invalid date');
            $(id).val('');
            return;
        }
    }
    if (ty != '' && ty != 'Present') {
        if (isNaN(todt)) {
            alert('invalid date');
            $(id).val('');
            return;
        }
    }
    if (fy != '' && ty != '') {
        if (ty != 'Present')
            if (fdt > todt) {
                alert("The From date should not be more recent than the To date. ");
                $(id).val('');
            }
    }
}

function cmonthchange1(id) {
    var fy = $('.CurrentFY').val();
    if (fy == '') {
        alert('Please select Year');
        $(id).val('');
    }
    if ($('.CurrentFY').val() == 'Present') {
        alert('Not allowed to select Month if year is set to Present');
        $(id).val('');
    }
    $('.CurrentFD').val('');
}

function cdaychange1(id) {
    var fy = $('.CurrentFM').val();
    if (fy == '') {
        alert('Please select month');
        $(id).val('');
    }
}

function ctmonthchange1(id) {
    var fy = $('.CurrentTY').val();
    if (fy == '') {
        alert('Please select Year');
        $(id).val('');
    }
    if ($('.CurrentTY').val() == 'Present') {
        alert('Not allowed to select Month if year is set to Present');
        $(id).val('');
    }
    $('.CurrentTD').val('');
}

function ctdaychange1(id) {
    var fy = $('.CurrentTM').val();
    if (fy == '') {
        alert('Please select month');
        $(id).val('');
    }
}

function historyyearvalidation1(id) {
    var fy = '';
    var ty = '';
    var fm = '';
    var tm = '';
    var fd = '';
    var td = '';
    switch ($(id).attr("id")) {
        case "EmpFY":
            fy = $(id).val();
            break;
        case "EmpTY":
            ty = $(id).val();
            break;
        case "EmpTM":
            tm = $(id).attr('selectedIndex');
            break;
        case "EmpFM":
            fm = $(id).attr('selectedIndex');
            break;
        case "EmpFD":
            fd = $(id).val();
            break;
        case "EmpTD":
            td = $(id).val();
            break;
    }
    $(id).siblings('select').each(function () {
        switch ($(this).attr("id")) {
            case "EmpFY":
                fy = $(this).val();
                break;
            case "EmpTY":
                ty = $(this).val();
                break;
            case "EmpTM":
                tm = $(this).attr('selectedIndex');
                break;
            case "EmpFM":
                fm = $(this).attr('selectedIndex');
                break;
            case "EmpFD":
                fd = $(this).val();
                break;
            case "EmpTD":
                td = $(this).val();
                break;
        }
    });

    $(id).parent().parent().siblings().find('select').each(function () {
        switch ($(this).attr("id")) {
            case "EmpFY":
                fy = $(this).val();
                break;
            case "EmpTY":
                ty = $(this).val();
                break;
            case "EmpTM":
                tm = $(this).attr('selectedIndex');
                break;
            case "EmpFM":
                fm = $(this).attr('selectedIndex');
                break;
            case "EmpFD":
                fd = $(this).val();
                break;
            case "EmpTD":
                td = $(this).val();
                break;
        }
    });

    var fdt;
    var todt;

    fm = (fm == '' ? '01' : (fm < 10 ? '0' + fm : fm));
    tm = (tm == '' ? '12' : (tm < 10 ? '0' + tm : tm));
    fd = fd == '' ? '01' : (fd < 10 ? '0' + fd : fd);
    td = td == '' ? fd : (td < 10 ? '0' + td : td);

    fdt = new Date(fy + '-' + fm + '-' + fd);
    todt = new Date(ty + '-' + tm + '-' + td);

    if (fy != '') {
        if (isNaN(fdt)) {
            alert('invlaid date');
            $(id).val('');
            return;
        }
    }

    if (ty != '' && ty != 'Present') {
        if (isNaN(todt)) {
            alert('invalid date');
            $(id).val('');
            return;
        }
    }

    if (fy != '' && ty != '') {
        if (ty != 'Present')
            if (fdt > todt) {
                alert("The From date should not be more recent than the To date. ");
                $(id).val('');
            }
    }
}

function yearchange1(id) {
    $(id).siblings('select').each(function () { this.value = ''; });
    return true;
}

function fmonthchange1(id) {
    var fy = '';
    var ty = '';
    var fm = '';
    var tm = '';
    var fd = '';
    var td = '';
    $(id).siblings('select').each(function () {
        switch ($(this).attr("id")) {
            case "EmpFY":
                fy = $(this).val();
                break;
            case "EmpFM":
                fm = $(this).attr('selectedIndex');
                break;
            case "EmpFD":
                fd = $(this).val('');
                break;
        }
    });
    if (fy == '') {
        alert('Please select Year');
        $(id).val('');
        return false;
    }
    if (fy == 'Present') {
        alert('Not allowed to select Month if year is set to Present');
        $(id).val('');
    }
    return true;
}

function fdaychange1(id) {
    var fy = '';
    var ty = '';
    var fm = '';
    var tm = '';
    var fd = '';
    var td = '';
    $(id).siblings('select').each(function () {
        switch ($(this).attr("id")) {
            case "EmpFY":
                fy = $(this).val();
                break;
            case "EmpFM":
                fm = $(this).attr('selectedIndex');
                break;
            case "EmpFD":
                fd = $(this).val();
                break;
        }
    });
    if (fm == '') {
        alert('Please select month');
        $(id).val('');
        return false;
    }
    return true;
}

function tmonthchange1(id) {
    var fy = '';
    var ty = '';
    var fm = '';
    var tm = '';
    var fd = '';
    var td = '';
    $(id).siblings('select').each(function () {
        switch ($(this).attr("id")) {
            case "EmpTY":
                fy = $(this).val();
                break;
            case "EmpTM":
                fm = $(this).attr('selectedIndex');
                break;
            case "EmpTD":
                fd = $(this).val('');
                break;
        }
    });
    if (fy == '') {
        alert('Please select Year');
        $(id).val('');
        return false;
    }
    if (fy == 'Present') {
        alert('Not allowed to select Month if year is set to Present');
        $(id).val('');
    }
    return true;
}

function tdaychange1(id) {
    var fy = '';
    var ty = '';
    var fm = '';
    var tm = '';
    var fd = '';
    var td = '';
    $(id).siblings('select').each(function () {
        switch ($(this).attr("id")) {
            case "EmpTY":
                fy = $(this).val();
                break;
            case "EmpTM":
                fm = $(this).attr('selectedIndex');
                break;
            case "EmpTD":
                fd = $(this).val();
                break;
        }
    });
    if (fm == '') {
        alert('Please select month');
        $(id).val('');
        return false;
    }
    return true;
}

function phonetypechange1(id) {
    var txt = $(id).find('option:selected').text();
    $(id).siblings('input').val(txt);
}

function checkValid1() {
    var valid = true;
    var valcontact = false;
    if ($('.CoverLetter1').val().length > 2000) {
        alert("Cover letter exceeds maximum character length. Please reduce the number of characters below the maximum, or attach your Cover Letter as a separate file. Then try clicking Submit again.");
        valid = false;
    }
    if ($('.CurrentCurrency1').val() != '') {
        if ($(".CurrentFrequency1").val() == '-1') {
            valid = false;
            //                $("#CurrentFrequency").focus();
            alert('Please select a frequency for Current Remuneration.');
        }
    }
    if ($('.ExpCurrency1').val() != '') {
        if ($(".ExpFrequency1").val() == '-1') {
            //                $("#ExpFrequency").focus();
            alert('Please select a frequency for Expected Remuneration.');
            valid = false;
        }
    }
    $('.PTCV1').each(function () {
        if ($(this).val() != '-1') {
            if (valcontact == false) {
                var tr = $(this).parent().parent().siblings()[0];
                var tr1 = $(this).parent().parent().siblings()[2];
                if ($(tr).find('input').val() == '' || $(tr1).find('input').val() == '') {
                    alert('In the Contact Details section, please select a Country Code and enter your contact number in the Number field.');
                    valcontact = true;
                    valid = false;
                }
            }
        }
        else {
            if (valcontact == false) {
                var pr = $(this).parent().parent().siblings()[0];
                var pr1 = $(this).parent().parent().siblings()[2];
                if ($(pr).find('input').val() != '' || $(pr1).find('input').val() != '') {
                    alert('In the Contact Details section, please select Phone Type in the Type field.');
                    valcontact = true;
                    valid = false;
                }
            }
        }
    });
    valcontact = false;

    if ($('.CurrentOccupationA').val() != '') {
        if ($('.CurrentOccupationIdA').val() == '') {
            $('.CurrentOccupationA').val('');
            alert('The Occupation field contains invalid data. Please make a selection from the list without editing the selection you make.');
            $('.CurrentOccupationA').focus();
            valid = false;
            return false;
        }
    }

    if ($('.CurrentIndustryA').val() != '') {
        if ($('.CurrentIndustryIdA').val() == '') {
            $('.CurrentIndustryA').val('');
            alert('The Industry field contains invalid data. Please make a selection from the list without editing the selection you make.');
            $('.CurrentIndustryA').focus();
            valid = false;
            return false;
        }
    }

    return valid;
}

function nospaces1(t) {
    if (t.value.match(/\s/g)) {
        alert('Skype Names do not contain spaces');
        t.value = t.value.replace(/\s/g, '');
    }
}

function isNumber1(e) {
    if (e.ctrlKey || e.metaKey)
        return true;
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
}

function isPhoneNumber1(evt) {
    if (e.ctrlKey || e.metaKey)
        return true;
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        var charCode = (e.which) ? e.which : e.keyCode;
        if (charCode == 45 || charCode == 32)
            return true;
        return false;
    }
}

function LettersWithSpaceOnly(evt) {
    evt = (evt) ? evt : event;
    var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
      ((evt.which) ? evt.which : 0));
    if (charCode > 32 && (charCode < 65 || charCode > 90) &&
      (charCode < 97 || charCode > 122)) {
        if (charCode == 45)
            return true;
        return false;
    }
    return true;
}

var counter = 2;
function removeCVMiddlename(id) {
    var div = $(id).parent().parent();
    if (div != null)
        div.html('');
}
function addCVControl(id) {
    var newTextBoxDiv = $(document.createElement('div')).attr("id", 'TextBoxDiv' + counter).attr("style", "clear:both");
    var ele = '<input type="text" name="MiddleName" id="MiddleName" value="" onkeypress="return LettersWithSpaceOnly(event);" >';
    ele += '<div style="float:right;margin-top: 7px; margin-right: 9px;"><img style="display: block !important;visibility: inherit !important;" id="Img5" class="phoneRemove" onclick="removeCVMiddlename(this)" src="../../Content/images/Cross - white on red - 12 pixel.png" onmouseover="showtip(this)" alt="Remove selected"></div>';
    newTextBoxDiv.after().html(ele);
    newTextBoxDiv.appendTo("#divCVmiddle");
}

function removeCVPhone(id) {
    var div = $(id).parent().parent().parent().parent().parent();
    if (div != null)
        div.html('');
}
function addCVPhoneControl(id) {
    var c1 = $(id).siblings('table')[0];
    var tbl = $(c1).clone();
    var newTextBoxDiv = $(document.createElement('div'))
     .attr("id", 'phoneDiv' + counter);
    $(c1).clone().appendTo(newTextBoxDiv);
    newTextBoxDiv.find("input").val('');
    newTextBoxDiv.find("div").show();
    newTextBoxDiv.appendTo("#divCVPhone");
    countryAutoA();
}

function removeCVEmployment(id) {
    var div = $(id).parent().parent().parent().parent().parent();
    if (div != null)
        div.html('');
}
function addCVEmploymentControl(id) {
    var c1 = $(id).siblings('table')[0];
    var newTextBoxDiv = $(document.createElement('div')).attr("id", 'employmentDiv' + counter);
    $(c1).clone().appendTo(newTextBoxDiv);
    newTextBoxDiv.find("input").val('');
    newTextBoxDiv.find("div").show();
    newTextBoxDiv.appendTo("#divCVEmployment");
    employmentHistory();
}

function showtip(id) {
    var img = $(id);
    var title = $(img).attr("alt");
    //$(img).opentip(title, { showEffect: 'blindDown', tipJoint: "top left", removeElementsOnHide: true, stem: true, target: true, tipjoint: ['center', 'top'], hideTrigger: "tip", hideTriggers: ["trigger", "tip"], showOn: "creation", hideOn: "mouseout", fixed: true, background: "rgb(234, 236, 240)", borderColor: "rgb(187, 187, 187)" });
}

function showTagtip(id) {
    var img = $(id);
    var title = $(img).attr("tag");
//    $(img).opentip(title, { showEffect: 'blindDown', tipJoint: "top left", removeElementsOnHide: true, stem: true, target: true, tipjoint: ['center', 'top'], hideTrigger: "tip", hideTriggers: ["trigger", "tip"], showOn: "creation", hideOn: "mouseout", fixed: true, background: "rgb(234, 236, 240)", borderColor: "rgb(187, 187, 187)" });
}

function checkCurrentAmount1(id) {
    var famt = parseInt($('#CurrentFromAmt').val());
    var tamt = parseInt($('#CurrentToAmt').val());
    var cur = $('.CurrentCurrency1').val();
    if (famt > 0 || tamt > 0) {
        if (cur == '') {
            alert('You need to select a currency within the "Remuneration" section.');
            $(id).val('0');
            $('.CurrentCurrency1').focus();
            return;
        }
    }
    if (famt > 0 && tamt > 0) {
        if (tamt < famt) {
            alert('From amount should not be higher than To amount');
            $(id).val('0');
        }
    }
    else {
        if (famt > 0)
            $('#CurrentToAmt').focus();
        else
            if (tamt > 0)
                $(".CurrentFrequency1").focus();
    }
    if ($(id).attr('id') == 'CurrentToAmt') {
        if (tamt < famt) {
            alert('From amount should not be higher than To amount');
            $('#CurrentFromAmt').val('0');
            return false;
        }
        //            if ($(".CurrentFrequency1").val() == '-1') {
        //                $(".CurrentFrequency1").focus();
        //            }
    }
}

function checkExpAmount1(id) {
    var famt = parseInt($('#ExpFromAmt').val());
    var tamt = parseInt($('#ExpToAmt').val());
    var cur = $('.ExpCurrency1').val();
    if (famt > 0 || tamt > 0) {
        if (cur == '') {
            alert('You need to select a currency within the "Remuneration" section.');
            $(id).val('0');
            $('.ExpCurrency1').focus();
            return;
        }
    }
    if (famt > 0 && tamt > 0) {
        if (famt > 0 && tamt > 0) {
            if (tamt < famt) {
                alert('From amount should not be higher than To amount');
                $(id).val('0');
            }
        }
    }
    else {
        if (famt > 0)
            $('#ExpToAmt').focus();
        else
            if (tamt > 0)
                $(".ExpFrequency1").focus();
    }
    if ($(id).attr('id') == 'ExpToAmt') {
        if (tamt < famt) {
            alert('From amount should not be higher than To amount');
            $('#ExpFromAmt').val('0');
            return false;
        }
        //    if ($(".ExpFrequency1").val() == '-1') {
        //        $(".ExpFrequency1").focus();
        //    }
    }
}

function checkCurrentFrequency1() {
    if ($('.CurrentCurrency1').val() != '') {
        if ($(".CurrentFrequency1").val() == '-1') {
            //            $(".CurrentFrequency1").focus();
            alert('Please select a frequency for Current Remuneration.');

            return false;
        }
    }
}

function checkExpFrequency1() {
    if ($('.ExpCurrency1').val() != '') {
        if ($(".ExpFrequency1").val() == '-1') {
            //            $(".ExpFrequency1").focus();
            alert('Please select a frequency for Expected Remuneration.');
            return false;
        }
    }
}

$(document).ready(function () {
    $('.CoverLetter1').keyup(function () {
        textCounter(this, 'desCoverLettContent1', 2000, null);
    })
    $("#CurrentFromAmt").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#CurrentToAmt").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#ExpFromAmt").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#ExpToAmt").keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    var txt = $('.CurrentFrequency1').find('option:selected').text();
    $('.CurrentFreqDesc1').val(txt);

    var txt = $('.ExpFrequency1').find('option:selected').text();
    $('.ExpFreqDesc1').val(txt);

    $('.CurrentFrequency1').change(function () {
        var txt = $(this).find('option:selected').text();
        $('.CurrentFreqDesc1').val(txt);
    });

    $('.ExpFrequency1').change(function () {
        var txt = $(this).find('option:selected').text();
        $('.ExpFreqDesc1').val(txt);
    });

    //$('.file1').preimage();
});

function textCounter(field, cntfield, maxlimit, remHiddenFieldName) {
    var remainingLen = maxlimit - field.value.length;
    var objCnt = document.getElementById(cntfield);
    var drawStr = "";
    if (remainingLen < 0) {
        drawStr = "<font size='1' color='red'>characters left: " + remainingLen + "</font>";
    } else {
        drawStr = "<font size='1'>characters left: " + remainingLen + "</font>";
    }
    if (remHiddenFieldName != null) {
        drawStr += "<input type='hidden' name='";
        drawStr += remHiddenFieldName;
        drawStr += "' value='";
        drawStr += remainingLen;
        drawStr += "'>";
    }
    objCnt.innerHTML = drawStr;
}

function clearContents1(ele) {
    var file = $('.' + ele + '1'); //document.getElementById(ele);
    file.select();
    file.val('');
    file.focus();
    if (ele == 'file2') {
        $('#prev_file2').html('');
    }
}

function get_filename1(obj, type, ftype) {
    if (type == 2) {
        if ($('.Attachresume1')[0].files[0] != null) {
            var file = $('.Attachresume1')[0].files[0].name; // $('.Attachresume').val(); //  document.getElementById('Attachresume').files[0];
            if (file != '') {
                if (file == obj.files[0].name) {
                    alert('The Cover Letter you are trying to attach has the same name and size as the file you have attached as your CV. Please choose the correct file, or you may proceed without attaching a Cover Letter.');
                    clearContents1(obj.id);
                    return;
                }
            }
        }
    }
    if (type == 1) {
        if ($('.CoverLetterOptional1')[0].files[0] != null) {
            var file = $('.CoverLetterOptional1')[0].files[0].name; // $('.CoverLetterOptional').val(); // document.getElementById('CoverLetterOptional').files[0];
            if (file != '') {
                if (file == obj.files[0].name) {
                    alert('The CV you are trying to attach has the same name and size as the file you have attached as your Cover Letter. Please choose a different file as your CV, or remove the file you have attached as your Cover Letter.');
                    clearContents1(obj.id);
                    return;
                }
            }
        }
    }
    $.ajax({
        type: 'get',
        url: '<%= Url.Action("CheckFileExt", "Work")%>',
        data: {
            ext: obj.value,
            type: type,
            size: obj.files[0].size,
            ftype: ftype
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.data == "1") {
                alert(data.msg);
                clearContents1(obj.id);
            }
        }
    })
}

$(function () {
    $(".BirthDate1").val('');
    $(".Occupation1").val('');
    $(".date11").datepicker({
        dateFormat: 'dd M yy', changeMonth: true,
        changeYear: true, yearRange: "-73", maxDate: '-1d'
    });
    $("#btnsubmit").click(function () {
        var country = '';
        $(".NationSelect1 > option").each(function () {
            country = $(this).text() + "\n" + country;
        });
        $(".Countrylist1").val(country);

        $(".NationSelect1").find("option").attr("selected", true);
    });
    //$("#progress").hide();
});

function cvAlert() {
    alert('Do not use this section if you are interested in a specific vacancy. If you have an interest in a specific vacancy, search for the vacancy then Click APPLY ONLINE.');
}

function onlyAlphabets(e, t) {
    try {
        if (window.event) {
            var charCode = window.event.keyCode;
        }
        else if (e) {
            var charCode = e.which;
        }
        else { return true; }
        if ((charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123) || (charCode == 32) || (charCode == 45) || (charCode == 8) || (charCode == 9) || (charCode == 46) || (charCode == 189))
            return true;
        else
            return false;
    }
    catch (err) {
        alert(err.Description);
    }
}

function validateEmailId(email) {
    var expr = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (expr.test(email)) {
        mesg.innerHTML = "";
        return true;
    }
    else {
        mesg.style.color = "red";
        mesg.innerHTML = "Please provide a valid email address";
        return false;
    }
}

function checklength(pswd) {
    if (pswd.length >= 6) {
        pwdmsg.innerHTML = "";
        return true;
    }
    else {
        pwdmsg.style.color = "red";
        pwdmsg.innerHTML = "password should contain minimum 6 chars";
        return false;
    }
}

function checkTypeid(check) {
    if (typeof (check.SelectedLocationList) != 'undefined' && check.SelectedLocationList.length == 0) {
        alert("add atleast one Location");
        return false;
    }
    else if (typeof (check.Empoccupation) != 'undefined' && check.Empoccupation.value == "") {
        alert("occupation cannot be blank");
        return false;
    }
    else if (typeof (check.EmpIndustry) != 'undefined' && check.EmpIndustry.value == "") {
        alert("industry cannot be blank");
        return false;
    }
    else if (typeof (check.TypeId) != 'undefined' && check.TypeId.value == "0") {
        alert("select Job Type");
        return false;
    }
    else if (typeof (check.Remuneration) != 'undefined' && check.Remuneration.length == 0) {
        alert("add atleast one payscale");
        return false;
    }
    else if (typeof (check.Title) != 'undefined' && check.Title.value == "") {
        alert("title is mandatory");
        return false;
    }
    else if (typeof (check.JobContent) != 'undefined' && check.JobContent.value == "") {
        alert("JobContent is mandatory");
        return false;
    }
    else if (typeof (check.DutiesList) != 'undefined' && check.DutiesList.length == 0) {
        alert("add atleast one responsibility");
        return false;
    }
    else if (typeof (check.MustHaveList) != 'undefined' && check.MustHaveList.length == 0) {
        alert("add atleast one must haves");
        return false;
    }
    else if (typeof (check.NiceToHaveList) != 'undefined' && check.NiceToHaveList.length == 0) {
        alert("add atleast one nice To haves");
        return false;
    }
    else {
        valid = true;
    }
}

$("#SelectAll").click(function () {
    $("#IndustrySelect").find("option").attr("selected", this.checked);
})

$("#SelectAllOccupation").click(function () {
    $(".OccupationAlert").find("option").attr("selected", this.checked);
})

$("#Subscribe").click(function () {
    debugger;
    $(".OccupationAlert").find("option").attr("selected", true);
    $(".IndustryAlert").find("option").attr("selected", true);
    $("#WorkTypeSelect").find("option").attr("selected", true);
    $(".LocationAlert").find("option").attr("selected", true);
    var industry = '';
    $(".IndustryAlert > option:selected").each(function () {
        industry = industry + " " + $(this).text() + "<br/>";
    });
    $("#IndustryNameList").val(industry);

    var occupation = '';
    $(".OccupationAlert > option:selected").each(function () {
        occupation = occupation + " " + $(this).text() + "<br/>";
    });
    $("#OccupationNameList").val(occupation);

    var location = '';
    $(".LocationAlert > option:selected").each(function () {
        location = location + " " + $(this).text() + "<br/>";
    });
    $("#LocationNameList").val(location);
});

$("#btnOccremove").click(function () {
    $(".OccupationAlert > option:selected").each(function () {
        $(this).remove();
    });

    if ($(".OccupationAlert > option").length == 0)
        $("#divAlertOccSel").hide();
});
$("#btnIndRemove").click(function () {
    $(".IndustryAlert > option:selected").each(function () {
        $(this).remove();
    });

    if ($(".IndustryAlert > option").length == 0)
        $("#divAlertIndSel").hide();
});

$("#btnLocREmove").click(function () {
    $(".LocationAlert > option:selected").each(function () {
        if ($(this).val().split(':')[1] == '5') {
            $("#RemoveSelectedLocation").append($('<option>').text($(this).text()).val($(this).text()));
        }
        else {
            $("#RemoveSelectedLocation").append($('<option>').text($(this).val()).val($(this).val()));
        }

        $(this).remove();
    });

    if ($(".LocationAlert > option").length == 0)
        $("#divAlertLocSel").hide();
});


function checkCriteria(check) {
    var valid;
    if (checkFormFields(check)) {
        var empties = $('.required').filter(function () {
            hideProgress();
            return $.trim($(this).val()) == '';
        });
        if (empties.length) {
            alert("One or more of the Essential Criteria needs your comment please.");
            valid = false;
        }
        else
            valid = true;
    }
    else
        valid = false;
    if (!valid)
        hideProgress();
    return valid;
}

function textCounter(field, cntfield, maxlimit, remHiddenFieldName) {
    var remainingLen = maxlimit - field.value.length;
    var objCnt = document.getElementById(cntfield);
    var drawStr = "";
    if (remainingLen < 0) {
        drawStr = "<font size='1' color='red'>characters left: " + remainingLen + "</font>";
    } else {
        drawStr = "<font size='1'>characters left: " + remainingLen + "</font>";
    }
    if (remHiddenFieldName != null) {
        drawStr += "<input type='hidden' name='";
        drawStr += remHiddenFieldName;
        drawStr += "' value='";
        drawStr += remainingLen;
        drawStr += "'>";
    }
    objCnt.innerHTML = drawStr;
}
$(document).ready(function () {
    $("#progress").hide();
    $(".date").datepicker({
        dateFormat: 'dd M yy', changeMonth: true,
        changeYear: true, yearRange: "-73", maxDate: '-1d'
    });
});

$(function () {
    $("#applyButton").click(function () {
        $("#progress").show();
    });
});

function hideProgress() {
    $("#progress").hide();
}
function clearContents(ele) {
    var file = document.getElementById(ele);
    file.select();
    file.value = "";
    file.focus();
}

function get_filename(obj, type, ftype) {
    if (type == 2) {
        var file = document.getElementById('attachment').files[0];
        if (file != null) {
            if (file.name == obj.files[0].name) {
                alert('The Cover Letter you are trying to attach has the same name and size as the file you have attached as your CV. Please choose the correct file, or you may proceed without attaching a Cover Letter.');
                clearContents(obj.id);
                return;
            }
        }
    }
    if (type == 1) {
        var file = document.getElementById('coverLetterOptional').files[0];
        if (file != null) {
            if (file.name == obj.files[0].name) {
                alert('The CV you are trying to attach has the same name and size as the file you have attached as your Cover Letter. Please choose a different file as your CV, or remove the file you have attached as your Cover Letter.');
                clearContents(obj.id);
                return;
            }
        }
    }
    $.ajax({
        type: 'get',
        url: '<%= Url.Action("CheckFileExt", "Work")%>',
        data: {
            ext: obj.value,
            type: type,
            size: obj.files[0].size,
            ftype: ftype
        },
        cache: false,
        dataType: 'json',
        success: function (data) {
            if (data.data == "1") {
                alert(data.msg);
                clearContents(obj.id);
            }
        }
    })
}