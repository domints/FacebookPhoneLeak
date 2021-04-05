publicSalt = '';

checkPhone = () => {
    let value = document.getElementById("phoneNumber").value;
    let number = normalizePhoneNumber(value);
    let hash = getHash(number);
    resetResult();
    $.post({
            url: "/api/search/byPhone",
            data: JSON.stringify(hash),
            contentType: 'text/json',
            success: function (data, textStatus, jqXHR) {
                console.log(data, textStatus, jqXHR);
                setupResult(data);
            },
            dataType: 'json'
        });
}

checkId = () => {
    let value = document.getElementById("profileId").value;
    let hash = getHash(value);
    resetResult();
    $.post({
        url: "/api/search/byId",
        data: JSON.stringify(hash),
        contentType: 'text/json',
        success: function (data, textStatus, jqXHR) {
            setupResult(data);
        },
        dataType: 'json'
    });
}

setupResult = (data) => {
    if(!data.entryExists)
    {
        $("#result-clean").show();
        $("#result-card").hide();
        return;
    }

    if(data.entryExists && data.hasName && (data.hasEmail || data.hasBirthdate))
        $("#result-danger").show();
    else
        $("#result-warning").show();

    let resultTable = $("#result-table");
    for (let val in data)
    {
        let dataField = resultTable.find("#" + val);
        if(dataField.length == 0)
            continue;
        
            dataField.text(data[val] ? "Yes" : "No");
    }

    $("#result-spinner").hide();
    $("#result-table").show();
}

resetResult = () => {
    $("#result-clean").hide();
    $("#result-danger").hide();
    $("#result-warning").hide();
    $("#result-table").hide();
    $("#result-card").show();
    $("#result-spinner").show();
}

normalizePhoneNumber = (number) => {
    let cleaned = replaceAll(number, " ", "");
    cleaned = replaceAll(cleaned, "-", "");
    cleaned = replaceAll(cleaned, "+", "");
    while (cleaned.indexOf("0") == 1) {
        cleaned = cleaned.substr(1);
    }

    return cleaned.replace(/^0+/, '');
}

replaceAll = (input, oldTxt, newTxt) => {
    return input.split(oldTxt).join(newTxt);
}

getHash = (value) => {
    return sjcl.codec.base64.fromBits(sjcl.misc.pbkdf2(value, publicSalt, 500, 256));
}

$(function() {
    $.get("api/search/publicSalt",
        function (data, textStatus, jqXHR) {
            publicSalt = data;
        }
    );
    $.get("api/search/collections",
        function (data) {
            let regionList = $("#regionList");
            for(let region of data)
            {
                let regionElement = $('<div class="col"></div>').text(region);
                regionList.append(regionElement);
            }
            $("#region-spinner").hide();
            $("#region-container").show();
        }
    )
});